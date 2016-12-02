(function () {

    'use strict';

    angular.module('MyOffice.app')
        .service('CurrencyService', ['$resource', CurrencyService]);

    function CurrencyService($resource) {
        return $resource('api/currency/', {}, {
            query: { method: 'GET', params: {}, isArray: true },
        });
    };
})();
