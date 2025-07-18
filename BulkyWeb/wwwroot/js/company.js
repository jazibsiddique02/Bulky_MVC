﻿var dataTable;

$(document).ready(function() {
    loadDataTable();
})

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": { url: '/admin/company/getall' },
        "columns": [
            { data: "name", "width": "15%"},
            { data: "streetAddress", "width": "15%"},
            { data: "city", "width": "10%"},
            //{ data: "state", "width": "15%"},
            //{ data: "postalCode", "width": "15%"},
            { data: "phoneNumber", "width": "10%" },
            {
                data: "id",
                "render": function (data) {
                    return `<div class="btn-group w-75" role="group">
                    <a href="/Admin/Company/Upsert?id=${data}" class = "btn btn-primary mx-2"> <i class="bi bi-pencil-square"></i>Edit</a>
                    <a onClick=Delete("/Admin/Company/Delete?id=${data}") class="btn btn-danger mx-2"><i class="bi bi-trash"></i>Delete</a>
                    </div>`
                },
                "width": "30%"
            }
        ]
    });
}


function Delete(url) {
    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax ({
                url: url,
                type: "DELETE",
                success: function (data) {
                    dataTable.ajax.reload(); // reload the datatable
                    toastr.success(data.message);
                }
            })
        }
    })
}



