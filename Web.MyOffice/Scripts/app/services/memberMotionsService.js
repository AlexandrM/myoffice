"use strinct;"

services.factory('MemberMotionsService', ['$resource',
    function ($resource) {
        return $resource('api/memberMotions/', {}, {
            query: { method: 'GET', params: {}, isArray: false },
            delete: { method: 'DELETE', params: {}, isArray: false },
        });
    }]);
