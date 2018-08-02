'use strict';

/**
 * SearchBlox Search Results
 *
 * Search Results client written in Angular
 */

var angular = require('angular');
var uiRouter = require('angular-ui-router');
var ngCookies = require('angular-cookies');
var loadingBar = require('angular-loading-bar');
var checklistModel = require('./directives/checklist-model');
var uiUtils = require('./vendor/ui-utils')();

module.exports = angular.module('searchbloxSearchResultsApp',
    [
        uiRouter,
        ngCookies,
        'ui.utils',
        'angular-loading-bar',
        'checklist-model',
        'utilityServices'
    ]
);