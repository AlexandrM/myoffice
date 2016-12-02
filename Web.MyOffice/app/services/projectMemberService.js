(function () {

    'use strict';

    angular.module('MyOffice.app')
        .service('ProjectMemberService', ['$resource', ProjectMemberService]);

    function ProjectMemberService($resource) {
        return $resource('api/projectMember/', {}, {
            post: { method: 'POST', params: {}, isArray: false },
            put: { method: 'PUT', params: {}, isArray: false },
            delete: { method: 'DELETE', params: {}, isArray: false },
        });
    };
})();
