$(document).ready(() => {
    // #region General
    $('.select').select2({
        minimumResultsForSearch: Infinity,
        width: 'auto'
    });

    // page table variable
    let TableVar = { PageIndex: 1, PageSize: 10, OrderBy: "Title", OrderType: "desc", Includes: null }
    let count = 0;
    //#endregion

    //#region Functions

    //select all items
    function custList(tableVar) {
        if (tableVar == null) {
            tableVar = TableVar;
            $('#tblCust th.sorting_desc').removeClass("sorting_desc").attr("data-sort-type", "").addClass("sorting");
            $('#tblCust th.sorting_asc').removeClass("sorting_asc").attr("data-sort-type", "").addClass("sorting");
            $('#tblCust th[data-sort="0"]').removeClass("sorting").attr("data-sort-type", "false").addClass("sorting_desc");
        };
        let url = getUrl('Customer/Get');
        $.ajax({
            url: url,
            type: 'Get',
            data: tableVar,
            success: function (Result) {
                NoRecords(false);
                if (Result.Data.List.length > 0 && Result.Message == "Success") {
                    let source = $('#custSource').html(),
                        template = Handlebars.compile(source),
                        list = template({ list: Result.Data.List });
                    $('#tblCust tbody').html(list);
                    count = Result.Data.Count;
                    CreatePagingInfo(count, Result.Data.List.length, '#custInfo', TableVar);
                } else {
                    NoRecords(true);
                }
            },
            error: function (Result) {
                notifiction(3, Result.Message);
            }
        })
    }

    //refresh list
    function ReloadList() {
        TableVar.OrderBy = "Title";
        TableVar.OrderType = "desc";
        TableVar.Includes = null;
        $(".panel #getcust").val("");
        $(".panel #pageSize").val("10").trigger("change");
        custList();
    }

    //go to next page's of items
    $("#custPaging").delegate('a.paginate_button', "click", function () {
        var newIndex = $(this).attr("data-index");
        TableVar.PageIndex = newIndex;
        custList(TableVar);
    })

    //change pagesize's of items
    $("#pagesize").change(function () {
        var newPageSize = $(this).val();
        TableVar.PageSize = newPageSize;
        TableVar.PageIndex = 1;
        custList(TableVar);
    })

    //orderby items list
    $('#tblCust th').click(function (e) {
        let isSrortEnable = $(e.currentTarget).hasClass("sorting_disabled");
        if (!isSrortEnable) {
            var newOrderby = $(this).attr("data-sort");
            var newOrderType = $(this).attr("data-sort-type");
            $('#tblCust th.sorting_desc').removeClass("sorting_desc").attr("data-sort-type", "").addClass("sorting");
            $('#tblCust th.sorting_asc').removeClass("sorting_asc").attr("data-sort-type", "").addClass("sorting");
            if (newOrderType == "") {
                TableVar.OrderType = "desc";
                $(this).removeClass("sorting sorting_asc").attr("data-sort-type", "desc").addClass("sorting_desc");
            }
            else if (newOrderType == "desc") {
                TableVar.OrderType = "";
                $(this).removeClass("sorting sorting_desc").attr("data-sort-type", "").addClass("sorting_asc");
            }
            else {
                TableVar.OrderType = false;
                $(this).removeClass("sorting").attr("data-sort-type", "desc").addClass("sorting_desc");
            }
            TableVar.OrderBy = newOrderby;
            custList(TableVar);
        }
    })

    //search item
    function getcust() {
        let key = $('#getcust').val();
        if (key != "") {
            var newPageSize = $("#pagesize").val();
            TableVar.PageSize = newPageSize;
            TableVar.PageIndex = 1;
            TableVar.Includes = key;
            custList(TableVar);
        }
        else {
            custList();
        }
    }


    //change status
    function changeStatus(id) {
        if (id != undefined) {
            swal({
                title: "آیا از انجام این کار اطمینان دارید ؟",
                type: "warning",
                showCancelButton: true,
                confirmButtonColor: "#EF6C00",
                confirmButtonText: "بله ، انجام شود",
                cancelButtonText: "خیر ، لغو شود",
                closeOnConfirm: true,
                closeOnCancel: true
            }, (isConfirm) => {
                if (isConfirm) {
                    let url = getUrl("Customer/ChangeStatus");
                    $.ajax({
                        url: url,
                        type: 'POST',
                        data: id,
                        processData: false,
                        contentType: "application/json",
                        success: function (Result) {
                            if (Result.Message == "Success") {
                                custList();
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
            });
        }
    }
    //#endregion

    //#region Usage
    custList();
    $('.panel a[data-action="reload"]').on('click', () => {
        ReloadList();
    });
    $('.panel #getcust').on('focusout', () => {
        getcust();
    });
    $('.panel').delegate('#changeStatus', 'click', (e) => {
        let id = $(e.currentTarget).attr("data-id");
        changeStatus(id);
    });
    //#endregion
});

