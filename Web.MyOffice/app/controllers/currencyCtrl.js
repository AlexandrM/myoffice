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
                $scope.currencyTypes = data.types.filter(function(type){
                    return type.Name !== 'OTHER';
                });
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

            $scope.newCurrency = currency;
            $scope.newCurrency.isEdited = Object.keys(currency).length>0;
            var currentDate = new Date();
            currentDate.setTime(Date.now());
            var options = {
                year: 'numeric',
                month: 'numeric',
                day: 'numeric',
                hour: 'numeric',
                minute: 'numeric'
            };
            $scope.newRate = { Value: currency.Value, DateTime: currentDate.toLocaleDateString(navigator.language, options) };
            $scope.openForm('CurrencyAdd','modal-sm');
        };

        $scope.setNewRateDate = function () {
            $('#newRateDate').datetimepicker({ locale: navigator.language });
        };

        $scope.postCurrency = function(newCurrency, newRate) {
            CurrencyService.postCurrency({
                    Id: newCurrency.Id,
                    Name: newCurrency.Name,
                    ShortName: newCurrency.ShortName,
                    CurrencyType: newCurrency.CurrencyType,
                    MyCurrency: newCurrency.MyCurrency,
                    Value: newCurrency.Value
                },
                function () {
                    if (newRate && newRate.Value !== undefined) {
                        CurrencyService.putCurrencyRate({
                            Id: 0,
                            Currency:newCurrency,
                            CurrencyId:newCurrency.Id,
                            DateTime: newRate.DateTime,
                            Value:newRate.Value
                        }, function () {
                            if (windows['CurrencyAdd'] !== undefined) {
                                $scope.refresh();
                            };
                        });
                    } else  {
                        if (windows['CurrencyAdd'] !== undefined) {
                            $scope.refresh();
                        };
                    };
                    $scope.closeWindow('CurrencyAdd');
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
