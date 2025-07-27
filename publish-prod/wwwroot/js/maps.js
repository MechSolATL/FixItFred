import { Loader } from 'https://cdn.jsdelivr.net/npm/@googlemaps/js-api-loader@1.15.1/+esm';

window.technicianMap = {
    map: null,
    marker: null,
    loader: null,
    follow: true,
    simulationTimer: null,
    simulationLat: null,
    simulationLng: null,
    simulationStep: 0.0005,
    isSimulated: false,
    directionsRenderer: null,
    directionsService: null,
    init: async function (elementId, lat, lng, techName, apiKey, truckIconUrl, isSimulated = false) {
        if (!window.google || !window.google.maps) {
            this.loader = new Loader({
                apiKey: apiKey,
                version: 'weekly',
                libraries: ['places']
            });
            await this.loader.load();
        }
        const map = new google.maps.Map(document.getElementById(elementId), {
            center: { lat: lat, lng: lng },
            zoom: 15,
            mapTypeControl: false,
            streetViewControl: false,
            fullscreenControl: false,
            gestureHandling: 'greedy',
        });
        const marker = new google.maps.Marker({
            position: { lat: lat, lng: lng },
            map: map,
            title: techName,
            icon: {
                url: isSimulated ? '/images/truck-sim.png' : truckIconUrl,
                scaledSize: new google.maps.Size(48, 48)
            }
        });
        this.map = map;
        this.marker = marker;
        this.simulationLat = lat;
        this.simulationLng = lng;
        this.isSimulated = isSimulated;
        // Responsive fullscreen for mobile
        if (window.innerWidth < 600) {
            map.setOptions({ zoom: 16 });
            document.getElementById(elementId).style.height = '90vh';
        }
        this.directionsRenderer = null;
        this.directionsService = null;
    },
    updateMarker: function (lat, lng) {
        if (this.marker) {
            this.marker.setPosition({ lat: lat, lng: lng });
            if (this.follow && this.map) {
                this.map.panTo({ lat: lat, lng: lng });
            }
        }
    },
    setFollow: function (enabled) {
        this.follow = enabled;
    },
    startSimulation: function () {
        if (this.simulationTimer) return;
        this.simulationTimer = setInterval(() => {
            this.simulationLat += (Math.random() - 0.5) * this.simulationStep;
            this.simulationLng += (Math.random() - 0.5) * this.simulationStep;
            this.updateMarker(this.simulationLat, this.simulationLng);
        }, 2000 + Math.random() * 3000);
    },
    stopSimulation: function () {
        if (this.simulationTimer) {
            clearInterval(this.simulationTimer);
            this.simulationTimer = null;
        }
    },
    drawRouteAndEta: async function (lat, lng, destinationAddress, apiKey, infoElementId) {
        if (!window.google || !window.google.maps) return;
        if (!this.map) return;
        if (!this.directionsService) this.directionsService = new google.maps.DirectionsService();
        if (!this.directionsRenderer) {
            this.directionsRenderer = new google.maps.DirectionsRenderer({ suppressMarkers: true });
            this.directionsRenderer.setMap(this.map);
        }
        const request = {
            origin: { lat: lat, lng: lng },
            destination: destinationAddress,
            travelMode: google.maps.TravelMode.DRIVING
        };
        this.directionsService.route(request, function (result, status) {
            if (status === 'OK') {
                window.technicianMap.directionsRenderer.setDirections(result);
                // Show ETA and distance
                const leg = result.routes[0].legs[0];
                const infoDiv = document.getElementById(infoElementId);
                if (infoDiv) {
                    infoDiv.innerHTML = `<b>ETA:</b> ${leg.duration.text} <b>Distance:</b> ${leg.distance.text}`;
                }
            } else {
                const infoDiv = document.getElementById(infoElementId);
                if (infoDiv) infoDiv.innerHTML = '<span class="text-danger">Unable to calculate route/ETA.</span>';
            }
        });
    }
};

window.technicianMapSignalR = {
    updateMarker: function (technicianId, lat, lng) {
        // For multi-tech: create marker if not exists, else update
        if (window.technicianMap) {
            window.technicianMap.updateMarker(lat, lng);
        }
    }
};
