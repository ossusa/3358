/**
 * Results Controller
 */

var app = require('../app');
var _ = require('underscore');
var dotdotdot = require('dotdotdot/src/js/jquery.dotdotdot.js');

module.exports = function() {

    /*========================================================================\
    |   LOOKING TO MAKE CHANGES TO THIS WIDGET? CHECK OUT searchBlox.js!     |
    `=======================================================================*/

    var dateRanges = [
                {
                    "name": "Past Week",
                    "startdate": new Date(new Date().getTime()
                                          - 1000 * 60 * 60 * 24 * 7),
                    "enddate": new Date()
                },
                {
                    "name": "Past Month",
                    "startdate": new Date(new Date().getTime()
                                          - 1000 * 60 * 60 * 24 * 7 * 4),
                    "enddate": new Date()
                },
                {
                    "name": "Past Year",
                    "startdate": new Date(new Date().setYear(
                        new Date().getFullYear() - 1)),
                    "enddate": new Date()
                },
                {
                    "name": "Past 5 Years",
                    "startdate": new Date(new Date().setYear(
                        new Date().getFullYear() - 5)),
                    "enddate": new Date()
                },
                {
                    "name": "Older",
                    "startdate": new Date(new Date().setYear(
                        new Date().getFullYear() - 2016)),
                    "enddate": new Date()
                }
    ];

	app.controller('ResultsController', ['$scope', '$rootScope', '$sce', '$state', 'utilService', 'searchBloxService',
		function($scope, $rootScope, $sce, $state, utilService, searchBloxService) {

		    /* If you want to access variables or functions in results.html,
             * you can put them in the $scope variable and simply access them
             * by name. (i.e. you define $scope.myFunction(). You can now call
             * it in results.html like {{myFunction()}}.
             */

		    /**
			 * Get the search results.
			 */
		    $scope.getResults = function () {
		        var searchParams = {
		            q: $scope.searchPhrase,
		            //start: $scope.result.start,
		            rows: $scope.result.rows,
		            sort: $scope.sortBy,
		            filterParams: $scope.filterParams
		        };

		        $scope.isSearchDone = false;

		        //searchBloxService = searchBlox.js
		        searchBloxService.getResults(searchParams).then(
                    function (res) //success
                    {
                        $scope.handleSearchResponse(res);
                    },
                    function (res) //failure
                    {
                        console.log(res);
                    })['finally'](function (res) {
                        $scope.isSearchDone = true;
                    });

                //Start a loop that keeps descriptions' length in check.
		        startLoop();
		    };

		    /**
			 * Handle the response from the SearchBlox search.
			 *
			 * @param  {object} res The results retrieved from SearchBlox.
			 */
		    $scope.handleSearchResponse = function (res) {
		        $scope.result.recordCount = res["results"]["@lastpage"];
		        $scope.result.hits = res["results"]["@hits"];

		        var results = res.results;
		        if (!results.result || results.result.length == undefined) {
		            $scope.result.records = [results.result, getDummyRecord()];
		        }
		        else {
		            $scope.result.records = results.result;
		        }

		        /* Best bets are results that are always returned when a given
                 * search is performed. They're returned in the results as
                 * "ads". 
                 *
                 * If none are returned, it looks like "ads": [],
                 *
                 * If only one is returned, it is returned as:
                 * "ads": {"ad": {"@no": "1",..., "@uid": "2"}},. 
                 *
                 * If more than one is returned, it looks like this:
                 * "ads": [{"@no": "1",...},...,{"@no": "n",...}],
                 *
                 * To access it in results.html, we'll store it in 
                 * $scope.result.bestBets .
                 */
		        var bestBets = res["ads"];
		        if(bestBets){
		            if (bestBets instanceof Array) {
		                $scope.result.bestBets = bestBets;
		            }
		            else {
		                $scope.result.bestBets = [
                            bestBets.ad,
                            getDummyRecord()
		                ];
		            }
		        }
		        else {
		            $scope.result.bestBets = [];
		        }

		        // populate the facets
		        $scope.result.facetFields = {};
		        if (typeof res == 'object' && res["facets"]) {
		            _.map(res.facets, f => {
		                var subfs = []; //subfs = subfacets

		                if (f["@count"] > 0) {
		                    var ints = f["int"];
		                    /* In the case where there is only one subfacet, we
                             * do not get an array, so we need to make it one.
                             */
		                    if (typeof ints.map !== "function") {
		                        ints = [f["int"]];
		                    }

		                    //Convert them to the correct format
		                    ints.map(sub => {
		                        subfs.push({
		                            "name": sub["@name"],
		                            "count": sub["#text"],
		                            "sbparam": 'f.' + f['@name'] + '.filter=' + sub['@name']
		                        });
		                    });
		                }

		                /* If we only have 1 element, angular will turn
				         * it into an array of ONLY VALUES. So, we need to
				         * add a dummy node. Make sure to take care of this
                         * node in results.html.
                         */
		                if (subfs.length == 1) {
		                    subfs.push({ "name": "", "count": "-1" });
		                }

		                $scope.result.facetFields[f["@name"]] = subfs;
		            });
		        }
                //Our special dateRange facet
		        var dRN = $scope.getDateRangeName();
		        if (dRN != undefined) {
		            $scope.result.facetFields[dRN] =
                        $scope.getDRFacet();
		        }

		        // grab the max number of pages for the result set
		        $scope.maxPage = $scope.getMaxPage($scope.result.rows,
                                                   $scope.result.recordCount);

		        //Array of every facet name for convenience in results.html
		        $scope.result.facetsKeys = [];
		        Object.keys($scope.result["facetFields"]).map(item => {
		            $scope.result.facetsKeys.push(item);
		        });
		    };

            /*--- Helper Functions ---*/

		    /* Function to be given to panel-headings to make the subfacets 
             * collapse.
             */
		    $scope.collapseFacet = function (index) {
		        /* If this stops working, check that the repeat uses ng-hide
                 * and not ng-if. Since ng-if does not actually put the text in
                 * the page, the index will be wrong if it is used.
                 */
		        var content = $('.panel-collapse')[index];
                
		        $(content).slideToggle(500);
		    }

		    /**
			 * The current page number.
			 *
			 * @type {Number}
			 */
		    $scope.currentPage = $state.params.pageNumber;

		    /* An array of objects containing a startdate and enddate,
             * to be used in constructing a url for a searchblox query, as well
             * as a name to be used in the view.
             */
		    $scope.dateRanges = dateRanges;

            /* Converts HTML entities in a given string to their respective symbols
             * and returns that string.
             * If str is not a string, it is simply returned.
             */
		    $scope.decodeHTMLEntities = function (str) {
		        if (!str || typeof str != "string") {
		            return str;
		        }

		        var decoded = str.replace(/&amp;/g, "&");
		        decoded = decoded.replace(/&nbsp;/g, " ");
		        decoded = decoded.replace(/&semi;/g, ";");
		        decoded = decoded.replace(/&colon;/g, ":");
		        decoded = decoded.replace(/&sol;/g, "/");
		        decoded = decoded.replace(/&bsol;/g, "\\");
		        decoded = decoded.replace(/&lt;/g, "<");
		        decoded = decoded.replace(/&gt;/g, ">");
		        decoded = decoded.replace(/&quest;/g, "?");
		        decoded = decoded.replace(/&\S{2,6};/g, "");

                return decoded;
		    }

		    /* Returns a facet with hard-coded fields that allow the user to
             * sort results into date ranges.
             */
		    $scope.getDRFacet = function () {
		        var to_ret = [];

		        for (var i = 0; i < $scope.dateRanges.length; i++) {
		            var sd = $scope.formatDate($scope.dateRanges[i].startdate);
		            var ed = $scope.formatDate($scope.dateRanges[i].enddate);

		            to_ret.push({
		                "name": $scope.dateRanges[i].name,
                        "sbparam": 'startdate=' + sd + '&enddate=' + ed
		            });
		        }
                
		        return to_ret;
		    };

		    /**
			 * The category names that are excluded from the search results.
			 *
			 * @type {Array}
			 */
		    $scope.excludedCategories = [];

		    /**
			 * Execute a brand new search.
			 *
			 * @param {string} searchPhrase
			 */
		    $scope.executeSearch = utilService.goToResults;

		    /**
			 * Execute a search when the user presses enter in the search box.
			 *
			 * @param  {object} e The event object.
			 */
		    $scope.executeSearchOnEnter = function (e) {
		        if (e.keyCode === 13) {
		            e.preventDefault();
		            searchBloxService.pageNumber = 1;
		            $scope.executeSearch($scope.newSearchPhrase);
		        }
		    };

		    /**
			 * Get a subset of a given facet array.
			 *
			 * @param  {array} facet       The original facet.
			 * @param  {array} filterArray The array of names to filter on.
			 * @return {array}             The filtered list of facets.
			 */
		    $scope.filterFacetsByNames = function (facet, filterArray) {
		        var newFacetFields = [];

		        for (var i = 0; i < facet.length; i++) {
		            if (filterArray.indexOf(facet[i].name.toLowerCase()) > -1) {
		                newFacetFields.push(facet[i]);
		            }
		        }

		        return newFacetFields;
		    };

		    /**
			 * Get an excluded subset of a given facet array.
			 *
			 * @param  {array} facet       The original facet.
			 * @param  {array} filterArray The array of names to filter on.
			 * @return {array}             The filtered list of facets.
			 */
		    $scope.filterFacetsWithoutNames = function (facet, filterArray) {
		        var newFacetFields = [];
		        var lowerCaseExcludedCategories = $scope.excludedCategories.join('|').toLowerCase().split('|');

		        for (var i = 0; i < facet.length; i++) {
		            if (filterArray.indexOf(facet[i].name.toLowerCase()) === -1 && lowerCaseExcludedCategories.indexOf(facet[i].name.toLowerCase()) === -1) {
		                newFacetFields.push(facet[i]);
		            }
		        }

		        return newFacetFields;
		    };

		    /**
			 * The filter parameters.
			 *
			 * @type {Object}
			 */
		    $scope.filterParams = $state.params.filterParams ? JSON.parse($state.params.filterParams) : {};

		    /**
			 * The facet names for Focus Areas (a special type of category).
			 *
			 * @type {Array}
			 */
		    $scope.focusAreas = [
				'advocacy',
				'affordable care act',
				'catholic identity',
				'community benefit',
				'diversity disparities',
				'eldercare',
				'environment',
				'ethics',
				'human trafficking',
				'immigration',
				'international outreach',
				'leadership formation',
				'management operations',
				'medical care',
				'mission',
				'pastoral care',
				'sponsorship',
				'nursing',
				'diversity & disparities',
				'physician engagement',
				'prayers'
		    ];

		    /* Returns a string that represents the given date, d, in the
             * format: YYYYMMDD+000000. Returns the empty string if d was not
             * a date.
             */
		    $scope.formatDate = function (d) {
		        if (typeof d.getMonth !== 'function') {
		            return "";
		        }

		        /* The checkboxes will not be checked when the page refreshes
                 * if the value isn't the same, so to reduce the odds of that
                 * happening, we're going to make hour, minute, and second be
                 * 00 every time. Now the only issue is that they will become
                 * unchecked if someone is searching when the day rolls over.
                 * TODO: Fix this issue.
                 */
		        var second = "00";
		        var minute = "00";
		        var hour = "00";
		        var day = ("00" + d.getDate()).slice(-2);
		        var month = ("00" + d.getMonth()).slice(-2);
		        var year = "" + d.getFullYear();
		        var formatted = year + month + day + hour + minute + second;

		        return formatted;
		    }

		    /* Returns the date range name provided by the widget. If one was
             * not provided, returns undefined.
             */
		    $scope.getDateRangeName = function () {
		        var name = window.IAFCSearch.dateRangeName;
		        if (!name || name == "N/A") {
		            return undefined;
		        }
		        return name;
		    }

		    /**
			 * Extract a filename from the full path.
			 *
			 * @param  {string} url The URL of the file.
			 * @return {string}     The filename.
			 */
		    $scope.getFilenameFromUrl = function (url) {
		        if (!url || typeof url != "string") {
		            return url;
		        }

		        var replaced = url;
		        if (replaced.substr(-1) === '/') {
		            replaced = replaced.substring(0, replaced.length - 1);
		        }
		        replaced = replaced ? replaced.replace(/^.*[\\\/]/, '') : "";

		        return replaced;
		    };

		    /**
			 * Get the query logic for the filter parameters.
			 *
			 * @param  {boolean} asString Return the results as a string.
			 * @return {string|array}
			 */
		    $scope.getFilterParamsQuery = function (asString) {
		        var keys = _.keys($scope.filterParams);
		        var queryArray = [];

		        _.each(keys, function (key) {
		            for (var i = 0; i < $scope.filterParams[key].length; i++) {
		                queryArray.push(key + ":" + $scope.filterParams[key][i]);
		            }
		        });

		        if (asString) {
		            return queryArray.join(" AND ");
		        }

		        return queryArray;
		    };

		    /**
			 * Convert the filter parameters object to a serialized string.
			 *
			 * @return {string}
			 */
		    $scope.getFilterParamsString = function () {
		        return JSON.stringify($scope.filterParams);
		    };

		    /**
			 * Get the maximum number of pages for the search results.
			 *
			 * @param  {number} rows
			 * @param  {number} maxResults
			 * @return {number}
			 */
		    $scope.getMaxPage = function (rows, maxResults) {
		        var maxPage = maxResults / rows;
		        return Math.ceil(maxPage);
		    };

		    /**
			 * Get the list of page numbers that are appropriate for pagination.
			 *
			 * @param max
			 * @param current
			 * @returns {Array}
			 */
		    $scope.getPageNumbers = function (max, current) {
		        var pages = [];
		        var start = 1;
		        var stop = (max < 10) ? max : 10;

		        // if the current page is greater than 6, the current page
		        // should be in the "middle" of the pagination list
		        if (current > 6 && max > 9) {
		            start = current - 5;
		            stop = current + 5;
		        }

		        // if the current page is within 5 of the last page, the pagination list
		        // should always range between 10-minus the last page and the last page
		        if ((current + 5) > max) {
		            if (max > 10) {
		                start = max - 10;
		            }
		            stop = max;
		        }

		        for (start; start <= stop; start++) {
		            pages.push(start);
		        }

		        return pages;
		    };

		    /**
			 * Get an appropriate title based on whether the record has one.
			 *
			 * @param  {object} record The single search result record.
			 * @return {string} 	   The title.
			 */
		    $scope.getRecordTitle = function (record) {
		        if(!record && typeof record != "object"){
		            return record;
		        }

		        if (!record.title && !record["@title"]) {
		            return $scope.getFilenameFromUrl(
                              record.url || record["@url"]);
		        }
		        return record.title || record["@title"];
		    };

		    /**
			 * Get the starting record for the current search query.
			 *
			 * @param  {number} page
			 * @param  {number} rows
			 * @return {number}
			 */
		    $scope.getStartRecord = function (page, rows) {
		        var start = 0;

		        if (page > 1) {
		            start = page - 1;
		        }

		        return start * rows;
		    };

		    /**
			 * Navigate to next results page.
			 */
		    $scope.goToNextPage = function () {
		        var newPage = $scope.currentPage + 1;

		        if (newPage <= $scope.result.recordCount) {
		            $scope.goToPage(newPage);
		        }
		    };

		    /**
			 * Navigate to a specific results page.
			 *
			 * @param  {number} pageNumber
			 * @param  {object} filterParams
			 */
		    $scope.goToPage = function (pageNumber, filterParams) {
		        searchBloxService.setPageNumber(pageNumber);

		        if (filterParams) {
		            $state.go('app.results', {
		                searchPhrase: $scope.searchPhrase,
		                pageNumber: pageNumber,
		                filterParams: filterParams,
		                sortBy: $scope.sortBy
		            });
		        } else {
		            $state.go('app.results', {
		                searchPhrase: $scope.searchPhrase,
		                pageNumber: pageNumber,
		                sortBy: $scope.sortBy
		            });
		        }
		    };

		    /**
			 * Navigate to previous results page.
			 */
		    $scope.goToPrevPage = function () {
		        var newPage = $scope.currentPage - 1;

		        if (newPage > 0) {
		            $scope.goToPage(newPage);
		        }
		    };

		    /**
			 * Determine if a given facets category has an item checked.
			 *
			 * @param  {array} facets       The array of facets.
			 * @param  {array} filterParams The array of filters.
			 * @return {boolean}
			 */
		    $scope.hasFacetsChecked = function (facets, filterParams) {
		        if (!facets || !filterParams) return false;

		        for (var i = 0; i < filterParams.length; i++) {
		            if (_.findWhere(facets, {
		                name: filterParams[i]
		            })) {
		                return true;
		            }
		        }

		        return false;
		    };

		    /**
			 * Determine if the search query has filters applied.
			 *
			 * @return {boolean}
			 */
		    $scope.hasFiltersApplied = function () {
		        var keys = _.keys($scope.filterParams);
		        var queryArray = [];

		        _.each(keys, function (key) {
		            for (var i = 0; i < $scope.filterParams[key].length; i++) {
		                queryArray.push(key + "%3A" + $scope.filterParams[key][i]);
		            }
		        });

		        if (!queryArray.length) {
		            return false;
		        }

		        return true;
		    };

		    /* Takes in two strings. text is the string that contains words to
             * be highlighted. highlight_str is the string that contains the
             * words to highlight. If either text or highlight_str is not a 
             * string, will return whatever text is. Otherwise, returns text
             * with the highlighted parts wrapped in <mark></mark> tags.
             */
		    $scope.highlightText = function (text, highlight_str) {
		        if (typeof text != "string"
                    || typeof highlight_str != "string") {
		            console.log(text+" "+highlight_str);
		            return text;
		        }

		        /* Doesn't work without the wrapper. Not sure why. It throws an
                 * error that says sce was given an unsafe value, but it wasn't
                 * the result of the trustAsHTML used in this method, so idk.
                 */
		        function wrapper() {
		            var to_return = sanitize(text);
		            var terms = highlight_str.split(" ");

		            //We want to highlight each term individually, not together
		            for (var i = 0; i < terms.length; i++) {
		                var term = terms[i];
		                var reg;

		                /* This try-catch exists as a quick way to check if
                         * what the user entered is already a valid regex
                         */
		                try {
		                    //TODO: Figure out if this opens up injection
		                    //vulnerabilities
		                    var temp = new RegExp(term, "gi");
		                }
		                catch (e) {
		                    //Deal with any special regex characters
		                    var regtest = /[\*\\\.\?\$\+\(\)\[\]\{\}]/g;
		                    if (regtest.test(term)) {
		                        term = term.replace(/[^a-zA-Z0-9\*]/gi,
                                    function (s) { return "\\" + s });
		                        term = term.replace(/\*/g, "");
		                    }
		                }

		                //Need to be careful about "mark" as a search term
                        if (term.toLowerCase().indexOf("mark") != -1) {
                            term = term.replace(/mark/gi, "mark(?!>)");
                        }

                        reg = new RegExp(term, "gi");

                        //case where nothing to match
                        if (reg.toString() == "/(?:)/gi") {
                            return text;
                        }

                        //otherwise, mark
                        to_return = to_return.replace(reg, function (s) {
                            return "<mark>" + s + "</mark>";
                        });

		                return to_return;
		            }
		        }
                
		        var to_return = wrapper();
		        to_return = !to_return || to_return == "" ? text : to_return;

		        return $sce.trustAsHtml(to_return);
		    }

		    /**
			 * Initialize the controller.
			 */
		    $scope.init = function () {

		        if ($scope.searchPhrase) {
		            searchBloxService.setPageNumber($scope.currentPage);

		            $scope.categoryLimiter = IAFCSearch.categoryLimiter.length ? IAFCSearch.categoryLimiter : null;

		            $scope.metatagLimiter = IAFCSearch.metatagLimiter.length ? IAFCSearch.metatagLimiter : 'metatag.categories';

		            $scope.excludedCategories = IAFCSearch.excludedCategories.length ? splitValues(IAFCSearch.excludedCategories) : [];

		            $scope.newSearchPhrase = $scope.searchPhrase;

		            //$scope.result.start = $scope.getStartRecord($scope.currentPage, $scope.result.rows);

		            $scope.watchFilters();

		            $scope.getResults();
		        }

		    };

		    /**
			 * The maximum results page number.
			 *
			 * @type {Number}
			 */
		    $scope.maxPage = 1;

		    /* Returns the max number of records allowed to be displayed on a 
             * page.
             */
		    $scope.recordsPerPage = function () {
		        return 10;
		    }

		    /**
			 * The result object.
			 *
			 * @type {Object}
			 */
		    $scope.result = {
		        recordCount: 0,
		        start: 0,
		        rows: 10,
		        records: [],
		        highlighting: {},
		        facetFields: {}
		    };

		    /**
			 * The search phrase.
			 *
			 * @type {string}
			 */
		    $scope.searchPhrase = $state.params.searchPhrase;

		    /**
			 * Change the property in which to sort the results by.
			 *
			 * @param  {string} sortBy The property name.
			 */
		    $scope.setSortBy = function (sortBy) {
		        searchBloxService.setSortBy(sortBy);
		        $scope.sortBy = sortBy;
		        $scope.goToPage($scope.currentPage, $scope.getFilterParamsString());
		    };

		    $scope.showSearchBox = function () {
		        var ssb = window.IAFCSearch.showSearchBox.toLowerCase();
		        return ssb === "true";
		    }

		    /* This function is meant to be used in results.html. It should be
             * used for view portions, not logic portions. It will return the
             * version of the facet or subfacet's name that looks best to the
             * user.
             *
             * If you're looking for the special version of a facet name and
             * not the special version of a subfacet name, simply do not pass
             * the subfacet parameter.
             */
		    $scope.specialName = function (facet, subfacet) {
		        //First check the non-hard-coded ones. (Facets only)
		        if (subfacet === undefined) {
		            var friends = searchBloxService.getFriendlyFacets();
		            var len = friends.length;
		            for (var i = 0; i < len; i++) {
		                if (friends[i]["facet"] == facet) {
		                    return convertQuotes(friends[i]["alias"]);
		                }
		            }
		        }

		        //These are all hard-coded, but done in such a way as to be 
		        //centralized and easily altered.
		        switch (facet.toLowerCase()) {
		            // If you're curious about the undefined's, they're
		            // there as the case where you didn't pass in a subfacet.
		            case "webcategory":
		                switch (subfacet.toLowerCase()) {
		                    case undefined:
		                        return "Category";
		                    default:
		                }
		                break;
		            case "colname":
		                switch (subfacet) {
		                    case undefined:
		                        return "Collection";
		                    case "cnn":
                            case "mcaa":
		                        return subfacet.toUpperCase();
		                    default:
		                        return subfacet.replace(/-/g, " ");
		                }
		                break;
		            case "contenttype":
		                switch (subfacet) {
		                    case undefined:
		                        return "Document Type";
		                    case "excel":
		                        return "Excel";
		                    case "news":
		                        return "News";
		                    case "ppt":
		                        return "PowerPoint";
		                    case "word":
		                        return "MS Word";
                            case "gif":
                            case "html":
                            case "jpg":
                            case "pdf":
                            case "png":
                                return subfacet.toUpperCase();
		                    default:
		                }
		                break;
		            default:
		        }

		        //If no special case, strip out underscores, decode HTML 
                //entities and return
		        return $scope.decodeHTMLEntities( 
                      convertQuotes(
                        underscoresToSpaces(subfacet === undefined
                        ? facet : subfacet)
                      )
                    );
		    }

		    /**
			 * The property to sort the results by.
			 *
			 * @type {String}
			 */
		    $scope.sortBy = $state.params.sortBy ? $state.params.sortBy : 'relevance';

			/**
			 * Get the public template path.
			 *
			 * @return {string}
			 */
			$scope.templatePath = utilService.getTemplatePath();

		    /* Converts The Passed String To TitleCase, Placed Here For
             * Convenience In Results.html
             *
             * Was used for facets, but is now defunct.
             *
             * From http://stackoverflow.com/a/196991
             */
			$scope.toTitleCase = function (str) {
			    return str.replace(/\w\S*/g, function (txt) {
			        return txt.charAt(0).toUpperCase()
                           + txt.substr(1).toLowerCase();
			    });
			}

		    /**
			 * Transform facet results into objects with counts.
			 *
			 * @param  {array} facets
			 * @return {array}
			 */
			$scope.transformFacetsArray = function (facets) {
			    var facetArray = [];

			    // NOTE: The facets are delivered in an odd configuration.
			    // It's a flat array of a facet name followed by the count for that facet
			    // (e.g. ['facet-1', 10, 'facet-2', 5, ...]).
			    // This mapping will structure the facets as an array of objects containing
			    // the facet name and the count.
			    for (var i = 0; i < facets.length; i = i + 2) {
			        facetArray.push({
			            "name": facets[i],
			            "count": facets[i + 1]
			        });
			    }

			    return facetArray;
			};

			/**
			 * Add watchers for the facet filters.
			 */
			$scope.watchFilters = function() {
				$scope.$watch(
					function(scope) {
						return scope.filterParams;
					},
					function(newVal, oldVal) {
						if (!_.isEqual(newVal, oldVal)) {
							$scope.goToPage(1, $scope.getFilterParamsString());
						}
					},
					true
				);
			};

			/**
			 * Execute the initalization function.
			 */
			$scope.init();

		    /*
             * Private
             */

		    /* If the user entered a quotation mark in the form, it will show
             * up as &amp;quot;. This function will strip any of those out of
             * the given string.
             */
			function convertQuotes(str) {
			    if (typeof str === "string") {
			        return str.replace(/&amp;quot;/g, "\"");
			    }
			    return str;
			}

		    /* Returns a record with useless data.
             */
			function getDummyRecord() {
			    return {
			        "@no": "-1",
			        "@id": "",
			        "alpha": "",
			        "Author": "",
			        "author": "",
			        "bitly-verification": "",
			        "col": 0,
			        "colname": "http://www.iacp.org",
			        "contenttype": "HTML",
			        "context": { "text": "", "highlight": "" },
			        "description": "This is a dummy record.",
			        "filename": [],
			        "format-detection": "telephone=no",
			        "indexdate": Date.now,
			        "keywords": [],
			        "language": "en",
			        "lastmodified": Date.now,
			        "msvalidate.01": "",
			        "news_keywords": "Fire",
			        "og-description": "This is a dummy record.",
			        "og-image": "",
			        "og-sitename": "",
			        "og-type": "article",
			        "og-title": "",
			        "og-url": "",
			        "sb_boost": "1",
			        "score": 0,
			        "serverf": "web2",
			        "size": 0,
			        "title": "",
			        "twitter-card": "",
			        "twitter-description": "",
			        "twitter-image": "",
			        "twitter-site": "",
			        "twitter-title": "",
			        "uid": "-1",
			        "url": "",
			        "viewport": "width=0"
			    }
			}

		    /* Takes in a string and converts all potentially harmful
             * characters. NOTE: Not 100% safe from XSS attacks, but a good
             * first line of defense. TODO: Make this more secure.
             */
			function sanitize(str) {
                var entityMap = {
                    "&": "&amp;",
                    "<": "&lt;",
                    ">": "&gt;",
                    '"': '&quot;',
                    "'": '&#39;',
                    "/": '&#x2F;'
                };
                
                return String(str).replace(/[&<>"'\/]/g, function (s) {
                    return entityMap[s];
                });
            }

			function splitValues(valueString) {
			    var values = valueString.trim();
			    return values.split(',');
			}

		    /* STARTS and resets a loop that truncates descriptions in the 
             * search results. This is necessary because we want to allow for
             * multiple lines before truncation.
             *
             * http://stackoverflow.com/a/1542294
             */
			function startLoop() {
			    var delay = 100; // expressed in miliseconds
			    var iterations = 0;
			    var myInterval = 0;
			    var prevWidth = 0;
			    var dotdotdot = $(".dot-dot-dot");

			    function updateDescriptions() {

			        //This check is done so that selected text does not get 
                    //deselected while the descriptions remains the same.
			        dotdotdot = $(".dot-dot-dot");
			        var curWidth = dotdotdot[0] != undefined ?
                        dotdotdot[0].offsetWidth : 0;

			        if (curWidth != prevWidth) {
			            dotdotdot.dotdotdot({ ellipsis: "..." });
			            prevWidth = curWidth;
			        }

			        var restart = false;
                    if (++iterations == 200) {
                        delay = 1000;
                        clearInterval(myInterval);
                        restart = true;
                    } else if (iterations == 1000) {
                        delay == 5000;
                        clearInterval(myInterval);
                        restart = true;
                    } else if (iterations > 5000) {
                        window.clearInterval(myInterval);
                        clearInterval(myInterval);
                        searchBloxService.descriptionUpdatesOn = false;
                    }
                    
                    if (restart) {
                        myInterval = window.setInterval(function () {
                            updateDescriptions()
                        }, delay);
                    }
			    }

                //Done so that using facets doesn't start multiple loops
			    if (searchBloxService.descriptionUpdatesOn == false) {
			        searchBloxService.descriptionUpdatesOn = true;
			        myInterval = window.setInterval(function () {
			            updateDescriptions();
			        }, delay);
			    } else {
			        for (var i = 0; i < 1000; i++) {
			            updateDescriptions();
			        }
			    }
			}

		    /* Converts all underscores in a string to spaces.
             */
			function underscoresToSpaces(str) {
			    return str.replace(/_/g, " ");
			}
        }
	]);

	app.filter('subfacetSorter', function () {
	    function getVal(sub) {
	        if (sub == undefined) {
	            return 0;
	        }
	        for (var i = 0; i < dateRanges.length; i++) {
	            if (sub.name == dateRanges[i].name) {
	                return "" + i;
	            }
	        }

	        return sub.name;
	    }

	    return function (items, field) {
	        var filtered = [];

	        items.map(function (item) {
	            filtered.push(item);
	        });
	        filtered.sort(function (a, b) {
	            return (getVal(a) > getVal(b) ? 1 : -1);
	        });
	        return filtered;
	    }
	});

};
