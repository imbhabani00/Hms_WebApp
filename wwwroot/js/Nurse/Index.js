$(document).ready(function () {
    LoadNurseList();
});

function LoadNurseList() {
    let items = $("#ItemsPerPageNurseList").val();
    $.ajax({
        url: '/Nurse/GetList',
        type: 'GET',
        data: {
            PageNumber: $("#hdn_NursePage").val(),
            PageSize: items,
        },
        cache: false,
        async: true,
        success: function (data) {
            $("#nurseListDiv").html(data);
            if (typeof LoadSelect2Picker === 'function') {
                LoadSelect2Picker();
            }
            return true;
        },
        error: function () {
            $("#nurseListDiv").html('<div class="alert alert-danger text-center"><i class="fas fa-exclamation-triangle me-2"></i>Error loading nurses. Please try again.</div>');
        }
    });
}

// Items per page change
$("#ItemsPerPageNurseList").change(function () {
    $("#hdn_NursePage").val("1");
    LoadNurseList();
});

// Search with debounce
let searchTimeout;
$("#nurseSearch").on('keyup', function () {
    clearTimeout(searchTimeout);
    searchTimeout = setTimeout(function () {
        $("#hdn_NursePage").val("1");
        LoadNurseList();
    }, 500);
});