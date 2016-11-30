"use strinct;"

services.factory('CurrencyService', ['$resource',
    function ($resource) {
        return $resource('api/currency/', {}, {
            query: { method: 'GET', params: {}, isArray: true },
        });
    }]);
