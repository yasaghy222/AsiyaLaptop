$(document).ready(() => {
    // #region General
    let ProcCatList = {};
    //#endregion

    //#region Functions
    function CatList(pid) {
        if (pid == undefined) {
            let url = getUrl("Product/CatList");
            $.ajax({
                url: url,
                type: "Get",
                success: (Result) => {
                    if (Result.Message == "Success") {
                        ProcCatList = Result.Data;
                        let source = $('#procCatSource').html(),
                            template = Handlebars.compile(source),
                            parentList = $.grep(ProcCatList, (item) => {
                                return (item.PID == null)
                            }),
                            list = template({ list: parentList });
                        $('#TblProcCat tbody').html(list);
                    }
                    else {
                        notifiction(1, Result.Message);
                    }
                },
                error: (Result) => {
                    notifiction(1, Result.Message);
                }
            });
        }
        else {
            let source = $('#procCatSource').html(),
                template = Handlebars.compile(source),
                childList = $.grep(ProcCatList, (item) => {
                    return (item.PID == pid);
                }),
                list = template({ list: childList });
            $('#' + pid + ' td table tbody').html(list);
        }
    };
    function delCat(id) {
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
                let url = getUrl("Product/DelCat");
                $.ajax({
                    url: url,
                    type: 'POST',
                    data: id,
                    processData: false,
                    contentType: "application/json",
                    success: function (Result) {
                        if (Result.Message == "Success") {
                            CatList();
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
    CatList();
    $('.panel a[data-action="reload"]').on('click', () => {
        CatList();
    });
    $(".panel").delegate("#getChild", "click", (e) => {
        let PID = $(e.currentTarget).attr('data-id');
        CatList(PID);
    });
    $(".panel").delegate("#del", "click", (e) => {
        let id = $(e.currentTarget).attr("data-id"),
            hasChild = $(e.currentTarget).attr("data-hasChild"),
            assignCount = $(e.currentTarget).attr("data-assignCount");
        if (hasChild == "false" && assignCount == 0) {
            delCat(id);
        } else {
            notifiction(2, "شما تنها می توانید دسته هایی را که فرزند و اختصاصی ندارند را حذف نمایید.");
        }
    });
    //#endregion
});

