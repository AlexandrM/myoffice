"use strict;"

services.factory('projectTaskCommentService', ['$resource',
    function ($resource) {
        return $resource('api/projectTaskComment/', {}, {
            query: { method: 'GET', params: {}, isArray: true },
            get: { method: 'GET', params: {}, isArray: false },
            post: { method: 'POST', params: {}, isArray: false },
            put: { method: 'PUT', params: {}, isArray: false },
            delete: { method: 'DELETE', params: {}, isArray: false },
        });
    }]);
