$(document).ready(() => {
    // #region General
    $('.select').select2();
    $(".touchspin").TouchSpin({
        max: 10
    });
    //#endregion

    //#region Functions

    //select all items
    function MediaList() {
        let url = getUrl('Media/Get');
        $.ajax({
            url: url,
            type: 'Get',
            success: function (Result) {
                NoRecords(false);
                if (Result.Data.length > 0 && Result.Message == "Success") {
                    let source = $('#MediaSource').html(),
                        template = Handlebars.compile(source);
                    let list = template({ list: Result.Data });
                    $('#TblMedia tbody').html(list);
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

    //delete media link
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
                let url = getUrl("Media/Delete");
                $.ajax({
                    url: url,
                    type: 'POST',
                    data: id,
                    processData: false,
                    contentType: "application/json",
                    success: function (Result) {
                        if (Result.Message == "Success") {
                            MediaList();
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

    //add edit
    let modal = $("#addEditMedia"),
        modalTitle = $("#addEditMedia .modal-title"),
        frmMedia = $("#frmMedia"),
        Demo = $("#frmMedia #Demo"),
        DemoBtn = $("#frmMedia #DemoBtn"),
        ID = $("#frmMedia #ID"),
        Title = $("#frmMedia #Title"),
        Link = $("#frmMedia #Link"),
        Sort = $("#frmMedia #Sort"),
        Location = $("#frmMedia #Location"),
        frmMediaMode = (id) => {
            let temp = (id == undefined || id == 0) ? "Add" : "Edit";
            return temp;
        };

    function getfrmMedia(e) {
        let t = $(e.currentTarget),
            id = t.attr("data-id"),
            title = t.attr("data-title"),
            link = t.attr("data-link"),
            sort = t.attr("data-sort"),
            location = t.attr("data-location");

        if (location == undefined || location == "0") {
            Location.val("0");
        } else {
            Location.val(location);
        };
        $('.select').select2();

        if (frmMediaMode(id) == "Add") {
            DemoBtn.html("انتخاب");
            modalTitle.html("افزودن مدیا");
            ID.val("0");
            Title.val("");
            Link.val("");
            Sort.val("0");
            Demo.attr("src", "/Areas/Admin/Content/Images/demoUpload.jpg");
        }
        else {
            DemoBtn.html("ویرایش");
            modalTitle.html("ویرایش مدیا");
            ID.val(id);
            Title.val(title);
            Link.val(link);
            Sort.val(sort);
            Demo.attr("src", "Files/Media/" + id + ".jpg");
        }
    }
    function savefrmMedia(e) {
        let Img = $("#frmMedia #Image"),
            isImgValid = validateImage(Img, frmMediaMode(ID.val())),
            isTitleValid = validateInput(Title, "این فیلد اجباری است."),
            isLinkValid = validateInput(Link, "این فیلد اجباری است.");
        if (isImgValid && isTitleValid && isLinkValid) {
            startAnimate("#addEditMedia .modal-content");

            let formData = new FormData(),
                data = $("#frmMedia").serializeArray(),
                url = getUrl("media/AddEdit");

            $.each(data, (i) => {
                formData.append(data[i].name, data[i].value);
            });
            if (ID.val() != null) {
                formData.append("Image", Img[0].files[0]);
            }
            $.ajax({
                url: url,
                type: 'POST',
                data: formData,
                processData: false,
                contentType: false,
                success: function (Result) {
                    if (Result.Message == "Success") {
                        finishAnimate("#addEditMedia .modal-content");
                        notifiction(0, "عمیات با موفقیت انجام شد.");
                        modal.modal("hide");
                        MediaList();
                    } else {
                        notifiction(1, Result.Message);
                        finishAnimate("#addEditMedia .modal-content");
                    }
                },
                error: function (Result) {
                    finishAnimate("#addEditMedia .modal-content");
                    notifiction(1, Result.Message);
                }
            });
        }
    }
    //#endregion

    //#region Usage
    MediaList();
    frmMedia.delegate("#Image", "change", (e) => {
        validateImage($(e.currentTarget), "لطفا تصویر را انتخاب کنید.");
    });
    frmMedia.delegate("#Title", "keyup", (e) => {
        validateInput(Title, "این فیلد نمی تواند خالی باشد.");
    });
    frmMedia.delegate("#Link", "keyup", (e) => {
        validateInput(Link, "این فیلد نمی تواند خالی باشد.");
    });
    $(".panel").delegate("#btnAddEdit", "click", () => {
        savefrmMedia();
    });
    $('.panel a[data-action="reload"]').on('click', () => {
        MediaList()
    });
    $(".content-wrapper").delegate("#addEdit", "click", (e) => {
        getfrmMedia(e);
    });
    $(".panel").delegate("#del", "click", (e) => {
        let id = $(e.currentTarget).attr("data-id");
        Delete(id);
    });
    //#endregion
});

