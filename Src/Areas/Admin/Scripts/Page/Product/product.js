$(document).ready(() => {
    // #region General
    $('.select').select2({
        minimumResultsForSearch: Infinity,
        width: 'auto'
    });

    // page table variable
    let TableVar = { PageIndex: 1, PageSize: 10, OrderBy: "Title", OrderType: "desc", Includes: null }
    let count = 0;

    Handlebars.registerHelper('for', (n) => {
        var resualt = '';
        for (var i = 0; i < n; i++) {
            resualt += '<i class="icon-star-full2 text-warning"></i>';
        }
        if (n < 5) {
            let diff = 5 - n;
            for (var i = 0; i < diff; i++) {
                resualt += '<i class="icon-star-empty3 text-warning"></i>';
            }
        }
        return resualt;
    });
    Handlebars.registerHelper('checkOff', (price, offPrice) => {
        let resualt = '';
        let numberOff = currencyToNumber(offPrice),
            numberPrice = currencyToNumber(price);
        if (numberOff > 0) {
            let calc = numberPrice - numberOff;
            calc = numberToCurrency(calc);
            resualt = '<del class="text-grey-400">' + price + '</del> <span class="text-green-800 text-bold text-large">' + calc + ' تومان </span>';
        }
        else {
            resualt = '<span class="text-green-800 text-bold text-large">' + price + ' تومان</span>';
        }
        return resualt;
    });
    //#endregion

    //#region Functions

    //select all items
    function ProcList(tableVar) {
        if (tableVar == null) {
            tableVar = TableVar;
            $('#tblProc th.sorting_desc').removeClass("sorting_desc").attr("data-sort-type", "").addClass("sorting");
            $('#tblProc th.sorting_asc').removeClass("sorting_asc").attr("data-sort-type", "").addClass("sorting");
            $('#tblProc th[data-sort="0"]').removeClass("sorting").attr("data-sort-type", "false").addClass("sorting_desc");
        };
        let url = getUrl('Product/Get');
        $.ajax({
            url: url,
            type: 'Get',
            data: tableVar,
            success: function (Result) {
                NoRecords(false);
                if (Result.Data.List.length > 0 && Result.Message == "Success") {
                    let source = $('#procSource').html(),
                        template = Handlebars.compile(source),
                        list = template({ list: Result.Data.List });
                    $('#tblProc tbody').html(list);
                    count = Result.Data.Count;
                    CreatePagingInfo(count, Result.Data.List.length, '#procInfo', TableVar);
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
        $(".panel #getProc").val("");
        $(".panel #pageSize").val("10").trigger("change");
        ProcList();
    }

    //go to next page's of items
    $("#procPaging").delegate('a.paginate_button', "click", function () {
        var newIndex = $(this).attr("data-index");
        TableVar.PageIndex = newIndex;
        ProcList(TableVar);
    })

    //change pagesize's of items
    $("#pagesize").change(function () {
        var newPageSize = $(this).val();
        TableVar.PageSize = newPageSize;
        TableVar.PageIndex = 1;
        ProcList(TableVar);
    })

    //orderby items list
    $('#tblProc th').click(function (e) {
        let isSrortEnable = $(e.currentTarget).hasClass("sorting_disabled");
        if (!isSrortEnable) {
            var newOrderby = $(this).attr("data-sort");
            var newOrderType = $(this).attr("data-sort-type");
            $('#tblProc th.sorting_desc').removeClass("sorting_desc").attr("data-sort-type", "").addClass("sorting");
            $('#tblProc th.sorting_asc').removeClass("sorting_asc").attr("data-sort-type", "").addClass("sorting");
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
            ProcList(TableVar);
        }
    })

    //search item
    function getProc() {
        let key = $('#getProc').val();
        if (key != "") {
            var newPageSize = $("#pagesize").val();
            TableVar.PageSize = newPageSize;
            TableVar.PageIndex = 1;
            TableVar.Includes = key;
            ProcList(TableVar);
        }
        else {
            ProcList();
        }
    }

    //#endregion

    //#region Usage
    ProcList();
    $('.panel a[data-action="reload"]').on('click', () => {
        ReloadList();
    });
    $('.panel #getProc').on('focusout', () => {
        getProc();
    });
    //#endregion
});

