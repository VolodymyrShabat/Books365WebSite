
var dataTable;

$(document).ready(function () {
    loadDataTable();
});
function notify() {
    toastr.info('Successfully updated');
}

function loadDataTable() {
    dataTable = $('#dataTable2').DataTable({
        "ajax": {
            "url": "/Home/GetUserBooks",
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "book.isbn", "width": "5%" },
            { "data": "book.title", "width": "10%" },
            { "data": "book.genre", "width": "10%" },
            { "data": "book.pages", "width": "10%" },
            { "data": "book.author", "width": "15%" },
            { "data": "status.status", "width": "15%" },
            { "data": "status.pagesRead", "width": "5%" },
            { "data": "status.dateStarted", "width": "10%" },

            {
                "data": "book.isbn",
                "render": function (data) {
                    return `
                            <div class="text-center">
                                <a href="/Home/Upsert/${data}" class="tableButton btn btn-info text-white" style="cursor:pointer">
                                    <i class="fas fa-edit"></i> 
                                </a>
                                <a onclick=Delete("/Home/Delete/${data}") class="tableButton btn btn-dark text-white" style="cursor:pointer">
                                    <i class="fas fa-trash"></i>  
                                </a>
                            </div>
                           `;
                }, "width": "20%"
            }

        ],
        "language": {
            "emptyTable": "No data found"
        },
        "width": "100%"
    });
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
