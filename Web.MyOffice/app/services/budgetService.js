(function () {

    'use strict';

    angular.module('MyOffice.app')
        .service('BudgetService', ['$resource', BudgetService]);

    function BudgetService($resource) {
        return $resource('api/UserBudgets', {}, {
            getUsersBudgets: { method: 'GET', params: {}, isArray: false },
            postBudget: { method: 'PUT', params: {}, isArray: false },
            putBudgetUserList: { method: 'POST', params: {}, isArray: false },
            delete: { method: 'DELETE', params: {}, isArray: false },
            findUser: { url: 'api/UserBudgets/findUser', method: 'get', params: {}, isArray: false },
            deleteUser: { url: 'api/UserBudgets/deleteUser', method: 'DELETE', params: {}, isArray: false },
            addUser: { url: 'api/UserBudgets/addUser', method: 'POST', params: {}, isArray: false },
        });
    };
})();
