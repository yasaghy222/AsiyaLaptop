$(document).ready(() => {
    // #region General
    let modal = $("#addEditBrand"),
        modalTitle = $("#addEditBrand .modal-title"),
        frmProcBrand = $("#FrmProcBrand"),
        BrandID = $("#FrmProcBrand #ID"),
        BrandTitle = $("#FrmProcBrand #Title"),
        DemoBrand = $("#FrmProcBrand #DemoBrand"),
        DemoBrandBtn = $("#addEditBrand #DemoBrandBtn"),
        frmProcBrandMode = (id) => {
            let temp = (id == undefined || id == -1) ? "Add" : "Edit";
            return temp;
        };

    //#endregion

    //#region Functions
    function BrandList() {
        let url = getHost("Product/BrandList");
        $.ajax({
            url: url,
            type: "Get",
            success: (Resualt) => {
                if (Resualt.Message == "Success") {
                    let source = $('#procBrandSource').html(),
                        template = Handlebars.compile(source),
                        list = template({ list: Resualt.Data });
                    $('#TblProcBrand tbody').html(list);
                }
                else {
                    notifiction(1, Resualt.Message);
                }
            },
            error: (Resualt) => {
                notifiction(1, Resualt.Message);
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
                let url = getHost("Product/DelBrand");
                $.ajax({
                    url: url,
                    type: 'POST',
                    data: id,
                    processData: false,
                    contentType: "application/json",
                    success: function (Resualt) {
                        if (Resualt.Message == "Success") {
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
    function getFrmProcBrand(e) {
        let target = $(e.currentTarget),
            id = target.attr("data-id"),
            title = target.attr("data-title");

        if (frmProcBrandMode(id) == "Add") {
            BrandID.val("-1");
            BrandTitle.val("");
            DemoBrandBtn.html("انتخاب");
            modalTitle.html("افزودن تصویر");
            DemoBrand.attr("src", "/Areas/Admin/Content/Images/demoUpload.jpg");
        }
        else {
            BrandID.val(id);
            BrandTitle.val(title);
            DemoBrandBtn.html("ویرایش");
            modalTitle.html("ویرایش تصویر");
            DemoBrand.attr("src", "Files/ProcBrand/" + BrandID.val() + ".jpg");
        }
    };
    function saveFrmProcBrand() {
        debugger;
        let ProcBrand = $("#FrmProcBrand #Image"),
            isProcBrandsValid = validateImage(ProcBrand, frmProcBrandMode(BrandID.val())),
            isTitleValid = validateInput(BrandTitle, "لطفا یک مورد را انتخاب کنید."),
            isValid = (isProcBrandsValid && isTitleValid) ? true : false;

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

            let url = getHost("Product/AddEditBrand");
            $.ajax({
                url: url,
                type: 'POST',
                data: frmProcBrandData,
                processData: false,
                contentType: false,
                success: function (Resualt) {
                    if (Resualt.Message == "Success") {
                        finishAnimate("#addEditBrand .modal-content");
                        notifiction(0, "عمیات با موفقیت انجام شد.");
                        modal.modal("hide");
                        BrandList();
                    } else {
                        notifiction(1, Resualt.Message);
                        finishAnimate("#addEditBrand .modal-content");
                    }
                },
                error: function (Resualt) {
                    finishAnimate("#addEditBrand .modal-content");
                    notifiction(1, Resualt.Message);
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

