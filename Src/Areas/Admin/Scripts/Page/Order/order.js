$(document).ready(() => {
    // #region General
    $('.select').select2({
        minimumResultsForSearch: Infinity,
        width: 'auto'
    });

    // page table variable
    let TableVar = { PageIndex: 1, PageSize: 10, OrderBy: "ID", OrderType: "desc", Includes: null }
    let count = 0;
    //#endregion

    //#region Functions

    //select all items
    function OrderList(tableVar) {
        if (tableVar == null) {
            tableVar = TableVar;
            $('#tblOrder th.sorting_desc').removeClass("sorting_desc").attr("data-sort-type", "").addClass("sorting");
            $('#tblOrder th.sorting_asc').removeClass("sorting_asc").attr("data-sort-type", "").addClass("sorting");
            $('#tblOrder th[data-sort="0"]').removeClass("sorting").attr("data-sort-type", "false").addClass("sorting_desc");
        };
        let url = getHost('Order/Get');
        $.ajax({
            url: url,
            type: 'Get',
            data: tableVar,
            success: function (Resualt) {
                NoRecords(false);
                if (Resualt.Data.List.length > 0 && Resualt.Message == "Success") {
                    let source = $('#orderSource').html(),
                        template = Handlebars.compile(source),
                        list = template({ list: Resualt.Data.List });
                    $('#tblOrder tbody').html(list);
                    count = Resualt.Data.Count;
                    CreatePagingInfo(count, Resualt.Data.List.length, '#orderInfo', TableVar);
                } else {
                    NoRecords(true);
                }
            },
            error: function (Resualt) {
                notifiction(3, Resualt.Message);
            }
        })
    }

    //refresh list
    function ReloadList() {
        TableVar.OrderBy = "Title";
        TableVar.OrderType = "desc";
        TableVar.Includes = null;
        $(".panel #getOrder").val("");
        $(".panel #pageSize").val("10").trigger("change");
        OrderList();
    }

    //go to next page's of items
    $("#orderPaging").delegate('a.paginate_button', "click", function () {
        var newIndex = $(this).attr("data-index");
        TableVar.PageIndex = newIndex;
        OrderList(TableVar);
    })

    //change pagesize's of items
    $("#pagesize").change(function () {
        var newPageSize = $(this).val();
        TableVar.PageSize = newPageSize;
        TableVar.PageIndex = 1;
        OrderList(TableVar);
    })

    //orderby items list
    $('#tblOrder th').click(function (e) {
        let isSrortEnable = $(e.currentTarget).hasClass("sorting_disabled");
        if (!isSrortEnable) {
            var newOrderby = $(this).attr("data-sort");
            var newOrderType = $(this).attr("data-sort-type");
            $('#tblOrder th.sorting_desc').removeClass("sorting_desc").attr("data-sort-type", "").addClass("sorting");
            $('#tblOrder th.sorting_asc').removeClass("sorting_asc").attr("data-sort-type", "").addClass("sorting");
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
            OrderList(TableVar);
        }
    })

    //search item
    function getOrder() {
        let key = $('#getOrder').val();
        if (key != "") {
            var newPageSize = $("#pagesize").val();
            TableVar.PageSize = newPageSize;
            TableVar.PageIndex = 1;
            TableVar.Includes = key;
            OrderList(TableVar);
        }
        else {
            OrderList();
        }
    }

    //#endregion

    //#region Usage
    OrderList();
    $('.panel a[data-action="reload"]').on('click', () => {
        ReloadList();
    });
    $('.panel #getOrder').on('focusout', () => {
        getOrder();
    });
    //#endregion
});

