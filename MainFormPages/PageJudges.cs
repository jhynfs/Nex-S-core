using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MongoDB.Driver;
using Microsoft.Web.WebView2.WinForms;
using Microsoft.Web.WebView2.Core;
using NexScore.Models;

namespace NexScore.MainFormPages
{
    public partial class PageJudges : UserControl
    {
        private WebView2? _web;
        private bool _isLoading;
        private string? _lastEventId;

        public PageJudges()
        {
            InitializeComponent();

            // Initialize when page loads
            this.Load += async (_, __) =>
            {
                await EnsureWebAsync();
                if (AppSession.CurrentEvent != null)
                    await LoadAndRenderAsync(AppSession.CurrentEvent);
            };

            // Re-render when page becomes visible
            this.VisibleChanged += async (_, __) =>
            {
                if (this.Visible && AppSession.CurrentEvent != null)
                {
                    await EnsureWebAsync();
                    await LoadAndRenderAsync(AppSession.CurrentEvent);
                }
            };

            // Re-render when current event changes
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

        // Ensure WebView2 exists and is initialized
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
            catch (WebView2RuntimeNotFoundException)
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

                string html = BuildHtml(ordered);
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

        private static string BuildHtml(List<JudgeModel> judges)
        {
            string colorHeader = "#3A3D74";
            string colorRow = "#2C2E58";
            string colorText = "#F7F6ED";
            string bg = "transparent";

            // If you want IP column later: change grid-template-columns to: 160px 1fr 160px and add header + cell.
            var css = $@"
html,body{{margin:0;padding:0;background:{bg};color:{colorText};font-family:'Lexend Deca',Segoe UI,Arial,sans-serif;font-size:14px;}}
.page{{padding:10px 14px 40px 14px;}}
.table{{width:100%; border-radius:2px; overflow:hidden;}}
.thead{{display:grid; grid-template-columns:160px 1fr; background:{colorHeader}; font-weight:700;}}
.thead .th{{padding:8px 12px;}}
.row{{display:grid; grid-template-columns:160px 1fr; background:{colorRow}; margin-top:1px; min-height:28px; align-items:center;}}
.cell{{padding:6px 12px; white-space:nowrap; overflow:hidden; text-overflow:ellipsis;}}
.no{{font-weight:600;}}
.muted{{opacity:.85; padding:10px;}}
@media (max-width:520px) {{
  .thead,.row{{grid-template-columns:120px 1fr;}}
}}
";

            var sb = new StringBuilder();
            sb.Append("<!doctype html><html><head><meta charset='utf-8'>");
            sb.Append("<style>").Append(css).Append("</style></head><body><div class='page'>");

            sb.Append("<div class='table'>");
            sb.Append("<div class='thead'>")
              .Append("<div class='th'>Judge Number</div>")
              .Append("<div class='th'>Judge Name</div>")
              // Uncomment for IP column:
              // .Append("<div class='th'>IP Address</div>")
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
                    string ip = (j.IPAddress ?? "").Trim();

                    // Judge Name = title + space + name if title exists
                    string displayName = string.IsNullOrEmpty(title) ? name : $"{title} {name}";

                    string safeNumber = WebUtility.HtmlEncode(string.IsNullOrEmpty(number) ? "—" : number);
                    string safeName = WebUtility.HtmlEncode(string.IsNullOrWhiteSpace(displayName) ? "(Unnamed Judge)" : displayName);
                    string safeIp = WebUtility.HtmlEncode(string.IsNullOrWhiteSpace(ip) ? "" : ip);

                    sb.Append("<div class='row'>")
                      .Append("<div class='cell no'>").Append(safeNumber).Append("</div>")
                      .Append("<div class='cell'>").Append(safeName).Append("</div>")
                      // Uncomment for IP column:
                      // .Append("<div class='cell'>").Append(safeIp).Append("</div>")
                      .Append("</div>");
                }
            }

            sb.Append("</div></div></body></html>");
            return sb.ToString();
        }
    }
}