(function () {
    // Elements
    const $ = id => document.getElementById(id);
    const ev = $('eventId'), judge = $('judge'), contestant = $('contestant');
    const phaseSel = $('phase'), segmentSel = $('segment');
    const criteriaArea = $('criteriaArea');
    const btnReload = $('btnReload');
    const btnSubmitAll = $('btnSubmitAll');
    const statusStruct = $('structureStatus');
    const submitStatus = $('submitStatus');
    const board = $('board');

    // Helpers for tolerant JSON (PascalCase vs camelCase)
    const pick = (obj, ...names) => {
        for (const n of names) if (obj && n in obj) return obj[n];
        return undefined;
    };

    const qp = new URLSearchParams(location.search);
    const qpEventId = qp.get('eventId');

    if (qpEventId) {
        ev.value = qpEventId.trim();
        localStorage.setItem('eventId', ev.value);
    } else if (!ev.value) {
        const stored = localStorage.getItem('eventId');
        if (stored) ev.value = stored;
    }

    let structure = null; // normalized: { phases:[{key, sequence, name, isIndependent, weight, segments:[{sequence,name,weight,criteria:[{name,weight}]}]}] }
    let contestants = []; // [{id, number, fullName}]
    let judges = [];      // [{id, number, name, title}]

    function normStructure(raw) {
        const phases = pick(raw, 'phases', 'Phases') || [];
        const normPhases = [];
        for (const p of phases) {
            const sequence = pick(p, 'sequence', 'Sequence');
            const name = pick(p, 'name', 'Name') || '';
            const weight = Number(pick(p, 'weight', 'Weight') ?? 0);
            const isIndependent = !!pick(p, 'isIndependent', 'IsIndependent');
            const segmentsRaw = pick(p, 'segments', 'Segments') || [];
            const key = (sequence != null) ? `P${sequence}` : `IND-${(name || '').trim().toLowerCase().replace(/\s+/g, '_')}`;

            const segments = [];
            for (const s of segmentsRaw) {
                const sSeq = pick(s, 'sequence', 'Sequence');
                const sName = pick(s, 'name', 'Name') || '';
                const sWeight = Number(pick(s, 'weight', 'Weight') ?? 0);
                const critRaw = pick(s, 'criteria', 'Criteria') || [];
                const criteria = critRaw.map(c => ({
                    name: pick(c, 'name', 'Name') || '',
                    weight: Number(pick(c, 'weight', 'Weight') ?? 0)
                }));
                segments.push({ sequence: sSeq, name: sName, weight: sWeight, criteria });
            }

            normPhases.push({ key, sequence, name, isIndependent, weight, segments });
        }
        return { phases: normPhases };
    }

    function normContestants(rawList) {
        return (rawList || []).map(x => ({
            id: pick(x, 'id', 'Id'),
            number: pick(x, 'number', 'Number'),
            fullName: pick(x, 'fullName', 'FullName') || ''
        }));
    }

    function normJudges(rawList) {
        return (rawList || []).map(x => ({
            id: pick(x, 'id', 'Id'),
            number: pick(x, 'number', 'Number'),
            name: pick(x, 'name', 'Name') || '',
            title: pick(x, 'title', 'Title') || ''
        }));
    }

    async function api(path) {
        const r = await fetch(path);
        if (!r.ok) throw new Error(`${r.status} ${r.statusText}`);
        return r.json();
    }

    async function loadStructure() {
        if (!ev.value) return;
        statusStruct.textContent = 'Loading structure...';
        try {
            const raw = await api(`/api/structure?eventId=${encodeURIComponent(ev.value)}`);
            structure = normStructure(raw);
            statusStruct.textContent = structure.phases.length ? 'Structure loaded.' : 'No phases found.';
            fillPhases();
            fillSegments();
            renderCriteriaInputs();
        } catch (e) {
            statusStruct.innerHTML = `<span class="err">Failed to load structure</span>`;
            console.error(e);
        }
    }

    async function loadContestants() {
        if (!ev.value) return;
        try {
            const raw = await api(`/api/contestants?eventId=${encodeURIComponent(ev.value)}`);
            contestants = normContestants(raw);
            contestant.innerHTML = '';
            for (const c of contestants) {
                const opt = document.createElement('option');
                opt.value = c.id;
                opt.textContent = `#${c.number} ${c.fullName}`;
                contestant.appendChild(opt);
            }
        } catch (e) {
            console.error('contestants load failed', e);
            contestant.innerHTML = '';
        }
    }

    async function loadJudges() {
        if (!ev.value) return;
        try {
            const raw = await api(`/api/judges?eventId=${encodeURIComponent(ev.value)}`);
            judges = normJudges(raw);
            judge.innerHTML = '';
            for (const j of judges) {
                const opt = document.createElement('option');
                opt.value = j.id;
                opt.textContent = `${j.number} - ${j.name}`;
                judge.appendChild(opt);
            }
        } catch (e) {
            console.error('judges load failed', e);
            judge.innerHTML = '';
        }
    }

    function fillPhases() {
        phaseSel.innerHTML = '';
        if (!structure || !structure.phases.length) return;
        for (const p of structure.phases) {
            const opt = document.createElement('option');
            opt.value = p.key;
            opt.textContent = p.sequence != null ? `Phase ${p.sequence}: ${p.name || 'Unnamed'} (${p.weight}%)` : `Independent: ${p.name || 'Unnamed'}`;
            opt.dataset.key = p.key;
            phaseSel.appendChild(opt);
        }
    }

    function getSelectedPhase() {
        if (!structure || !structure.phases.length) return null;
        const key = phaseSel.value;
        return structure.phases.find(p => p.key === key) || null;
    }

    function fillSegments() {
        segmentSel.innerHTML = '';
        const p = getSelectedPhase();
        if (!p) return;
        for (const s of p.segments) {
            const opt = document.createElement('option');
            opt.value = `S${s.sequence}`;
            opt.textContent = `Seg ${s.sequence}: ${s.name || 'Unnamed'} (${s.weight}%)`;
            opt.dataset.seq = String(s.sequence);
            segmentSel.appendChild(opt);
        }
    }

    function getSelectedSegment() {
        const p = getSelectedPhase();
        if (!p) return null;
        const sVal = segmentSel.value; // "S1"
        const seq = Number(sVal.replace(/^S/i, ''));
        return p.segments.find(x => Number(x.sequence) === seq) || null;
    }

    function renderCriteriaInputs() {
        criteriaArea.innerHTML = '';
        const seg = getSelectedSegment();
        if (!seg || !seg.criteria.length) {
            criteriaArea.innerHTML = '<div class="muted">No criteria under selected segment.</div>';
            btnSubmitAll.disabled = true;
            return;
        }

        for (const c of seg.criteria) {
            const row = document.createElement('div');
            row.className = 'criteria-row';
            const label = document.createElement('div');
            label.textContent = `${c.name || 'Unnamed'} (${c.weight}%)`;
            const inputCell = document.createElement('div');
            const input = document.createElement('input');
            input.type = 'number';
            input.min = '0'; input.max = '100'; input.step = '0.1';
            input.placeholder = '0 - 100';
            input.dataset.critName = c.name || '';
            inputCell.appendChild(input);

            row.appendChild(label);
            row.appendChild(inputCell);
            criteriaArea.appendChild(row);
        }

        btnSubmitAll.disabled = false;
    }

    async function submitAll() {
        submitStatus.textContent = '';
        const p = getSelectedPhase();
        const s = getSelectedSegment();
        if (!p || !s) {
            submitStatus.innerHTML = '<span class="err">Select phase and segment first.</span>';
            return;
        }
        if (!judge.value || !contestant.value) {
            submitStatus.innerHTML = '<span class="err">Select judge and contestant first.</span>';
            return;
        }

        const inputs = criteriaArea.querySelectorAll('input[type="number"]');
        const toSend = [];
        inputs.forEach(inp => {
            const val = inp.value.trim();
            if (val === '') return;
            const raw = parseFloat(val);
            if (isNaN(raw)) return;
            const critName = inp.dataset.critName || '';
            const phaseKey = p.key;
            const criteriaId = `${phaseKey}::${s.sequence}:${critName}`.toLowerCase();

            toSend.push({
                EventId: ev.value,
                ContestantId: contestant.value,
                JudgeId: judge.value,
                PhaseId: phaseKey,
                SegmentId: `S${s.sequence}`,
                CriteriaId: criteriaId,
                RawValue: raw,
                MaxValue: 100
            });
        });

        if (!toSend.length) {
            submitStatus.innerHTML = '<span class="err">Enter at least one score.</span>';
            return;
        }

        try {
            for (const dto of toSend) {
                const r = await fetch('/api/score', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(dto)
                });
                if (!r.ok) throw new Error(`POST failed: ${r.status}`);
            }
            submitStatus.innerHTML = '<span class="ok">Scores saved.</span>';
            // clear inputs
            inputs.forEach(i => i.value = '');
            await refreshBoard();
        } catch (e) {
            console.error(e);
            submitStatus.innerHTML = '<span class="err">Failed to save one or more scores.</span>';
        }
    }

    async function refreshBoard() {
        if (!ev.value) return;
        try {
            const data = await api(`/api/leaderboard?eventId=${encodeURIComponent(ev.value)}`);
            board.innerHTML = '';
            (data || []).sort((a, b) => (a.rank ?? a.Rank) - (b.rank ?? b.Rank)).forEach(row => {
                const rank = pick(row, 'rank', 'Rank');
                const contestantId = pick(row, 'contestantId', 'ContestantId');
                const eventScore = Number(pick(row, 'eventScore', 'EventScore') ?? 0);
                const tr = document.createElement('tr');
                tr.innerHTML = `<td>${rank}</td><td>${contestantId}</td><td>${eventScore.toFixed(3)}</td>`;
                board.appendChild(tr);
            });
        } catch (e) {
            console.error('leaderboard load failed', e);
        }
    }

    // Events
    btnReload.addEventListener('click', async () => {
        localStorage.setItem('eventId', ev.value);
        await Promise.all([loadJudges(), loadContestants(), loadStructure(), refreshBoard()]);
    });

    phaseSel.addEventListener('change', () => {
        fillSegments();
        renderCriteriaInputs();
    });

    segmentSel.addEventListener('change', () => {
        renderCriteriaInputs();
    });

    btnSubmitAll.addEventListener('click', submitAll);

    // Initial auto-load
    (async () => {
        if (ev.value) {
            await Promise.all([loadJudges(), loadContestants(), loadStructure(), refreshBoard()]);
        } else {
            statusStruct.innerHTML = '<span class="muted">No eventId provided. Append ?eventId=<ObjectId> to the URL or set it manually.</span>';
        }
    })();
})();