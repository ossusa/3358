'use strict';

/**
 * NLN Angular apps
 */

var angular = require('angular');

var pagination = require('./vendor/dir-pagination')(angular);

module.exports = angular.module('iafcCalendarApp', ['angularUtils.directives.dirPagination']);
