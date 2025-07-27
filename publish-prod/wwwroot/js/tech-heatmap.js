// /wwwroot/js/tech-heatmap.js
async function loadHeatmap(techId, start, end, metric, containerId) {
    const url = `/api/technician/${techId}/heatmap?start=${start}&end=${end}`;
    const resp = await fetch(url);
    if (!resp.ok) {
        document.getElementById(containerId).innerHTML = '<div class="text-danger">No data available.</div>';
        return;
    }
    const data = await resp.json();
    renderHeatmap(containerId, data, metric);
}

function renderHeatmap(containerId, data, metric) {
    // data: [{ DayOfWeek, HourOfDay, Jobs, Delays, Callbacks }]
    const days = ['Sun','Mon','Tue','Wed','Thu','Fri','Sat'];
    const grid = Array.from({length: 7}, () => Array(24).fill(0));
    let max = 0;
    data.forEach(cell => {
        const val = cell[metric];
        grid[cell.DayOfWeek][cell.HourOfDay] = val;
        if (val > max) max = val;
    });
    // Build grid HTML
    let html = '<div class="heatmap-grid">';
    for (let d = 0; d < 7; d++) {
        for (let h = 0; h < 24; h++) {
            const val = grid[d][h];
            const color = colorScale(val, max);
            html += `<div class='heatmap-cell' title='${days[d]} ${h}:00\n${metric}: ${val}' style='background:${color};'>${val > 0 ? val : ''}</div>`;
        }
    }
    html += '</div>';
    // Add hour labels
    html = '<div class="d-flex mb-1"><div style="width:48px;"></div>' +
        Array.from({length:24}, (_,h)=>`<div style='width:36px;text-align:center;font-size:0.8em;'>${h}</div>`).join('') + '</div>' + html;
    // Add day labels
    html = '<div>' +
        days.map((d,i)=>`<div class='heatmap-label' style='position:absolute;top:${32*i+48}px;left:0;width:48px;height:28px;'>${d}</div>`).join('') +
        '<div style="margin-left:48px;">' + html + '</div></div>';
    document.getElementById(containerId).innerHTML = html;
    // Legend
    renderHeatmapLegend(max, metric);
}

function colorScale(val, max) {
    if (max === 0) return '#f8f9fa';
    // Blue scale for jobs, orange for delays, red for callbacks
    const base = {
        Jobs: [0,123,255],
        Delays: [255,140,0],
        Callbacks: [220,53,69]
    }[window.metricOverride || document.getElementById('metricSelect').value] || [0,123,255];
    const alpha = val/max;
    const r = Math.round(255 - (255-base[0])*alpha);
    const g = Math.round(255 - (255-base[1])*alpha);
    const b = Math.round(255 - (255-base[2])*alpha);
    return `rgb(${r},${g},${b})`;
}

function renderHeatmapLegend(max, metric) {
    let html = `<span class='me-2'>Legend (${metric}):</span>`;
    for (let i = 0; i <= 5; i++) {
        const val = Math.round(max * i / 5);
        html += `<span class='me-1' style='display:inline-block;width:32px;height:18px;background:${colorScale(val,max)};border:1px solid #ccc;text-align:center;font-size:0.8em;'>${val}</span>`;
    }
    document.getElementById('heatmap-legend').innerHTML = html;
}
