$(document).ready(() => {
    //#region Functions
    //select all items
    function PageList() {
        let url = getUrl('Page/Get');
        $.ajax({
            url: url,
            type: 'Get',
            success: function (Result) {
                NoRecords(false);
                if (Result.Data.length > 0 && Result.Message == "Success") {
                    pageList = Result.Data;
                    let source = $('#pageSource').html(),
                        template = Handlebars.compile(source);
                    ParentList = $.grep(pageList, (item) => {
                        return (item.PID == null)
                    });
                    let list = template({ list: ParentList });
                    $('#TblPage tbody').html(list);
                    $("#count").html(`${Result.Data.length} مورد`);
                } else {
                    NoRecords(true);
                }
            },
            error: function (Result) {
                notifiction(3, Result.Message);
            }
        });
    }

    //delete page link
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
                let url = getUrl("Page/Delete");
                $.ajax({
                    url: url,
                    type: 'POST',
                    data: id,
                    processData: false,
                    contentType: "application/json",
                    success: function (Result) {
                        if (Result.Message == "Success") {
                            PageList();
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
    PageList();
    $('.panel a[data-action="reload"]').on('click', () => {
        PageList()
    });
    $(".panel").delegate("#del", "click", (e) => {
        let id = $(e.currentTarget).attr("data-id");
        Delete(id);
    });
    //#endregion
});

