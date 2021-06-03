
var dataTable;

$(document).ready(function () {
    loadDataTable();
});
function notify() {
    toastr.info('Successfully updated');
}

function loadDataTable() {
    dataTable = $('#dataTable').DataTable({
        "ajax": {
            "url": "/Home/Books",
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "isbn", "width": "10%" },
            { "data": "title", "width": "25%" },
            { "data": "genre", "width": "20%" },
            { "data": "pages", "width": "20%" },
            { "data": "author", "width": "15%" },
            {
                "data": "isbn",
                "render": function (data) {
                    return `
                            <div class="text-center">
                                <a href="/Home/AddStatus/${data}" onclick='Success()' class="tableButton btn btn-danger text-white" style="cursor:pointer">
                                    <i class="fas fa-plus"></i>
                                </a>
                            </div>
                           `;
                }, "width": "10%"
            }

        ],
        "language": {
            "emptyTable": "No data found"
        },
        "width": "100%"
    });
}


function Success() {
    alert('You successfully added new book into your reading statuses')
}

function Delete(url) {
    swal({
        title: "Are you sure?",
        text: "One deleted, you will not be able to recover",
        icon: "warning",
        buttons: true,
        dangerMode: true
    }).then((willDelete => {
        if (willDelete) {
            $.ajax({
                type: "DELETE",
                url: url,
                success: function (data) {
                    if (data.success) {
                        toastr.options = {
                            "closeButton": true,
                            "debug": false,
                            "newestOnTop": false,
                            "progressBar": true,
                            "preventDuplicates": true,
                            "onclick": null,
                            "showDuration": "100",
                            "hideDuration": "500",
                            "timeOut": "2000",
                            "extendedTimeOut": "500",
                            "showEasing": "swing",
                            "hideEasing": "linear",
                            "showMethod": "show",
                            "hideMethod": "hide",
                            "positionClass": 'toast-bottom-right'
                        };
                        toastr.info(data.msg);
                        dataTable.ajax.reload();
                    }
                }
            })
        }
    }))
}













function Toast(type, css, msg) {
    this.type = type;
    this.css = css;
    this.msg = msg;
}

