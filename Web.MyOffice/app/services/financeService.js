(function () {

    'use strict';

    angular.module('MyOffice.app')
        .service('FinanceService', ['$resource', FinanceService]);

    function FinanceService($resource) {
        return $resource('api/finance/', {}, {
            get: { method: 'GET', params: {}, isArray: false },
        });
    };
})();
