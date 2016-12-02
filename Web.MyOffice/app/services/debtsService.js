(function () {

    'use strict';

    angular.module('MyOffice.app')
        .service('DebtsService', ['$resource', DebtsService]);

    function DebtsService($resource) {
        return $resource('api/debts/', {}, {
            get: { method: 'GET', params: {}, isArray: false },
        });
    };
})();
