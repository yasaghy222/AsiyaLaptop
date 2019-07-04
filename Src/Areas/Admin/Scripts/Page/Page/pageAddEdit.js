$(document).ready(() => {
    // #region General
    $(".styled").uniform({
        radioClass: 'choice'
    });
    $(".select").select2();
    $(".touchspin").TouchSpin({
        max: 200000000
    });
    $('.maxlength').maxlength({
        placement: 'bottom-left'
    });

    $('.summernote').summernote();
    $(".link-dialog input[type=checkbox], .note-modal-frmPage input[type=radio]").uniform({
        radioClass: 'choice'
    });

    $(".note-image-input").uniform({
        fileButtonClass: 'action btn bg-slate'
    });

    $('.tokenfield').tokenfield();
    //#endregion

    //#region PageAddEdit
    let frmPage = $("#FrmPage");
    function saveFrmPage(e) {
        let isValid = frmPage.valid(),
            targetId = $(e.currentTarget).attr("id");

        if (isValid) {
            btnSaveOnOff();
            startAnimate(".tab-content");
            let data = {
                ID: $("#ID").val(),
                Title: $("#Title").val(),
                Link: $("#Link").val(),
                SeoDesc: $("#SeoDesc").val(),
                Keywords: $("#Keywords").val(),
                Body: $("#Body").val(),
                Status: $("#Status").val()
            },
                url = getUrl("Page/AddEdit");
            $.ajax({
                url: url,
                type: 'POST',
                data: JSON.stringify(data),
                pageessData: false,
                contentType: "application/json",
                success: function (Result) {
                    if (Result.Message == "Success") {
                        finishAnimate(".tab-content");
                        btnSaveOnOff();
                        notifiction(0, "عمیات با موفقیت انجام شد.");
                        if (targetId == "btnSaveNext") {
                            pageLoad('/Admin/Page', 'لیست صفحات', 'pageList');
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
        saveFrmPage(e);
    });
    $("#btnSaveNext").on("click", (e) => {
        saveFrmPage(e);
    });
    //#endregion
});
