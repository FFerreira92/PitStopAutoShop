function onOverlayClick() {
    var dialog = document.getElementById("dialog").ej2_instances[0];
    dialog.hide();
};

function onAddEstimateButtonClick() {
    debugger;
    var dialog = document.getElementById("dialog").ej2_instances[0];
    dialog.header = "Add Estimate";
    dialog.content = "New client?";
    dialog.show();
};

function ondlgYesButtonClick() {
    window.location.href = "/Customer/Create";
};

function ondlgNoButtonClick() {
    window.location.href = "/Customer";
};
