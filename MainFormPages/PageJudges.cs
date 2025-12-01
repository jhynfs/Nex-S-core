using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MongoDB.Driver;
using Microsoft.Web.WebView2.WinForms;
using NexScore.Models;

namespace NexScore.MainFormPages
{
    public partial class PageJudges : UserControl
    {
        private Panel _topBar;
        private Label _lblTitle;

        private WebView2? _web;
        private bool _isLoading;
        private string? _lastEventId;

        public PageJudges()
        {
            InitializeComponent();
            InitializeTopBar();

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

        private void InitializeTopBar()
        {
            _topBar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(58, 61, 116),
                Padding = new Padding(0)
            };

            _lblTitle = new Label
            {
                Text = "Judges",
                AutoSize = true,
                ForeColor = Color.White,
                Font = new Font("Lexend Deca", 13f, FontStyle.Bold),
                Location = new Point(18, 13)
            };

            _topBar.Controls.Add(_lblTitle);
            this.Controls.Add(_topBar);
            _topBar.BringToFront();
        }

        private async Task EnsureWebAsync()
        {
            if (_web != null) return;

            _web = new WebView2
            {
                Dock = DockStyle.Fill,
                DefaultBackgroundColor = System.Drawing.Color.Transparent
            };

            pnlMainJudges.Controls.Clear();
            pnlMainJudges.Controls.Add(_web);

            try
            {
                await _web.EnsureCoreWebView2Async();
                _web.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
                _web.CoreWebView2.Settings.AreDevToolsEnabled = false;
                _web.CoreWebView2.Settings.IsZoomControlEnabled = false;
            }
            catch (Microsoft.Web.WebView2.Core.WebView2RuntimeNotFoundException)
            {
                pnlMainJudges.Controls.Clear();
                pnlMainJudges.Controls.Add(new Label
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

                var judges = await Database.Judges
                    .Find(j => j.EventId == evt.Id)
                    .ToListAsync();

                var ordered = judges
                    .OrderBy(j => int.TryParse(j.Number, out var n) ? n : int.MaxValue)
                    .ThenBy(j => j.Number, StringComparer.OrdinalIgnoreCase)
                    .ToList();

                var baseUrl = ApiBase.Get();
                string html = BuildHtml(ordered, evt.Id, baseUrl);
                _web.NavigateToString(html);

                _lastEventId = evt.Id;
            }
            catch (Exception ex)
            {
                string err = $"<html><body style='background:transparent;color:#ccc;font-family:Consolas,monospace;padding:16px;'>Error loading judges:<br>{WebUtility.HtmlEncode(ex.Message)}</body></html>";
                _web?.NavigateToString(err);
            }
            finally
            {
                UseWaitCursor = false;
                _isLoading = false;
            }
        }

        private static string BuildHtml(List<JudgeModel> judges, string eventId, string baseUrl)
        {
            string colorHeader = "#3A3D74";
            string colorRow = "#2C2E58";
            string colorText = "#F7F6ED";
            string colorBtn = "#4E6AF2";
            string colorBtnHover = "#3957D9";
            string bg = "transparent";

            var css = $@"
html,body{{margin:0;padding:0;background:{bg};color:{colorText};font-family:'Lexend Deca',Segoe UI,Arial,sans-serif;font-size:20px;}}
.page{{padding:10px 14px 40px 14px;}}
.table{{width:100%; border-radius:2px; overflow:hidden;}}
.thead{{display:grid; grid-template-columns:160px 1fr 140px; background:{colorHeader}; font-weight:700;}}
.thead .th{{padding:8px 12px;}}
.row{{display:grid; grid-template-columns:160px 1fr 140px; background:{colorRow}; margin-top:1px; min-height:36px; align-items:center;}}
.cell{{padding:6px 12px; white-space:nowrap; overflow:hidden; text-overflow:ellipsis;}}
.no{{font-weight:600;}}
.btn-copy{{
  display:inline-block; padding:6px 10px; background:{colorBtn}; color:#fff; border:none;
  border-radius:6px; cursor:pointer; font-size:17px; font-weight:600;
}}
.btn-copy:hover{{ background:{colorBtnHover}; }}
.muted{{opacity:.85; padding:10px;}}
@media (max-width:600px) {{
  .thead,.row{{grid-template-columns:120px 1fr 120px;}}
}}
";

            var js = @"
async function copyLink(url, el){
  try{
    await navigator.clipboard.writeText(url);
    const old = el.textContent;
    el.textContent = 'Copied!';
    setTimeout(()=>{ el.textContent = old; }, 1200);
  }catch(e){
    try{
      const ta = document.createElement('textarea');
      ta.value = url; document.body.appendChild(ta); ta.select(); document.execCommand('copy'); document.body.removeChild(ta);
      const old = el.textContent;
      el.textContent = 'Copied!';
      setTimeout(()=>{ el.textContent = old; }, 1200);
    }catch(_){
      alert('Copy failed. URL:\n' + url);
    }
  }
}";

            var sb = new StringBuilder();
            sb.Append("<!doctype html><html><head><meta charset='utf-8'>");
            sb.Append("<style>").Append(css).Append("</style>");
            sb.Append("<script>").Append(js).Append("</script>");
            sb.Append("</head><body><div class='page'>");

            sb.Append("<div class='table'>");
            sb.Append("<div class='thead'>")
              .Append("<div class='th'>Judge Number</div>")
              .Append("<div class='th'>Judge Name</div>")
              .Append("<div class='th'> </div>")
              .Append("</div>");

            if (judges == null || judges.Count == 0)
            {
                sb.Append("<div class='muted'>No judges defined for this event.</div>");
            }
            else
            {
                foreach (var j in judges)
                {
                    string number = (j.Number ?? "").Trim();
                    string title = (j.Title ?? "").Trim();
                    string name = (j.Name ?? "").Trim();

                    string judgeId = j.JudgeId ?? j.Id ?? "";
                    string displayName = string.IsNullOrEmpty(title) ? name : $"{title} {name}";

                    string safeNumber = WebUtility.HtmlEncode(string.IsNullOrEmpty(number) ? "—" : number);
                    string safeName = WebUtility.HtmlEncode(string.IsNullOrWhiteSpace(displayName) ? "(Unnamed Judge)" : displayName);

                    string link = $"{baseUrl}/JudgeClient/index.html?eventId={Uri.EscapeDataString(eventId)}&judgeId={Uri.EscapeDataString(judgeId)}";

                    sb.Append("<div class='row'>")
                      .Append("<div class='cell no'>").Append(safeNumber).Append("</div>")
                      .Append("<div class='cell'>").Append(safeName).Append("</div>")
                      .Append("<div class='cell'>")
                        .Append("<button class='btn-copy' onclick=\"copyLink('").Append(link.Replace("'", "\\'")).Append("', this)\">Copy Link</button>")
                      .Append("</div>")
                      .Append("</div>");
                }
            }

            sb.Append("</div></div></body></html>");
            return sb.ToString();
        }
    }
}