$(document).ready(() => {
    // #region General
    $(".select").select2();
    $('.maxlength').maxlength({
        placement: 'bottom-left'
    });
    //#region AdminAddEdit
    let frmAdmin = $("#FrmAdmin");
    function saveFrmAdmin(e) {
        let isValid = frmAdmin.valid(),
            targetId = $(e.currentTarget).attr("id");
        if (isValid) {
            btnSaveOnOff();
            startAnimate(".tab-content");
            let data = {
                ID: $("#ID").val(),
                Name: $("#Name").val(),
                Family: $("#Family").val(),
                Phone: $("#Phone").val(),
                NatCode: $("#NatCode").val(),
                BDate: $("#BDate").val(),
                Pass: $("#Pass").val(),
                Status: $("#Status").val()
            };
            let url = getUrl("Admin/AddEdit");
            $.ajax({
                url: url,
                type: 'POST',
                data: JSON.stringify(data),
                AdminessData: false,
                contentType: "application/json",
                success: function (Result) {
                    if (Result.Message == "Success") {
                        finishAnimate(".tab-content");
                        btnSaveOnOff();
                        notifiction(0, "عمیات با موفقیت انجام شد.");
                        if (targetId == "btnSaveNext") {
                            pageLoad('/Admin/Admin', 'لیست مدیران', 'adminList');
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
    $(".content-wrapper").delegate("#Status", "click", () => {
        let Status = $("#Status");
        if (Status.hasClass("checked") == true) {
            Status.val("false");
            Status.removeClass("checked");
        } else {
            Status.val("true");
            Status.addClass("checked");
        };
    });
    $("#btnSave").on("click", (e) => {
        saveFrmAdmin(e);
    });
    $("#btnSaveNext").on("click", (e) => {
        saveFrmAdmin(e);
    });
    //#endregion
});
