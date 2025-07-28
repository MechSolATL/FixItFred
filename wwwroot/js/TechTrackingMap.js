// Sprint 91.7.Part5.2: Enhanced Technician Tracker Map Logic (Leaflet.js + Diagnostics)
(function () {
    let map, markers = {}, polylines = {};
    let techs = window.TechTrackingTechs || [];
    let filterServiceType = '', filterStatus = '', filterTruckId = '';

    function getStatusBadge(status) {
        switch (status) {
            case 'En Route': return '<span class="tech-status-badge" style="background:#007bff">?? En Route</span>';
            case 'Working': return '<span class="tech-status-badge" style="background:#28a745">?? Working</span>';
            case 'Idle': return '<span class="tech-status-badge" style="background:#ffc107;color:#222;">?? Idle</span>';
            case 'Delayed': return '<span class="tech-status-badge" style="background:#e74c3c">?? Delayed</span>';
            default: return `<span class='tech-status-badge'>${status}</span>`;
        }
    }
    function getServiceColor(type) {
        switch ((type||'').toLowerCase()) {
            case 'hvac': return 'blue';
            case 'plumbing': return 'red';
            case 'septic': return 'brown';
            default: return 'gray';
        }
    }
    function getAlertDot(t) {
        if (t.IsStaleSignalAlert) return '<span class="pulse-alert" title="Stale location"></span>';
        if (t.IsIdleAlert) return '<span class="idle-glow" title="Idle too long"></span>';
        if (t.IsMissedJobAlert) return '<span class="missed-job-ring" title="Missed job"></span>';
        return '';
    }
    function formatTimeAgo(dateString) {
        if (!dateString) return 'N/A';
        const date = new Date(dateString);
        const now = new Date();
        const diffMs = now - date;
        const diffMin = Math.round(diffMs / 60000);
        if (diffMin < 1) return 'just now';
        if (diffMin < 60) return `${diffMin} min ago`;
        const diffHr = Math.floor(diffMin / 60);
        if (diffHr < 24) return `${diffHr} hr ago`;
        const diffDay = Math.floor(diffHr / 24);
        return `${diffDay} day${diffDay > 1 ? 's' : ''} ago`;
    }
    function renderMarkers() {
        Object.values(markers).forEach(m => map.removeLayer(m));
        Object.values(polylines).forEach(p => map.removeLayer(p));
        markers = {}; polylines = {};
        let filtered = techs.filter(t =>
            (!filterServiceType || t.ServiceType === filterServiceType) &&
            (!filterStatus || t.Status === filterStatus) &&
            (!filterTruckId || t.TruckId.toLowerCase().includes(filterTruckId.toLowerCase()))
        );
        filtered.forEach(t => {
            let icon = L.divIcon({
                className: 'tech-marker',
                html: `<div style='background:${getServiceColor(t.ServiceType)};color:white;padding:4px 10px;border-radius:8px;font-weight:bold;box-shadow:0 2px 8px #0003;position:relative;'>${t.Name}<br><span style='font-size:0.9em;'>${t.TruckId}</span>${getAlertDot(t)}</div>`
            });
            let marker = L.marker([t.Latitude, t.Longitude], { icon }).addTo(map);
            marker.on('click', function () { showTechInfoPanel(t); });
            // Enhanced popup content
            const popupContent = `
              <b>${t.Name}</b><br>
              <span>Truck ID: ${t.TruckId}</span><br>
              <span>Status: ${t.Status}</span><br>
              <span>Reason: ${t.StatusReason || t.statusReason || ''}</span><br>
              <span>Jobs Today: ${t.JobCount ?? t.jobCount ?? ''}</span><br>
              <span>Last Updated: ${formatTimeAgo(t.LastUpdated || t.lastUpdated)}</span>
            `;
            marker.bindPopup(popupContent);
            marker.bindTooltip(`${t.Name} (${t.TruckId})`, {permanent: false, direction: 'top'});
            markers[t.TechnicianId] = marker;
            // Visual alerts
            marker.on('add', function() {
                setTimeout(() => {
                    if (marker._icon) {
                        if (t.HasAlertFlag ?? t.hasAlertFlag) {
                            marker._icon.classList.add('tech-marker-alert');
                        }
                        if (t.HasStaleLocation ?? t.hasStaleLocation) {
                            marker._icon.classList.add('tech-marker-stale');
                        }
                    }
                }, 0);
            });
            if (t.GhostTrail && t.GhostTrail.length > 1) {
                let latlngs = t.GhostTrail.map(p => [p.Latitude, p.Longitude]);
                let poly = L.polyline(latlngs, { color: getServiceColor(t.ServiceType), weight: 3, dashArray: '6,8', opacity: 0.7 }).addTo(map);
                polylines[t.TechnicianId] = poly;
            }
        });
    }
    function showTechInfoPanel(t) {
        let panel = document.getElementById('tech-info-panel');
        if (!panel) {
            panel = document.createElement('div');
            panel.id = 'tech-info-panel';
            document.body.appendChild(panel);
        }
        let lastUpdated = t.LastUpdated ? new Date(t.LastUpdated) : null;
        let now = new Date();
        let minAgo = lastUpdated ? Math.round((now - lastUpdated) / 60000) : null;
        let staleWarn = t.IsStaleSignalAlert ? `<span style='color:#ff5252;font-weight:bold;'>Stale (${minAgo} min ago)</span>` : (lastUpdated ? `${minAgo} min ago` : 'N/A');
        panel.innerHTML = `
            <div style='display:flex;align-items:center;gap:8px;'>
                <span style='font-size:1.3em;font-weight:bold;'>${t.Name}</span>
                <span class='tech-status-badge' style='background:#555;'>${t.TruckId}</span>
                ${getAlertDot(t)}
            </div>
            <div style='margin-top:6px;'>
                <b>Status:</b> ${getStatusBadge(t.Status)} <span style='margin-left:8px;'>${t.StatusReason}</span>
            </div>
            <div><b>Jobs Today:</b> ${t.JobCount}</div>
            <div><b>Last Updated:</b> ${staleWarn}</div>
            <div><b>ETA:</b> ${t.ETA}</div>
        `;
        panel.style.display = 'block';
    }
    function populateFilters() {
        let svcTypes = [...new Set(techs.map(t => t.ServiceType))];
        let statuses = [...new Set(techs.map(t => t.Status))];
        let svcSel = document.getElementById('filterServiceType');
        let statSel = document.getElementById('filterStatus');
        svcSel.innerHTML = '<option value="">All</option>' + svcTypes.map(s => `<option>${s}</option>`).join('');
        statSel.innerHTML = '<option value="">All</option>' + statuses.map(s => `<option>${s}</option>`).join('');
    }
    function attachFilterEvents() {
        document.getElementById('filterServiceType').onchange = function () { filterServiceType = this.value; renderMarkers(); };
        document.getElementById('filterStatus').onchange = function () { filterStatus = this.value; renderMarkers(); };
        document.getElementById('filterTruckId').oninput = function () { filterTruckId = this.value; renderMarkers(); };
    }
    function setupSignalR() {
        if (window.signalR) {
            const connection = new signalR.HubConnectionBuilder()
                .withUrl("/hubs/techtracking")
                .build();
            connection.on("ReceiveLocationUpdate", function (technicianList) {
                techs = technicianList;
                populateFilters();
                renderMarkers();
            });
            connection.start().catch(err => console.error("SignalR connection error:", err));
        }
    }
    window.addEventListener('DOMContentLoaded', function () {
        if (!window.L || !document.getElementById('techTrackingMap')) return;
        map = L.map('techTrackingMap').setView([33.75, -84.39], 11);
        L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
            maxZoom: 19,
            attribution: '© OpenStreetMap contributors'
        }).addTo(map);
        populateFilters();
        attachFilterEvents();
        renderMarkers();
        setupSignalR();
        // Hide info panel on map click
        map.on('click', function () {
            let panel = document.getElementById('tech-info-panel');
            if (panel) panel.style.display = 'none';
        });
    });
})();
