$(document).ready(() => {
    // #region General
    let modal = $("#addEditBrand"),
        modalTitle = $("#addEditBrand .modal-title"),
        frmProcBrand = $("#FrmProcBrand"),
        BrandID = $("#FrmProcBrand #ID"),
        BrandTitle = $("#FrmProcBrand #Title"),
        BrandEnTitle = $("#FrmProcBrand #EnTitle"),
        DemoBrand = $("#FrmProcBrand #DemoBrand"),
        DemoBrandBtn = $("#addEditBrand #DemoBrandBtn"),
        frmProcBrandMode = (id) => {
            let temp = (id == undefined || id == -1) ? "Add" : "Edit";
            return temp;
        };

    //#endregion

    //#region Functions
    function BrandList() {
        let url = getUrl("Product/BrandList");
        $.ajax({
            url: url,
            type: "Get",
            success: (Result) => {
                if (Result.Message == "Success") {
                    let source = $('#procBrandSource').html(),
                        template = Handlebars.compile(source),
                        list = template({ list: Result.Data });
                    $('#TblProcBrand tbody').html(list);
                }
                else {
                    notifiction(1, Result.Message);
                }
            },
            error: (Result) => {
                notifiction(1, Result.Message);
            }
        });
    };
    function delBrand(id) {
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
                let url = getUrl("Product/DelBrand");
                $.ajax({
                    url: url,
                    type: 'POST',
                    data: id,
                    processData: false,
                    contentType: "application/json",
                    success: function (Result) {
                        if (Result.Message == "Success") {
                            BrandList();
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
    function getFrmProcBrand(e) {
        let target = $(e.currentTarget),
            id = target.attr("data-id"),
            title = target.attr("data-title"),
            enTitle = target.attr("data-enTitle");

        if (frmProcBrandMode(id) == "Add") {
            BrandID.val("-1");
            BrandTitle.val("");
            BrandEnTitle.val("");
            DemoBrandBtn.html("انتخاب");
            modalTitle.html("افزودن تصویر");
            DemoBrand.attr("src", "/Areas/Admin/Content/Images/demoUpload.jpg");
        }
        else {
            BrandID.val(id);
            BrandTitle.val(title);
            BrandEnTitle.val(enTitle);
            DemoBrandBtn.html("ویرایش");
            modalTitle.html("ویرایش تصویر");
            DemoBrand.attr("src", "Files/ProcBrand/" + BrandID.val() + ".jpg");
        }
    };
    function saveFrmProcBrand() {
        let ProcBrand = $("#FrmProcBrand #Image"),
            isProcBrandsValid = validateImage(ProcBrand, frmProcBrandMode(BrandID.val())),
            isTitleValid = validateInput(BrandTitle, "لطفا یک مورد را انتخاب کنید."),
            isEnTitleValid = validateInput(BrandEnTitle, "لطفا یک مورد را انتخاب کنید."),
            isValid = (isProcBrandsValid && isTitleValid && isEnTitleValid) ? true : false;

        if (isValid) {
            startAnimate("#addEditBrand .modal-content");

            let frmProcBrandData = new FormData(),
                frmProcBrandValue = frmProcBrand.serializeArray();

            $.each(frmProcBrandValue, (i) => {
                frmProcBrandData.append(frmProcBrandValue[i].name, frmProcBrandValue[i].value);
            });
            if (ProcBrand.val() != null) {
                frmProcBrandData.append("Image", ProcBrand[0].files[0]);
            }

            let url = getUrl("Product/AddEditBrand");
            $.ajax({
                url: url,
                type: 'POST',
                data: frmProcBrandData,
                processData: false,
                contentType: false,
                success: function (Result) {
                    if (Result.Message == "Success") {
                        finishAnimate("#addEditBrand .modal-content");
                        notifiction(0, "عمیات با موفقیت انجام شد.");
                        modal.modal("hide");
                        BrandList();
                    } else {
                        notifiction(1, Result.Message);
                        finishAnimate("#addEditBrand .modal-content");
                    }
                },
                error: function (Result) {
                    finishAnimate("#addEditBrand .modal-content");
                    notifiction(1, Result.Message);
                }
            });
        }
    };
    //#endregion

    //#region Usage
    BrandList();
    frmProcBrand.delegate("#Image", "change", (e) => {
        validateImage($(e.currentTarget), "لطفا تصویر را انتخاب کنید.");
    });
    frmProcBrand.delegate("#Title", "keyup", (e) => {
        validateInput(BrandTitle, "این فیلد نمی تواند خالی باشد.");
    });
    frmProcBrand.delegate("#EnTitle", "keyup", (e) => {
        validateInput(BrandEnTitle, "این فیلد نمی تواند خالی باشد.");
    });
    $("#addEditBrand #btnAddEdit").on("click", () => {
        saveFrmProcBrand();
    });
    $("#addEdit").on("click", (e) => {
        getFrmProcBrand(e);
    });
    $(".panel").delegate("#addEdit", "click", (e) => {
        getFrmProcBrand(e);
    });
    $('.panel a[data-action="reload"]').on('click', () => {
        BrandList();
    });
    $(".panel").delegate("#del", "click", (e) => {
        let id = $(e.currentTarget).attr("data-id"),
            assignCount = $(e.currentTarget).attr("data-assignCount");
        if (assignCount == 0) {
            delBrand(id);
        } else {
            notifiction(2, "شما تنها می توانید برندهایی را که اختصاصی ندارند حذف نمایید.");
        }
    });
    //#endregion
});

