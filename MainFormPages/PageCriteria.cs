using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using NexScore;
using NexScore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NexScore.MainFormPages
{
    public partial class PageCriteria : UserControl
    {
        private bool _isLoading;
        private string? _lastEventId;
        private WebView2? _web;

        public PageCriteria()
        {
            InitializeComponent();

            this.Load += async (_, __) =>
            {
                await EnsureWebAsync();
                if (AppSession.CurrentEvent != null)
                    await LoadAndRenderAsync(AppSession.CurrentEvent);
            };

            this.VisibleChanged += async (_, __) =>
            {
                if (this.Visible && AppSession.CurrentEvent != null)
                {
                    await EnsureWebAsync();
                    await LoadAndRenderAsync(AppSession.CurrentEvent);
                }
            };

            AppSession.CurrentEventChanged += async evt =>
            {
                if (!IsHandleCreated || IsDisposed) return;
                if (InvokeRequired)
                {
                    BeginInvoke(new Action(async () =>
                    {
                        await EnsureWebAsync();
                        await LoadAndRenderAsync(evt);
                    }));
                }
                else
                {
                    await EnsureWebAsync();
                    await LoadAndRenderAsync(evt);
                }
            };
        }

        private async Task EnsureWebAsync()
        {
            if (_web != null) return;

            _web = new WebView2
            {
                Dock = DockStyle.Fill,
                DefaultBackgroundColor = System.Drawing.Color.Transparent
            };

            pnlMainPhasesCri.Controls.Clear();
            pnlMainPhasesCri.Controls.Add(_web);

            try
            {
                await _web.EnsureCoreWebView2Async();
                _web.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
                _web.CoreWebView2.Settings.AreDevToolsEnabled = false;
                _web.CoreWebView2.Settings.IsZoomControlEnabled = false;
            }
            catch (WebView2RuntimeNotFoundException)
            {
                pnlMainPhasesCri.Controls.Clear();
                pnlMainPhasesCri.Controls.Add(new Label
                {
                    Text = "WebView2 runtime is not installed.\nPlease install Microsoft Edge WebView2 Runtime.",
                    ForeColor = System.Drawing.Color.LightGray,
                    Font = new System.Drawing.Font("Lexend Deca", 10f),
                    AutoSize = true,
                    Padding = new Padding(12),
                    Location = new System.Drawing.Point(10, 10)
                });
            }
        }

        private async Task LoadAndRenderAsync(EventModel evt)
        {
            if (_web?.CoreWebView2 == null) return;
            if (_isLoading) return;
            _isLoading = true;

            try
            {
                UseWaitCursor = true;

                var phases = await LoadPhasesAsync(evt);
                string html = BuildHtml(phases);
                _web.NavigateToString(html);

                _lastEventId = evt.Id;
            }
            catch (Exception ex)
            {
                string err = $"<html><body style='background:transparent;color:#ccc;font-family:Consolas,monospace;padding:16px;'>Error loading criteria:<br>{WebUtility.HtmlEncode(ex.Message)}</body></html>";
                _web?.NavigateToString(err);
            }
            finally
            {
                UseWaitCursor = false;
                _isLoading = false;
            }
        }

        private async Task<List<PhaseModel>> LoadPhasesAsync(EventModel evt)
        {
            var col = Database.GetCollection<EventStructureModel>("EventStructures");
            var typed = await col.Find(s => s.EventId == evt.Id).FirstOrDefaultAsync();
            var phases = typed?.Phases;
            if (phases != null && phases.Count > 0)
                return phases;

            var rawCol = Database.GetCollection<BsonDocument>("EventStructures");
            var ors = new List<FilterDefinition<BsonDocument>>();
            if (ObjectId.TryParse(evt.Id, out var oidMain))
                ors.Add(Builders<BsonDocument>.Filter.Eq("eventId", oidMain));
            ors.Add(Builders<BsonDocument>.Filter.Eq("eventId", evt.Id));

            if (!string.IsNullOrWhiteSpace(evt.EventId) && evt.EventId != evt.Id)
            {
                if (ObjectId.TryParse(evt.EventId, out var oidAlt))
                    ors.Add(Builders<BsonDocument>.Filter.Eq("eventId", oidAlt));
                ors.Add(Builders<BsonDocument>.Filter.Eq("eventId", evt.EventId));
            }

            var filter = ors.Count > 0 ? Builders<BsonDocument>.Filter.Or(ors)
                                       : FilterDefinition<BsonDocument>.Empty;

            var raw = await rawCol.Find(filter).FirstOrDefaultAsync();
            if (raw == null) return new List<PhaseModel>();

            var arr = raw.GetValue("phases", new BsonArray()).AsBsonArray;
            var list = new List<PhaseModel>(arr.Count);
            foreach (var item in arr)
            {
                if (!item.IsBsonDocument) continue;
                try { list.Add(BsonSerializer.Deserialize<PhaseModel>(item.AsBsonDocument)); }
                catch { /* skip malformed */ }
            }
            return list;
        }

        private string BuildHtml(List<PhaseModel> phases)
        {
            string colorPhase = "#3A3D74";   
            string colorSegment = "#323464"; 
            string colorRow = "#2C2E58";     
            string colorText = "#F7F6ED";   
            string bg = "transparent";

            var css = $@"
:root {{
  --indent-seg: 28px;
  --indent-criteria: 44px;
  --right-gutter: 18px; /* adjust this to add space from the far right edge */
}}

html,body{{margin:0;padding:0;background:{bg};color:{colorText};font-family:'Lexend Deca',Segoe UI,Arial,sans-serif;font-size:16px;}}
.page{{padding:10px 14px 40px 14px;}}

.phase{{margin:14px 0 8px 0;background:{colorPhase};border-radius:2px;}}
.line{{display:grid;grid-template-columns:1fr auto;align-items:center;gap:8px;padding:6px 10px;white-space:nowrap;overflow:hidden;text-overflow:ellipsis;}}
.phase .title{{font-weight:700;font-size:17px;}}
.phase .weight,.segment .weight{{font-weight:700;}}

/* Phase: mirror right gutter only */
.phase .line{{padding-right:var(--right-gutter);}}

/* Segment: left indent + mirrored right padding (to criteria boundary) plus right gutter */
.segment{{background:{colorSegment};margin-top:2px;}}
.segment .line{{padding-left:var(--indent-seg); padding-right:calc(var(--indent-criteria) + var(--right-gutter));}}

/* Criteria start under segment NAME */
.criteria{{padding:2px 0 6px 0;}}
.criteria.indented{{padding-left:var(--indent-criteria);}}

/* Criteria rows: mirrored right padding (same as left indent) plus right gutter */
.criteria-row{{display:grid;grid-template-columns:1fr auto;align-items:center;background:{colorRow};margin:1px 0;height:26px;overflow:hidden;padding-right:calc(var(--indent-criteria) + var(--right-gutter));}}
.criteria-row .name{{padding:0 8px;white-space:nowrap;overflow:hidden;text-overflow:ellipsis;}}
.criteria-row .wt{{padding:0 8px;text-align:right;font-variant-numeric:tabular-nums;}}

.muted{{opacity:.85;padding:10px;}}
";

            var sb = new StringBuilder();
            sb.Append("<!doctype html><html><head><meta charset='utf-8'>");
            sb.Append("<style>").Append(css).Append("</style></head><body><div class='page'>");

            if (phases == null || phases.Count == 0)
            {
                sb.Append("<div class='muted'>No phases / segments / criteria defined yet.</div>");
            }
            else
            {
                foreach (var phase in phases.OrderBy(p => p.Sequence ?? int.MaxValue))
                {
                    string phaseNo = phase.Sequence.HasValue ? WebUtility.HtmlEncode(phase.Sequence.Value.ToString()) : "–";
                    string phaseName = string.IsNullOrWhiteSpace(phase.Name) ? "(Unnamed Phase)" : WebUtility.HtmlEncode(phase.Name);
                    string phaseWt = $"{phase.Weight:0.##}%";

                    sb.Append("<div class='phase'>")
                      .Append("<div class='line'><div class='title'>Phase No. ").Append(phaseNo)
                      .Append("&emsp;&emsp;&emsp;").Append(phaseName)
                      .Append("</div><div class='weight'>").Append(phaseWt).Append("</div></div>");

                    var segments = (phase.Segments ?? new List<SegmentModel>()).OrderBy(s => s.Sequence).ToList();
                    foreach (var seg in segments)
                    {
                        string segName = string.IsNullOrWhiteSpace(seg.Name) ? "(Unnamed Segment)" : WebUtility.HtmlEncode(seg.Name);
                        string segWt = $"{seg.Weight:0.##}%";

                        sb.Append("<div class='segment'>")
                          .Append("<div class='line'><div>Segment No. ").Append(WebUtility.HtmlEncode(seg.Sequence.ToString()))
                          .Append("&emsp;&emsp;&emsp;").Append(segName)
                          .Append("</div><div class='weight'>").Append(segWt).Append("</div></div>");

                        var criteria = seg.Criteria ?? new List<CriteriaModel>();
                        if (criteria.Count > 0)
                        {
                            sb.Append("<div class='criteria indented'>");
                            foreach (var crit in criteria)
                            {
                                string cNameRaw = (crit.Name ?? "").Trim();
                                if (string.IsNullOrEmpty(cNameRaw) && crit.Weight <= 0) continue;
                                string cName = WebUtility.HtmlEncode(string.IsNullOrEmpty(cNameRaw) ? "(Unnamed Criteria)" : cNameRaw);
                                string cWt = $"{crit.Weight:0.##}%";

                                sb.Append("<div class='criteria-row'><div class='name'>").Append(cName)
                                  .Append("</div><div class='wt'>").Append(cWt)
                                  .Append("</div></div>");
                            }
                            sb.Append("</div>");
                        }

                        sb.Append("</div>"); // segment
                    }

                    sb.Append("</div>"); // phase
                }
            }

            sb.Append("</div></body></html>");
            return sb.ToString();
        }
    }
}