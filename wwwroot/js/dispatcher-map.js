// Sprint 33.3 - Zone Map View
// This file powers the live technician zone map on the Dispatcher page.
// Requires: Leaflet.js loaded on page

window.addEventListener('DOMContentLoaded', function () {
    if (!window.L || !document.getElementById('zoneMap')) return;
    var map = L.map('zoneMap').setView([33.75, -84.39], 11); // Center on Atlanta by default
    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        maxZoom: 19,
        attribution: '© OpenStreetMap contributors'
    }).addTo(map);

    // Sprint 33.3 - Demo: Fetch technician data from a global JS variable or API
    // For real use, replace with AJAX call to an endpoint that returns live techs
    var techs = window.DispatcherTechMarkers || [];
    // Example: [{ id: 1, name: 'Alice', lat: 33.76, lng: -84.4, jobs: 3, eta: '12:30', currentJob: 'Water Heater' }]
    techs.forEach(function (t) {
        var marker = L.marker([t.lat, t.lng]).addTo(map);
        marker.bindPopup(
            `<b>${t.name}</b><br/>Jobs: ${t.jobs}<br/>ETA: ${t.eta || 'N/A'}<br/>Current: ${t.currentJob || 'N/A'}`
        );
        marker.bindTooltip(`${t.name} (${t.jobs})`, {permanent: false, direction: 'top'});
    });

    // Optional: Draw zone boundaries or heatmap (stub)
    if (window.DispatcherZonePolygons) {
        window.DispatcherZonePolygons.forEach(function (zone) {
            L.polygon(zone.coords, {color: zone.color || 'blue', weight: 2, fillOpacity: 0.08}).addTo(map).bindTooltip(zone.name);
        });
    }
});
// End Sprint 33.3 - Zone Map View

// Sprint 33.3 - SignalR Integration for Live Technician Map
// This file powers the live technician zone map on the Dispatcher page.
// Requires: Leaflet.js loaded on page

(function () {
    let map, markers = {};
    function getColorByStatus(status) {
        switch (status) {
            case 'Idle': return 'blue';
            case 'En Route': return 'orange';
            case 'Working': return 'green';
            default: return 'gray';
        }
    }
    function createOrUpdateMarker(t) {
        let key = t.id;
        let color = getColorByStatus(t.status || 'Idle');
        let icon = L.icon({
            iconUrl: `https://raw.githubusercontent.com/pointhi/leaflet-color-markers/master/img/marker-icon-${color}.png`,
            iconSize: [25, 41],
            iconAnchor: [12, 41],
            popupAnchor: [1, -34],
            shadowUrl: 'https://unpkg.com/leaflet@1.9.4/dist/images/marker-shadow.png',
            shadowSize: [41, 41]
        });
        if (markers[key]) {
            markers[key].setLatLng([t.lat, t.lng]);
            markers[key].setIcon(icon);
            markers[key].setPopupContent(`<b>${t.name}</b><br/>Jobs: ${t.jobs}<br/>ETA: ${t.eta || 'N/A'}<br/>Current: ${t.currentJob || 'N/A'}`);
        } else {
            let marker = L.marker([t.lat, t.lng], {icon: icon}).addTo(map);
            marker.bindPopup(`<b>${t.name}</b><br/>Jobs: ${t.jobs}<br/>ETA: ${t.eta || 'N/A'}<br/>Current: ${t.currentJob || 'N/A'}`);
            marker.bindTooltip(`${t.name} (${t.jobs})`, {permanent: false, direction: 'top'});
            markers[key] = marker;
        }
    }
    window.addEventListener('DOMContentLoaded', function () {
        if (!window.L || !document.getElementById('zoneMap')) return;
        map = L.map('zoneMap').setView([33.75, -84.39], 11);
        L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
            maxZoom: 19,
            attribution: '© OpenStreetMap contributors'
        }).addTo(map);
        var techs = window.DispatcherTechMarkers || [];
        techs.forEach(createOrUpdateMarker);
        if (window.DispatcherZonePolygons) {
            window.DispatcherZonePolygons.forEach(function (zone) {
                L.polygon(zone.coords, {color: zone.color || 'blue', weight: 2, fillOpacity: 0.08}).addTo(map).bindTooltip(zone.name);
            });
        }
        // Sprint 33.3 - SignalR: Listen for live updates
        if (window.signalR) {
            let connection = new signalR.HubConnectionBuilder().withUrl("/etahub?zone=all").build();
            connection.on("UpdateTechnicianLocation", function (tech) {
                createOrUpdateMarker(tech);
            });
            connection.start().catch(function (err) {
                console.error("SignalR connection error (zone map):", err.toString());
            });
        }
    });
    // Expose for manual test
    window.DispatcherMapUpdateTech = createOrUpdateMarker;
})();
// End Sprint 33.3 - SignalR Integration

// Sprint 49.0 Patch Log: Mapbox Directions API integration for route optimization
// This file powers the live technician zone map and route optimization on the Dispatcher page.
// Requires: Leaflet.js loaded on page
// Mapbox API fallback: If no key, use mock route

const MAPBOX_TOKEN = window.MapboxToken || '';

function plotOptimizedRoute(tech, customer) {
    if (!window.L || !window.zoneMap) return;
    const map = window.zoneMap;
    // Remove previous route
    if (window.optimizedRouteLayer) {
        map.removeLayer(window.optimizedRouteLayer);
    }
    if (MAPBOX_TOKEN) {
        // Call Mapbox Directions API
        const url = `https://api.mapbox.com/directions/v5/mapbox/driving/${tech.lng},${tech.lat};${customer.lng},${customer.lat}?geometries=geojson&access_token=${MAPBOX_TOKEN}`;
        fetch(url)
            .then(resp => resp.json())
            .then(data => {
                if (data.routes && data.routes.length > 0) {
                    const coords = data.routes[0].geometry.coordinates.map(c => [c[1], c[0]]);
                    window.optimizedRouteLayer = L.polyline(coords, {color: 'red', weight: 5}).addTo(map);
                    // Update ETA display
                    if (window.updateEtaModal) {
                        window.updateEtaModal(data.routes[0].duration);
                    }
                }
            })
            .catch(() => mockRoute(tech, customer));
    } else {
        mockRoute(tech, customer);
    }
}
function mockRoute(tech, customer) {
    // Draw straight line as fallback
    const map = window.zoneMap;
    const coords = [[tech.lat, tech.lng], [customer.lat, customer.lng]];
    window.optimizedRouteLayer = L.polyline(coords, {color: 'gray', weight: 3, dashArray: '8,8'}).addTo(map);
    if (window.updateEtaModal) {
        window.updateEtaModal(1800); // Mock ETA: 30 min
    }
}
window.plotOptimizedRoute = plotOptimizedRoute;
// Usage: plotOptimizedRoute({lat, lng}, {lat, lng})
// End Sprint 49.0 Patch Log
