(function () {

    'use strict';

    angular.module('MyOffice.app')
        .service('MemberMotionsService', ['$resource', MemberMotionsService]);

    function MemberMotionsService($resource) {
        return $resource('api/memberMotions/', {}, {
            query: { method: 'GET', params: {}, isArray: false },
            delete: { method: 'DELETE', params: {}, isArray: false },
        });
    };
})();
