/**
 * Establish the application routes.
 *
 * @type {exports}
 */

var app = require('../app');
var $ = require('jquery');

module.exports = function() {
    'use strict';

    app.config([
        '$stateProvider', '$urlRouterProvider',
        function($stateProvider, $urlRouterProvider) {

            var templateBase = '/MvcAssets/SBAngularSearchResults/public/';

            /**
             * Set the default route
             */
            $urlRouterProvider.otherwise("/");

            /**
             * Setup the routes
             */
            $stateProvider
                .state('app', {
                    url: "/",
                    views: {
                        'content': {
                            templateUrl: templateBase + "partials/search-form.html",
                            controller: 'MainController'
                        }
                    }
                })
                .state('app.results', {
                    url: "{searchPhrase}/page={pageNumber:int}/{filterParams:string}?sortBy",
                    params: {
                        pageNumber: 1,
                        filterParams: {
                            value: null,
                            squash: true
                        }
                    },
                    views: {
                        'content@': {
                            templateUrl: templateBase + 'partials/results.html',
                            controller: 'ResultsController'
                        }
                    }
                });

        }]);

    /**
     * Listeners for various rootScope events.
     */
    app.run(['$rootScope', '$state', 'utilService', function($rootScope, $state, utilService) {
        $rootScope.loadingRecords = false;

        $rootScope.$on('$stateChangeError', function(event, toState, toParams, fromState, fromParams, error) {
            event.preventDefault();
        });

        $rootScope.$on('$stateChangeStart', function(event, toState, toParams, fromState, fromParams, error) {
            $('.modal').modal('hide');
        });

        $rootScope.$on('cfpLoadingBar:started', function() {
            $rootScope.loadingRecords = true;
        });

        $rootScope.$on('cfpLoadingBar:completed', function() {
            $rootScope.loadingRecords = false;
        });
    }]);

};
