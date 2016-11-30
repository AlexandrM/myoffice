"use strinct;"

controllers.controller('projectTaskCtrl', ['$scope', '$route', '$routeParams', '$location', 'ProjectTaskService', 'projectTaskCommentService',
    function ($scope, $route, $routeParams, $location, ProjectTaskService, projectTaskCommentService) {
        $scope.forms = {};
        $scope.refresh = function () {
            $scope.projectTask = ProjectTaskService.get({ id: $routeParams.id, projectId: $routeParams.projectId }, function () {
            });
        };
        $scope.setState = function (projectTask, state) {
            projectTask.State = state;
            ProjectTaskService.setState(projectTask, function () {
                $scope.refresh();
            });
        }
        $scope.save = function (projectTask) {
            ProjectTaskService.post(projectTask, function (data) {
                $routeParams.id = data.Id;
                $route.updateParams($routeParams);
                $scope.refresh();
            });
        }
        $scope.history = function (projectTask) {
            bootbox.dialog({
                className: "modal-dialog-bootbox",
                buttons: {
                    success: {   
                        label: ASE.L.Close,
                        className: "btn-primary",
                        callback: function() {}
                    },               
                },
                title: ASE.L.History,
                message: projectTask.History,
            });
        }
        $scope.postComment = function (comment) {
            projectTaskCommentService.post(comment, function () {
                $scope.refresh();
            });
        }
        $scope.putComment = function (comment) {
            projectTaskCommentService.put(comment, function () {
                $scope.refresh();
            });
        }
    }]);
