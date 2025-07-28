// Sprint 91.7 Part 6.4: SignalR client for tool transfer events
let connection = new signalR.HubConnectionBuilder()
    .withUrl("/hubs/tooltracking")
    .build();

connection.on("toolAssigned", function (data) {
    ToolTrackerUI.flashToolRow(data.ToolId, "assigned");
});

connection.on("toolConfirmed", function (data) {
    ToolTrackerUI.flashToolRow(data.ToolId, "confirmed");
});

connection.on("toolTransferFailed", function (data) {
    ToolTrackerUI.showTransferError(data.ToolId, data.Reason);
});

connection.start().catch(function (err) {
    return console.error(err.toString());
});

// UI helpers for row animation/indicators
window.ToolTrackerUI = {
    flashToolRow: function (toolId, type) {
        let row = document.querySelector(`tr[data-toolid='${toolId}']`);
        if (!row) return;
        let badge = row.querySelector('.tool-alert-badge');
        if (!badge) {
            badge = document.createElement('span');
            badge.className = 'tool-alert-badge ms-2';
            row.cells[1].appendChild(badge);
        }
        badge.textContent = type === "assigned" ? "New Assignment" : "Confirmed";
        badge.classList.add('pulse-' + type);
        setTimeout(() => badge.classList.remove('pulse-' + type), 2000);
    },
    showTransferError: function (toolId, reason) {
        let row = document.querySelector(`tr[data-toolid='${toolId}']`);
        if (!row) return;
        let badge = row.querySelector('.tool-alert-badge');
        if (!badge) {
            badge = document.createElement('span');
            badge.className = 'tool-alert-badge ms-2';
            row.cells[1].appendChild(badge);
        }
        badge.textContent = 'Transfer Failed: ' + reason;
        badge.classList.add('pulse-failed');
        setTimeout(() => badge.classList.remove('pulse-failed'), 2500);
    }
};
