(function () {

    'use strict';

    angular.module('MyOffice.app')
        .service('BudgetsService', ['$resource', BudgetsService]);

    function BudgetsService($resource) {
        return $resource('api/UserBudgets', {}, {
            getUsersBudgets: { method: 'GET', params: {}, isArray: false },
            postBudget: { method: 'PUT', params: {}, isArray: false },
            putBudgetUserList: { method: 'POST', params: {}, isArray: false }
        });
    };
})();
