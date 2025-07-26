// MediaQueueHandler.js
// Sprint 68.0 – Phase 1–2: Media Upload Retry Queue & Background Sync Engine
// Handles failed media uploads, queues in IndexedDB, and background retry logic

const MEDIA_DB_NAME = 'MediaUploadQueueDB';
const MEDIA_STORE_NAME = 'MediaQueue';
const RETRY_INTERVAL_MS = 60000;

function openMediaDb() {
    return new Promise((resolve, reject) => {
        const request = indexedDB.open(MEDIA_DB_NAME, 1);
        request.onupgradeneeded = function (event) {
            const db = event.target.result;
            if (!db.objectStoreNames.contains(MEDIA_STORE_NAME)) {
                db.createObjectStore(MEDIA_STORE_NAME, { keyPath: 'id', autoIncrement: true });
            }
        };
        request.onsuccess = function (event) {
            resolve(event.target.result);
        };
        request.onerror = function (event) {
            reject(event);
        };
    });
}

async function queueMediaUpload(mediaItem) {
    const db = await openMediaDb();
    const tx = db.transaction(MEDIA_STORE_NAME, 'readwrite');
    tx.objectStore(MEDIA_STORE_NAME).add(mediaItem);
    return tx.complete;
}

async function getQueuedMedia() {
    const db = await openMediaDb();
    const tx = db.transaction(MEDIA_STORE_NAME, 'readonly');
    const store = tx.objectStore(MEDIA_STORE_NAME);
    return new Promise((resolve) => {
        const items = [];
        store.openCursor().onsuccess = function (event) {
            const cursor = event.target.result;
            if (cursor) {
                items.push(cursor.value);
                cursor.continue();
            } else {
                resolve(items);
            }
        };
    });
}

async function removeMediaItem(id) {
    const db = await openMediaDb();
    const tx = db.transaction(MEDIA_STORE_NAME, 'readwrite');
    tx.objectStore(MEDIA_STORE_NAME).delete(id);
    return tx.complete;
}

function isOnline() {
    return navigator.onLine;
}

async function flushMediaQueue() {
    if (!isOnline()) return;
    const queued = await getQueuedMedia();
    for (const item of queued) {
        // Replace with actual upload logic
        try {
            const success = await tryUploadMedia(item);
            if (success) {
                await removeMediaItem(item.id);
                showUploadFeedback('Upload complete', 'success');
            } else {
                showUploadFeedback('Queued for retry', 'warning');
            }
        } catch {
            showUploadFeedback('Queued for retry', 'warning');
        }
    }
    updateSyncChip();
}

async function tryUploadMedia(item) {
    // TODO: Implement actual upload logic (AJAX/fetch)
    // Simulate success/failure for now
    return Math.random() > 0.5;
}

function showUploadFeedback(message, status) {
    // Patch UI feedback logic here
    // e.g., display toast or status chip
    const chip = document.getElementById('media-sync-chip');
    if (chip) {
        chip.textContent = message;
        chip.className = status;
    }
}

async function updateSyncChip() {
    const queued = await getQueuedMedia();
    const chip = document.getElementById('media-sync-chip');
    if (chip) {
        chip.textContent = `${queued.length} pending media items`;
        chip.className = queued.length > 0 ? 'warning' : 'success';
    }
}

// Manual retry button handler
window.retryFailedUploads = async function () {
    await flushMediaQueue();
};

// Background flush engine
setInterval(flushMediaQueue, RETRY_INTERVAL_MS);
window.addEventListener('online', flushMediaQueue);
window.addEventListener('DOMContentLoaded', updateSyncChip);

// Export for testing
window.queueMediaUpload = queueMediaUpload;
window.updateSyncChip = updateSyncChip;

// Usage: queueMediaUpload({ TechnicianId, ServiceRequestId, fileMeta, timestamp, UploadIntent });
