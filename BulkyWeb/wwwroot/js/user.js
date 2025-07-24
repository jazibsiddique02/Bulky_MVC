var datatable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    datatable = $('#tblData').DataTable({
        "ajax": { url: '/admin/user/getall' },
        "columns": [ // column names are case sensitive, must match the names that the api returns
            { data: "name", "width": "20%" },
            { data: "email", "width": "15%" },
            { data: "phoneNumber", "width": "10%" },
            { data: "company.name", "width": "15%" },
            { data: "role", "width": "10%" },
            {
                data: { id: 'id', lockoutEnd: "lockoutEnd" },
                "render": function (data) {
                    var today = new Date().getTime();
                    var lockout = new Date(data.lockoutEnd).getTime();

                    if (lockout > today) {
                        // user is locked and we have to unlock.
                        return `<div class="text-center">
                                <a onclick=LockUnlock('${data.id}') class="btn btn-success text-white" style="cursor:pointer; width: 100px;">
                                    <i class="bi bi-unlock-fill"></i> Unlock
                                </a>
                                <a href="/admin/user/rolemanagement?userId=${data.id}" class="btn btn-danger text-white" style="cursor:pointer; width: 150px;">
                                    <i class="bi bi-pencil-square"></i> Permission
                                </a>
                                </div>`
                    }
                    else {
                        // user is unlock and we have to lock him for some reason.
                        return `<div class="text-center">
                                <a onclick=LockUnlock('${data.id}')  class="btn btn-danger text-white" style="cursor:pointer; width: 100px;">
                                    <i class="bi bi-lock-fill"></i> Lock
                                </a>
                                <a href="/admin/user/rolemanagement?userId=${data.id}" class="btn btn-danger text-white" style="cursor:pointer; width: 150px;">
                                    <i class="bi bi-pencil-square"></i> Permission
                                </a>
                                </div>`
                    }


                },
                width: "30%"
            }

        ]
    });
}

function LockUnlock(id) {
    $.ajax({
        type: "POST",
        url: `User/LockUnlock`,
        data: JSON.stringify(id),
        contentType: "application/json",
        success: function (data) {
            if (data.success) {
                datatable.ajax.reload();
                toastr.success(data.message);
            }
        }
    });
}



