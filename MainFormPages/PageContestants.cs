using System;
using System.Collections.Generic;
using System.Drawing; // Added
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
        // --- UI Fields ---
        private Panel _topBar;
        private Label _lblTitle;
        // -----------------

        private WebView2? _web;
        private bool _isLoading;
        private string? _mappedEventFolder;

        public PageContestants()
        {
            InitializeComponent();

            // --- Init Top Bar ---
            InitializeTopBar();
            // --------------------

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
                Text = "Contestants",
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
            return NexScore.Helpers.PathHelpers.ContestantPhotosFolder;
        }

        private async Task MapEventAssetsAsync(EventModel evt)
        {
            if (_web?.CoreWebView2 == null) return;
            string folder = GetPortraitsFolder(evt.EventName);
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            if (!string.Equals(folder, _mappedEventFolder, StringComparison.OrdinalIgnoreCase))
            {
                _web.CoreWebView2.SetVirtualHostNameToFolderMapping(
            "appassets",
            folder,
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
            string colorHeader = "#3A3D74";
            string colorRow = "#2C2E58";
            string colorRowAlt = "#2A2C54";
            string colorText = "#F7F6ED";
            string colorMuted = "#C8C7BD";
            string bg = "transparent";

            var css = $@"
html,body{{margin:0;padding:0;background:{bg};color:{colorText};font-family:'Lexend Deca',Segoe UI,Arial,sans-serif;font-size:20px;}}
.page{{padding:10px 14px 40px 14px;}}
.actions{{margin-bottom:12px;display:flex;gap:10px;flex-wrap:wrap;align-items:center;}}
.filter-select{{padding:6px 10px;border-radius:6px;background:#272a4d;color:{colorText};border:1px solid #444;font-family:'Lexend Deca';}}
.note{{font-size:16px;opacity:.8;margin-top:4px;}}
.table{{width:100%;border-radius:2px;overflow:hidden;}}
.thead{{display:grid;grid-template-columns:84px 90px 1fr 1fr 120px 80px 140px;background:{colorHeader};font-weight:700;}}
.thead .th{{padding:8px 12px;}}
.thead .th.center{{text-align:center;}}
.row{{display:grid;grid-template-columns:84px 90px 1fr 1fr 120px 80px 140px;background:{colorRow};margin-top:1px;min-height:32px;align-items:center;transition:.25s;}}
.row.eliminated{{opacity:.55;filter:grayscale(.4);}}
.cell{{padding:6px 12px;white-space:nowrap;overflow:hidden;text-overflow:ellipsis;}}
.no,.gender,.age,.status{{text-align:center;}}
.photo{{padding:6px;}}
.photo .phobox{{width:64px;height:80px;border-radius:4px;background:#1E2040;display:flex;align-items:center;justify-content:center;color:{colorMuted};font-size:16px;}}
.photo img{{width:64px;height:80px;border-radius:4px;object-fit:cover;display:block;background:#111;}}
.muted{{opacity:.85;padding:10px;}}
@media (max-width:900px){{.thead,.row{{grid-template-columns:84px 80px 1fr 1fr 110px 70px 130px;}}}}
@media (max-width:730px){{.thead,.row{{grid-template-columns:84px 70px 1fr 1fr 110px 0px 130px;}} .age{{display:none;}}}}
@media (max-width:560px){{.thead,.row{{grid-template-columns:84px 64px 1fr 0px 90px 0px 120px;}} .rep{{display:none;}} .age{{display:none;}}}}
";

            var portraitsFolder = GetPortraitsFolder(evt.EventName);
            var sb = new StringBuilder();
            sb.Append("<!doctype html><html><head><meta charset='utf-8'><style>")
              .Append(css)
              .Append("</style></head><body><div class='page'>");

            sb.Append("<div class='actions'>")
              .Append("<label style='font-weight:600;'>Filter:</label>")
              .Append("<select class='filter-select' id='flt'>")
              .Append("<option value='all'>All</option>")
              .Append("<option value='active'>Active Only</option>")
              .Append("<option value='elim'>Eliminated Only</option>")
              .Append("</select>")
              .Append("<div class='note'>Use the Results page to eliminate / restore contestants. This list always loads all.</div>")
              .Append("</div>");

            sb.Append("<div class='table'>");
            sb.Append("<div class='thead'>")
              .Append("<div class='th'>Photo</div>")
              .Append("<div class='th center'>No.</div>")
              .Append("<div class='th'>Contestant Name</div>")
              .Append("<div class='th'>Representing</div>")
              .Append("<div class='th center'>Gender</div>")
              .Append("<div class='th center'>Age</div>")
              .Append("<div class='th center'>Status</div>")
              .Append("</div>");

            if (contestants.Count == 0)
            {
                sb.Append("<div class='muted'>No contestants defined.</div>");
            }
            else
            {
                foreach (var c in contestants)
                {
                    string no = c.Number.ToString();
                    string name = (c.FullName ?? "").Trim();
                    string rep = (c.Representing ?? "").Trim();
                    string gender = (c.Gender ?? "").Trim();
                    string age = c.Age.HasValue ? c.Age.Value.ToString() : "";
                    bool active = c.IsActive;
                    string photo = (c.PhotoPath ?? "").Trim();
                    string fileName = Path.GetFileName(photo);
                    string expectedPath = !string.IsNullOrEmpty(fileName)
        ? Path.Combine(portraitsFolder, fileName)
        : "";
                    string photoHtml;
                    try
                    {
                        if (!string.IsNullOrEmpty(fileName) && File.Exists(expectedPath))
                        {
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

                    sb.Append("<div class='row")
                      .Append(active ? "" : " eliminated")
                      .Append("' data-active='").Append(active ? "1" : "0").Append("'>");

                    sb.Append("<div class='cell photo'>").Append(photoHtml).Append("</div>");
                    sb.Append("<div class='cell no'>").Append(WebUtility.HtmlEncode(no)).Append("</div>");
                    sb.Append("<div class='cell name'>").Append(WebUtility.HtmlEncode(name)).Append("</div>");
                    sb.Append("<div class='cell rep'>").Append(WebUtility.HtmlEncode(rep)).Append("</div>");
                    sb.Append("<div class='cell gender'>").Append(WebUtility.HtmlEncode(gender)).Append("</div>");
                    sb.Append("<div class='cell age'>").Append(WebUtility.HtmlEncode(age)).Append("</div>");
                    sb.Append("<div class='cell status'>").Append(active ? "Active" : "Eliminated").Append("</div>");
                    sb.Append("</div>");
                }
            }

            sb.Append("</div>");

            sb.Append(@"
<script>
(function(){
  const flt = document.getElementById('flt');
  function applyFilter(){
    const val = flt.value;
    document.querySelectorAll('.row').forEach(r=>{
      const a = r.getAttribute('data-active');
      if(val==='all'){ r.style.display='grid'; }
      else if(val==='active'){ r.style.display = a==='1' ? 'grid' : 'none'; }
      else if(val==='elim'){ r.style.display = a==='0' ? 'grid' : 'none'; }
    });
  }
  flt.addEventListener('change', applyFilter);
  applyFilter();
})();
</script>");

            sb.Append("</div></body></html>");
            return sb.ToString();
        }
    }
}