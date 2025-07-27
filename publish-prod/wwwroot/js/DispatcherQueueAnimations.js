// Sprint 47.4 Patch Log: Dispatch queue animation and ETA adjustments.
// ...existing code...
// Kanban drag-and-drop and ETA update animation logic
window.DispatcherQueueAnimations = {
    animateJobMovement: function(jobId, fromStatus, toStatus) {
        const jobEl = document.querySelector(`[data-job-id='${jobId}']`);
        if (!jobEl) return;
        jobEl.classList.add('job-moving');
        setTimeout(() => {
            jobEl.classList.remove('job-moving');
            jobEl.classList.add('job-arrived');
            setTimeout(() => jobEl.classList.remove('job-arrived'), 1200);
        }, 800);
    },
    animateETAUpdate: function(zone) {
        document.querySelectorAll(`.draggable-job[data-zone='${zone}']`).forEach(el => {
            el.classList.add('eta-updated');
            setTimeout(() => el.classList.remove('eta-updated'), 1200);
        });
    }
};
