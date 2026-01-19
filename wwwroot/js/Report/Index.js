$(document).ready(function () {
    LoadReportList();
});

function LoadReportList() {
    let items = $("#ItemsPerPageReportList").val();
    $.ajax({
        url: '/Report/GetList',
        type: 'GET',
        data: {
            PageNumber: $("#hdn_ReportPage").val(),
            PageSize: items,
            searchTerm: $("#reportSearch").val(),
            reportType: $("#reportTypeFilter").val()
        },
        cache: false,
        async: true,
        success: function (data) {
            $("#reportListDiv").html(data);
            return true;
        },
        error: function () {
            $("#reportListDiv").html('<div class="alert alert-danger text-center"><i class="fas fa-exclamation-triangle me-2"></i>Error loading reports. Please try again.</div>');
        }
    });
}

function generateReport() {
    window.open('/Report/Generate?type=all', '_blank');
}

// Items per page change
$("#ItemsPerPageReportList").change(function () {
    $("#hdn_ReportPage").val("1");
    LoadReportList();
});

$("#reportTypeFilter").change(function () {
    $("#hdn_ReportPage").val("1");
    LoadReportList();
});

let searchTimeout;
$("#reportSearch").on('keyup', function () {
    clearTimeout(searchTimeout);
    searchTimeout = setTimeout(function () {
        $("#hdn_ReportPage").val("1");
        LoadReportList();
    }, 500);
});