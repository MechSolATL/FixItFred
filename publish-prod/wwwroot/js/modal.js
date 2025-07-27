// modal.js: Handles modal loading for service requests
function openModal(serviceType) {
    fetch('/ModalQuestion?handler=Get&service=' + serviceType)
        .then(function (response) {
            if (!response.ok) throw new Error('Modal fetch failed');
            return response.text();
        })
        .then(function (html) {
            document.getElementById("modalHost").innerHTML = html;
        })
        .catch(function (err) {
            console.error("Modal load error:", err);
            alert("Could not load service request at this time.");
        });
}
