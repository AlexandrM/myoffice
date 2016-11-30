"use strinct;"

services.factory('FinanceService', ['$resource',
    function ($resource) {
        return $resource('api/finance/', {}, {
            get: { method: 'GET', params: {}, isArray: false },
        });
    }]);
