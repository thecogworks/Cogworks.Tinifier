angular.module("umbraco").controller("Cogworks.Tinifier.TinifierEditTImage.Controller",
    function ($scope, $routeParams, $http, $injector, notificationsService,appState) {
        // Get the ID from the route parameters (URL)
        var timageId = $routeParams.id;

        let metaData = [];
        let actions = appState.getMenuState('menuActions');
        _.each(actions, function (action) {
            if (action.alias === 'Tinifier_Settings')
                metaData = action.metaData;
        });
        // RecycleBinFolderId
        var recycleBinFolderId = -21;

        // Get the timage from the API
        $scope.timage = null;
        $scope.thistory = {
            OccuredAt: "-",
            Ratio: "-",
            OriginSize: "-",
            OptimizedSize: "-"
        };
        $scope.date = null;
        $scope.percent = "-";
        $scope.status = null;

        // Check if user choose Image or recycle bin folder
        if (timageId === recycleBinFolderId) {
            notificationsService.error("Error", "You cant`t tinify Folder!");
            return;
        }


        // Get Image information
        $http.get(metaData.imageStats).then(
            function (response) {
                if (response.data.history != null && response.data.history.IsOptimized) {
                    $scope.date = response.data.history.OccuredAt.replace("T", " ");
                    response.data.history.OccuredAt = $scope.date;
                    $scope.status = "Optimized";
                    $scope.thistory = response.data.history;
                    $scope.timage = response.data.tImage;
                    $scope.percent = ((1 - response.data.history.Ratio) * 100).toFixed(2) + "%";
                } else {
                    $scope.timage = response.data.tImage;
                    $scope.status = "Not optimized";
                    document.getElementById("pandaDiv").style.display = "none";
                }
            },
            function (response) {
                if (response.Error === 1) {
                    notificationsService.warning("Warning", response.message);
                }
                else {
                    notificationsService.error("Error", response.message);
                }
        });
    });