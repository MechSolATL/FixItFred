// Sprint 91.7: Technician Tracker Map Logic (Leaflet.js)
(function () {
    let map, markers = {}, polylines = {};
    let techs = window.TechTrackingTechs || [];
    let filterServiceType = '', filterStatus = '', filterTruckId = '';

    function getStatusBadge(status) {
        switch (status) {
            case 'En Route': return '<span style="color:#007bff">?? En Route</span>';
            case 'Working': return '<span style="color:#28a745">?? Working</span>';
            case 'Idle': return '<span style="color:#2ecc40">?? Idle</span>';
            case 'Delayed': return '<span style="color:#e74c3c">?? Delayed</span>';
            default: return status;
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
    function renderMarkers() {
        // Remove old
        Object.values(markers).forEach(m => map.removeLayer(m));
        Object.values(polylines).forEach(p => map.removeLayer(p));
        markers = {}; polylines = {};
        // Filter
        let filtered = techs.filter(t =>
            (!filterServiceType || t.ServiceType === filterServiceType) &&
            (!filterStatus || t.Status === filterStatus) &&
            (!filterTruckId || t.TruckId.toLowerCase().includes(filterTruckId.toLowerCase()))
        );
        filtered.forEach(t => {
            // Marker
            let icon = L.divIcon({
                className: 'tech-marker',
                html: `<div style='background:${getServiceColor(t.ServiceType)};color:white;padding:4px 10px;border-radius:8px;font-weight:bold;box-shadow:0 2px 8px #0003;'>${t.Name}<br><span style='font-size:0.9em;'>${t.TruckId}</span></div>`
            });
            let marker = L.marker([t.Latitude, t.Longitude], { icon }).addTo(map);
            marker.bindPopup(
                `<b>${t.Name}</b> <span style='float:right;'>${t.TruckId}</span><br>` +
                `Service: <span style='color:${getServiceColor(t.ServiceType)}'>${t.ServiceType}</span><br>` +
                `Status: ${getStatusBadge(t.Status)}<br>` +
                `ETA: <b>${t.ETA}</b><br>`
            );
            markers[t.TechnicianId] = marker;
            // Ghost trail
            if (t.GhostTrail && t.GhostTrail.length > 1) {
                let latlngs = t.GhostTrail.map(p => [p.Latitude, p.Longitude]);
                let poly = L.polyline(latlngs, { color: getServiceColor(t.ServiceType), weight: 3, dashArray: '6,8', opacity: 0.7 }).addTo(map);
                polylines[t.TechnicianId] = poly;
            }
        });
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
    // --- SignalR Integration ---
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
        setupSignalR(); // Sprint 91.7.Part4: Use SignalR for live updates
        // pollUpdates(); // Disabled polling in favor of SignalR
    });
})();
