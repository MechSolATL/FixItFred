// Sprint 49.0 Patch Log: Mobile Dispatcher JS for collapsible cards and swipe

document.addEventListener('DOMContentLoaded', function () {
    document.querySelectorAll('.toggle-card-btn').forEach(btn => {
        btn.addEventListener('click', function () {
            const cardBody = btn.closest('.job-card-mobile').querySelector('.card-body');
            cardBody.classList.toggle('collapse');
        });
    });
    // Optional: swipe left/right to collapse/expand
    document.querySelectorAll('.job-card-mobile').forEach(card => {
        let startX = null;
        card.addEventListener('touchstart', function (e) {
            startX = e.touches[0].clientX;
        });
        card.addEventListener('touchend', function (e) {
            if (startX !== null) {
                let endX = e.changedTouches[0].clientX;
                if (endX - startX > 60) {
                    // Swipe right: expand
                    card.querySelector('.card-body').classList.remove('collapse');
                } else if (startX - endX > 60) {
                    // Swipe left: collapse
                    card.querySelector('.card-body').classList.add('collapse');
                }
            }
            startX = null;
        });
    });
});
// End Sprint 49.0 Patch Log
