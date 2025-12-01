using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Web.WebView2.WinForms;
using MongoDB.Driver;
using NexScore.Models;

namespace NexScore.MainFormPages
{
    public partial class PageResults : UserControl
    {
        private WebView2? _web;
        private bool _loading;
        private System.Windows.Forms.Timer? _refreshTimer;

        private Panel? _topBar;
        private Label? _lblTitle;
        private Label? _lblEvent;
        private Button? _btnRefresh;
        private Button? _btnPrint;
        private ComboBox? _cboFilter;
        private Label? _lblUpdated;

        private List<string>? _lastPhaseKeys;
        private Dictionary<string, string>? _lastPhaseNames;

        private string _filterMode = "active";

        public PageResults()
        {
            InitializeComponent();

            this.Load += async (_, __) =>
            {
                EnsureTopBar();
                await EnsureWebAsync();
                if (AppSession.CurrentEvent != null)
                    await RenderAsync(AppSession.CurrentEvent.Id);
                StartAutoRefresh();
            };

            this.VisibleChanged += async (_, __) =>
            {
                if (this.Visible && AppSession.CurrentEvent != null)
                {
                    await EnsureWebAsync();
                    await RenderAsync(AppSession.CurrentEvent.Id);
                    StartAutoRefresh();
                }
                else
                {
                    StopAutoRefresh();
                }
            };

            AppSession.CurrentEventChanged += async evt =>
            {
                if (!IsHandleCreated || IsDisposed) return;
                if (InvokeRequired)
                    BeginInvoke(new Action(async () =>
                    {
                        await EnsureWebAsync();
                        await RenderAsync(evt.Id);
                    }));
                else
                {
                    await EnsureWebAsync();
                    await RenderAsync(evt.Id);
                }
            };
        }

        private void EnsureTopBar()
        {
            if (_topBar != null) return;

            _topBar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(58, 61, 116)
            };

            _lblTitle = new Label
            {
                Text = "Results",
                AutoSize = true,
                ForeColor = Color.White,
                Font = new Font("Lexend Deca", 13f, FontStyle.Bold),
                Location = new Point(18, 13)
            };

            _lblEvent = new Label
            {
                Text = "Event: —",
                AutoSize = true,
                ForeColor = Color.Gainsboro,
                Font = new Font("Lexend Deca", 11f),
                Location = new Point(110, 19)
            };

            _btnRefresh = new Button
            {
                Text = "Refresh",
                Width = 95,
                Height = 38,
                BackColor = Color.FromArgb(78, 106, 242),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Lexend Deca", 11, FontStyle.Bold),
                Location = new Point(380, 11),
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };
            _btnRefresh.FlatAppearance.BorderSize = 0;
            _btnRefresh.Click += async (_, __) =>
            {
                if (AppSession.CurrentEvent != null)
                    await RenderAsync(AppSession.CurrentEvent.Id);
            };

            _btnPrint = new Button
            {
                Text = "Print Results",
                Width = 125,
                Height = 38,
                BackColor = Color.FromArgb(78, 106, 242),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Lexend Deca", 11, FontStyle.Bold),
                Location = new Point(485, 11),
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };
            _btnPrint.FlatAppearance.BorderSize = 0;
            _btnPrint.Click += async (_, __) =>
            {
                try
                {
                    await PrintActiveResultsAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Print failed: " + ex.Message, "Print", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            _cboFilter = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(640, 16),
                Width = 160,
                Font = new Font("Lexend Deca", 11, FontStyle.Regular)
            };
            _cboFilter.Items.AddRange(new object[]
            {
                "Active Only",
                "Eliminated Only",
                "All Contestants"
            });
            _cboFilter.SelectedIndex = 0;
            _cboFilter.SelectedIndexChanged += async (_, __) =>
            {
                _filterMode = _cboFilter.SelectedIndex switch
                {
                    0 => "active",
                    1 => "elim",
                    2 => "all",
                    _ => "active"
                };
                if (AppSession.CurrentEvent != null)
                    await RenderAsync(AppSession.CurrentEvent.Id);
            };

            _lblUpdated = new Label
            {
                Text = "Updated: —",
                AutoSize = true,
                ForeColor = Color.Gainsboro,
                Location = new Point(830, 20),
                Font = new Font("Lexend Deca", 10, FontStyle.Regular)
            };

            _topBar.Controls.AddRange(new Control[] {
                _lblTitle!, _lblEvent!, _btnRefresh!, _btnPrint!, _cboFilter!, _lblUpdated!
            });

            this.Controls.Add(_topBar);
            _topBar.BringToFront();
        }

        private void WebView_WebMessageReceived(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs e)
        {
            var msg = e.TryGetWebMessageAsString();
            if (msg == "print-complete")
            {
                StartAutoRefresh();
            }
        }

        private async Task EnsureWebAsync()
        {
            if (_web != null) return;

            _web = new WebView2
            {
                Dock = DockStyle.Fill,
                DefaultBackgroundColor = Color.Transparent
            };

            pnlMainResults.Controls.Clear();
            pnlMainResults.Controls.Add(_web);

            try
            {
                await _web.EnsureCoreWebView2Async();
                var s = _web.CoreWebView2.Settings;
                s.AreDefaultContextMenusEnabled = false;
                s.AreDevToolsEnabled = false;
                s.IsZoomControlEnabled = false;

                _web.CoreWebView2.WebMessageReceived -= WebView_WebMessageReceived;
                _web.CoreWebView2.WebMessageReceived += WebView_WebMessageReceived;
            }
            catch (Microsoft.Web.WebView2.Core.WebView2RuntimeNotFoundException)
            {
                pnlMainResults.Controls.Clear();
                pnlMainResults.Controls.Add(new Label
                {
                    Text = "WebView2 runtime is not installed.\nInstall Microsoft Edge WebView2 Runtime.",
                    ForeColor = Color.LightGray,
                    Font = new Font("Lexend Deca", 10f),
                    AutoSize = true,
                    Padding = new Padding(12),
                    Location = new Point(10, 10)
                });
            }
        }

        private async Task PrintActiveResultsAsync()
        {
            if (_web?.CoreWebView2 == null)
            {
                MessageBox.Show("Web view is not ready.", "Print", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            StopAutoRefresh();

            string js = @"
    (function(){
        // Black/white print CSS: only the .view.active is shown
        var style = document.getElementById('results-print-style');
        if(!style){
            style = document.createElement('style');
            style.id = 'results-print-style';
            style.innerHTML = `
                @media print {
                    .view:not(.active){ display:none!important; }
                    body, html{background:#fff !important; color:#000 !important;}
                    .table, .thead, .row, .cell, .filters, .pill, .breakdown, .muted, .btn, .status { 
                        background: transparent !important; 
                        color: #000 !important; 
                        border-color: #000 !important; 
                        box-shadow:none !important;
                        filter:none !important;
                    }
                    .elim, .name-del{ color:#888 !important; text-decoration:line-through !important;}
                    .btn, .btn:hover, .btn:active{ background: #fff!important; color:#000!important; border: 1px solid #000!important;}
                    .nav, .filters, .note, .btn, .muted { display:none!important; }
                }
            `;
            document.head.appendChild(style);
        }
        window.onafterprint = function() {
            if(window.chrome && window.chrome.webview)
                window.chrome.webview.postMessage('print-complete');
        };
    })();
    ";
            await _web.CoreWebView2.ExecuteScriptAsync(js);

            await Task.Delay(300);
            await _web.CoreWebView2.ExecuteScriptAsync("window.print();");
        }

        private async Task RenderAsync(string eventId)
        {
            if (_web?.CoreWebView2 == null) return;
            if (_loading) return;
            _loading = true;

            try
            {
                UseWaitCursor = true;

                var evt = await Database.Events.Find(e => e.Id == eventId).FirstOrDefaultAsync();
                _lblEvent!.Text = "Event: " + (evt?.EventName ?? evt?.EventId ?? eventId);

                var aggregated = await Database.AggregatedScores
                    .Find(a => a.EventId == eventId)
                    .ToListAsync();

                var structure = await Database.EventStructures
                    .Find(s => s.EventId == eventId)
                    .FirstOrDefaultAsync();

                var contestants = await Database.Contestants
                    .Find(c => c.EventId == eventId)
                    .ToListAsync();
                var contestantsMap = contestants.ToDictionary(c => c.Id, c => c);

                var rawScores = await Database.Scores
                    .Find(sc => sc.EventId == eventId)
                    .ToListAsync();

                var finalAgg = (aggregated.Count == 0)
                    ? ComputeAggregatesClientSide(eventId, rawScores, structure)
                    : aggregated;

                var phaseNames = BuildPhaseNames(structure);
                var phaseWeightsFraction = BuildPhaseWeightFractions(structure);

                var rowsAll = finalAgg.Select(a =>
                {
                    contestantsMap.TryGetValue(a.ContestantId, out var c);

                    var rawPhasePerf = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
                    foreach (var kv in a.PhaseScores)
                    {
                        if (phaseWeightsFraction.TryGetValue(kv.Key, out var wFrac) && wFrac > 0)
                            rawPhasePerf[kv.Key] = kv.Value / wFrac;
                        else
                            rawPhasePerf[kv.Key] = kv.Value;
                    }
                    foreach (var kv in a.IndependentPhaseScores)
                        rawPhasePerf[kv.Key] = kv.Value;

                    return new ResultRow
                    {
                        ContestantId = a.ContestantId,
                        Number = c?.Number ?? 0,
                        Name = c?.FullName ?? "",
                        IsActive = c?.IsActive != false,
                        EventScore = a.EventScore,
                        Rank = a.Rank,
                        TieBreak = a.TieBreakInfo ?? "",
                        PhaseScoresWeighted = a.PhaseScores,
                        IndependentScores = a.IndependentPhaseScores,
                        RawPhasePerf = rawPhasePerf
                    };
                }).ToList();

                var rowsFiltered = _filterMode switch
                {
                    "active" => rowsAll.Where(r => r.IsActive).ToList(),
                    "elim" => rowsAll.Where(r => !r.IsActive).ToList(),
                    _ => rowsAll
                };

                var orderedPhases = BuildPhaseOrder(structure, rowsFiltered);

                _lastPhaseNames = phaseNames;
                _lastPhaseKeys = orderedPhases.ToList();

                var html = BuildHtml(rowsFiltered, rowsAll, phaseNames, orderedPhases, eventId);
                _web.NavigateToString(html);

                _lblUpdated!.Text = "Updated: " + DateTime.Now.ToString("HH:mm:ss");
            }
            catch (Exception ex)
            {
                _web?.NavigateToString("<html><body style='font-family:Consolas;color:#ccc;padding:10px;'>Error:<br>" +
                    WebUtility.HtmlEncode(ex.Message) + "</body></html>");
            }
            finally
            {
                UseWaitCursor = false;
                _loading = false;
            }
        }

        private List<AggregatedScore> ComputeAggregatesClientSide(
            string eventId,
            List<ScoreEntry> rawScores,
            EventStructureModel? structure)
        {
            var phaseMeta = new Dictionary<string, (bool independent, decimal weight)>(StringComparer.OrdinalIgnoreCase);
            if (structure?.Phases != null)
            {
                foreach (var p in structure.Phases)
                {
                    string key = p.Sequence.HasValue ? "P" + p.Sequence.Value : "IND-" + SanitizePhaseName(p.Name);
                    phaseMeta[key] = (p.IsIndependent, p.Weight);
                }
            }

            var byContestantPhase = rawScores
                .GroupBy(s => s.ContestantId)
                .ToDictionary(
                    g => g.Key,
                    g => g.GroupBy(s => s.PhaseId)
                          .ToDictionary(
                              pg => pg.Key,
                              pg =>
                              {
                                  var fracs = pg.Select(sc =>
                                  {
                                      double max = sc.MaxValue > 0 ? sc.MaxValue : 100;
                                      double f = sc.RawValue / max;
                                      if (f < 0) f = 0;
                                      if (f > 1) f = 1;
                                      return f;
                                  }).ToList();
                                  return fracs.Count == 0 ? 0.0 : fracs.Average();
                              }
                          ),
                    StringComparer.OrdinalIgnoreCase
                );

            var aggregates = new List<AggregatedScore>();

            foreach (var cEntry in byContestantPhase)
            {
                string contestantId = cEntry.Key;
                var phaseScoresRaw = cEntry.Value;

                var mainPhaseWeightedFractions = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
                var independentPhaseFractions = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);

                double eventScore = 0.0;

                foreach (var kv in phaseScoresRaw)
                {
                    var phaseId = kv.Key;
                    var rawFraction = kv.Value;

                    if (rawFraction < 0) rawFraction = 0;
                    if (rawFraction > 1) rawFraction = 1;

                    if (phaseMeta.TryGetValue(phaseId, out var meta))
                    {
                        if (meta.independent)
                        {
                            independentPhaseFractions[phaseId] = rawFraction;
                            eventScore += rawFraction; 
                        }
                        else
                        {
                            double weightFrac = (double)(meta.weight / 100m);
                            double weightedValue = rawFraction * weightFrac;
                            mainPhaseWeightedFractions[phaseId] = weightedValue;
                            eventScore += weightedValue;
                        }
                    }
                    else
                    {
                        independentPhaseFractions[phaseId] = rawFraction;
                        eventScore += rawFraction;
                    }
                }

                var agg = new AggregatedScore
                {
                    EventId = eventId,
                    ContestantId = contestantId,
                    PhaseScores = mainPhaseWeightedFractions,
                    IndependentPhaseScores = independentPhaseFractions,
                    EventScore = eventScore,
                    ComputedAt = DateTime.UtcNow
                };

                aggregates.Add(agg);
            }

            int rank = 1;
            foreach (var a in aggregates.OrderByDescending(a => a.EventScore))
                a.Rank = rank++;

            return aggregates;
        }

        private static Dictionary<string, string> BuildPhaseNames(EventStructureModel? s)
        {
            var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (s?.Phases == null) return map;
            foreach (var p in s.Phases)
            {
                string key = p.Sequence.HasValue ? "P" + p.Sequence.Value : "IND-" + SanitizePhaseName(p.Name);
                string label = p.Sequence.HasValue ? $"Phase {p.Sequence}: {p.Name}" : $"Independent: {p.Name}";
                map[key] = label;
            }
            return map;
        }

        private static Dictionary<string, double> BuildPhaseWeightFractions(EventStructureModel? s)
        {
            var map = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
            if (s?.Phases == null) return map;
            foreach (var p in s.Phases)
            {
                if (p.IsIndependent) continue;
                string key = p.Sequence.HasValue ? "P" + p.Sequence.Value : "IND-" + SanitizePhaseName(p.Name);
                double frac = (double)(p.Weight / 100m);
                map[key] = frac;
            }
            return map;
        }

        private static List<string> BuildPhaseOrder(EventStructureModel? s, List<ResultRow> rowsFiltered)
        {
            var list = new List<string>();
            if (s?.Phases != null)
            {
                foreach (var p in s.Phases.OrderBy(p => p.Sequence ?? int.MaxValue))
                {
                    string key = p.Sequence.HasValue ? "P" + p.Sequence.Value : "IND-" + SanitizePhaseName(p.Name);
                    list.Add(key);
                }
            }
            else
            {
                list = rowsFiltered
                    .SelectMany(r => r.RawPhasePerf.Keys)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(k => k, StringComparer.OrdinalIgnoreCase)
                    .ToList();
            }
            return list;
        }

        private static string SanitizePhaseName(string? name) =>
            string.IsNullOrWhiteSpace(name) ? "phase" : name.Trim().Replace(" ", "_").ToLowerInvariant();

        private static string BuildHtml(
            List<ResultRow> rowsFiltered,
            List<ResultRow> rowsAll,
            Dictionary<string, string> phaseNames,
            List<string> phaseKeys,
            string eventId)
        {
            var displayRanks = ComputeDisplayRanks(rowsAll);

            string FilterPills()
            {
                var sb = new StringBuilder();
                sb.Append("<div class='filters' id='filters'>");
                sb.Append("<div class='pill active' data-target='overall'>Overall</div>");
                foreach (var key in phaseKeys)
                {
                    sb.Append("<div class='pill' data-target='").Append(WebUtility.HtmlEncode(key)).Append("'>")
                      .Append(WebUtility.HtmlEncode(phaseNames.TryGetValue(key, out var nm) ? nm : key))
                      .Append("</div>");
                }
                sb.Append("</div>");
                return sb.ToString();
            }

            var css = @"
html,body{margin:0;padding:0;background:transparent;color:#F7F6ED;font-family:'Lexend Deca',Segoe UI,Arial,sans-serif;font-size:20px;}
.page{padding:12px 16px 70px 16px;}
.filters{display:flex;gap:10px;flex-wrap:wrap;margin:0 0 10px 0;}
.pill{background:#2C2E58;border:1px solid #4c4f8a;color:#fff;border-radius:999px;padding:6px 12px;cursor:pointer;font-weight:600;font-size:18px;user-select:none;}
.pill.active{background:#4E6AF2;border-color:#4E6AF2;}
.nav{display:flex;align-items:center;gap:14px;margin-bottom:10px;}
.nav .arrow{cursor:pointer;background:#3A3D74;border-radius:6px;padding:8px 12px;user-select:none;font-weight:600;}
.nav .arrow:hover{background:#4A4D8E;}
.nav .label{font-weight:600;letter-spacing:.4px;}
.table{width:100%;border-radius:4px;overflow:hidden;margin-top:8px;}
.thead{display:grid;grid-template-columns:80px 80px 1fr 140px 110px 120px 36px;background:#3A3D74;font-weight:700;}
.th{padding:8px 12px;}
.th.center{text-align:center;}
.row{display:grid;grid-template-columns:80px 80px 1fr 140px 110px 120px 36px;background:#2C2E58;margin-top:1px;min-height:34px;align-items:center;font-size:19px;}
.cell{padding:6px 12px;white-space:nowrap;overflow:hidden;text-overflow:ellipsis;}
.rank,.no,.score,.status{text-align:center;}
.toggle{text-align:center;cursor:pointer;user-select:none;opacity:.9;font-size:22px;line-height:1;}
.toggle.hidden{visibility:hidden;}
.breakdown{display:none;background:#2A2C54;}
.breakdown.show{display:block;}
.inner{padding:10px 16px;}
.kv{display:grid;grid-template-columns:1fr auto;gap:6px;margin:2px 0;font-size:18px;}
.kv .k{color:#C8C7BD;}
.kv .v{text-align:right;font-variant-numeric:tabular-nums;}
.muted{opacity:.85;padding:10px;font-size:15px;}
.view{display:none;} .view.active{display:block;}
.elim{opacity:.55;filter:grayscale(.4);}
.name-del{text-decoration:line-through;}
.btn{background:#4E6AF2;color:#fff;border:none;border-radius:6px;cursor:pointer;font-weight:600;font-size:16px;padding:6px 10px;}
.btn:hover{background:#3957D9;}
.note{font-size:17px;opacity:.85;margin-bottom:6px;}
";

            var sb = new StringBuilder();
            sb.Append("<!doctype html><html><head><meta charset='utf-8'><style>")
              .Append(css)
              .Append("</style></head><body><div class='page'>");

            sb.Append(FilterPills());
            sb.Append("<div class='note'>Contestant filter applied in top bar (Active / Eliminated / All).</div>");

            // Navigation arrows
            sb.Append("<div class='nav'><div class='arrow' id='navPrev'>&#9664;</div><div class='label' id='navLabel'></div><div class='arrow' id='navNext'>&#9654;</div></div>");

            // Overall view
            sb.Append("<div class='view active' data-view='overall'>");
            sb.Append("<div class='table'><div class='thead'>");
            sb.Append("<div class='th center'>Rank</div><div class='th center'>No.</div><div class='th'>Contestant</div>");
            sb.Append("<div class='th center'>Event %</div><div class='th center'>Status</div><div class='th center'>Action</div><div class='th'></div></div>");

            var orderedOverall = rowsFiltered
                .OrderBy(r => displayRanks.TryGetValue(r.ContestantId, out var rr) ? rr : int.MaxValue)
                .ThenByDescending(r => r.EventScore)
                .ThenBy(r => r.Number)
                .ThenBy(r => r.Name, StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (orderedOverall.Count == 0)
            {
                sb.Append("<div class='muted'>No scores available for this filter.</div>");
            }
            else
            {
                int idxO = 0;
                foreach (var r in orderedOverall)
                {
                    bool hasBreak = r.PhaseScoresWeighted.Count > 0 || r.IndependentScores.Count > 0 || r.RawPhasePerf.Count > 0;
                    string bdId = "bd_overall_" + (idxO++);
                    int rankToShow = displayRanks.TryGetValue(r.ContestantId, out var rk) && rk < int.MaxValue ? rk : 0;

                    sb.Append("<div class='row").Append(r.IsActive ? "" : " elim")
                      .Append("' id='row_").Append(r.ContestantId).Append("'>")
                      .Append("<div class='cell rank'>").Append(rankToShow > 0 ? rankToShow.ToString() : "—").Append("</div>")
                      .Append("<div class='cell no'>").Append(r.Number == 0 ? "—" : r.Number.ToString()).Append("</div>")
                      .Append("<div class='cell ").Append(r.IsActive ? "" : "name-del").Append("'>").Append(WebUtility.HtmlEncode(r.Name)).Append("</div>")
                      .Append("<div class='cell score'>").Append((r.EventScore * 100).ToString("0.000")).Append("</div>")
                      .Append("<div class='cell status'>").Append(r.IsActive ? "Active" : "Eliminated").Append("</div>")
                      .Append("<div class='cell action'><button class='btn' data-action='toggle' data-id='").Append(r.ContestantId).Append("'>")
                      .Append(r.IsActive ? "Eliminate" : "Restore").Append("</button></div>")
                      .Append("<div class='cell toggle ").Append(hasBreak ? "" : "hidden").Append("' data-target='").Append(bdId).Append("'>")
                      .Append(hasBreak ? "▸" : "").Append("</div></div>");

                    if (hasBreak)
                    {
                        sb.Append("<div class='breakdown' id='").Append(bdId).Append("'><div class='inner'>");
                        if (r.PhaseScoresWeighted.Count > 0)
                        {
                            sb.Append("<div class='kv'><div class='k'>Main Phases (weighted)</div><div class='v'></div></div>");
                            foreach (var kv in r.PhaseScoresWeighted.OrderBy(k => k.Key, StringComparer.OrdinalIgnoreCase))
                            {
                                sb.Append("<div class='kv'><div class='k'>")
                                  .Append(WebUtility.HtmlEncode(phaseNames.TryGetValue(kv.Key, out var pn) ? pn : kv.Key))
                                  .Append("</div><div class='v'>").Append(kv.Value.ToString("0.000")).Append("</div></div>");
                            }
                        }
                        if (r.IndependentScores.Count > 0)
                        {
                            sb.Append("<div class='kv' style='margin-top:8px'><div class='k'>Independent Phases</div><div class='v'></div></div>");
                            foreach (var kv in r.IndependentScores.OrderBy(k => k.Key, StringComparer.OrdinalIgnoreCase))
                            {
                                sb.Append("<div class='kv'><div class='k'>")
                                  .Append(WebUtility.HtmlEncode(phaseNames.TryGetValue(kv.Key, out var pn2) ? pn2 : kv.Key))
                                  .Append("</div><div class='v'>").Append(kv.Value.ToString("0.000")).Append("</div></div>");
                            }
                        }
                        if (r.RawPhasePerf.Count > 0)
                        {
                            sb.Append("<div class='kv' style='margin-top:8px'><div class='k'>Per-phase % (raw)</div><div class='v'></div></div>");
                            foreach (var kv in r.RawPhasePerf.OrderBy(k => k.Key, StringComparer.OrdinalIgnoreCase))
                            {
                                sb.Append("<div class='kv'><div class='k'>")
                                  .Append(WebUtility.HtmlEncode(phaseNames.TryGetValue(kv.Key, out var pn3) ? pn3 : kv.Key))
                                  .Append("</div><div class='v'>").Append((kv.Value * 100).ToString("0.000")).Append("</div></div>");
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(r.TieBreak))
                        {
                            sb.Append("<div class='kv' style='margin-top:8px'><div class='k'>Tie-break</div><div class='v'>")
                              .Append(WebUtility.HtmlEncode(r.TieBreak)).Append("</div></div>");
                        }
                        sb.Append("</div></div>");
                    }
                }
            }
            sb.Append("</div></div>"); // end overall

            // Phase views
            foreach (var key in phaseKeys)
            {
                var ranked = rowsFiltered
                    .Select(r => new { Row = r, Score = r.RawPhasePerf.TryGetValue(key, out var v) ? v : double.NaN })
                    .OrderByDescending(x => x.Score)
                    .ThenBy(x => x.Row.Number)
                    .ThenBy(x => x.Row.Name, StringComparer.OrdinalIgnoreCase)
                    .ToList();

                sb.Append("<div class='view' data-view='").Append(WebUtility.HtmlEncode(key)).Append("'>");
                sb.Append("<div class='table'><div class='thead'>");
                sb.Append("<div class='th center'>Rank</div><div class='th center'>No.</div><div class='th'>Contestant</div>");
                sb.Append("<div class='th center'>").Append(WebUtility.HtmlEncode((phaseNames.TryGetValue(key, out var nm) ? nm : key) + " %")).Append("</div>");
                sb.Append("<div class='th center'>Status</div><div class='th center'>Action</div><div class='th'></div></div>");

                if (ranked.Count == 0)
                {
                    sb.Append("<div class='muted'>No scores available for this filter.</div>");
                }
                else
                {
                    int localRank = 1;
                    int bdIdx = 0;
                    foreach (var item in ranked)
                    {
                        var r = item.Row;
                        bool hasBreak = r.PhaseScoresWeighted.Count > 0 || r.IndependentScores.Count > 0 || r.RawPhasePerf.Count > 0;
                        string bdId = "bd_" + key + "_" + (bdIdx++);
                        bool hasScore = !double.IsNaN(item.Score);

                        sb.Append("<div class='row").Append(r.IsActive ? "" : " elim")
                          .Append("' id='row_").Append(r.ContestantId).Append("'>")
                          .Append("<div class='cell rank'>").Append(hasScore ? (localRank++).ToString() : "—").Append("</div>")
                          .Append("<div class='cell no'>").Append(r.Number == 0 ? "—" : r.Number.ToString()).Append("</div>")
                          .Append("<div class='cell ").Append(r.IsActive ? "" : "name-del").Append("'>").Append(WebUtility.HtmlEncode(r.Name)).Append("</div>")
                          .Append("<div class='cell score'>").Append(hasScore ? (item.Score * 100).ToString("0.000") : "—").Append("</div>")
                          .Append("<div class='cell status'>").Append(r.IsActive ? "Active" : "Eliminated").Append("</div>")
                          .Append("<div class='cell action'><button class='btn' data-action='toggle' data-id='").Append(r.ContestantId).Append("'>")
                          .Append(r.IsActive ? "Eliminate" : "Restore").Append("</button></div>")
                          .Append("<div class='cell toggle ").Append(hasBreak ? "" : "hidden").Append("' data-target='").Append(bdId).Append("'>")
                          .Append(hasBreak ? "▸" : "").Append("</div></div>");

                        if (hasBreak)
                        {
                            sb.Append("<div class='breakdown' id='").Append(bdId).Append("'><div class='inner'>");
                            if (r.PhaseScoresWeighted.Count > 0)
                            {
                                sb.Append("<div class='kv'><div class='k'>Main Phases (weighted)</div><div class='v'></div></div>");
                                foreach (var kv in r.PhaseScoresWeighted.OrderBy(k => k.Key, StringComparer.OrdinalIgnoreCase))
                                {
                                    sb.Append("<div class='kv'><div class='k'>")
                                      .Append(WebUtility.HtmlEncode(phaseNames.TryGetValue(kv.Key, out var pn) ? pn : kv.Key))
                                      .Append("</div><div class='v'>").Append(kv.Value.ToString("0.000")).Append("</div></div>");
                                }
                            }
                            if (r.IndependentScores.Count > 0)
                            {
                                sb.Append("<div class='kv' style='margin-top:8px'><div class='k'>Independent Phases</div><div class='v'></div></div>");
                                foreach (var kv in r.IndependentScores.OrderBy(k => k.Key, StringComparer.OrdinalIgnoreCase))
                                {
                                    sb.Append("<div class='kv'><div class='k'>").Append(WebUtility.HtmlEncode(phaseNames.TryGetValue(kv.Key, out var pn2) ? pn2 : kv.Key))
                                      .Append("</div><div class='v'>").Append(kv.Value.ToString("0.000")).Append("</div></div>");
                                }
                            }
                            if (r.RawPhasePerf.Count > 0)
                            {
                                sb.Append("<div class='kv' style='margin-top:8px'><div class='k'>Per-phase % (raw)</div><div class='v'></div></div>");
                                foreach (var kv in r.RawPhasePerf.OrderBy(k => k.Key, StringComparer.OrdinalIgnoreCase))
                                {
                                    sb.Append("<div class='kv'><div class='k'>")
                                      .Append(WebUtility.HtmlEncode(phaseNames.TryGetValue(kv.Key, out var pn3) ? pn3 : kv.Key))
                                      .Append("</div><div class='v'>").Append((kv.Value * 100).ToString("0.000")).Append("</div></div>");
                                }
                            }
                            if (!string.IsNullOrWhiteSpace(r.TieBreak))
                            {
                                sb.Append("<div class='kv' style='margin-top:8px'><div class='k'>Tie-break</div><div class='v'>")
                                  .Append(WebUtility.HtmlEncode(r.TieBreak)).Append("</div></div>");
                            }
                            sb.Append("</div></div>");
                        }
                    }
                }

                sb.Append("</div></div>");
            }

            // JS for navigation, filter pills, toggle breakdown, eliminate
            var baseUrl = ApiBase.Get();
            sb.Append("<script>");
            sb.Append("const baseUrl='").Append(baseUrl.Replace("'", "\\'")).Append("';");
            sb.Append("const eventId='").Append(WebUtility.HtmlEncode(eventId)).Append("';");
            sb.Append("const views=[...document.querySelectorAll('.view')]; let idx=0;");
            sb.Append("const label=document.getElementById('navLabel'); const prev=document.getElementById('navPrev'); const next=document.getElementById('navNext');");
            sb.Append("function cur(){return views[idx].getAttribute('data-view');}");
            sb.Append("function updateLabel(){label.textContent='View: '+cur();}");
            sb.Append("function show(){views.forEach((v,i)=>v.classList.toggle('active',i===idx)); document.querySelectorAll('.pill').forEach(p=>p.classList.toggle('active', p.getAttribute('data-target')===cur())); updateLabel();}");
            sb.Append("prev.onclick=function(){idx=(idx-1+views.length)%views.length;show();}; next.onclick=function(){idx=(idx+1)%views.length;show();};");
            sb.Append("document.getElementById('filters').addEventListener('click',e=>{const t=e.target;if(!t.classList.contains('pill')) return;const target=t.getAttribute('data-target');const i=views.findIndex(v=>v.getAttribute('data-view')===target); if(i>=0){idx=i;show();}});");
            sb.Append("document.addEventListener('click',async e=>{const t=e.target;if(!t) return;");
            sb.Append("if(t.classList.contains('toggle')){const id=t.getAttribute('data-target');const bd=document.getElementById(id);if(!bd)return;const open=bd.classList.toggle('show');t.textContent=open?'▾':'▸';return;}");
            sb.Append("if(t.dataset.action==='toggle'){const cid=t.getAttribute('data-id');if(!cid)return;const row=document.getElementById('row_'+cid);if(!row)return;const eliminated=row.classList.contains('elim');t.disabled=true;const old=t.textContent;t.textContent='...';");
            sb.Append("try{const body={eventId:eventId,updates:[{contestantId:cid,isActive:eliminated?true:false}]};");
            sb.Append("const r=await fetch(baseUrl+'/api/contestants/update-active',{method:'POST',headers:{'Content-Type':'application/json'},body:JSON.stringify(body)});");
            sb.Append("if(!r.ok) throw 0;");
            sb.Append("if(eliminated){row.classList.remove('elim');row.querySelector('.status').textContent='Active';const nameCell=row.querySelector('.name-del');if(nameCell) nameCell.classList.remove('name-del');t.textContent='Eliminate';}");
            sb.Append("else{row.classList.add('elim');row.querySelector('.status').textContent='Eliminated';const nameCell=row.querySelector('.cell:nth-child(3)');if(nameCell) nameCell.classList.add('name-del');t.textContent='Restore';}}");
            sb.Append("catch{t.textContent='Error';setTimeout(()=>{t.textContent=old;},1200);} finally{t.disabled=false;} } });");
            sb.Append("if(views.length<=1){prev.style.display='none';next.style.display='none';}");
            sb.Append("show();");
            sb.Append("</script>");

            sb.Append("</div></body></html>");
            return sb.ToString();
        }

        private static Dictionary<string, int> ComputeDisplayRanks(List<ResultRow> rows)
        {
            var map = new Dictionary<string, int>(StringComparer.Ordinal);
            bool anyPre = rows.Any(r => r.Rank > 0);
            if (anyPre)
            {
                foreach (var r in rows)
                    map[r.ContestantId] = r.Rank > 0 ? r.Rank : int.MaxValue;
                return map;
            }
            int rank = 1;
            foreach (var r in rows
                .OrderByDescending(x => x.EventScore)
                .ThenBy(x => x.Number)
                .ThenBy(x => x.Name, StringComparer.OrdinalIgnoreCase))
            {
                map[r.ContestantId] = rank++;
            }
            return map;
        }

        // Result row used only for HTML generation
        private class ResultRow
        {
            public string ContestantId { get; set; } = "";
            public int Number { get; set; }
            public string Name { get; set; } = "";
            public bool IsActive { get; set; } = true;
            public double EventScore { get; set; }
            public int Rank { get; set; }
            public string TieBreak { get; set; } = "";
            public Dictionary<string, double> PhaseScoresWeighted { get; set; } = new();
            public Dictionary<string, double> IndependentScores { get; set; } = new();
            public Dictionary<string, double> RawPhasePerf { get; set; } = new();
        }

        private void StartAutoRefresh()
        {
            if (_refreshTimer != null) return;
            _refreshTimer = new System.Windows.Forms.Timer();
            _refreshTimer.Interval = 5000; // 5 seconds
            _refreshTimer.Tick += async (s, e) =>
            {
                if (!this.Visible) return;
                if (AppSession.CurrentEvent == null) return;
                if (_loading) return;
                try
                {
                    await RenderAsync(AppSession.CurrentEvent.Id);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("[Results] Auto refresh failed: " + ex);
                }
            };
            _refreshTimer.Start();
        }

        private void StopAutoRefresh()
        {
            try
            {
                _refreshTimer?.Stop();
                _refreshTimer?.Dispose();
            }
            catch { }
            _refreshTimer = null;
        }
    }
}