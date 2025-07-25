﻿var datatable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    datatable = $('#tblData').DataTable({
        "ajax": { url: '/admin/product/getall' },
        "columns": [ // column names are case sensitive, must match the names that the api returns
            { data: "title", "width": "20%" },
            { data: "isbn", "width": "15%" },
            { data: "listPrice", "width": "10%" },
            { data: "author", "width": "15%" },
            { data: "category.name", "width": "15%" },
            {
                data: 'id',
                "render": function (data) {
                    return `<div class = "w-75 btn-group" role="group">
                            <a href="/Admin/Product/Upsert?id=${data}" class = "btn btn-primary mx-2"> <i class="bi bi-pencil-square"></i> Edit<a/>
                            <a onClick=Delete("/Admin/Product/Delete?id=${data}") class="btn btn-danger mx-2"><i class="bi bi-trash"></i> Delete </a>
                            <div/>`
                },
                width:"25%"
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
            $.ajax({
                url: url,
                type: "DELETE",
                success: function (data) {
                    datatable.ajax.reload(); // reload the datatable
                    toastr.success(data.message);
                }
            })
        }
    });
}
