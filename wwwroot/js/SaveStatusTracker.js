// SaveStatusTracker.js
// Sprint 68.3 - Technician Save Tracker UX
// Universal save status feedback for technician actions

(function() {
    let chip;
    function ensureChip() {
        chip = document.querySelector('.save-status-chip');
        if (!chip) {
            chip = document.createElement('div');
            chip.className = 'save-status-chip syncing';
            chip.textContent = '?? Saving…';
            document.body.appendChild(chip);
        }
    }
    window.showSaveStatus = function(status) {
        ensureChip();
        chip.classList.remove('syncing', 'success', 'error', 'offline');
        switch (status) {
            case 'syncing':
                chip.textContent = navigator.onLine ? '?? Saving…' : '? Queued (Offline)';
                chip.classList.add(navigator.onLine ? 'syncing' : 'offline');
                break;
            case 'success':
                chip.textContent = '? Saved!';
                chip.classList.add('success');
                setTimeout(() => chip.style.opacity = '0', 2000);
                break;
            case 'error':
                chip.textContent = '? Save Failed. Retry?';
                chip.classList.add('error');
                chip.style.opacity = '1';
                break;
        }
        chip.style.opacity = '1';
        if (status !== 'error') {
            setTimeout(() => {
                if (chip.classList.contains('success') || chip.classList.contains('syncing') || chip.classList.contains('offline')) {
                    chip.style.opacity = '0';
                }
            }, 2000);
        }
    };
    window.retrySaveStatus = function() {
        showSaveStatus('syncing');
    };
    window.addEventListener('online', () => {
        if (chip && chip.classList.contains('offline')) {
            showSaveStatus('syncing');
        }
    });
})();
