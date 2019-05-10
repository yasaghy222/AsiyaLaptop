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
    $(".link-dialog input[type=checkbox], .note-modal-frmProc input[type=radio]").uniform({
        radioClass: 'choice'
    });

    $(".note-image-input").uniform({
        fileButtonClass: 'action btn bg-slate'
    });

    $('.tokenfield').tokenfield();

    let ProcID = $("#FrmProc #ID");

    //tab switch
    $(".tabbable .nav-tabs li a").on("click", (e) => {
        let t = e.currentTarget,
            isActive = $(t).parent().hasClass("active"),
            dataTab = $(t).attr("data-tab"),
            id = $("#ID").val();

        if (!isActive) {
            if (id == "-1") {
                notifiction(2, "ابتدا مشخصات محصول را ثبت نمائید.");
            }
            else {
                swichTab(t);
                btnSaveOnOff();
                switch (dataTab) {
                    case "#Details":
                        btnSaveOnOff();
                        break;
                    case "#Imgs":
                        ImgList();
                        break;
                    case "#Props":
                        PropList();
                }
            }
        }
    });
    //#endregion

    //#region ProcAddEdit
    let frmProc = $("#FrmProc"),
        ProcImg = $("#FrmProc #Image"),
        BrandId = $("#FrmProc #BrandID"),
        frmProcMode = ProcID.val() != -1 ? "Edit" : "Add";

    function saveFrmProc(e) {
        let isFormValid = frmProc.valid(),
            isProcImgValid = validateImage(ProcImg, frmProcMode),
            isBrandValid = validateInput(BrandId, "لطفا یک مورد را انتخاب کنید.", "0"),
            isCatValid = validateInput($("#FrmProc #CatID[checked='checked']"), "لطفا یک مورد را انتخاب کنید.", "0"),
            isValid = (isFormValid && isProcImgValid && isBrandValid && isCatValid) ? true : false,
            targetId = $(e.currentTarget).attr("id");

        if (isValid) {
            btnSaveOnOff();
            startAnimate(".tab-content");

            let frmProcData = new FormData(),
                frmProcValue = frmProc.serializeArray();

            $.each(frmProcValue, (i) => {
                debugger;
                if (frmProcValue[i].name == "FullDesc") {
                    frmProcData.append(frmProcValue[i].name, HtmlEncode($("#FullDesc")));
                }
                else {
                    frmProcData.append(frmProcValue[i].name, frmProcValue[i].value);
                }
            })
            if (ProcImg.val() != null) {
                frmProcData.append("Image", ProcImg[0].files[0]);
            }

            let url = getHost("Product/AddEdit");
            $.ajax({
                url: url,
                type: 'POST',
                data: frmProcData,
                processData: false,
                contentType: false,
                success: function (Resualt) {
                    if (Resualt.Message == "Success") {
                        ProcID.val(Resualt.Data);
                        finishAnimate(".tab-content");
                        btnSaveOnOff();
                        notifiction(0, "عمیات با موفقیت انجام شد.");
                        if (targetId == "btnSaveNext") {
                            pageLoad('/Admin/Product', 'لیست محصولات', 'procList');
                        }
                    } else {
                        notifiction(1, Resualt.Message);
                        finishAnimate(".tab-content");
                    }
                },
                error: function (Resualt) {
                    finishAnimate(".tab-content");
                    notifiction(1, Resualt.Message);
                }
            });
        }
    }

    frmProc.delegate("#Image", "change", (e) => {
        validateImage(ProcImg, frmProcMode);
    });
    frmProc.delegate("#CatID", "change", (e) => {
        $("#FrmProc #CatID[checked='checked']").removeAttr("checked");
        $(e.currentTarget).attr("checked", "checked");
        validateInput($(e.currentTarget), "لطفا یک مورد را انتخاب کنید.", "0");
    });
    frmProc.delegate("#BrandID", "change", (e) => {
        validateInput($(e.currentTarget), "لطفا یک مورد را انتخاب کنید.", "0");
        frmProc.delegate("#BrandID", "change", (e) => {
            validateInput($(e.currentTarget), "لطفا یک مورد را انتخاب کنید.", "0");
        });
    });

    $("#btnSave").on("click", (e) => {
        saveFrmProc(e);
    });
    $("#btnSaveNext").on("click", (e) => {
        saveFrmProc(e);
    });
    //#endregion

    //#region ProcImg
    let modal = $("#addEditImg"),
        modalTitle = $("#addEditImg .modal-title"),
        frmProcImg = $("#FrmProcImg"),
        ImgID = $("#FrmProcImg #ID"),
        ImgTitle = $("#FrmProcImg #ImgTitle"),
        DemoImg = $("#FrmProcImg #DemoImg"),
        DemoImgBtn = $("#addEditImg #DemoImgBtn"),
        frmProcImgMode = (id) => {
            let temp = (id == undefined || id == -1) ? "Add" : "Edit";
            return temp;
        };

    function ImgList() {
        let url = getHost("Product/ImgList", "?procID=" + ProcID.val() + "");
        $.ajax({
            url: url,
            type: "Get",
            data: ProcID,
            success: (Resualt) => {
                if (Resualt.Message == "Success") {
                    if (Resualt.Data.length > 0) {
                        NoRecords(false);
                        let source = $('#procImgSource').html(),
                            template = Handlebars.compile(source),
                            list = template({ list: Resualt.Data });
                        $('#tblProcImg tbody').html(list);
                        $("#Imgs #count").html(Resualt.Data.length + " مورد ");
                    }
                    else {
                        NoRecords(true);
                    }
                }
                else {
                    notifiction(1, Resualt.Message);
                }
            },
            error: (Resualt) => {
                notifiction(1, Resualt.Message);
            }
        });
    }
    function getFrmProcImg(e) {
        let target = $(e.currentTarget),
            id = target.attr("data-id"),
            title = target.attr("data-title");

        if (frmProcImgMode(id) == "Add") {
            ImgID.val("-1");
            ImgTitle.val("");
            DemoImgBtn.html("انتخاب");
            modalTitle.html("افزودن برند");
            DemoImg.attr("src", "/Areas/Admin/Content/Images/demoUpload.jpg");
        }
        else {
            ImgID.val(id);
            ImgTitle.val(title);
            DemoImgBtn.html("ویرایش");
            modalTitle.html("ویرایش برند");
            DemoImg.attr("src", "Files/Product/" + ProcID.val() + "_" + id + ".jpg");
        }
    };
    function saveFrmProcImg() {
        let ProcImgs = $("#FrmProcImg #ImgImage"),
            isProcImgsValid = validateImage(ProcImgs, frmProcImgMode(ImgID.val())),
            isTitleValid = validateInput(ImgTitle, "لطفا یک مورد را انتخاب کنید."),
            isValid = (isProcImgsValid && isTitleValid) ? true : false;

        if (isValid) {
            startAnimate("#addEditImg .modal-content");

            let frmProcImgData = new FormData(),
                frmProcImgValue = frmProcImg.serializeArray();

            frmProcImgData.append("ProcID", ProcID.val());
            $.each(frmProcImgValue, (i) => {
                frmProcImgData.append(frmProcImgValue[i].name, frmProcImgValue[i].value);
            });
            if (ProcImgs.val() != null) {
                frmProcImgData.append("Image", ProcImgs[0].files[0]);
            }

            let url = getHost("Product/AddEditImg");
            $.ajax({
                url: url,
                type: 'POST',
                data: frmProcImgData,
                processData: false,
                contentType: false,
                success: function (Resualt) {
                    if (Resualt.Message == "Success") {
                        finishAnimate("#addEditImg .modal-content");
                        notifiction(0, "عمیات با موفقیت انجام شد.");
                        modal.modal("hide");
                        ImgList();
                    } else {
                        notifiction(1, Resualt.Message);
                        finishAnimate("#addEditImg .modal-content");
                    }
                },
                error: function (Resualt) {
                    finishAnimate("#addEditImg .modal-content");
                    notifiction(1, Resualt.Message);
                }
            });
        }
    };
    function delImg(id) {
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
                let url = getHost("Product/DelImg");
                $.ajax({
                    url: url,
                    type: 'POST',
                    data: id,
                    processData: false,
                    contentType: "application/json",
                    success: function (Resualt) {
                        if (Resualt.Message == "Success") {
                            refreshImgList();
                            swal({
                                title: "عملیات حذف انجام شد .",
                                text: "رکورد مد نظر شما با موفقیت حذف گردید .",
                                confirmButtonColor: "#66BB6A",
                                confirmButtonText: "باشه",
                                type: "success"
                            });
                        }
                        else {
                            notifiction(1, Resualt.Message);
                        }
                    },
                    error: function (Resualt) {
                        notifiction(1, Resualt.Message);
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

    frmProcImg.delegate("#ImgImage", "change", (e) => {
        validateImage($(e.currentTarget), "لطفا تصویر را انتخاب کنید.");
    });
    frmProcImg.delegate("#ImgTitle", "keyup", (e) => {
        validateInput(ImgTitle, "این فیلد نمی تواند خالی باشد.");
    });

    $("#Imgs").delegate("#addEdit", "click", (e) => {
        getFrmProcImg(e);
    });
    $("#addEditImg #btnAddEdit").on("click", () => {
        saveFrmProcImg();
    });
    $("#Imgs").delegate("#del", "click", (e) => {
        let id = $(e.currentTarget).attr("data-id");
        delImg(id);
    });
    //#endregion

    //#region ProcProp
    let ProcPropList = {};
    function PropList(pid) {
        if (pid == undefined) {
            let url = getHost("Product/PropList/", `?procOp=${ProcID.val()}&catID=${$("#FrmProc #CatID[checked='checked']").val()}`);
            $.ajax({
                url: url,
                type: "Get",
                data: {
                    procID: ProcID.val(),
                    catId: $("#FrmProc #CatID[checked='checked']").val()
                },
                success: (Resualt) => {
                    if (Resualt.Message == "Success") {
                        if (Resualt.Data.length > 0) {
                            NoRecords(false);
                            ProcPropList = Resualt.Data;
                            let source = $('#procPropSource').html(),
                                template = Handlebars.compile(source),
                                parentList = $.grep(ProcPropList, (item) => {
                                    return (item.PID == null && item.HasChild)
                                }),
                                list = template({ list: parentList });
                            $('#tblProcProp tbody').html(list);
                        }
                        else {
                            NoRecords(true);
                        }
                    }
                    else {
                        notifiction(1, Resualt.Message);
                    }
                },
                error: (Resualt) => {
                    notifiction(1, Resualt.Message);
                }
            });
        }
        else {
            let source = $('#procPropSource').html(),
                template = Handlebars.compile(source),
                childList = $.grep(ProcPropList, (item) => {
                    return (item.PID == pid);
                }),
                list = template({ list: childList });
            $('#' + pid + ' td table tbody').html(list);
        }
    };
    function EditProcProp(pcpgID) {
        let model = {
            PCPGID: pcpgID,
            ProcID: ProcID.val(),
            Value: $(`#tblProcProp input#${pcpgID}`).val()
        };
        swal({
            title: "ویرایش انجام شود؟",
            type: "warning",
            showCancelButton: true,
            confirmButtonColor: "#558B2F",
            confirmButtonText: "بله ، ویرایش شود",
            cancelButtonText: "خیر ، لغو شود",
            closeOnConfirm: false,
            closeOnCancel: false
        }, (isConfirm) => {
            if (isConfirm) {
                let url = getHost("Product/EditProp");
                $.ajax({
                    url: url,
                    type: 'POST',
                    data: JSON.stringify(model),
                    processData: false,
                    contentType: "application/json",
                    success: function (Resualt) {
                        if (Resualt.Message == "Success") {
                            swal({
                                title: "عملیات ویرایش انجام شد .",
                                text: "رکورد مد نظر شما با موفقیت ویرایش گردید .",
                                confirmButtonColor: "#66BB6A",
                                confirmButtonText: "باشه",
                                type: "success"
                            });
                        }
                        else {
                            notifiction(1, Resualt.Message);
                        }
                    },
                    error: function (Resualt) {
                        notifiction(1, Resualt.Message);
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

    };
    $("#Props").delegate("#getChild", "click", (e) => {
        let PID = $(e.currentTarget).attr('data-id');
        PropList(PID);
    });
    $("#Props").delegate("#addEdit", "click", (e) => {
        let pcpgID = $(e.currentTarget).attr("data-id");
        EditProcProp(pcpgID);
    });
    //#endregion
});
