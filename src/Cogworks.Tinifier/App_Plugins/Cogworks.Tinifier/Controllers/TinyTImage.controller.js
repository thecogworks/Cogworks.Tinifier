angular.module("umbraco").controller("Cogworks.Tinifier.TinyTImage.Controller",
    function ($scope, $routeParams, $http, $injector, navigationService, notificationsService,appState) {
        // Get the ID from the route parameters (URL)
        var timageId = $routeParams.id;

        var arrOfNames = [];
        var selectedImages = document.querySelectorAll(".umb-media-grid__item.umb-outline.umb-outline--surrounding.ng-scope.-selectable.-selected");


        let metaDataTinify = {};
        let metaDataUndoTinify = {};
        let actions = appState.getMenuState('menuActions');
        _.each(actions, function (action) {
            if (action.alias === 'Tinifier_Button')
            metaDataTinify = action.metaData;
            if (action.alias === 'Undo_Tinifier_Button')
            metaDataUndoTinify = action.metaData;
        });

        for (var i = 0; i < selectedImages.length; i++) {
            var innerHtml = selectedImages[i].innerHTML;
            var regex = /<img.*?src=['"](.*?)['"]/;
            var src = regex.exec(innerHtml)[1];
            var slice = src.split("?")[0];
            arrOfNames.push(slice);
        }

        // RecycleBinFolderId
        var recycleBinFolderId = "-21";

        // Get from the API
        $scope.timage = null;

        $scope.cancel = function () {
            navigationService.hideDialog();
        };

        // Tinify Image and show notification
        $scope.tinify = function () {
            navigationService.hideDialog();
            // Check if user choose Image or recycle bin folder
            if (timageId === recycleBinFolderId) {
                notificationsService.error("Error", "You can`t tinify RecycleBin Folder!");
                return;
            }

            if (timageId == null) {
                notificationsService.error("Error", "If you wish to optimize full Media folder, please, go to Tinifier section and click Tinify everything");
                return;
            }

            notificationsService
                .add({
                    headline: "Tinifing started",
                    message: "click <a href=\"/umbraco/#/tinifier\" target=\"_blank\">here</a> to see tinifier configuration",
                    url: '/umbraco/#/tinifier',
                    type: 'success'
                });

            var url =  arrOfNames.length !== 0
                ? "/umbraco/backoffice/api/Tinifier/TinyTImages?imageRelativeUrls=" + arrOfNames.join(',')
                :  metaDataTinify.tinifyImage;

            $http.get(url)
                .then(successHandler, errorHandler);
        };

        $scope.undoTinify = function () {
            navigationService.hideDialog();
            // Check if user choose Image or recycle bin folder
            if (timageId === recycleBinFolderId) {
                notificationsService.error("Error", "You can`t tinify RecycleBin Folder!");
                return;
            }

            if (timageId == null) {
                notificationsService.error("Error", "Undo tinifing all Media folder is not supported");
                return;
            }

            notificationsService.add({
                headline: "Undo Tinifing started",
                type: 'success'
            });

            var url = metaDataUndoTinify.undoTinifyImage;

            $http.get(url).then(successHandler, errorHandler);
        };

        $scope.tinifyAll = function () {
            $http.put(`/umbraco/backoffice/api/Tinifier/TinifyEverything`)
                .then(successHandler, errorHandler);
        };

        function successHandler(response) {
            notificationsService.add({
                headline: response.data.Message,
                type: 'success'
            });
        }

        function errorHandler(response) {
            notificationsService.add({
                headline: response.data.Message,
                type: 'error'
            });
        }
    });