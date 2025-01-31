var dataTable;
$(document).ready(function () {
    loadData();
});

function loadData() {
    dataTable = $("#productTbl").DataTable({
        "ajax": {
            "url": "/Admin/Product/GetData",
            "dataSrc": "data"
        },
        "columns": [
            { "data": "productName" },
            { "data": "productDescription" },
            { "data": "price" },
            { "data": "category.categoryName" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                <a href="/Admin/Product/Edit/${data}" class="btn btn-success">Edit</a>
                <a onClick=DeleteProduct("/Admin/Product/Delete/${data}") class="btn btn-danger">Delete</a>
            `
                }
            }
        ]
    });
}

function DeleteProduct(url) {
    const swalWithBootstrapButtons = Swal.mixin({
        customClass: {
            confirmButton: "btn btn-success",
            cancelButton: "btn btn-danger"
        },
        buttonsStyling: true
    });
    swalWithBootstrapButtons.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonText: "Yes, delete it!",
        cancelButtonText: "No, cancel!",
        reverseButtons: true
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: "Delete",
                success: function (data) {
                    if (data.success) {
                        dataTable.ajax.reload();
                        toastr.options = {
                            "closeButton": true,
                            "debug": false,
                            "newestOnTop": true,
                            "progressBar": true,
                            "positionClass": "toast-top-right",
                            "preventDuplicates": false,
                            "onclick": null,
                            "showDuration": "300",
                            "hideDuration": "1000",
                            "timeOut": "5000",
                            "extendedTimeOut": "1000",
                            "showEasing": "swing",
                            "hideEasing": "linear",
                            "showMethod": "fadeIn",
                            "hideMethod": "fadeOut"
                        }
                        toastr["error"](`${data.message}`, "Deleted")
                    } else {
                        toastr.error(data.message);
                    }
                }
            })
            swalWithBootstrapButtons.fire({
                title: "Deleted!",
                text: "Your file has been deleted.",
                icon: "success"
            });
        } else if (
            result.dismiss === Swal.DismissReason.cancel
        ) {
            swalWithBootstrapButtons.fire({
                title: "Cancelled",
                text: "Your file is safe :)",
                icon: "error"
            });
        }
    });
}