// Sprint 85.0 — Admin Drop Alert UI + Toast Integration
// This file powers animated toasts for TrustScore drop alerts on the Admin Dashboard.
window.addEventListener('DOMContentLoaded', function () {
    if (!window.latestDropAlerts || !Array.isArray(window.latestDropAlerts)) return;
    window.latestDropAlerts.forEach(function(alert) {
        var delta = alert.previousScore - alert.currentScore;
        var message = `?? Technician TrustScore dropped by ${delta} points!`;
        // Use showToast from dashboard page if available
        if (typeof window.showToast === 'function') {
            window.showToast(message, 'Severe');
        } else {
            // Fallback: simple toast
            var toast = document.createElement('div');
            toast.className = 'toast align-items-center text-bg-danger border-0 position-fixed top-0 end-0 m-4 show';
            toast.setAttribute('role', 'alert');
            toast.setAttribute('aria-live', 'assertive');
            toast.setAttribute('aria-atomic', 'true');
            toast.innerHTML = `<div class='d-flex'><div class='toast-body'>${message}</div><button type='button' class='btn-close btn-close-white me-2 m-auto' data-bs-dismiss='toast' aria-label='Close'></button></div>`;
            document.body.appendChild(toast);
            setTimeout(function() { toast.remove(); }, 6000);
        }
    });
});
// End Sprint 85.0 — Admin Drop Alert UI + Toast Integration
