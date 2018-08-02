'use strict';

/**
 * Establish the application routes.
 *
 * @type {exports}
 */

var app = require('../app');
var $ = require('jquery');

module.exports = function() {

    app.config(['$httpProvider',
        function($httpProvider) {
            //$httpProvider.interceptors.push('jsonpInterceptor');
        }
    ]);

};