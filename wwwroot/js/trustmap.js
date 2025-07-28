// Sprint 84.8 Phase 2 — TrustMap Interactivity + GeoCluster
// This file powers the interactive technician trust map on the Admin TrustMap page.
// Requires: Leaflet.js loaded on page

window.addEventListener('DOMContentLoaded', function () {
    if (!window.L || !document.getElementById('trustmap')) return;
    var map = L.map('trustmap').setView([33.75, -84.39], 10); // Default center (Atlanta)
    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        maxZoom: 19,
        attribution: '© OpenStreetMap contributors'
    }).addTo(map);

    fetch('/Admin/TrustMap?handler=TrustMapData')
        .then(r => r.json())
        .then(function (groups) {
            groups.forEach(function (group) {
                var city = group.City || group.Region || 'Unknown';
                var avgScore = group.AvgHeatScore || 0;
                var techs = group.Technicians || [];
                var markerLatLng = null;
                // Try to find a tech with lat/lon for city marker
                for (var i = 0; i < techs.length; i++) {
                    if (techs[i].Latitude && techs[i].Longitude) {
                        markerLatLng = [techs[i].Latitude, techs[i].Longitude];
                        break;
                    }
                }
                // Fallback: random jitter for cityless
                if (!markerLatLng && techs.length > 0) {
                    markerLatLng = [33.75 + (Math.random() - 0.5) * 0.2, -84.39 + (Math.random() - 0.5) * 0.2];
                }
                if (!markerLatLng) return;
                // Color by avg heat
                var color = avgScore >= 80 ? '#4caf50' : avgScore >= 60 ? '#ffeb3b' : avgScore >= 40 ? '#ff9800' : '#f44336';
                var pulse = avgScore < 40;
                var markerHtml = `<div class='trustmap-marker' style='background:${color};${pulse ? "animation:pulse 1.2s infinite alternate;" : ""}'>${city}</div>`;
                var icon = L.divIcon({
                    className: 'trustmap-divicon',
                    html: markerHtml,
                    iconSize: [60, 24],
                    iconAnchor: [30, 12]
                });
                var marker = L.marker(markerLatLng, { icon: icon }).addTo(map);
                // Hover summary
                var summary = `<b>${city}</b><br/>Techs: ${group.TechnicianCount}<br/>Avg Score: ${Math.round(avgScore)}<br/>Last Activity: ${group.LastActivity || 'N/A'}`;
                marker.bindTooltip(summary, { direction: 'top', sticky: true });
                // List all techs in popup
                var popup = `<b>${city}</b><br/><ul style='padding-left:1em;'>`;
                techs.forEach(function (t) {
                    popup += `<li>${t.Name} <span style='color:${color};font-weight:bold;'>${t.HeatScore}</span>${t.HeatScore < 40 ? " <span class='pulse-icon text-danger' title='Low Trust'>&#9888;</span>" : ""}</li>`;
                });
                popup += '</ul>';
                marker.bindPopup(popup);
            });
        });
});

// CSS for marker and pulse
const style = document.createElement('style');
style.innerHTML = `
.trustmap-marker {
    border-radius: 12px;
    color: #222;
    font-weight: bold;
    padding: 2px 10px;
    box-shadow: 0 2px 8px rgba(0,0,0,0.18);
    border: 2px solid #fff;
    font-size: 1rem;
    text-align: center;
}
.trustmap-divicon { background: transparent; border: none; }
@keyframes pulse { 0% { opacity: 1; } 100% { opacity: 0.4; } }
`;
document.head.appendChild(style);
// End Sprint 84.8 Phase 2 — TrustMap Interactivity + GeoCluster
