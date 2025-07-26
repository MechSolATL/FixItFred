// Heatmap.js - Sprint 48.1
// Handles heatmap fading transitions and pulse animation for SLA breach

document.addEventListener('DOMContentLoaded', function () {
    // Fade transition for heatmap blocks
    const rows = document.querySelectorAll('.heatmap-table tr');
    rows.forEach(row => {
        row.addEventListener('transitionend', function () {
            row.classList.remove('pulse');
        });
    });
    // Optionally, listen for custom events to trigger pulse
    window.triggerHeatmapPulse = function (techId) {
        const row = document.querySelector(`tr[data-tech-id='${techId}']`);
        if (row) {
            row.classList.add('pulse');
            setTimeout(() => row.classList.remove('pulse'), 2000);
        }
    };
});
