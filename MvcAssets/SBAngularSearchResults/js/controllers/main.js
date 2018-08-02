'use strict';

/**
 * Main Controller
 */

var app = require('../app');

module.exports = function() {

    app.controller('MainController', ['$scope', 'utilService',
        function($scope, utilService) {

            $scope.templatePath = utilService.getTemplatePath();

            $scope.executeSearch = utilService.goToResults;

            $scope.newSearchPhrase = '';

            $scope.executeSearchOnEnter = function(e) {
                if (e.keyCode === 13) {
                    e.preventDefault();
                    $scope.executeSearch($scope.newSearchPhrase);
                }
			};

        }]);

};
