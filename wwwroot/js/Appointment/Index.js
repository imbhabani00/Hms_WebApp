$(document).ready(function () {
    LoadAppointmentList();
});

function LoadAppointmentList() {
    let items = $("#ItemsPerPageAppointmentList").val();
    $.ajax({
        url: '/Appointment/GetList',
        type: 'GET',
        data: {
            PageNumber: $("#hdn_AppointmentPage").val(),
            PageSize: items,
        },
        cache: false,
        async: true,
        success: function (data) {
            $("#appointmentListDiv").html(data);
            if (typeof LoadSelect2Picker === 'function') {
                LoadSelect2Picker();
            }
            return true;
        },
        error: function () {
            $("#appointmentListDiv").html('<div class="alert alert-danger text-center"><i class="fas fa-exclamation-triangle me-2"></i>Error loading appointments. Please try again.</div>');
        }
    });
}

// Items per page change
$("#ItemsPerPageAppointmentList").change(function () {
    $("#hdn_AppointmentPage").val("1");
    LoadAppointmentList();
});

// Search with debounce
let searchTimeout;
$("#appointmentSearch").on('keyup', function () {
    clearTimeout(searchTimeout);
    searchTimeout = setTimeout(function () {
        $("#hdn_AppointmentPage").val("1");
        LoadAppointmentList();
    }, 500);
});