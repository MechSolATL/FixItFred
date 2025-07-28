// Sprint 85.0 — TrustScore Trend Chart UI
// Sprint 85.0 — Trust Trends Chart Logic + Filters
window.addEventListener('DOMContentLoaded', function () {
    if (!window.Chart || !document.getElementById('trendChart')) return;
    const ctx = document.getElementById('trendChart').getContext('2d');
    let chart;
    async function loadChart() {
        const techId = document.querySelector('select[name="TechnicianId"]').value;
        const dateRange = document.querySelector('input[name="DateRangeIndex"]').value;
        const resp = await fetch(`?handler=ChartData&technicianId=${techId}&dateRangeIndex=${dateRange}`);
        const data = await resp.json();
        // Group by technician
        const techGroups = {};
        data.forEach(pt => {
            if (!techGroups[pt.technicianId]) techGroups[pt.technicianId] = { name: pt.technicianName, points: [] };
            techGroups[pt.technicianId].points.push(pt);
        });
        // Build datasets
        const datasets = Object.values(techGroups).map(g => {
            // Trend color logic
            let trend = 0;
            if (g.points.length > 1) {
                const first = g.points[0].heatScore, last = g.points[g.points.length-1].heatScore;
                trend = last - first;
            }
            let color = trend > 2 ? '#4caf50' : trend < -2 ? '#f44336' : '#ffeb3b';
            return {
                label: g.name,
                data: g.points.map(p => ({ x: p.date, y: p.heatScore })),
                borderColor: color,
                backgroundColor: color + '33',
                tension: 0.2,
                pointRadius: 3,
                pointHoverRadius: 6,
                fill: false
            };
        });
        // X labels (dates)
        const labels = [...new Set(data.map(p => p.date))];
        if (chart) chart.destroy();
        chart = new Chart(ctx, {
            type: 'line',
            data: { labels, datasets },
            options: {
                responsive: true,
                interaction: { mode: 'nearest', intersect: false },
                plugins: {
                    tooltip: {
                        callbacks: {
                            title: items => items[0].label,
                            label: ctx => `Date: ${ctx.parsed.x}, HeatScore: ${ctx.parsed.y}`
                        }
                    },
                    legend: { display: true }
                },
                scales: {
                    x: { type: 'time', time: { unit: 'day', tooltipFormat: 'yyyy-MM-dd' } },
                    y: { min: 0, max: 100, title: { display: true, text: 'HeatScore' } }
                }
            }
        });
    }
    document.querySelector('select[name="TechnicianId"]').addEventListener('change', loadChart);
    document.querySelector('input[name="DateRangeIndex"]').addEventListener('input', function() {
        document.querySelector('form').submit();
    });
    loadChart();
});
// Sprint 85.0 — TrustScore Trend Chart UI
// Sprint 85.0 — Trust Trends Chart Logic + Filters
