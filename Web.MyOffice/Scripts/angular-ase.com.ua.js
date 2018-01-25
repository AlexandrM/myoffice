(function () {

    'use strict';

    Date.prototype.toJSON = function () { return moment(this).format(); }

    var module = angular.module('ase.com.ua', []);

    window.l = function (p1, p2, p3, p4, p5) {
        if (arguments.length > 5) {
            console.log(arguments);
        } else if (p5) {
            console.log(p1, p2, p3, p4, p5);
        } else if (p4) {
            console.log(p1, p2, p3, p4);
        } else if (p3) {
            console.log(p1, p2, p3);
        } else if (p2) {
            console.log(p1, p2);
        } else if (p1) {
            console.log(p1);
        }
    };
    /*
        https://github.com/Eonasdan/bootstrap-datetimepicker
        eonasdan-bootstrap-datetimepicker
        <input type="text" class="form-control" datepicker ng-model="motion.DateTime">
    */
    angular.module('ase.com.ua').directive('datepicker', function ($parse) {
        return {
            require: '?ngModel',
            restrict: 'A',
            link: function ($scope, elem, $attrs, ctrl) {
                elem.datetimepicker({
                    locale: moment.locale(),
                    format: $attrs.moDateInput || 'L'
                });

                elem.on('dp.hide', function (e) {
                    ctrl.$setViewValue(e.target.value);
                });
            }
        };
    });

    /*
        Formating DateTime in model
        <input type="text" datepicker mo-date-input="L LT" ng-model="DateTime">
        {{DateTime | amDateFormat:'L LT'}} from angular-moment
    */
    angular.module('ase.com.ua').directive('moDateInput', function ($window) {
        return {
            require: '^ngModel',
            restrict: 'A',
            link: function (scope, elm, attrs, ctrl) {
                var moment = $window.moment;
                var dateFormat = attrs.moDateInput;
                attrs.$observe('moDateInput', function (newValue) {
                    if (dateFormat === newValue || !ctrl.$modelValue) {
                        return;
                    }
                    dateFormat = newValue;
                    ctrl.$modelValue = new Date(ctrl.$setViewValue);
                });

                ctrl.$formatters.unshift(function (modelValue) {
                    if (!dateFormat || !modelValue) {
                        return '';
                    }
                    var retVal = moment(new Date(modelValue)).format(dateFormat);
                    return retVal;
                });

                ctrl.$parsers.unshift(function (viewValue) {
                    var date = moment(viewValue, dateFormat).toDate();
                    var tzoffset = (new Date()).getTimezoneOffset() * 60000; //offset in milliseconds
                    var localISOTime = (new Date(date - tzoffset)).toISOString().slice(0, -1);
                    return localISOTime;
                });
            }
        };
    });

    /*

    */
    angular.module('ase.com.ua').directive('convertToNumber', function () {
        return {
            require: 'ngModel',
            link: function (scope, element, attrs, ngModel) {
                ngModel.$parsers.push(function (val) {
                    return parseInt(val, 10);
                });
                ngModel.$formatters.push(function (val) {
                    return '' + val;
                });
            }
        };
    });

    angular.module('ase.com.ua').filter('getBy', function () {
        return function (arr, prop, val, valprop, array) {
            if (arr === undefined) {
                return null;
            }
            if (array) {
                array = [];
            }
            var i = 0, len = arr.length;
            for (; i < len; i++) {
                var cval;
                if ((prop === undefined) | (prop === '')) {
                    cval = arr[i];
                } else {
                    cval = arr[i][prop];
                }
                if (cval === val) {
                    if (valprop !== undefined) {
                        if (array !== undefined) {
                            array.push(arr[i][valprop]);
                        } else {
                            return arr[i][valprop];
                        }
                    } else {
                        if (array !== undefined) {
                            array.push(arr[i][valprop]);
                        } else {
                            return arr[i];
                        }
                    }
                }
            }
            if (array !== undefined) {
                return array;
            }
            return null;
        };
    });

    angular.module('ase.com.ua').filter('each', function () {
        return function (array, prop, callBack) {
            for (var i = 0; i < array.length; i++) {
                //if ()
            }
        };
    });

    angular.module('ase.com.ua').directive('animateMe', function () {
        return function (scope, element, attrs) {
            scope.$watch(attrs.animateMe, function () {
                element.show(300).delay(900).hide(300);
            });
        };
    });

    angular.module('ase.com.ua').controller('ReuseableModalController', function ($scope, $uibModalInstance, data) {
        $scope.data = data;

        $scope.ok = function () {
            $uibModalInstance.close($scope.data);
        };

        $scope.cancel = function () {
            $uibModalInstance.dismiss('cancel');
        };
    });

    angular.module('ase.com.ua').directive('dynamic', function ($compile) {
        return {
            restrict: 'A',
            replace: true,
            link: function (scope, ele, attrs) {
                scope.$watch(attrs.dynamic, function (html) {
                    ele.html(html);
                    $compile(ele.contents())(scope);
                });
            }
        };
    });

    angular.module('ase.com.ua').filter('numberFixedLen', function () {
        return function (a, b) {
            return (1e4 + "" + a).slice(-b);
        };
    });

    angular.module('ase.com.ua').filter('iif', function () {
        return function (input, trueValue, falseValue) {
            return input ? trueValue : falseValue;
        };
    });

    angular.module('ase.com.ua').filter('orderByProperty', function () {
        return function (input, attribute) {
            if (!angular.isObject(input))
                return input;

            var array = [];
            for (var objectKey in input) {
                array.push(input[objectKey]);
            }

            array.sort(function (a, b) {
                if (a[attribute] < b[attribute]) return -1;
                if (a[attribute] > b[attribute]) return 1;
                return 0;
            });

            return array;
        }
    });

    angular.module('ase.com.ua').filter('minutes2hm', function ($UserSettingsService) {
        return function (min, apply) {
            apply = apply || $UserSettingsService.filters.minutes2hm.apply;
            if (apply === false) {
                return min;
            }
            var h = Math.floor(min / 60).toString();
            min = (min - (h * 60)).toString();
            return '' + '00'.substring(0, 2 - h.length) + h + ':' + '00'.substring(0, 2 - min.length) + min;
        };
    });

    angular.module('ase.com.ua')
        .service('$UserSettingsService', ['$resource', function () {
            var settings = {
                filters: {
                    minutes2hm: {
                        apply: true,
                    }
                }
            };

            return settings;
        }]);    
}());
