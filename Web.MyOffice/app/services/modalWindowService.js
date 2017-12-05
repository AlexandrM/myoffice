(function () {
    angular.module('MyOffice.app').service('ModalWindowService', ['$http', '$uibModal', ModalWindowService]);
    windows = {};

    function ModalWindowService($http, $uibModal) {
        return {
            open: function (controllerName, viewName, scope,  size) {

                $http.get('api/ModalWindow/', { params: { controllerName: controllerName, viewName: viewName } }).then(function (response) {
                    windows[viewName] = $uibModal.open({
                        animation: true,
                        template: response.data,
                        size: size,
                        backdrop: false,
                        keyboard: true,
                        scope: scope
                    });
                });
            },
            close: function (name) {
                if (windows[name] !== undefined) {
                    windows[name].closed.$$state.status = 0;
                    windows[name].close();
                }
        }
        };
    }
})();
