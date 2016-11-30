"use strinct;"

services.factory('MemberService', ['$resource',
    function ($resource) {
        return $resource('api/member/', {}, {
            query: { method: 'GET', params: {}, isArray: true },
        });
    }]);
