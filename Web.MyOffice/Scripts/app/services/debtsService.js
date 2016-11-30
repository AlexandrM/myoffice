"use strinct;"

services.factory('DebtsService', ['$resource',
    function ($resource) {
        return $resource('api/debts/', {}, {
            get: { method: 'GET', params: {}, isArray: false },
        });
    }]);
