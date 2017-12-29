(function () {

    'use strict';

    angular.module('MyOffice.app')
        .service('budgetAccountsService', ['$resource', budgetAccountsService]);

    function budgetAccountsService($resource) {
        return $resource('api/BudgetAccounts/', {}, {
            getUsersBudgets: { method: 'GET', params: {}, isArray: false },
            getAccountMotionsFlag: { method: 'GET', params: {}, isArray: false },
            postCategory: { method: 'PUT', params: {}, isArray: false },
            deleteCategory: { method: 'GET', params: {}, isArray: false },
            putAccount: { method: 'POST', params: {}, isArray: false },
            deleteAccount: { method: 'DELETE', params: {}, isArray: false }
        });
    };
})();
