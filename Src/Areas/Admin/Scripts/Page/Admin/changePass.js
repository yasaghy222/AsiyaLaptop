$(document).ready(() => {
    // #region General
    $('.maxlength').maxlength({
        placement: 'bottom-left'
    });
    //#region ChangePass
    let frmChangePass = $("#FrmChangePass");
    function saveFrmChangePass(e) {
        let isValid = frmChangePass.valid(),
            targetId = $(e.currentTarget).attr("id");
        if (isValid) {
            btnSaveOnOff();
            startAnimate(".tab-content");
            let data = {
                OldPass: $("#OldPass").val(),
                NewPass: $("#NewPass").val(),
                ConfirmNewPass: $("#ConfirmNewPass").val(),
            };
            let url = getUrl("Account/ChangePass");
            $.ajax({
                url: url,
                type: 'POST',
                data: JSON.stringify(data),
                ChangePassessData: false,
                contentType: "application/json",
                success: function (Result) {
                    if (Result.Message == "Success") {
                        finishAnimate(".tab-content");
                        btnSaveOnOff();
                        notifiction(0, "عمیات با موفقیت انجام شد.");
                        if (targetId == "btnSaveNext") {
                            pageLoad('/Admin/Dashboard', 'پنل مدریت لپتاپ آسیا',);
                        }
                    } else {
                        notifiction(1, Result.Message);
                        finishAnimate(".tab-content");
                    }
                },
                error: function (Result) {
                    finishAnimate(".tab-content");
                    notifiction(1, Result.Message);
                }
            });
        }
    }
    $("#btnSave").on("click", (e) => {
        saveFrmChangePass(e);
    });
    $("#btnSaveNext").on("click", (e) => {
        saveFrmChangePass(e);
    });
    //#endregion
});
