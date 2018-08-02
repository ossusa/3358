/**
 * API Service
 */

var app = require('../app');

module.exports = function() {

	app
		.factory('apiPathFactory', apiPathFactory)
		.factory('mockApiPathFactory', mockApiPathFactory);

    ////////////

    /**
     * Get the root API path for the current environment.
     *
     * @return {string} The API path.
     */
	function apiPathFactory() {
	    if (!window.IAFCSearch.searchBloxServerIp)
	        return "10.236.57.201";

        return window.IAFCSearch.searchBloxServerIp;
    }

    /**
     * Get the root API path for the mocked services.
     *
     * @return {string} The API path.
     */
    function mockApiPathFactory() {
        return '/MvcAssets/SBAngularSearchResults/public/mock-api/';
    }

};
