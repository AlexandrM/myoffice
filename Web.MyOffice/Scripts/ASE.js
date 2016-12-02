function L() {
    console.log(arguments);
}

function l() {
    console.log(arguments);
}

var ASE = {

    LOnLOffEnabled: true,
    
    Init: function(root)
    {
        ASE.BaseUrl = root;
        $(document).ajaxStart(function (evt, request, settings) {
            if (ASE.LOnLOffEnabled)
                ASE.LOn();
        });
        $(document).ajaxStop(function (event, xhr, options) {
            ASE.LOff();
        });
    },

    //Page to Object
    P2O: function (list) {
        var result = {};

        $.each(list, function (index, value) {
            if ($(value).prop("tagName") == "SELECT") {
                var list = [];
                $.each($(value)[0].options, function (index, value) {
                    if (value.selected)
                        list.push(value.value);
                });
                result[value.id] = list;
            }
            else
                result[value.id] = value.value;
        });

        return result;
    },

    //Object to Page
    O2P: function (obj) {
        for (var key in obj) {
            var elem = $("#" + key);
            if (elem.hasClass("ase-html"))
                elem.html(obj[key]);
            else
                elem[0].value = obj[key];
        }
    },

    BaseUrl: '',

    PostState: function (url, listOrObject, options) {
        options = options || {};
        options["type"] = "POST";
        options["async"] = true;
        options["url"] = url;
        if (typeof (listOrObject) == "string")
            options["data"] = ASE.P2O($(listOrObject));
        else if (listOrObject.length == undefined)
            options["data"] = listOrObject;
        else
            options["data"] = ASE.P2O(listOrObject);
        options["traditional"] = true;

        ASE.AjaxState(options);
    },

    AjaxState: function (options) {
        var beforeSend = options["beforeSend"];
        var complete = options["complete"];
        var success = options["success"];

        options["beforeSend"] = function (jqXHR, settings) {
            ASE.LOn();

            if (beforeSend != undefined)
                beforeSend(jqXHR, settings);
        };

        options["complete"] = function (jqXHR, textStatus) {
            if (complete != undefined)
                complete(jqXHR, textStatus);

            ASE.LOff();
        };

        options["success"] = function (data, textStatus, jqXHR) {
            ASE.O2P(data);

            if (success != undefined)
                success(data, textStatus, jqXHR);
        };


        $.ajax(options);
    },

    ConfirmAction: function (href, text) {
        bootbox.confirm(text, function (result) {
            if (result) {
                window.location = href;
            }
        });

        return false;
    },

    ConfirmActionSubmit: function (elem, mode, text) {
        bootbox.confirm(text, function (result) {
            if (result) {
                var form = $(elem).closest("form");
                form.validate().settings.ignore = "*";

                var hidden = document.createElement("input");
                hidden.type = "hidden";
                hidden.name = "formMode";
                hidden.value = mode;
                form.append(hidden);

                form.submit();
            }
        });

        return false;
    },

    Delete: function (element, text, action, controller, id) {
        bootbox.confirm(text, function (result) {
            if (result) {
                data = {};
                data["id"] = id;
                $(element).closest(".partial").find('input').each(function (index, value) {
                    data[$(value)[0].id] = $(value).val();
                });

                $.ajax({
                    type: "POST",
                    url: ASE.BaseUrl + controller + '/' + action,
                    data: data,
                    dataType: "json",
                    success: function (response) {
                        $.ajax({
                            type: "POST",
                            url: ASE.BaseUrl + controller + '/' + response.Action,
                            data: data,
                            dataType: "json",
                            success: function (data) {
                                $(element).closest(".partial").html(data.html);
                            }
                        });
                    }
                });
            }
        });

        return false;
    },

    SearchTextBoxTimer: 0,
    SearchTextBox: function (elem) {
        clearTimeout(this.SearchTextBoxTimer);
        if ((event == null) || (($(elem)[0].value.length > 0) | (event.keyCode == 8) | (event.keyCode == 13) | (event.keyCode == 46))) {
            this.SearchTextBoxTimer = setTimeout(function () { ASE.SearchTextBoxStart.call(elem) }, 1000);
        }
    },

    SearchTextBoxStart: function () {
        data = {};
        //data["isSelect"] = this.getAttribute('isSelect');

        $(this).closest(".partial").find('input').each(function (index, value) {
            data[$(value)[0].id] = $(value).val();
        });
        var div = this;
        $.ajax({
            type: "POST",
            url: this.getAttribute('href'),
            data: data,
            success: function (data) {
                $(div).closest(".partial").html(data.html);
            }
        });
    },

    Refresh: function(element) {
        data = {};
        $(element).closest(".partial").find('input').each(function (index, value) {
            data[$(value)[0].id] = $(value).val();
        });

        $.ajax({
            type: "POST",
            url: window.location,
            data: data,
            dataType: "json",
            success: function (data) {
                $(element).closest(".partial").html(data.html);
            }
        });
    },

    AjaxRefresh: function (element) {
        $.ajax({
            type: "POST",
            url: $(element).attr('href'),
            data: {},
            dataType: "json",
            success: function (data) {
                ASE.Refresh(element);
            }
        });
    },

    Loading: undefined,

    LOn: function () {
        if (ASE.Loading == undefined)
            ASE.Loading = $('<div class="modal" id="LoadingDialog" style=""><div class="modal-dialog" style="width: 250px; padding-top: 100px;"><div class="modal-content" style=""><div class="modal-body" style=""><img style="height: 36px;" src="' + ASE.BaseUrl + '/images/ajax-loader.gif"> Ожидание сервера</div></div></div></div>');
        ASE.Loading.find('.modal-dialog').css("padding-top", ($(window).height() - 35) / 2);
        ASE.Loading.modal({ backdrop: "static" });
    },

    LOff: function () {
        ASE.Loading.modal('hide');
        //$('.modal-backdrop').remove();
    },

    Select: function (elem) {
        var input = $(elem).closest(".input-group").find("input");

        var data = {};
        data['value'] = input[0].value;
        data['type'] = $(input[0]).attr("objectType");
        data['customAction'] = $(input[0]).attr("customAction");
        data['customController'] = $(input[0]).attr("customController");
        data['selectResultFieldId'] = input[0].id;
        data['selectResultField'] = input[1].id;

        $.ajax({
            type: "POST",
            url: ASE.BaseUrl + 'GlobalSelect/Select',
            data: data,
            success: function (responseS) {
                data["isSelect"] = true;
                $.ajax({
                    type: "POST",
                    url: ASE.BaseUrl + responseS.Controller + '/' + responseS.Action,
                    data: data,
                    success: function (response) {
                        var dialog = '';
                        dialog += '<div class="modal">';
                        dialog += '<div class="modal-dialog" style="width: 1170px; padding-top: 0px;">';
                        dialog += '<div class="modal-content">';
                        dialog += '<div class="modal-header">';
                        dialog += '<button type="button" class="close" data-dismiss="modal"><span aria-hidden="true">&times;</span><span class="sr-only">Close</span></button>';
                        dialog += '<h4 class="modal-title">' + responseS.Title + '</h4>';
                        dialog += '</div>';
                        dialog += '<div class="modal-body partial">';
                        dialog += response.html;
                        dialog += '</div>';
                        //dialog += '<div class="modal-footer">';
                        //dialog += '<button type="button" class="btn btn-default" data-dismiss="modal">Close</button>';
                        //dialog += '</div>';
                        dialog += '</div>';
                        dialog += '</div>';
                        dialog += '</div>';

                        if ($(elem).closest(".input-group").find(".selectDialog").length == 0)
                            $(elem).closest(".input-group").append('<div class="selectDialog"></div>')

                        dialog = $('.modal', $(elem).closest(".input-group").find(".selectDialog").html(dialog))
                        dialog.modal({ width: 1170, keyboard: 'true' });
                        $('.modal-body', dialog).css('max-height', ($(window).height() - 120) + 'px');
                    },
                    beforeSend: function (data) {
                        ASE.LOn();
                    },
                    complete: function (data) {
                        ASE.LOff();
                    }
                });
            }
        });
    },

    SelectDo: function (modal, fId, fvalue, id, value) {
        $(modal).closest('.modal').modal('hide');
        $('#' + fId)[0].value = id;
        $('#' + fvalue)[0].value = value;

        if (window["OnASESelect" + fId] != null) {
            window["OnASESelect" + fId](fId, fvalue, id, value);
        }            
        else if (window["OnASESelect"] != null) {
            window["OnASESelect"](fId, fvalue, id, value);
        }
    },

    DataAjaxOnSuccess: function (result, action, xhr) {
        $(this).closest(".partial").html(result.html);
    },

    DataAjaxOnBegin: function (action, xhr) {
    },

    ApplyValue: function (id, value) {
        if ($('#' + id).prop("tagName") == "SELECT")
            $('#' + id).val(value);
        else
            $('#' + id)[0].value = value;
    },

    ValidationInit: function () {
        var elems = $("input[validate-minlength]");
        elems.each(function (idx, item) {
            $(item).rules("add", { minlength: 6 });
        });
    },

    AngularInit: function (name, requires, configFn) {
        app = angular.module(name, requires, configFn);

        app.directive('moDateInput', function ($window) {
            return {
                require: '^ngModel',
                restrict: 'A',
                link: function (scope, elm, attrs, ctrl) {
                    var moment = $window.moment;
                    var dateFormat = attrs.moDateInput;
                    attrs.$observe('moDateInput', function (newValue) {
                        if (dateFormat == newValue || !ctrl.$modelValue) return;
                        dateFormat = newValue;
                        ctrl.$modelValue = new Date(ctrl.$setViewValue);
                    });

                    ctrl.$formatters.unshift(function (modelValue) {
                        if (!dateFormat || !modelValue) return "";
                        var retVal = moment(modelValue).format(dateFormat);
                        return retVal;
                    });

                    ctrl.$parsers.unshift(function (viewValue) {
                        var date = moment(viewValue, dateFormat);
                        return date.format("YYYY-MM-DDTHH:mm:ss.SSSZZ");
                    });
                }
            };
        });

        app.directive('convertToNumber', function () {
            return {
                require: 'ngModel',
                link: function(scope, element, attrs, ngModel) {
                    ngModel.$parsers.push(function(val) {
                        return parseInt(val, 10);
                    });
                    ngModel.$formatters.push(function(val) {
                        return '' + val;
                    });
                }
            };
        });

        return app;
    }
}