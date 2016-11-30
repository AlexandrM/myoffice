"use strinct;"

services.factory('MemberDayReportService', ['$resource',
    function ($resource) {
        return $resource('api/memberDayReport/', {}, {
            query: { method: 'GET', params: {}, isArray: false },
            get: { method: 'GET', params: {}, isArray: false },
            post: { method: 'POST', params: {}, isArray: false },
            put: { method: 'PUT', params: {}, isArray: false },
            delete: { method: 'DELETE', params: {}, isArray: false },
        });
    }]);
