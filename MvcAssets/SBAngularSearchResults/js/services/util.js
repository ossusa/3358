/**
 * Utility Service
 *
 * A collection of utility methods to be used throughout the application.
 */

var app = require('../utilityModule');
var _ = require('underscore');
var moment = require('moment');

module.exports = function() {


    app.service('utilService', [
        'apiPathFactory', 'mockApiPathFactory', '$cookies', '$http', '$state',
        function(apiPathFactory, mockApiPathFactory, $cookies, $http, $state) {

            var service = {
                preventSubmitOnEnter: preventSubmitOnEnter,
                getTemplatePath: getTemplatePath,
                getApiPath: getApiPath,
                getMockApiPath: getMockApiPath,
                goToResults: goToResults
            };

            return service;

            ////////////

            /**
             * Prevent forms from submitting when the enter key is pressed within
             * a form input field.
             *
             * @param  {event} e The keystroke event.
             */
            function preventSubmitOnEnter(e) {
                if (e.keyCode === 13 && e.target.type !== "textarea") {
                    e.preventDefault();
                }
            }

            /**
             * Get the relative public path for templates.
             *
             * @return {string} The path string.
             */
            function getTemplatePath() {
                return '/MvcAssets/AssessmentFrameworkApp/public/';
            }

            /**
             * Get the base API path.
             *
             * @returns {string}
             */
            function getApiPath() {
                return apiPathFactory;
            }

            /**
             * Get the mock API path.
             *
             * @returns {string}
             */
            function getMockApiPath() {
                return mockApiPathFactory;
            }

            /**
             * Go the first results page based on the given search phrase.
             *
             * @param  {string} searchPhrase The string that's being queried on.
             * @param  {string} sortBy       The property to sort by.
             */
            function goToResults(searchPhrase, sortBy) {
                if (searchPhrase !== '') {
                    $state.go('app.results', {
                        searchPhrase: searchPhrase,
                        pageNumber: 1,
                        filterParams: null,
                        sort: sortBy
                    }, {
                        reload: true
                    });
                }
            }

        }]);

};
