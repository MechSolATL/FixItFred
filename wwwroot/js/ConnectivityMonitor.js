// ConnectivityMonitor.js - Sprint 67.3 Signal-Aware Patch
(function () {
    const banner = document.querySelector('.offline-banner');
    let lastStatus = navigator.onLine ? 'online' : 'offline';
    let unstableTimeout = null;
    let offlineSession = null;
    let technicianId = window.currentTechnicianId || null; // Should be set by Razor or login context
    let lastLocation = null;
    let pendingQueue = JSON.parse(localStorage.getItem('offlineSessionQueue') || '[]');

    function setBanner(status) {
        if (!banner) return;
        if (status === 'offline') {
            banner.classList.add('active');
            banner.textContent = 'Offline Mode: Changes will sync automatically when connection returns.';
        } else if (status === 'unstable') {
            banner.classList.add('active');
            banner.textContent = 'Connection Unstable: Some features may be delayed.';
        } else {
            banner.classList.remove('active');
        }
    }

    function fireStatusChanged(status) {
        window.dispatchEvent(new CustomEvent('OnlineStatusChanged', { detail: { status } }));
        setBanner(status);
        if (status === 'offline') {
            startOfflineSession();
        } else if (status === 'online') {
            endOfflineSession();
        }
    }

    function checkHeartbeat() {
        // Try to fetch a small resource to check latency
        const start = Date.now();
        fetch('/favicon.ico', { method: 'HEAD', cache: 'no-store' })
            .then(() => {
                const latency = Date.now() - start;
                if (latency > 1500) {
                    fireStatusChanged('unstable');
                    unstableTimeout = setTimeout(() => fireStatusChanged(navigator.onLine ? 'online' : 'offline'), 3000);
                } else {
                    fireStatusChanged(navigator.onLine ? 'online' : 'offline');
                }
            })
            .catch(() => {
                fireStatusChanged('offline');
            });
    }

    function getLocation(cb) {
        if (navigator.geolocation) {
            navigator.geolocation.getCurrentPosition(function (pos) {
                lastLocation = {
                    latitude: pos.coords.latitude,
                    longitude: pos.coords.longitude
                };
                cb(lastLocation);
            }, function () {
                cb(null);
            });
        } else {
            cb(null);
        }
    }

    function logSessionEvent(type) {
        getLocation(function (loc) {
            const payload = {
                technicianId: technicianId,
                eventType: type,
                timestamp: new Date().toISOString(),
                location: loc
            };
            if (navigator.onLine) {
                fetch('/api/signal/status', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(payload)
                }).catch(() => {
                    queueSessionEvent(payload);
                });
            } else {
                queueSessionEvent(payload);
            }
        });
    }

    function queueSessionEvent(payload) {
        pendingQueue.push(payload);
        localStorage.setItem('offlineSessionQueue', JSON.stringify(pendingQueue));
    }

    function flushQueue() {
        if (navigator.onLine && pendingQueue.length > 0) {
            pendingQueue.forEach(function (payload) {
                fetch('/api/signal/status', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(payload)
                });
            });
            pendingQueue = [];
            localStorage.setItem('offlineSessionQueue', '[]');
        }
    }

    function startOfflineSession() {
        if (!offlineSession) {
            offlineSession = {
                start: new Date().toISOString()
            };
            logSessionEvent('offline_start');
        }
    }
    function endOfflineSession() {
        if (offlineSession) {
            offlineSession.end = new Date().toISOString();
            logSessionEvent('offline_end');
            offlineSession = null;
            flushQueue();
        }
    }

    window.addEventListener('online', () => fireStatusChanged('online'));
    window.addEventListener('offline', () => fireStatusChanged('offline'));

    setInterval(checkHeartbeat, 10000); // Check every 10s
    setInterval(flushQueue, 15000); // Try to flush every 15s
    checkHeartbeat();
    setBanner(lastStatus);
})();
