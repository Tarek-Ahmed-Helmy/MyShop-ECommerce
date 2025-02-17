var dataTable;
$(document).ready(function () {
    loadData();
});

function loadData() {
    dataTable = $("#orderTbl").DataTable({
        "ajax": {
            "url": "/Admin/Order/GetData",
            "dataSrc": "data"
        },
        "columns": [
            { "data": "id" },
            { "data": "fullName" },
            { "data": "phoneNumber" },
            { "data": "applicationUser.email" },
            { "data": "orderStatus" },
            { "data": "totalAmount" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                <a href="/Admin/Order/Details/${data}" class="btn btn-warning">Details</a>
            `
                }
            }
        ]
    });
}