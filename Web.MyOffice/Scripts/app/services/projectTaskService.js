"use strict;"

services.factory('ProjectTaskService', ['$resource',
    function ($resource) {
        return $resource('api/projectTask/', {}, {
            get: { method: 'GET', params: {}, isArray: false },
            post: { method: 'POST', params: { mode: '' }, isArray: false },
            postComment: { method: 'POST', params: {}, isArray: false },
            addDay: { method: 'POST', params: { mode: 'addDay' }, isArray: false },
            setState: { method: 'POST', params: { mode: 'setState' }, isArray: false },
        });
    }]);
