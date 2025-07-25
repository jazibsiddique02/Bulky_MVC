﻿var dataTable;

$(document).ready(function () {
    let url = window.location.search  // to get url
    if (url.includes("pending")) {
        loadDataTable("pending");
    }
    else if (url.includes("completed")) {
        loadDataTable("completed");
    }
    else if (url.includes("approved")) {
        loadDataTable("approved");
    }
    else if (url.includes("inprocess")) {
        loadDataTable("inprocess");
    }
    else {
        loadDataTable("all");
    }
})

function loadDataTable(status) {
    dataTable = $('#tblData').DataTable({
        "ajax": { url: "/admin/order/getall?status=" + status },
        "columns": [
            { data: "id", width: "10%" },
            { data: "name", width: "15%" },
            { data: "phoneNumber", width: "15%" },
            { data: "applicationUser.email", width: "15%" },
            { data: "orderStatus", width: "10%" },
            { data: "orderTotal", width: "10%" },
            {
                data: "id",
                render: function (data) {
                    return `<div class="w-75 btn-group" role="group">
                        <a href="/Admin/Order/Details?orderId=${data}" class="btn btn-primary mx-2"> <i class="bi bi-pencil-square"></i> <a />
                    </div>`
                }
            }
        ]
    })
}