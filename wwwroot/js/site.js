// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

window.downloadFileFromBase64 = (fileName, base64) => {
    const link = document.createElement('a');
    link.download = fileName;
    link.href = 'data:application/json;base64,' + base64;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
};

// FixItFred Patch Log — Sprint 29B: ETA Real-Time Display Patch
// [2025-07-25T00:00:00Z] — Appended SignalR connection and ReceiveETA listener for ETA updates.

// FixItFred: SignalR ETA real-time listener
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/etaHub")
    .configureLogging(signalR.LogLevel.Information)
    .build();

connection.on("ReceiveETA", function (zoneId, message) {
    var etaDiv = document.getElementById("etaDisplay");
    if (etaDiv) {
        etaDiv.innerText = message + " (Last updated: " + new Date().toLocaleTimeString() + ")";
    }
});

connection.start().catch(function (err) {
    console.error("SignalR connection error: ", err.toString());
});
// /FixItFred

// FixItFred Patch Log — Sprint 29B: Hardened SignalR ETA UX
// [2025-07-25T00:00:00Z] — Added reconnect logic, null safety, and fallback UI for ETA display.

// FixItFred: Hardened SignalR reconnect
let retryAttempts = 0;
let maxRetries = 5;

async function startConnection() {
    try {
        await connection.start();
        console.log("SignalR connected.");
        retryAttempts = 0; // Reset
    } catch (err) {
        console.warn("SignalR connection failed. Retrying...", err);
        retryAttempts++;
        if (retryAttempts <= maxRetries) {
            setTimeout(startConnection, 2000);
        } else {
            const display = document.getElementById("etaDisplay");
            if (display) display.innerText = "ETA unavailable. Please check back.";
        }
    }
}

connection.onclose(() => {
    console.warn("SignalR disconnected. Attempting reconnect...");
    startConnection();
});

connection.on("ReceiveETA", function (zoneId, message) {
    const display = document.getElementById("etaDisplay");
    if (display && message) {
        display.innerText = message + " (Last updated: " + new Date().toLocaleTimeString() + ")";
    }
});

startConnection(); // 🔁 Initial connect
// /FixItFred

// FixItFred Patch Log — Sprint 29B-Expand: Multi-Zone ETA + History
// [2025-07-25T00:00:00Z] — Multi-zone ETA display, history tracking, and responsive support.

// FixItFred: Multi-Zone ETA Broadcast Handler
connection.on("ReceiveETA", function (zoneId, message) {
    const container = document.getElementById("etaContainer");
    if (!container) return;

    let zoneDiv = document.getElementById("eta-" + zoneId);
    const timestamp = new Date().toLocaleTimeString();

    if (!zoneDiv) {
        zoneDiv = document.createElement("div");
        zoneDiv.id = "eta-" + zoneId;
        container.appendChild(zoneDiv);
    }
    zoneDiv.innerText = `Zone ${zoneId} ETA: ${message} (Updated: ${timestamp})`;

    // FixItFred: ETA History Tracking
    let history = document.getElementById("eta-history-" + zoneId);
    if (!history) {
        history = document.createElement("ul");
        history.id = "eta-history-" + zoneId;
        container.appendChild(history);
    }
    const historyItem = document.createElement("li");
    historyItem.innerText = `→ ${timestamp}: ${message}`;
    history.prepend(historyItem);
});
// /FixItFred

// FixItFred Patch Log — Sprint 77.1: SignalR NotificationHub connection for real-time alerts
const notificationConnection = new signalR.HubConnectionBuilder()
    .withUrl("/notificationHub")
    .configureLogging(signalR.LogLevel.Information)
    .build();

notificationConnection.on("ReceiveNotification", function (message, severity) {
    var area = document.getElementById('notificationArea');
    if (area) {
        area.textContent = message;
        area.classList.remove('d-none', 'alert-warning', 'alert-danger', 'alert-info');
        area.classList.add(severity === 'Severe' ? 'alert-danger' : severity === 'Info' ? 'alert-info' : 'alert-warning');
        setTimeout(function() { area.classList.add('d-none'); }, 10000);
    }
});

async function startNotificationConnection() {
    try {
        await notificationConnection.start();
        console.log("SignalR NotificationHub connected.");
    } catch (err) {
        console.warn("SignalR NotificationHub connection failed. Retrying...", err);
        setTimeout(startNotificationConnection, 2000);
    }
}
notificationConnection.onclose(() => {
    startNotificationConnection();
});
startNotificationConnection();
// /FixItFred
