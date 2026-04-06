//EMAILEXPORT//
import { AddAction } from './export-utils.js';
var EmailExportInputs;
var RecieverAddress;
var Subject;
var Message;
var SenderAddress;
var Host;

function EMAILExport(webReport, SuccessMessage, FailureMessage) {
    var url = `/_fr/preview.sendEmail?reportId=${webReport.ID}`;

    var formData = new FormData();
    formData.append("Address", document.getElementById("Email").value);
    formData.append("Subject", document.getElementById("Subject").value);
    formData.append("MessageBody", document.getElementById("Message").value);
    formData.append("ExportFormat", document.getElementById("ExportFormat").value);
    formData.append("NameAttachmentFile", document.getElementById("NameAttachmentFile").value);

    fetch(url, {
        method: 'POST',
        body: formData
    })
        .then(response => {
            if (response.ok) {
                webReport.showPopup(`${SuccessMessage}`, true);
            } else {
                webReport.showPopup(`${FailureMessage}`, false);
            }
        })
        .catch(error => {
            webReport.showPopup(`${FailureMessage}`, false);
        })
}

AddAction('EMAILExport', EMAILExport);
