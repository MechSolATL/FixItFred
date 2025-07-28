// JS for Manager Tools admin actions
function showCancelModal(requestId) {
    document.getElementById('cancelRequestId').value = requestId || '';
    document.getElementById('cancelModal').style.display = 'block';
}
function closeCancelModal() {
    document.getElementById('cancelModal').style.display = 'none';
}
function showReassignModal(requestId) {
    document.getElementById('reassignRequestId').value = requestId || '';
    document.getElementById('reassignModal').style.display = 'block';
}
function closeReassignModal() {
    document.getElementById('reassignModal').style.display = 'none';
}
function showReopenModal(requestId) {
    document.getElementById('reopenRequestId').value = requestId || '';
    document.getElementById('reopenModal').style.display = 'block';
}
function closeReopenModal() {
    document.getElementById('reopenModal').style.display = 'none';
}
function disableSubmit(form) {
    var btns = form.querySelectorAll('button[type="submit"]');
    btns.forEach(function(btn) { btn.disabled = true; });
    return true;
}
// Optionally: add feedback display logic
