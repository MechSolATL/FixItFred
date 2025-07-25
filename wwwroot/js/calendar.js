// MVP-Core Sprint 38: Drag-to-Reschedule Calendar UI
// This script renders a Kanban+Calendar hybrid and enables drag-to-reschedule for jobs.

// Example data fetch (replace with real API or Razor model data)
async function fetchCalendarData() {
    // In production, fetch from an API endpoint or use Razor-injected JSON
    return window.calendarData || { jobs: [], technicians: [], days: [] };
}

function renderCalendarBoard(data) {
    const board = document.getElementById('calendar-board');
    board.innerHTML = '';
    // Render header row (tech names)
    const header = document.createElement('div');
    header.className = 'row mb-2';
    data.technicians.forEach(tech => {
        const col = document.createElement('div');
        col.className = 'col calendar-tech-header';
        col.innerHTML = `<strong>${tech.name}</strong>`;
        header.appendChild(col);
    });
    board.appendChild(header);
    // Render each day as a row
    data.days.forEach(day => {
        const row = document.createElement('div');
        row.className = 'row calendar-day-row mb-2';
        data.technicians.forEach(tech => {
            const col = document.createElement('div');
            col.className = 'col calendar-slot dropzone';
            col.dataset.techId = tech.id;
            col.dataset.day = day;
            // Jobs for this tech/day
            const jobs = data.jobs.filter(j => j.technicianId === tech.id && j.day === day);
            jobs.forEach(job => {
                const card = document.createElement('div');
                card.className = 'calendar-job draggable-job card card-body mb-1';
                card.draggable = true;
                card.dataset.jobId = job.id;
                card.innerHTML = `<div><strong>#${job.id}</strong> ${job.serviceType}<br><small>ETA: ${job.eta || ''}</small></div>`;
                col.appendChild(card);
            });
            row.appendChild(col);
        });
        board.appendChild(row);
    });
}

function enableDragDrop() {
    interact('.draggable-job').draggable({
        inertia: true,
        autoScroll: true,
        listeners: {
            move (event) {
                event.target.style.transform = `translate(${event.dx}px, ${event.dy}px)`;
            },
            end (event) {
                event.target.style.transform = '';
            }
        }
    });
    interact('.dropzone').dropzone({
        ondrop: function (event) {
            const jobId = event.relatedTarget.dataset.jobId;
            const techId = event.target.dataset.techId;
            const day = event.target.dataset.day;
            // Call backend API to update job assignment and ETA/SLA
            fetch('/api/dispatch/reschedule-job', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ jobId, techId, day })
            }).then(r => {
                if (r.ok) location.reload();
                else alert('Failed to reschedule job.');
            });
        }
    });
}

document.addEventListener('DOMContentLoaded', async function () {
    const data = await fetchCalendarData();
    renderCalendarBoard(data);
    enableDragDrop();
});
