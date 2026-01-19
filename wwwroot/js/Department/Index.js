$(document).ready(function () {
    LoadDepartmentList();
});

function LoadDepartmentList() {
    let items = $("#ItemsPerPageDepartmentList").val();
    $.ajax({
        url: '/Department/GetList',
        type: 'GET',
        data: {
            PageNumber: $("#hdn_DepartmentPage").val(),
            PageSize: items,
        },
        cache: false,
        async: true,
        success: function (data) {
            $("#departmentListDiv").html(data);
            return true;
        },
        error: function () {
            $("#departmentListDiv").html('<div class="alert alert-danger text-center"><i class="fas fa-exclamation-triangle me-2"></i>Error loading departments. Please try again.</div>');
        }
    });
}

// Items per page change
$("#ItemsPerPageDepartmentList").change(function () {
    $("#hdn_DepartmentPage").val("1");
    LoadDepartmentList();
});

// Search with debounce
let searchTimeout;
$("#departmentSearch").on('keyup', function () {
    clearTimeout(searchTimeout);
    searchTimeout = setTimeout(function () {
        $("#hdn_DepartmentPage").val("1");
        LoadDepartmentList();
    }, 500);
});