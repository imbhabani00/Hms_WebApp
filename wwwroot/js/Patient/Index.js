$(document).ready(function () {
    LoadPatientList();
});

function LoadPatientList() {
    let items = $("#ItemsPerPagePatientList").val();
    $.ajax({
        url: '/Patient/GetList',
        type: 'GET',
        data: {
            PageNumber: $("#hdn_PatientPage").val(),
            PageSize: items,
        },
        cache: false,
        async: true,
        success: function (data) {
            $("#patientListDiv").html(data);
            return true;
        },
        error: function () {
            $("#patientListDiv").html('<div class="alert alert-danger text-center"><i class="fas fa-exclamation-triangle me-2"></i>Error loading patients. Please try again.</div>');
        }
    });
}

// Items per page change
$("#ItemsPerPagePatientList").change(function () {
    $("#hdn_PatientPage").val("1");
    LoadPatientList();
});

// Search with debounce
let searchTimeout;
$("#patientSearch").on('keyup', function () {
    clearTimeout(searchTimeout);
    searchTimeout = setTimeout(function () {
        $("#hdn_PatientPage").val("1");
        LoadPatientList();
    }, 500);
});