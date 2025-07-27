// admin-backuplog.js: Handles admin action panel toggling for backup logs
function showActions() {
    var panel = document.getElementById('actions-panel');
    var button = document.getElementById('actions-tab');
    var isHidden = panel.classList.contains('hidden');

    if (isHidden) {
        panel.classList.remove('hidden');
        button.setAttribute('aria-expanded', 'true');
    } else {
        panel.classList.add('hidden');
        button.setAttribute('aria-expanded', 'false');
    }
}
