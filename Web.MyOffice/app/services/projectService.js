(function () {

    'use strict';

    angular.module('MyOffice.app')
        .service('ProjectService', ['$resource', ProjectService]);

    function ProjectService($resource) {
        return $resource('api/project/', {}, {
            query: { method: 'GET', params: {}, isArray: true },
            get: { method: 'GET', params: {}, isArray: false },
            post: { method: 'POST', params: {}, isArray: false },
            put: { method: 'PUT', params: {}, isArray: false },
            delete: { method: 'DELETE', params: {}, isArray: false },
        });
    };
})();
