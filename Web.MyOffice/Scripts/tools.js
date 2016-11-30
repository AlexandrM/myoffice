function L() {
    console.log(arguments);
}

var ASE_LoadingDialog = null;

function LoadingDialog(hide) {
    if (hide)
        ASE_LoadingDialog.modal('hide');
    else {
        if (ASE_LoadingDialog == null)
            ASE_LoadingDialog = $('<div class="modal" id="LoadingDialog" style=""><div class="modal-dialog" style="width: 250px"><div class="modal-content" style=""><div class="modal-body" style=""><img src="' 
                + window.location.protocol + "//" + window.location.host + "/" + window.location.pathname.split('/')[1]
                + '/Images/ajax-loader.gif">  Ожидание сервера</div></div></div></div>');
        $(ASE_LoadingDialog).find('.modal-dialog').css("padding-top", (parseInt($("html").css("height")) - 35) / 2);
        ASE_LoadingDialog.modal({ backdrop: "static" });
        ASE_LoadingDialog.draggable({ handle: ".modal-header" });
    }
}

function TableToExcel(table, file) {
    var dt = new Date();
    var day = dt.getDate();
    var month = dt.getMonth() + 1;
    var year = dt.getFullYear();
    var hour = dt.getHours();
    var mins = dt.getMinutes();
    var postfix = day + "." + month + "." + year + "_" + hour + "." + mins;
    var data_type = 'data:application/vnd.ms-excel';
    var table_div = document.getElementById(table);
    var table_html = table_div.outerHTML.replace(/ /g, '%20');

    var uri = 'data:application/vnd.ms-excel;base64,'
      , template = '<html xmlns:o="urn:schemas-microsoft-com:office:office" xmlns:x="urn:schemas-microsoft-com:office:excel" xmlns="http://www.w3.org/TR/REC-html40"><head><!--[if gte mso 9]><xml><x:ExcelWorkbook><x:ExcelWorksheets><x:ExcelWorksheet><x:Name>{worksheet}</x:Name><x:WorksheetOptions><x:DisplayGridlines/></x:WorksheetOptions></x:ExcelWorksheet></x:ExcelWorksheets></x:ExcelWorkbook></xml><![endif]--><meta http-equiv="content-type" content="text/plain; charset=UTF-8"/></head><body><table>{table}</table></body></html>'
      , base64 = function (s) {
          return window.btoa(unescape(encodeURIComponent(s)))
      }
      , format = function (s, c) {
          return s.replace(/{(\w+)}/g, function (m, p) { return c[p]; })
      }
    table = document.getElementById(table);

    var ctx = { worksheet: name || 'Worksheet', table: table.innerHTML }
    var a = document.createElement('a');
    a.id = 'temp';
    a.href = uri + base64(format(template, ctx))
    a.download = file + ' ' + postfix + '.xls';
    document.body.appendChild(a);
    setTimeout(function () {
        var e = document.createEvent("MouseEvents");
        e.initMouseEvent("click", true, false, window, 0, 0, 0, 0, 0, false, false, false, false, 0, null);
        a.dispatchEvent(e);
        document.body.removeChild(a);
    }, 66);
}

function DatePickerReadonly(from, readonly) {
    $(from).prop("readonly", readonly);
    $(from).removeClass("datepicker");
    if (readonly)
        $(from).datepicker('remove');
    else
        $(from).datepicker();
}

function DatePickerReset(from, to) {
    $(from)[0].value = $(to)[0].value;
    $(from).datepicker('remove');
    $(from).datepicker();
}

function GAError(url) {
    window.location = url
}

function ExpandCollapseAll(elem, tree) {
    if ($(elem).hasClass("glyphicon-chevron-right")) {
        $(elem).removeClass("glyphicon-chevron-right");
        $(elem).addClass("glyphicon-chevron-down");
        $("#" + tree).treegrid('expandAll');
    }
    else {
        $(elem).removeClass("glyphicon-chevron-down");
        $(elem).addClass("glyphicon-chevron-right");
        $("#" + tree).treegrid('collapseAll');
    }
}

function TreeGridAnyClick() {
    $(".treegridtr").click(function (e) {
        if ((e.target.className.indexOf("treegrid-expander") == -1) & (e.target.tagName != "A")) {
            $(this).treegrid('toggle');
        }
    });
}

function Value(element) {
    if (element.length == 0)
        return undefined;
    else
        return element[0].value;
}

function Checked(element) {
    if (element.length == 0)
        return undefined;
    else
        return element[0].checked;
}