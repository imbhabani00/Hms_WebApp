// Modal Popup (Bootstrap 5)
function LoadPopUp(fitScreen, url) {
    const modalEl = document.getElementById('myModal');

    // Close existing modal
    if (modalEl.classList.contains('show')) {
        bootstrap.Modal.getInstance(modalEl).hide();
        document.body.classList.remove('modal-open');
        document.querySelector('.modal-backdrop')?.remove();
    }

    $('#myModalContent').empty().load(url, function () {
        const newModal = new bootstrap.Modal(modalEl, {
            keyboard: false,
            backdrop: 'static'
        });
        newModal.show();
    });

    // Set modal size
    const $dialog = $(".modal-dialog");
    $dialog.removeClass('modal-sm modal-md modal-lg modal-xl modal-dialog-centered')
        .css('max-width', '');

    switch (fitScreen) {
        case "Modalxl": $dialog.addClass('modal-xl'); break;
        case "Modallg": $dialog.addClass('modal-lg'); break;
        case "Modalmd": $dialog.addClass('modal-md'); break;
        case "Modalsm": $dialog.addClass('modal-sm'); break;
        case "modalDelete":
            $dialog.addClass('modal-dialog-centered').css('max-width', '400px');
            break;
        default: $dialog.addClass('modal-lg'); break;
    }
    return false;
}

function CloseModalPopUp(modalClass) {
    $(modalClass).modal('hide');
}

function LoadDropDown(url, controlId, prefillValue, defaultText) {
    $.ajax({
        url: url, type: "GET", async: true,
        headers: { 'Authorization': common.GetToken() },
        success: function (data) {
            const $select = $('#' + controlId).empty();
            $select.append(defaultText ? `<option value="0">${defaultText}</option>` : '<option value="">Select</option>');

            $.each(data.lookupResults, function (i, item) {
                const selected = item.id == prefillValue ? 'selected' : '';
                $select.append(`<option value="${item.id}" data-code="${item.code || ''}" ${selected}>${item.name}</option>`);
            });
        }
    });
}

function LoadDropDownMultiple(url, controlId, prefillValue, defaultText) {
    $.ajax({
        url: url, type: "GET", async: false,
        headers: { 'Authorization': common.GetToken() },
        success: function (data) {
            const $select = $('#' + controlId).empty();
            $select.append(defaultText ? `<option value="0">${defaultText}</option>` : '<option value="">Select</option>');

            const prefill = prefillValue ? prefillValue.split(',') : [];
            $.each(data.lookupResults, function (i, item) {
                const selected = prefill.includes(item.id.toString()) ? 'selected' : '';
                $select.append(`<option value="${item.id}" ${selected}>${item.name}</option>`);
            });
        }
    });
}

function ShowToasterMessage(type, message) {
    toastr.options = {
        "closeButton": true,
        "progressBar": true,
        "positionClass": "toast-top-right",
        "preventDuplicates": true,
        "timeOut": "4000",
        "showDuration": "300",
        "hideDuration": "300"
    };

    switch (type) {
        case 'success':
            toastr.success(message);
            break;
        case 'error':
            toastr.error(message);
            break;
        case 'warning':
            toastr.warning(message);
            break;
    }
}



function isNumber(evt) {
    const charCode = evt.charCode || evt.keyCode;
    const value = evt.target.value;
    return (charCode == 46 && value.indexOf('.') == -1) || (charCode >= 48 && charCode <= 57);
}

function onlyNumbers(evt) {
    return (evt.charCode >= 48 && evt.charCode <= 57);
}

function noLeadingSpace(evt) {
    return evt.charCode !== 32 || evt.target.selectionStart > 0;
}

function initSelect2() {
    $('.select2').select2({ dropdownParent: $(this).parent() });
}

var common = {
    GetToken() {
        return document.cookie.split(';').reduce((r, v) => {
            const parts = v.split('=');
            return parts.shift().trim() == 'AccessToken' ?
                `Bearer ${decodeURIComponent(parts.join('='))}` : r;
        }, null);
    }
};
