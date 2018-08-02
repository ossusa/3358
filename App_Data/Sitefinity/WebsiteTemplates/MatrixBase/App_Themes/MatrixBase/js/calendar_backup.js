'use strict';

/**
 * IAFC Calendar
 */

var iafcCalendarApp = require('./app');
var apiService = require('./services')();
var moment = require('moment');
var _ = require('underscore');


module.exports = function () {

    if (!Array.prototype.filter) {
        Array.prototype.filter = function(fun/*, thisArg*/) {
            'use strict';

            if (this === void 0 || this === null) {
                throw new TypeError();
            }

            var t = Object(this);
            var len = t.length >>> 0;
            if (typeof fun !== 'function') {
                throw new TypeError();
            }

            var res = [];
            var thisArg = arguments.length >= 2 ? arguments[1] : void 0;
            for (var i = 0; i < len; i++) {
                if (i in t) {
                    var val = t[i];

                    // NOTE: Technically this should Object.defineProperty at
                    //       the next index, as push can be affected by
                    //       properties on Object.prototype and Array.prototype.
                    //       But that method's new, and collisions should be
                    //       rare, so use the more-compatible alternative.
                    if (fun.call(thisArg, val, i, t)) {
                        res.push(val);
                    }
                }
            }

            return res;
        };
    }

    iafcCalendarApp.controller('CalendarController', ['$scope', 'apiService', 'filterFilter', function ($scope, apiService, filterFilter) {

        // define scope variables
		$scope.search = {};
        $scope.calendarView = 'year';

        $scope.events = [];

		$scope.categories = [];

        $scope.locations = [
            {
                name: 'In-Person',
                key: 'isInPerson',
                selected: false
            },
            {
                name: 'Online',
                key: 'isOnline',
                selected: false
            }
        ];

        $scope.categorySelection = [];

        $scope.locationSelection = [];

        $scope.eventsLoaded = false;

        $scope.keyword = null;

        $scope.moment = moment;

        $scope.states =
            [
                {
                    "name": "Alabama",
                    "value": "AL"
                },
                {
                    "name": "Alaska",
                    "value": "AK"
                },
                {
                    "name": "American Samoa",
                    "value": "AS"
                },
                {
                    "name": "Arizona",
                    "value": "AZ"
                },
                {
                    "name": "Arkansas",
                    "value": "AR"
                },
                {
                    "name": "California",
                    "value": "CA"
                },
                {
                    "name": "Colorado",
                    "value": "CO"
                },
                {
                    "name": "Connecticut",
                    "value": "CT"
                },
                {
                    "name": "Delaware",
                    "value": "DE"
                },
                {
                    "name": "District Of Columbia",
                    "value": "DC"
                },
                {
                    "name": "Federated States Of Micronesia",
                    "value": "FM"
                },
                {
                    "name": "Florida",
                    "value": "FL"
                },
                {
                    "name": "Georgia",
                    "value": "GA"
                },
                {
                    "name": "Guam",
                    "value": "GU"
                },
                {
                    "name": "Hawaii",
                    "value": "HI"
                },
                {
                    "name": "Idaho",
                    "value": "ID"
                },
                {
                    "name": "Illinois",
                    "value": "IL"
                },
                {
                    "name": "Indiana",
                    "value": "IN"
                },
                {
                    "name": "Iowa",
                    "value": "IA"
                },
                {
                    "name": "Kansas",
                    "value": "KS"
                },
                {
                    "name": "Kentucky",
                    "value": "KY"
                },
                {
                    "name": "Louisiana",
                    "value": "LA"
                },
                {
                    "name": "Maine",
                    "value": "ME"
                },
                {
                    "name": "Marshall Islands",
                    "value": "MH"
                },
                {
                    "name": "Maryland",
                    "value": "MD"
                },
                {
                    "name": "Massachusetts",
                    "value": "MA"
                },
                {
                    "name": "Michigan",
                    "value": "MI"
                },
                {
                    "name": "Minnesota",
                    "value": "MN"
                },
                {
                    "name": "Mississippi",
                    "value": "MS"
                },
                {
                    "name": "Missouri",
                    "value": "MO"
                },
                {
                    "name": "Montana",
                    "value": "MT"
                },
                {
                    "name": "Nebraska",
                    "value": "NE"
                },
                {
                    "name": "Nevada",
                    "value": "NV"
                },
                {
                    "name": "New Hampshire",
                    "value": "NH"
                },
                {
                    "name": "New Jersey",
                    "value": "NJ"
                },
                {
                    "name": "New Mexico",
                    "value": "NM"
                },
                {
                    "name": "New York",
                    "value": "NY"
                },
                {
                    "name": "North Carolina",
                    "value": "NC"
                },
                {
                    "name": "North Dakota",
                    "value": "ND"
                },
                {
                    "name": "Northern Mariana Islands",
                    "value": "MP"
                },
                {
                    "name": "Ohio",
                    "value": "OH"
                },
                {
                    "name": "Oklahoma",
                    "value": "OK"
                },
                {
                    "name": "Oregon",
                    "value": "OR"
                },
                {
                    "name": "Palau",
                    "value": "PW"
                },
                {
                    "name": "Pennsylvania",
                    "value": "PA"
                },
                {
                    "name": "Puerto Rico",
                    "value": "PR"
                },
                {
                    "name": "Rhode Island",
                    "value": "RI"
                },
                {
                    "name": "South Carolina",
                    "value": "SC"
                },
                {
                    "name": "South Dakota",
                    "value": "SD"
                },
                {
                    "name": "Tennessee",
                    "value": "TN"
                },
                {
                    "name": "Texas",
                    "value": "TX"
                },
                {
                    "name": "Utah",
                    "value": "UT"
                },
                {
                    "name": "Vermont",
                    "value": "VT"
                },
                {
                    "name": "Virgin Islands",
                    "value": "VI"
                },
                {
                    "name": "Virginia",
                    "value": "VA"
                },
                {
                    "name": "Washington",
                    "value": "WA"
                },
                {
                    "name": "West Virginia",
                    "value": "WV"
                },
                {
                    "name": "Wisconsin",
                    "value": "WI"
                },
                {
                    "name": "Wyoming",
                    "value": "WY"
                }
            ];

        $scope.currentDate = moment();

        $scope.weekDates = [];

		$scope.clearFilter = function () {
            delete $scope.search.$;
            delete $scope.search.location;
            //$scope.locationSelection = [];
			for(var i=0; i < $scope.locations.length; i++){
				$scope.locations[i].selected = false;
			}
        };

        $scope.getWeekDates = function(){
            var dates = [$scope.currentDate];

            for (var i = 1; i < 8; i++) {
                dates.push(moment($scope.currentDate).add(i, 'days'));
            }

            return dates;
        };

        $scope.getCurrentDate = function () {
            return $scope.currentDate;
        };

        $scope.calendarNext = function () {
            if ($scope.calendarView == 'week') {
                $scope.currentDate.add(7, 'days');
                $scope.weekDates = $scope.getWeekDates();
            } else if ($scope.calendarView == 'month') {
                $scope.currentDate.add(1, 'month');
            } else if ($scope.calendarView == 'year') {
                $scope.currentDate.add(1, 'year');
            }
        };

        $scope.calendarPrev = function () {
            if ($scope.calendarView == 'week') {
                $scope.currentDate.subtract(7, 'days');
                $scope.weekDates = $scope.getWeekDates();
            } else if ($scope.calendarView == 'month') {
                $scope.currentDate.subtract(1, 'month');
            } else if ($scope.calendarView == 'year') {
                $scope.currentDate.subtract(1, 'year');
            }
        };

        $scope.calendarToday = function () {
            $scope.currentDate = moment();
            $scope.weekDates = $scope.getWeekDates();
        };

        $scope.init = function () {

            $scope.weekDates = $scope.getWeekDates();

            apiService.getFutureEvents().then(function (data) {

                $scope.events = jQuery.map(data.events, function(event){
                    event.eventStart = $scope.prettyDate(event.eventStart);
                    event.eventEnd = $scope.prettyDate(event.eventEnd);
                    event.startMonth = $scope.month(event.eventStart);
                    event.startDay = $scope.day(event.eventStart);
                    event.endMonth = $scope.month(event.eventEnd);
                    event.endDay = $scope.day(event.eventEnd);
                    event.eventMonths = event.startMonth;
                    event.eventDays = event.endDay;
					event.isOnline = event.location.state === 'Online';
					event.isInPerson = event.location.state !== 'Online';
					//event.topics = event.categories;
                    // event.topics = categoriesHide(event.categories);

					event.categories = _.reject(event.categories, function(category) {
						return category.indexOf('featured-') === 0;
					});

					event.topics  = event.categories.map(function(category){
						return category.replace(/-/g, " ");
					});


					  if(event.startMonth != event.endMonth){
                        event.eventMonths = event.startMonth + " - " + event.endMonth;
                      }
                      if(event.startDay != event.endDay){
                        event.eventDays = event.startDay + " - " + event.endDay;
                      }
                      if(event.location.city && event.location.state){
                        event.place = event.location.city + ", " + event.location.state;
                      }
                      else if(event.location.city || event.location.state){
                        event.place = event.location.city + event.location.state;
                      }

                    return event;
                });
            }, function (message) {
                alert(message.error);
            })['finally'](function(){
                $scope.eventsLoaded = true;
            });

        };

        $scope.prettyDate = function (date) {
            return moment(date).format('MMM-DD-YYYY');
        };
        $scope.month = function (date) {
            return moment(date).format('MMM');
        };
        $scope.day = function (date) {
            return moment(date).format(' D ');
        };

        // helper method to get selected filters
        $scope.selectedCategories = function selectedCategories() {
            return filterFilter($scope.categories, {selected: true});
        };

        // watch filters for changes
        $scope.$watch('categories|filter:{selected:true}', function (nv) {
            $scope.categorySelection = jQuery.map(nv, function (filter) {
                return filter.key;
            });
        }, true);

		$scope.$watch('locations|filter:{selected:true}', function (nv) {
            $scope.locationSelection = jQuery.map(nv, function (filter) {
                return filter.key;
            });
        }, true);

        $scope.init();


    }]);

    iafcCalendarApp.filter('selectedCategories', function () {
        return function (events, categories) {
            return events.filter(function (event) {

                if (categories.length === 0) return true;

                for (var i in event.categories) {
                    if (categories.indexOf(event.categories[i]) != -1) {
                        return true;
                    }
                }
                return false;

            });
        };
    });

    iafcCalendarApp.filter('onDate', function () {
        return function (events, date) {
            return events.filter(function (event) {

                if (event.eventStart == moment(date).format('MM/DD/YYYY')) {
                    return true;
                }
                return false;

            });
        };
    });

    iafcCalendarApp.filter('onMonth', function () {
        return function (events, date) {
            return events.filter(function (event) {

                if (moment(event.eventStart).format('MM/YYYY') == moment(date).format('MM/YYYY')) {
                    return true;
                }
                return false;

            });
        };
    });

    iafcCalendarApp.filter('onYear', function () {
        return function (events, date) {
            return events.filter(function (event) {

                if (moment(event.eventStart).format('YYYY') == moment(date).format('YYYY')) {
                    return true;
                }
                return false;

            });
        };
    });

	iafcCalendarApp.filter('locationFilter', function () {
        return function (events, locations) {
            return events.filter(function (event) {

				if (locations.length === 0) return true;

                for (var i = 0; i < locations.length; i++) {
					if (event[locations[i]] === true) {
						return true;
					}
				}

				return false;

            });
        };
    });

};
