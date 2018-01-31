(function () {
    'use strict';

    angular.module('MyOffice.app')
        .controller('currencyCtrl', ['$scope', 'CurrencyService', 'ModalWindowService', currencyCtrl]);

    function currencyCtrl($scope, CurrencyService, ModalWindowService) {
        $scope.showArchive = false;
        $scope.nameFilter = '';
        $scope.currencies = [];
        $scope.MWS = ModalWindowService;

        $scope.refresh = function () {
            CurrencyService.getCurrencyList({}, function (data) {
                $scope.currencies = data.currencies;
                $scope.currencyTypes = data.types;
            });
        };

        $scope.currencyEditDialog = function (currency) {
            $scope.newCurrency = currency || {};
            $scope.newCurrency.isEdited = !currency;
            $scope.newRate = { Value: currency.Value, DateTime: new Date() };
            ModalWindowService.open('CurrenciesController', 'CurrencyAdd', $scope, 'modal-sm');
        };

        $scope.addCurrency = function (newCurrency, newRate, form) {
            if (!form.$valid) {
                return;
            };
            CurrencyService.postCurrency({
                Id: newCurrency.Id,
                Name: newCurrency.Name,
                ShortName: newCurrency.ShortName,
                CurrencyType: newCurrency.CurrencyType,
                MyCurrency: newCurrency.MyCurrency,
                Value: newCurrency.Value
            }, function (data) {
                CurrencyService.putCurrencyRate({
                    Id: 0,
                    CurrencyId: data.Currency.Id,
                    DateTime: newRate.DateTime,
                    Value: newRate.Value
                }, function () {
                    $scope.refresh();
                    ModalWindowService.close('CurrencyAdd');
                });
            });
        };

        $scope.currencyDelete = function (currency) {
            CurrencyService.deleteCurrency({ currencyId: currency.Id, deleted: true }, function (data) {
                if (!data.ok) {
                    bootbox.alert(data.message);
                    return;
                }
                $scope.refresh();
            });
        };

        $scope.ratesUpdate = function (name) {
            CurrencyService.ratesUpdate({ name: name }, function () {
                $scope.refresh();
            });
        };

        $scope.setMyCurrency = function (currency) {
            currency.MyCurrency = true;
            CurrencyService.postCurrency(currency, function () {
                $scope.refresh();
            });
        };
    }
})();
