'use strict';

/**
 * IAFC Calendar Services
 */

var iafcCalendarApp = require('./app');
var _ = require('underscore');

module.exports = function () {

    iafcCalendarApp.service('apiService', ['$http', '$q', '$cacheFactory', function ($http, $q, $cacheFactory) {

        /**
         * Base API URL
         *
         * @type {string}
         */
        this.apiUrl = '/Api/';

        /**
         * Get the events.
         *
         * GET Events/GetAll
         * @returns {*}
         */
        this.getFutureEvents = function () {
            var deferred = $q.defer();
            var serviceUrl = this.apiUrl + 'Events/GetFuture';

            $http({
                url: serviceUrl,
                cache: true,
                responseType: 'json'
            }).success(function (data, status, headers, config) {
                deferred.resolve(data);
            }).error(function (data, status, headers, config) {
                deferred.reject({error: "There was an error retrieving the events."});
            });

            return deferred.promise;
        };

        /**
         * Get news items.
         *
         * GET NewsItems/GetAll
         * @return {*}
         */
        this.getNewsItems = function (keyword, year) {
            var deferred = $q.defer();
            var serviceUrl = this.apiUrl + 'NewsItems/GetAll';
            var params = {
                'summary': true
            };

            if (keyword) {
                params['term'] = keyword;
            }

            if (year && year != 'all') {
                params['year'] = year;
            }

            $http({
                url: serviceUrl,
                params: params,
                responseType: 'json'
            }).success(function (data, status, headers, config) {
                deferred.resolve(data);
            }).error(function (data, status, headers, config) {
                deferred.reject({error: "There was an error retrieving the news items."});
            });

            return deferred.promise;
        };

    }]);

};
