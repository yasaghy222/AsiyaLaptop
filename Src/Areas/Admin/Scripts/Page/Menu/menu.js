$(document).ready(() => {
    // #region General
    let menuList = {},
        ParentList = {};
    $('.select').select2();
    $(".touchspin").TouchSpin({
        max: 10
    });
    let count = 0;
    //#endregion

    //#region Functions

    //select all items
    function MenuList(pid) {
        if (pid == undefined) {
            let url = getUrl('Menu/Get');
            $.ajax({
                url: url,
                type: 'Get',
                success: function (Result) {
                    NoRecords(false);
                    if (Result.Data.length > 0 && Result.Message == "Success") {
                        menuList = Result.Data;
                        let source = $('#menuSource').html(),
                            template = Handlebars.compile(source);
                        ParentList = $.grep(menuList, (item) => {
                            return (item.PID == null)
                        });
                        let list = template({ list: ParentList });
                        $('#Tblmenu tbody').html(list);
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
        else {
            let source = $('#menuSource').html(),
                template = Handlebars.compile(source),
                childList = $.grep(menuList, (item) => {
                    return (item.PID == pid);
                }),
                list = template({ list: childList });
            $('#' + pid + ' td table tbody').html(list);
        }
    }

    //delete menu link
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
                let url = getUrl("Menu/Delete");
                $.ajax({
                    url: url,
                    type: 'POST',
                    data: id,
                    processData: false,
                    contentType: "application/json",
                    success: function (Result) {
                        if (Result.Message == "Success") {
                            MenuList();
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
    let modal = $("#addEditMenu"),
        modalTitle = $("#addEditMenu .modal-title"),
        frmMenu = $("#FrmMenu"),
        ID = $("#FrmMenu #ID"),
        Title = $("#FrmMenu #Title"),
        Link = $("#FrmMenu #Link"),
        PID = $("#FrmMenu #PID[checked='checked']"),
        Sort = $("#FrmMenu #Sort"),
        Status = $("#FrmMenu #Status"),
        frmMenuMode = (id) => {
            let temp = (id == undefined || id == 0) ? "Add" : "Edit";
            return temp;
        };

    function getFrmMenu(e) {
        let t = $(e.currentTarget),
            id = t.attr("data-id"),
            title = t.attr("data-title"),
            link = t.attr("data-link"),
            pid = t.attr("data-pid"),
            sort = t.attr("data-sort"),
            status = t.attr("data-status");

        clearMultiSelect($("#FrmMenu #TreeSelect"), "PID");
        if (status == undefined || status == "false") {
            Status.val("false");
            Status.removeClass("checked");
        } else {
            Status.val("true");
            Status.addClass("checked");
        };

        if (frmMenuMode(id) == "Add") {
            ID.val("0");
            Title.val("");
            Link.val("");
            $.each(menuList, (i) => {
                fillSelectMulti(menuList, menuList[i], 0, $("#FrmMenu #TreeSelect"), "PID", false);
            });
            Sort.val("0");
        }
        else {
            ID.val(id);
            Title.val(title);
            Link.val(link);
            $.each(menuList, (i) => {
                fillSelectMulti(menuList, menuList[i], pid, $("#FrmMenu #TreeSelect"), "PID", false);
            });
            Sort.val(sort);
        }
    }
    function saveFrmMenu(e) {
        isValid = validateInput(Title, "این فیلد اجباری است.");
        if (isValid) {
            startAnimate("#addEditMenu .modal-content");
            let data = {
                ID: ID.val(),
                Title: Title.val(),
                PID: $("#FrmMenu #PID[checked='checked']").val(),
                Link: Link.val(),
                Sort: Sort.val(),
                Status: Status.val(),
                HasChild: false
            },
                url = getUrl("Menu/AddEdit");
            $.ajax({
                url: url,
                type: 'POST',
                data: JSON.stringify(data),
                processData: false,
                contentType: "application/json",
                success: function (Result) {
                    if (Result.Message == "Success") {
                        finishAnimate("#addEditMenu .modal-content");
                        notifiction(0, "عمیات با موفقیت انجام شد.");
                        modal.modal("hide");
                        MenuList();
                    } else {
                        notifiction(1, Result.Message);
                        finishAnimate("#addEditMenu .modal-content");
                    }
                },
                error: function (Result) {
                    finishAnimate("#addEditMenu .modal-content");
                    notifiction(1, Result.Message);
                }
            });
        }
    }
    //#endregion

    //#region Usage
    MenuList();
    frmMenu.delegate("#Title", "keyup", (e) => {
        validateInput(Title, "این فیلد نمی تواند خالی باشد.");
    });
    $(".panel").delegate("#btnAddEdit", "click", () => {
        saveFrmMenu();
    });
    $('.panel a[data-action="reload"]').on('click', () => {
        MenuList()
    });
    $(".panel").delegate("#getChild", "click", (e) => {
        let PID = $(e.currentTarget).attr('data-id');
        MenuList(PID);
    });
    $(".content-wrapper").delegate("#addEdit", "click", (e) => {
        getFrmMenu(e);
    });
    $(".content-wrapper").delegate("#Status", "click", () => {
        if (Status.hasClass("checked") == "false") {
            Status.val("false");
            Status.removeClass("checked");
        } else {
            Status.val("true");
            Status.addClass("checked");
        };
    });
    $(".content-wrapper").delegate("#PID", "change", (e) => {
        $("#FrmMenu #PID[checked='checked']").removeAttr("checked");
        $(e.currentTarget).attr("checked", "checked");
    });
    $(".panel").delegate("#del", "click", (e) => {
        let id = $(e.currentTarget).attr("data-id"),
            hasChild = $(e.currentTarget).attr("data-hasChild");
        if (hasChild == "false") {
            Delete(id);
        } else {
            notifiction(2, "شما تنها می توانید دسته هایی را که فرزند و اختصاصی ندارند را حذف نمایید.");
        }
    });
    //#endregion
});

