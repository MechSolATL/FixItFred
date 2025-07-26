// ConnectivityMonitor.js
(function () {
    const banner = document.querySelector('.offline-banner');
    let lastStatus = navigator.onLine ? 'online' : 'offline';
    let unstableTimeout = null;

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

    window.addEventListener('online', () => fireStatusChanged('online'));
    window.addEventListener('offline', () => fireStatusChanged('offline'));

    setInterval(checkHeartbeat, 10000); // Check every 10s
    checkHeartbeat();

    // Initial banner state
    setBanner(lastStatus);
})();
