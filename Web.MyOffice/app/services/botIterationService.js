(function () {

    'use strict';

    angular.module('MyOffice.app')
        .service('BotIterationService', ['$resource', BotIterationService]);

    function BotIterationService($resource) {
        return $resource('https://skypebotmyoffice.azurewebsites.net/api/ProjectIteration', {}, {
            get: { method: 'GET', params: {}, isArray: false },
            query: { method: 'GET', params: {}, isArray: true },
        });
    };
})();
