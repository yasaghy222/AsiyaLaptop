$(document).ready(() => {
    // #region General
    $('.select').select2({
        minimumResultsForSearch: Infinity,
        width: 'auto'
    });
    //#endregion

    //#region Functions

    //select all items
    function AdminList(tableVar) {
        let url = getUrl('Admin/Get');
        $.ajax({
            url: url,
            type: 'Get',
            data: tableVar,
            success: function (Result) {
                NoRecords(false);
                if (Result.Data.length > 0 && Result.Message == "Success") {
                    let source = $('#adminSource').html(),
                        template = Handlebars.compile(source),
                        list = template({ list: Result.Data });
                    $('#tblAdmin tbody').html(list);
                    $("#count").html(Result.Data.length + " مورد ");
                } else {
                    NoRecords(true);
                }
            },
            error: function (Result) {
                notifiction(3, Result.Message);
            }
        })
    }

    //delete
    function Delete(id) {
        swal({
            title: "آیا از انجام این کار اطمینان دارید ؟",
            text: "شما قادر به بازیابی دوباره این رکورد نخواهید بود !",
            type: "warning",
            showCancelButton: true,
            confirmButtonColor: "#EF5350",
            confirmButtonText: "بله ، حذف شود",
            cancelButtonText: "خیر ، لغو شود",
            closeOnConfirm: true,
            closeOnCancel: true
        }, (isConfirm) => {
            if (isConfirm) {
                let url = getUrl("Admin/Delete");
                $.ajax({
                    url: url,
                    type: 'POST',
                    data: id,
                    processData: false,
                    contentType: "application/json",
                    success: function (Result) {
                        if (Result.Message == "Success") {
                            AdminList();
                            swal({
                                title: "عملیات حذف انجام شد .",
                                text: "رکورد مد نظر شما با موفقیت حذف گردید .",
                                confirmButtonColor: "#66BB6A",
                                confirmButtonText: "باشه",
                                type: "success"
                            });
                        }
                        else {
                            notifiction(1, Result.Message);
                        }
                    },
                    error: function (Result) {
                        notifiction(1, Result.Message);
                    }
                });
            }
            else {
                swal({
                    title: "در خواست لغو شد.",
                    text: "رکورد شما در صحت کامل قرار دارد :)",
                    confirmButtonColor: "#66BB6A",
                    confirmButtonText: "باشه",
                    type: "success"
                });
            }
        });
    }
    //#endregion

    //#region Usage
    AdminList();
    $('.panel a[data-action="reload"]').on('click', () => {
        AdminList();
    });
    $('.panel').delegate('#del', 'click', (e) => {
        let id = $(e.currentTarget).attr("data-id");
        Delete(id);
    });
    //#endregion
});

