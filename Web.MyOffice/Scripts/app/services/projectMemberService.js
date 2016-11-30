"use strinct;"

services.factory('ProjectMemberService', ['$resource',
    function ($resource) {
        return $resource('api/projectMember/', {}, {
            post: { method: 'POST', params: {}, isArray: false },
            put: { method: 'PUT', params: {}, isArray: false },
            delete: { method: 'DELETE', params: {}, isArray: false },
        });
    }]);
