$(document).ready(() => {
    // #region General
    $('.select').select2({
        minimumResultsForSearch: Infinity
    });
    //#endregion

    //#region Functions
    function ChangeStatus(e) {
        var targetId = $(e.currentTarget).attr("id"),
            id = $(e.currentTarget).attr("data-id"),
            status = $("#Status").val();
        swal({
            title: "آیا از انجام این کار اطمینان دارید ؟",
            type: "warning",
            showCancelButton: true,
            confirmButtonColor: "#EF5350",
            confirmButtonText: "بله ، اعمال شود",
            cancelButtonText: "خیر ، لغو شود",
            closeOnConfirm: true,
            closeOnCancel: true
        }, (isConfirm) => {
                if (isConfirm) {
                    let url = getUrl("Order/ChangeStatus"),
                        data = { ID: parseInt(id), Status: parseInt(status) };
                $.ajax({
                    url: url,
                    type: 'POST',
                    data: JSON.stringify(data),
                    processData: false,
                    contentType: "application/json",
                    success: function (Result) {
                        if (Result.Message == "Success") {
                            notifiction(0, "عمیات با موفقیت انجام شد.");
                            pageLoad('/Admin/Order', 'لیست سفارش ها', 'orderList');
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
                    confirmButtonColor: "#66BB6A",
                    confirmButtonText: "باشه",
                    type: "success"
                });
            }
        });
    }
    //#endregion

    //#region Usage
    $("#btnSaveNext").on("click", (e) => {
        ChangeStatus(e);
    });
    //#endregion
});

