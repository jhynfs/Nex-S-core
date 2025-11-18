using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Web.WebView2.WinForms;
using Microsoft.Web.WebView2.Core;
using MongoDB.Driver;
using NexScore.Models;

namespace NexScore.MainFormPages
{
    public partial class PageContestants : UserControl
    {
        private WebView2? _web;
        private bool _isLoading;
        private string? _lastEventId;
        private string? _mappedEventFolder; // track current mapping

        public PageContestants()
        {
            InitializeComponent();

            // Initialize WebView + load on page load
            this.Load += async (_, __) =>
            {
                await EnsureWebAsync();
                if (AppSession.CurrentEvent != null)
                    await LoadAndRenderAsync(AppSession.CurrentEvent);
            };

            // Re-render when visible (e.g., tab switch)
            this.VisibleChanged += async (_, __) =>
            {
                if (this.Visible && AppSession.CurrentEvent != null)
                {
                    await EnsureWebAsync();
                    await LoadAndRenderAsync(AppSession.CurrentEvent);
                }
            };

            // React to event changes
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

            pnlMainContestants.Controls.Clear();
            pnlMainContestants.Controls.Add(_web);

            try
            {
                await _web.EnsureCoreWebView2Async();
                _web.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
                _web.CoreWebView2.Settings.AreDevToolsEnabled = false;
                _web.CoreWebView2.Settings.IsZoomControlEnabled = false;
            }
            catch (WebView2RuntimeNotFoundException)
            {
                pnlMainContestants.Controls.Clear();
                pnlMainContestants.Controls.Add(new Label
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

        private static string GetPortraitsFolder(string? eventName)
        {
            // Matches SetupContestants saving location:
            // %LOCALAPPDATA%\NexScore\Portraits\<EventName>
            var baseFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "NexScore", "Portraits");
            if (string.IsNullOrWhiteSpace(eventName))
                return baseFolder;
            return Path.Combine(baseFolder, eventName);
        }

        private async Task MapEventAssetsAsync(EventModel evt)
        {
            if (_web?.CoreWebView2 == null) return;

            string folder = GetPortraitsFolder(evt.EventName);
            // Only re-map if folder changes
            if (!string.Equals(folder, _mappedEventFolder, StringComparison.OrdinalIgnoreCase))
            {
                _web.CoreWebView2.SetVirtualHostNameToFolderMapping(
                    "appassets",
                    Directory.Exists(folder) ? folder : GetPortraitsFolder(null),
                    CoreWebView2HostResourceAccessKind.Allow);
                _mappedEventFolder = folder;
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

                await MapEventAssetsAsync(evt);

                var contestants = await Database.Contestants
                    .Find(c => c.EventId == evt.Id)
                    .SortBy(c => c.Number)
                    .ToListAsync();

                string html = BuildHtml(contestants, evt);
                _web.NavigateToString(html);

                _lastEventId = evt.Id;
            }
            catch (Exception ex)
            {
                string err = $"<html><body style='background:transparent;color:#ccc;font-family:Consolas,monospace;padding:16px;'>Error loading contestants:<br>{WebUtility.HtmlEncode(ex.Message)}</body></html>";
                _web?.NavigateToString(err);
            }
            finally
            {
                UseWaitCursor = false;
                _isLoading = false;
            }
        }

        private static string BuildHtml(List<ContestantModel> contestants, EventModel evt)
        {
            string colorHeader = "#3A3D74"; // header
            string colorRow = "#2C2E58";    // row background
            string colorRowAlt = "#2A2C54"; // alt bg for advocacy
            string colorText = "#F7F6ED";
            string colorMuted = "#C8C7BD";
            string bg = "transparent";

            // Grid columns (desktop): Photo | No. | Name | Representing | Gender | Age | ▾
            // Small screens hide Age & Gender; tiny screens also hide Representing.
            var css = $@"
html,body{{margin:0;padding:0;background:{bg};color:{colorText};font-family:'Lexend Deca',Segoe UI,Arial,sans-serif;font-size:14px;}}
.page{{padding:10px 14px 40px 14px;}}

.table{{width:100%; border-radius:2px; overflow:hidden;}}
.thead{{display:grid; grid-template-columns:84px 90px 1fr 1fr 120px 80px 28px; background:{colorHeader}; font-weight:700;}}
.thead .th{{padding:8px 12px;}}
.thead .th.center{{text-align:center;}}  /* center selected header cells */
.row{{display:grid; grid-template-columns:84px 90px 1fr 1fr 120px 80px 28px; background:{colorRow}; margin-top:1px; min-height:28px; align-items:center;}}
.cell{{padding:6px 12px; white-space:nowrap; overflow:hidden; text-overflow:ellipsis;}}
.no, .gender, .age{{text-align:center;}}
.photo{{padding:6px;}}
.photo .phobox{{width:64px;height:80px;border-radius:4px;background:#1E2040;display:flex;align-items:center;justify-content:center;color:{colorMuted};font-size:11px;}}
.photo img{{width:64px;height:80px;border-radius:4px;object-fit:cover;display:block;background:#111;}}
.toggle{{text-align:center;cursor:pointer;user-select:none;color:{colorText};opacity:.9;}}
.toggle.hidden{{visibility:hidden;}}

.adv-row{{display:none; background:{colorRowAlt};}}
.adv-row.show{{display:block;}}
.adv-cell{{grid-column: 1 / -1; padding:10px 14px; color:{colorText}; line-height:1.35; white-space:pre-wrap; word-break:break-word;}}
.adv-muted{{opacity:.9; color:{colorMuted};}}

.muted{{opacity:.85; padding:10px;}}

/* Responsive tweaks */
@media (max-width: 900px) {{
  .thead, .row {{ grid-template-columns:84px 80px 1fr 1fr 100px 70px 28px; }}
}}
@media (max-width: 730px) {{
  .thead, .row {{ grid-template-columns:84px 70px 1fr 1fr 100px 0px 28px; }} /* hide Age col by giving 0px */
  .age{{ display:none; }}
}}
@media (max-width: 560px) {{
  .thead, .row {{ grid-template-columns:84px 64px 1fr 0px 100px 0px 28px; }} /* hide Representing + Age */
  .rep{{ display:none; }}
  .age{{ display:none; }}
}}
";

            // IMPORTANT: Collapsible advocacy logic (was missing if JS removed)
            var js = @"
document.addEventListener('click', function (e) {
  const t = e.target;
  if (t && t.classList.contains('toggle')) {
    const id = t.getAttribute('data-target');
    if (!id) return;
    const row = document.getElementById(id);
    if (!row) return;
    const isOpen = row.classList.toggle('show');
    t.textContent = isOpen ? '▾' : '▸';
  }
});";

            var sb = new StringBuilder();
            sb.Append("<!doctype html><html><head><meta charset='utf-8'>");
            sb.Append("<style>").Append(css).Append("</style></head><body><div class='page'>");

            sb.Append("<div class='table'>");
            sb.Append("<div class='thead'>")
              .Append("<div class='th'>Photo</div>")
              .Append("<div class='th center'>No.</div>")
              .Append("<div class='th'>Contestant Name</div>")
              .Append("<div class='th'>Representing</div>")
              .Append("<div class='th center'>Gender</div>")
              .Append("<div class='th center'>Age</div>")
              .Append("<div class='th'></div>")
              .Append("</div>");

            if (contestants == null || contestants.Count == 0)
            {
                sb.Append("<div class='muted'>No contestants defined for this event.</div>");
            }
            else
            {
                string portraitsFolder = GetPortraitsFolder(evt.EventName);
                int idx = 0;
                foreach (var c in contestants)
                {
                    string id = $"adv_{idx++}";
                    string no = c.Number.ToString() ?? "—";
                    string name = (c.FullName ?? "").Trim();
                    string rep = (c.Representing ?? "").Trim();
                    string gender = (c.Gender ?? "").Trim();
                    string age = c.Age.HasValue ? c.Age.Value.ToString() : "";
                    string advocacy = (c.Advocacy ?? "").Trim();
                    string photo = (c.PhotoPath ?? "").Trim();

                    // Photo handling: map file to https://appassets/<filename> if it lives in the portraits folder.
                    string photoHtml;
                    try
                    {
                        if (!string.IsNullOrEmpty(photo) &&
                            File.Exists(photo) &&
                            photo.StartsWith(portraitsFolder, StringComparison.OrdinalIgnoreCase))
                        {
                            var fileName = Path.GetFileName(photo);
                            var url = "https://appassets/" + Uri.EscapeDataString(fileName);
                            photoHtml = $"<img src=\"{url}\" alt=\"photo\">";
                        }
                        else
                        {
                            photoHtml = "<div class='phobox'>No photo</div>";
                        }
                    }
                    catch
                    {
                        photoHtml = "<div class='phobox'>No photo</div>";
                    }

                    bool hasAdv = !string.IsNullOrWhiteSpace(advocacy);

                    // Main row
                    sb.Append("<div class='row'>")
                      .Append("<div class='cell photo'>").Append(photoHtml).Append("</div>")
                      .Append("<div class='cell no'>").Append(WebUtility.HtmlEncode(no)).Append("</div>")
                      .Append("<div class='cell'>").Append(WebUtility.HtmlEncode(name)).Append("</div>")
                      .Append("<div class='cell rep'>").Append(WebUtility.HtmlEncode(rep)).Append("</div>")
                      .Append("<div class='cell gender'>").Append(WebUtility.HtmlEncode(gender)).Append("</div>")
                      .Append("<div class='cell age'>").Append(WebUtility.HtmlEncode(age)).Append("</div>");

                    // Chevron toggle (hidden if no advocacy)
                    if (hasAdv)
                        sb.Append("<div class='cell toggle' data-target='").Append(id).Append("'>▸</div>");
                    else
                        sb.Append("<div class='cell toggle hidden'></div>");

                    sb.Append("</div>"); // end main row

                    // Advocacy row (collapsed by default)
                    if (hasAdv)
                    {
                        sb.Append("<div class='row adv-row' id='").Append(id).Append("'>")
                          .Append("<div class='adv-cell'>")
                          .Append(WebUtility.HtmlEncode(advocacy))
                          .Append("</div></div>");
                    }
                }
            }

            sb.Append("</div>"); // table
            sb.Append("</div><script>").Append(js).Append("</script></body></html>");
            return sb.ToString();
        }
    }
}