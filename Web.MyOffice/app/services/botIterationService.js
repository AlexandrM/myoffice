(function () {

    'use strict';

    angular.module('MyOffice.app')
        .service('BotIterationService', ['$resource', BotIterationService]);

    function BotIterationService($resource) {
        return $resource('https://bot.ase.com.ua/api/ProjectIteration', {}, {
            get: { method: 'GET', params: {}, isArray: false },
            query: { method: 'GET', params: {}, isArray: true },
        });
    };
})();
