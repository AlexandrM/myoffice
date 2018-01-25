(function () {

    'use strict';

    angular.module('MyOffice.app')
        .service('CurrencyService', ['$resource', CurrencyService]);

    function CurrencyService($resource) {
        return $resource('api/currencies/', {}, {
            getCurrencyList: { method: 'GET', params: {}, isArray: false },
            updateRates: { url: 'api/currencies/updateRates', method: 'POST', params: {}, isArray: false },
            postCurrency: { url: 'api/currencies/postCurrency', method: 'POST', params: {}, isArray: false },
            putCurrencyRate: { url: 'api/currencies/postCrrencyRate', method: 'POST', params: {}, isArray: false },
            deleteCurrency: { url: 'api/currencies/deleteCurrency', method: 'DELETE', params: {}, isArray: false },
            ratesUpdate: { url: 'api/currencies/ratesUpdate', method: 'GET', params: {}, isArray: false },
        });
    };
})();
