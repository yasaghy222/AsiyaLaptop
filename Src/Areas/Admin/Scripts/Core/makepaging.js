/*make paging*/
function makePaging(totalPages, pageIndex) {
    var oPaging, i, j, k;
    var totalPages = parseInt(totalPages);

    pageIndex++;

    i = pageIndex;
    j = pageIndex - 1;
    k = pageIndex + 1;

    var oBefore, oAfter;
    oBefore = "";
    oAfter = "";

    while (j != 0 && j != i - 6) {
        oBefore = '<a class="paginate_button" data-index="' + (j) + '">' + j + '</a>' + oBefore;
        j--;
    }

    if ((i - 6) > 0)
        oBefore = '<a class="paginate_button previous" data-index="' + (i - 6) + '">قبلی</a>' + oBefore;


    for (; k < totalPages + 1 && k < i + 6; k++) {
        oAfter += '<a class="paginate_button" data-index="' + (k) + '">' + k + '</a>';
    }

    if ((i + 6) <= totalPages)
        oAfter += '<a class="paginate_button next" data-index="' + (i + 6) + '">بعدی</a>';

    oPaging = oBefore + '<a class="paginate_button current">' + i.toString() + "</a>" + oAfter;

    if (i != 1) {
        $('.dataTables_paginate').html(oPaging);
    }
}

var pageIndex = 1, pageSize = 10, orderby = 0;

function CreatePagingInfo(count, length, target, tableVar) {
    $('#count').html(count + ' مورد ')
    $(target).html('نمایش ' + ' ' + length + ' ' + 'رکورد از ' + ' ' + count + ' ' + 'رکورد .');
    if (tableVar != null) {
        makePaging(Math.ceil(count / tableVar.PageSize), tableVar.PageIndex - 1);
    }
}

function NoRecords(type) {
    if (type) {

        $("[id='dataTable']").addClass("hidden");
        $("[id='no-records']").removeClass("hidden");
    }
    else {
        $("[id='dataTable']").removeClass("hidden");
        $("[id='no-records']").addClass("hidden");
    }
}