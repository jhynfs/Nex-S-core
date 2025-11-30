using System;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using MongoDB.Driver;
using NexScore.Models;

namespace NexScore.MainFormPages
{
    public partial class PageScorecards : UserControl
    {
        private WebView2? _web;
        private bool _isLoading;
        private string? _lastEventId;
        private bool _htmlInjected;
        private bool _initSentOnce;
        private System.Windows.Forms.Timer? _autoRefreshTimer;

        public PageScorecards()
        {
            InitializeComponent();

            this.Load += async (_, __) =>
            {
                await EnsureWebAsync();
                if (AppSession.CurrentEvent != null)
                    await InitForEventAsync(AppSession.CurrentEvent);
            };

            this.VisibleChanged += async (_, __) =>
            {
                if (this.Visible && AppSession.CurrentEvent != null)
                {
                    await EnsureWebAsync();
                    await InitForEventAsync(AppSession.CurrentEvent);
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
                {
                    BeginInvoke(new Action(async () =>
                    {
                        await EnsureWebAsync();
                        await InitForEventAsync(evt);
                    }));
                }
                else
                {
                    await EnsureWebAsync();
                    await InitForEventAsync(evt);
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

            pnlMainScorecards.Controls.Clear();
            pnlMainScorecards.Controls.Add(_web);

            try
            {
                await _web.EnsureCoreWebView2Async();
                var s = _web.CoreWebView2.Settings;
                s.AreDefaultContextMenusEnabled = false;
                s.AreDevToolsEnabled = false;
                s.IsZoomControlEnabled = false;

                _web.CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceived;
                _web.CoreWebView2.NavigationCompleted += CoreWebView2_NavigationCompleted;
            }
            catch (WebView2RuntimeNotFoundException)
            {
                pnlMainScorecards.Controls.Clear();
                pnlMainScorecards.Controls.Add(new Label
                {
                    Text = "WebView2 runtime is not installed.\nInstall Microsoft Edge WebView2 Runtime.",
                    ForeColor = System.Drawing.Color.LightGray,
                    Font = new System.Drawing.Font("Lexend Deca", 10f),
                    AutoSize = true,
                    Padding = new Padding(12),
                    Location = new System.Drawing.Point(10, 10)
                });
            }
        }

        private void CoreWebView2_WebMessageReceived(object? sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            try
            {
                var msg = e.TryGetWebMessageAsString();
                if (string.IsNullOrWhiteSpace(msg)) return;

                // Try to parse JSON and inspect 'type'
                string? msgType = null;
                try
                {
                    using var doc = JsonDocument.Parse(msg);
                    if (doc.RootElement.TryGetProperty("type", out var t))
                        msgType = t.GetString();
                }
                catch
                {
                    // fallback to string contains (legacy)
                }

                if (string.Equals(msgType, "ready", StringComparison.OrdinalIgnoreCase) || msg.Contains("\"type\":\"ready\""))
                {
                    System.Diagnostics.Debug.WriteLine("[Scorecards] Received ready from HTML.");
                    _ = SafeSendInitAsync();
                }
                else if (string.Equals(msgType, "scorecards-error", StringComparison.OrdinalIgnoreCase) || msg.Contains("\"type\":\"scorecards-error\""))
                {
                    System.Diagnostics.Debug.WriteLine("[Scorecards] HTML error: " + msg);
                }
                else if (string.Equals(msgType, "score-saved", StringComparison.OrdinalIgnoreCase) || msg.Contains("\"type\":\"score-saved\""))
                {
                    System.Diagnostics.Debug.WriteLine("[Scorecards] Received score-saved from embedded judge UI: " + msg);

                    // Ask the embedded scorecards HTML to reload its content
                    try
                    {
                        if (_web?.CoreWebView2 != null)
                        {
                            var payload = new { type = "reloadAll" };
                            var json = JsonSerializer.Serialize(payload);
                            _web.CoreWebView2.PostWebMessageAsJson(json);
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("[Scorecards] Failed to post reloadAll: " + ex);
                    }

                    // Also send a lightweight init to ensure judges list is fresh (if needed)
                    _ = SafeSendInitAsync();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("[Scorecards] WebMessageReceived exception: " + ex);
            }
        }

        private async void CoreWebView2_NavigationCompleted(object? sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("[Scorecards] NavigationCompleted success=" + e.IsSuccess);
            if (!e.IsSuccess) return;
            if (_htmlInjected)
            {
                await SafeSendInitAsync();
            }
        }

        private async Task SafeSendInitAsync()
        {
            if (AppSession.CurrentEvent == null) return;
            if (_initSentOnce) return;
            _initSentOnce = true;

            await SendInitializationMessageAsync(AppSession.CurrentEvent.Id);
            await Task.Delay(200);
            await SendInitializationMessageAsync(AppSession.CurrentEvent.Id);
        }

        private async Task InitForEventAsync(EventModel evt)
        {
            if (_web?.CoreWebView2 == null) return;
            if (_isLoading) return;
            _isLoading = true;

            try
            {
                UseWaitCursor = true;
                _lastEventId = evt.Id;
                _initSentOnce = false;

                if (!_htmlInjected)
                {
                    var html = BuildEmbeddedScorecardsHtml();
                    _web.CoreWebView2.NavigateToString(html);
                    _htmlInjected = true;
                }
                else
                {
                    await SafeSendInitAsync();
                }
            }
            catch (Exception ex)
            {
                _web?.CoreWebView2.NavigateToString("<html><body style='font-family:Consolas;color:#ccc;padding:16px;'>Error:<br>" +
                    WebUtility.HtmlEncode(ex.Message) + "</body></html>");
            }
            finally
            {
                UseWaitCursor = false;
                _isLoading = false;
            }
        }

        private async Task SendInitializationMessageAsync(string eventId)
        {
            if (_web?.CoreWebView2 == null) return;

            var judges = await Database.Judges
                .Find(j => j.EventId == eventId)
                .ToListAsync();

            var baseUrl = ApiBase.Get();
            System.Diagnostics.Debug.WriteLine("[Scorecards] Using baseUrl: " + baseUrl);
            System.Diagnostics.Debug.WriteLine("[Scorecards] Judges count: " + judges.Count);

            var payload = new
            {
                type = "init",
                eventId,
                baseUrl,
                judges = judges.Select(j =>
                {
                    string number = (j.Number ?? "").Trim();
                    string title = (j.Title ?? "").Trim();
                    string name = (j.Name ?? "").Trim();
                    string judgeId = j.JudgeId ?? j.Id ?? "";
                    string displayName = string.IsNullOrEmpty(title) ? name : $"{title} {name}";
                    return new
                    {
                        judgeId,
                        number,
                        displayName
                    };
                }).ToList()
            };

            string json = JsonSerializer.Serialize(payload);
            System.Diagnostics.Debug.WriteLine("[Scorecards] Posting init payload length: " + json.Length);
            _web.CoreWebView2.PostWebMessageAsJson(json);
        }

        private string BuildEmbeddedScorecardsHtml() => """
<!doctype html>
<html lang="en">
<head>
<meta charset="utf-8">
<title>NexScore Judge Scorecards</title>
<meta name="viewport" content="width=device-width,initial-scale=1">
<link href="https://fonts.googleapis.com/css2?family=Lexend+Deca:wght@300;400;500;600;700&display=swap" rel="stylesheet">
<style>
:root{ --bg:#1e1f2f; --panel:#23264A; --panel2:#2C2E58; --header:#3A3D74; --border:#44496F; --accent:#4E6AF2; --accent-h:#3957D9; --text:#F7F6ED; --danger:#f87171; }
*{box-sizing:border-box;}
html,body{margin:0;padding:0;font-family:'Lexend Deca',Segoe UI,Arial,sans-serif;background:var(--bg);color:var(--text);font-size:19px;}
.page{padding:14px 18px 80px 18px;}
.controls-bar{display:flex;gap:12px;flex-wrap:wrap;margin:0 0 18px 0;}
.btn{background:var(--accent);color:#fff;border:none;border-radius:6px;cursor:pointer;font-weight:600;font-size:19px;padding:8px 14px;transition:.15s background;}
.btn:hover{background:var(--accent-h);}
.btn-small{background:var(--accent);color:#fff;border:none;border-radius:6px;cursor:pointer;font-weight:600;font-size:16px;padding:6px 10px;transition:.15s background;}
.btn-small:hover{background:var(--accent-h);}
.scorecards{min-width:520px;}
.scorecard{background:var(--panel);border:1px solid var(--border);border-radius:12px;padding:16px 18px;margin-bottom:28px;page-break-inside:avoid;}
.scorecard h3{margin:0 0 12px;font-size:24px;font-weight:600;letter-spacing:.4px;display:flex;align-items:center;gap:10px;}
.badge{background:var(--header);padding:4px 8px;border-radius:6px;font-size:16px;font-weight:600;}
.sc-table{width:100%;border-collapse:collapse;margin-top:4px;}
.sc-table th,.sc-table td{padding:6px 8px;font-size:17px;border-bottom:1px solid var(--border);text-align:left;font-variant-numeric:tabular-nums;white-space:nowrap;}
.sc-table th{background:var(--header);font-weight:600;}
.sc-table td.rank{text-align:center;font-weight:600;}
.sc-table td.pct{text-align:right;}
.toggle-crit{cursor:pointer;color:var(--accent);text-decoration:underline;font-size:16px;}
.toggle-crit:hover{color:var(--accent-h);}
.crit-details{display:none;margin-top:8px;background:#2b2e56;padding:10px 12px;border-radius:8px;}
.crit-details.show{display:block;}
.crit-row{display:grid;grid-template-columns:150px 130px 1fr 70px;gap:6px;padding:4px 0;border-bottom:1px solid #3a3d74;font-size:16px;}
.crit-row:last-child{border-bottom:none;}
.crit-head{font-weight:600;opacity:.85;}
.muted{opacity:.75;font-size:17px;padding:4px 2px;}
@media print{
    body,html{ background:#fff; color:#000; font-size: 12pt; }

    .page{ padding: 0 10px; }

    .controls-bar, .btn-small, .muted { display:none!important; }

    .scorecard{ 
        border: 1px solid #000; 
        padding: 10px; 
        margin-bottom: 20px; 
        page-break-inside: avoid;
        box-shadow: none; 
    }
    .scorecard h3{ color:#000; font-size: 16pt; margin-bottom: 5px; }

    .crit-row { 
        grid-template-columns: 60px 80px 1fr 60px !important; 
        font-size: 10pt; 
        border-bottom: 1px solid #ccc;
    }

    .crit-details{ 
        display: block !important; 
        background: transparent; 
        color: #000; 
        padding: 0; 
        margin-top: 5px;
    }

    .scorecard{ page-break-before: always; }
    .scorecard:first-of-type{ page-break-before: auto; }
}
/* end css */
</style>
</head>
<body>
<div class="page">
  <div class="controls-bar">
    <button class="btn" id="btnReloadAll">Reload Scorecards</button>
    <button class="btn" id="btnPrint">Print to PDF</button>
  </div>
  <div class="scorecards" id="scorecardsHost">
    <div style="opacity:.7;font-size:18px;">Waiting for initialization...</div>
  </div>
  <div class="muted">Printing: chooses 'Microsoft Print to PDF' or your real printer.</div>
</div>
<div style="display:none" id="dummy"></div>
<script>
let initData=null;
let contestants=[];
function escapeHtml(s){return (s||'').replace(/[&<>"']/g,c=>({'&':'&amp;','<':'&lt;','>':'&gt;','"':'&quot;','\'':'&#39;'}[c]));}
async function fetchJson(url){ const r=await fetch(url); if(!r.ok) throw new Error(url+' -> '+r.status+' '+r.statusText); return r.json(); }
async function loadContestants(){
  const raw=await fetchJson(initData.baseUrl + '/api/contestants?eventId=' + encodeURIComponent(initData.eventId));
  contestants=(raw||[]).map(c=>({ id:c.id||c.Id, number:c.number||c.Number, name:c.fullName||c.FullName||'', isActive:c.isActive!==false }));
}
async function buildScorecard(j, expandAllCriteria=false){
  const totals=await fetchJson(initData.baseUrl + '/api/judge-progress?eventId=' + encodeURIComponent(initData.eventId) + '&judgeId=' + encodeURIComponent(j.judgeId));
  const map={}; (totals||[]).forEach(t=> map[t.contestantId]=t.totalFraction);
  const card=document.createElement('div'); card.className='scorecard';
  const link=initData.baseUrl + '/JudgeClient/index.html?eventId='+encodeURIComponent(initData.eventId)+'&judgeId='+encodeURIComponent(j.judgeId);
  card.innerHTML=`
    <h3>${escapeHtml(j.displayName)} <span class='badge'>Judge ${escapeHtml(j.number||'')}</span>
      <button class='btn-small' data-jreload='${escapeHtml(j.judgeId)}' style='margin-left:auto;'>Reload</button>
    </h3>
    <table class='sc-table'>
      <thead><tr>
        <th style="width:50px;">Rank</th><th style="width:70px;">No.</th><th>Contestant</th><th style="width:110px;">Total %</th><th style="width:110px;">Details</th>
      </tr></thead>
      <tbody></tbody>
    </table>
    <div class='muted'>Link: <span style='font-size:16px;'>${escapeHtml(link)}</span></div>`;
  const tbody=card.querySelector('tbody');
  const enriched=contestants.map(c=>({ contestant:c, fraction:typeof map[c.id]==='number'?map[c.id]:null }))
    .sort((a,b)=>{
      if(a.fraction==null && b.fraction==null) return (a.contestant.number||9999)-(b.contestant.number||9999);
      if(a.fraction==null) return 1; if(b.fraction==null) return -1; return b.fraction-a.fraction;
    });
  let rank=1;
  // Helper for criteria details
  async function addCriteriaDetails(tr, jid, cid, autoExpand){
    let details=document.createElement('div'); details.className='crit-details';
    details.innerHTML='<div style="font-size:16px;opacity:.7;">Loading...</div>';
    tr.lastElementChild.appendChild(details);
    try{
      const rows=await fetchJson(initData.baseUrl + '/api/judge-scores?eventId=' + encodeURIComponent(initData.eventId) + '&judgeId=' + encodeURIComponent(jid) + '&contestantId=' + encodeURIComponent(cid));
      if(!Array.isArray(rows) || rows.length===0){ details.innerHTML='<div style="font-size:16px;opacity:.7;">No criteria scores.</div>'; }
      else{
        const wrap=document.createElement('div'); wrap.innerHTML='<div class="crit-row crit-head"><div>Phase</div><div>Segment</div><div>Criteria</div><div style="text-align:right;">Score</div></div>';
        rows.forEach(r=>{
          const phase=r.phaseId||r.PhaseId||'', segment=r.segmentId||r.SegmentId||'', crit=r.criteriaId||r.CriteriaId||'';
          const raw=r.rawValue!=null?r.RawValue??r.rawValue:r.RawValue;
          let critName=(crit.split(':').pop()||crit).replace(/[_\-]+/g,' ').trim();
          const line=document.createElement('div'); line.className='crit-row';
          line.innerHTML='<div>'+escapeHtml(phase)+'</div><div>'+escapeHtml(segment)+'</div><div>'+escapeHtml(critName)+'</div><div style="text-align:right;">'+escapeHtml(String(raw))+'</div>';
          wrap.appendChild(line);
        });
        details.innerHTML=''; details.appendChild(wrap);
      }
      if(autoExpand) details.classList.add('show');
    }catch(err){
      details.innerHTML='<div style="color:var(--danger);font-size:16px;">Failed to load.<br>'+escapeHtml(err.message)+'</div>';
      if(autoExpand) details.classList.add('show');
      window.chrome?.webview?.postMessage({type:'scorecards-error', error:err.message});
    }
  }
  // Rows for each contestant in this judge's scorecard
  for(let i=0;i<enriched.length;i++){
    const item=enriched[i];
    const tr=document.createElement('tr'); const has=item.fraction!=null;
    tr.innerHTML=
      `<td class='rank'>${has?rank++:'—'}</td>
      <td>${item.contestant.number ?? '—'}</td>
      <td>${escapeHtml(item.contestant.name)}</td>
      <td class='pct'>${has?(item.fraction*100).toFixed(3):'—'}</td>
      <td>${has||expandAllCriteria?'<span style="display:none" class="toggle-crit" data-jid="'+escapeHtml(j.judgeId)+'" data-cid="'+escapeHtml(item.contestant.id)+'">View Criteria</span>':''}</td>`;
    // Fifth (details) cell will have criteria details block injected below
    tbody.appendChild(tr);
    if(has||expandAllCriteria){
      // Fetch and show criteria by default for print mode
      addCriteriaDetails(tr, j.judgeId, item.contestant.id, expandAllCriteria);
    }
  }
  card.querySelector('[data-jreload]')?.addEventListener('click', async ev=>{
    const btn=ev.currentTarget; if(!btn) return;
    btn.disabled=true; btn.textContent='...';
    try{ card.remove(); await buildScorecard(j, false); }catch(err){ btn.textContent='Err'; }
    finally{ btn.disabled=false; if(btn.textContent==='Err') setTimeout(()=>btn.textContent='Reload',1500); }
  });
  document.getElementById('scorecardsHost').appendChild(card);
}
async function loadAll(expandAll=false){
  const host=document.getElementById('scorecardsHost');
  host.innerHTML='<div style="opacity:.7;font-size:18px;">Loading scorecards...</div>';
  try{
    if(!initData) throw new Error('Init data missing');
    await loadContestants(); host.innerHTML='';
    if(!initData.judges || !initData.judges.length){ host.innerHTML='<div style="opacity:.7;font-size:18px;">No judges defined for this event.</div>'; return; }
    // For print: pass expandAll=true so all criteria are loaded and shown
    for(const j of initData.judges){ await buildScorecard(j, expandAll); }
  }catch(e){
    host.innerHTML='<div style="color:var(--danger);">Failed to load scorecards.<br>'+escapeHtml(e.message)+'</div>';
    window.chrome?.webview?.postMessage({type:'scorecards-error', error:e.message});
  }
}
document.addEventListener('click', async e=>{
  const t=e.target; if(!t) return;
  if(t.id==='btnPrint'){
    const btn = t;
    const originalText = btn.textContent;
    btn.textContent = "Preparing...";
    btn.disabled = true;

    // 1. Load all data (expanded)
    await loadAll(true);

    // 2. Wait a moment for DOM reflow/paint
    // 500ms is usually safe for WebView2 to render the new table rows
    setTimeout(()=>{ 
        window.print(); 

        // Reset button state after print dialog opens/closes
        btn.textContent = originalText;
        btn.disabled = false;
    }, 500); 
}
  else if(t.id==='btnReloadAll'){ loadAll(false); }
});

function init(data){ initData=data; loadAll(false); }
window.chrome?.webview?.postMessage({type:'ready'});
if(window.chrome && window.chrome.webview){
  window.chrome.webview.addEventListener('message', e=>{ const d=e.data; if(!d) return; if(d.type==='init') init(d); else if(d.type==='reloadAll') loadAll(false); else if(d.type==='refreshScores') loadAll(false); });
}else{
  document.getElementById('scorecardsHost').innerHTML='<div style="color:#f87171;">Host messaging not available.</div>';
}
</script>
</body>
</html>
""";

        private void StartAutoRefresh()
        {
            if (_autoRefreshTimer != null) return;
            _autoRefreshTimer = new System.Windows.Forms.Timer();
            _autoRefreshTimer.Interval = 8000; // 8s periodic refresh
            _autoRefreshTimer.Tick += (s, e) =>
            {
                try
                {
                    if (_web?.CoreWebView2 != null)
                    {
                        var payload = new { type = "reloadAll" };
                        var json = JsonSerializer.Serialize(payload);
                        _web.CoreWebView2.PostWebMessageAsJson(json);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("[Scorecards] Auto refresh failed: " + ex);
                }
            };
            _autoRefreshTimer.Start();
        }

        private void StopAutoRefresh()
        {
            try
            {
                _autoRefreshTimer?.Stop();
                _autoRefreshTimer?.Dispose();
            }
            catch { }
            _autoRefreshTimer = null;
        }
    }
}