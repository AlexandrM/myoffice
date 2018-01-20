(function () {
    'use strict';

    angular.module('MyOffice.app')
        .controller('currencyCtrl', ['$scope', 'CurrencyService', 'ModalWindowService', currencyCtrl]);

    function currencyCtrl($scope, CurrencyService, ModalWindowService) {
        $scope.showArchive = false;
        $scope.nameFilter = '';
        $scope.currencies = [];

        $scope.refresh = function () {
            CurrencyService.getCurrencyList({}, function (data) {
                $scope.currencies = data.currencies;
                $scope.currencyTypes = data.types;
                $scope.baseCurrency = data.BaseCurrency;
                $scope.warnings = data.warnings;
            });
        };

        $scope.closeWindow = function (name) {
            ModalWindowService.close(name);
            $scope.refresh();
        };

        $scope.openForm = function (viewName, size) {
            ModalWindowService.open('CurrenciesController', viewName, $scope, size);
        };

        $scope.currencyEditDialog = function (currency) {
            $scope.newCurrency = currency === undefined ? {} : currency;
            $scope.newCurrency.isEdited = currency !== undefined;
            var value = 0;
            if (currency) {
                value = $scope.currencyTypes[currency.CurrencyType - 1].Name === $scope.baseCurrency ? 1 : currency.Value;
                value = currency.Value === undefined ||
                        currency.Value === null      ||
                        currency.Value === '' ? 0 : value;

            }

            var options = { year: 'numeric', month: 'numeric', day: 'numeric' };
            var date = (new Date()).toLocaleDateString(navigator.language, options);
            $scope.newRate = { Value: value, DateTime: date };
            $scope.openForm('CurrencyAdd','modal-sm');
        };

        $scope.setNewRateDate = function () {
            $('#newRateDate').datetimepicker({ locale: navigator.language });
        };

        $scope.postCurrency = function (newCurrency, newRate) {
            CurrencyService.postCurrency({
                Id: newCurrency.Id,
                Name: newCurrency.Name,
                ShortName: newCurrency.ShortName,
                CurrencyType: newCurrency.CurrencyType,
                MyCurrency: newCurrency.MyCurrency,
                Value: newCurrency.Value
            },
                function () {
                    if (newRate) {
                        CurrencyService.putCurrencyRate({
                            Id: 0,
                            Currency: newCurrency,
                            CurrencyId: newCurrency.Id,
                            DateTime: newRate.DateTime,
                            Value: newRate.Value
                        }, function () {
                            $scope.refresh();
                            $scope.closeWindow('CurrencyAdd');
                        });
                    } else {
                        $scope.refresh();
                        $scope.closeWindow('CurrencyAdd');
                    }
                });
        };

        $scope.addCurrency = function (newCurrency, currencyRate, form) {
            if (form.$valid) {
                var newCurrencyAlreadyExists = $scope.currencies.find(function (_currency) {
                    return +_currency.CurrencyType === +newCurrency.CurrencyType;
                }) !== undefined;
                if (!newCurrencyAlreadyExists || newCurrency.isEdited) {
                    $scope.postCurrency(newCurrency, currencyRate);
                } else {
                    bootbox.alert($scope.warnings[0]);
                }
            };
        };

        $scope.currencyArchive = function (currency) {
            if (!currency.MyCurrency) {
                CurrencyService.editCurrency({ currencyId: currency.Id, deleted: true }, function () {
                    $scope.refresh();
                });
            } else {
                bootbox.alert($scope.warnings[1]);
            }
        };

        $scope.restoreCurrency = function (currency) {
            CurrencyService.editCurrency({ currencyId: currency.Id, deleted: false }, function () {
                $scope.refresh();
            });
        };

        $scope.deleteCurrency = function (Id) {
            CurrencyService.deleteCurrency({ currencyId: Id }, function (data) {
                if (!data.result) {
                    bootbox.alert(data.message);
                }
                $scope.refresh();
            });
        };

        $scope.setMyCurrency = function (currency) {
            $scope.postCurrency(currency);
        };

        $scope.ratesUpdate = function(sourceName){
                CurrencyService.updateRates({ sourceName: sourceName }, function () {
                    $scope.refresh();
                });
            };
        }
})();
