(function () {

    'use strict';

    angular.module('MyOffice.app')
        .service('CurrencyService', ['$resource', CurrencyService]);

    function CurrencyService($resource) {
        return $resource('api/currencies/', {}, {
            getCurrencyList: { method: 'GET', params: {}, isArray: false },
            editCurrency: { method: 'GET', params: {}, isArray: false },
            updateRates: { method: 'GET', params: {}, isArray: false },
            postCurrency: { method: 'POST', params: {}, isArray: false },
            deleteCurrency: { method: 'DELETE', params: {}, isArray: false }
        });
    };
})();
