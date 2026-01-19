$(document).ready(function () {
    LoadDoctorList();
});

function LoadDoctorList() {
    let items = $("#ItemsPerPageDoctorList").val();
    $.ajax({
        url: '/Doctor/GetList',
        type: 'GET',
        data: {
            PageNumber: $("#hdn_DoctorPage").val(),
            PageSize: items,
        },
        cache: false,
        async: true,
        success: function (data) {
            $("#doctorListDiv").html(data);
            return true;
        },
        error: function () {
            $("#doctorListDiv").html('<div class="alert alert-danger text-center"><i class="fas fa-exclamation-triangle me-2"></i>Error loading doctors. Please try again.</div>');
        }
    });
}

// Items per page change
$("#ItemsPerPageDoctorList").change(function () {
    $("#hdn_DoctorPage").val("1");
    LoadDoctorList();
});

// Search with debounce
let searchTimeout;
$("#doctorSearch").on('keyup', function () {
    clearTimeout(searchTimeout);
    searchTimeout = setTimeout(function () {
        $("#hdn_DoctorPage").val("1");
        LoadDoctorList();
    }, 500);
});