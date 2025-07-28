// JS for drag-and-drop assignment in Dispatcher panel
// Requires: tech-card (draggable), job-card (drop target)
document.addEventListener('DOMContentLoaded', function () {
    let draggedTechId = null;
    document.querySelectorAll('.tech-card').forEach(el => {
        el.addEventListener('dragstart', e => {
            draggedTechId = el.getAttribute('data-tech-id');
            e.dataTransfer.effectAllowed = 'move';
            el.classList.add('opacity-50');
        });
        el.addEventListener('dragend', e => {
            el.classList.remove('opacity-50');
        });
    });
    document.querySelectorAll('.job-card').forEach(el => {
        el.addEventListener('dragover', e => {
            e.preventDefault();
            el.classList.add('ring', 'ring-blue-400');
        });
        el.addEventListener('dragleave', e => {
            el.classList.remove('ring', 'ring-blue-400');
        });
        el.addEventListener('drop', e => {
            e.preventDefault();
            el.classList.remove('ring', 'ring-blue-400');
            const jobId = el.getAttribute('data-job-id');
            if (draggedTechId && jobId) {
                // Call backend to assign tech to job (AJAX or fetch)
                fetch(`/api/dispatcher/assign`, {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({ techId: draggedTechId, jobId: jobId })
                }).then(r => r.json()).then(resp => {
                    if (resp.success) location.reload();
                    else alert('Assignment failed: ' + (resp.message || 'Unknown error'));
                });
            }
        });
    });
});
