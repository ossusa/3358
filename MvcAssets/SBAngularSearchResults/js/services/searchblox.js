/**
 * Seachblox Service
 */

var app = require('../app');
var _ = require('underscore');
var moment = require('moment');
var $ = require('jquery');

module.exports = function() {

    /*========================================================================\
     |               HOW TO MAKE CHANGES TO THIS SEARCH FEATURE               |
     |=====================================================================*//*
     |  Table of Contents:                                                    |
     |------------------------------------------------------------------------|
     | 1. Things You Need To Know                                             |
     | 2. Changing The Way Results Look                                       |
     | 3. Changing How The Results Work                                       |
     | 4. Changing How The URL is Formatted                                   |
     | 5. How To Pass Information Between Services                            |
     | 6. How To Get Information From The Widget                              |
     | 7. These instructions are annoying and in the way.                     |
     |                                                                        |
     |------------------------------------------------------------------------|
     | Things You Need To Know:                                               |
     |------------------------------------------------------------------------|
     | 1. What is a service?                                                  |
     |   A service is what some of the files return. It's just an object that |
     |   contains functions and variables. In order to interact with each     |
     |   other, services can be passed into other services.                   |
     | 2. What is ASP.NET?                                                    |
     |   ASP.NET is used to make dynamic webpages, meaning that the content   |
     |   of a webpage can change depending on a variety of factors, like      |
     |   whether or not the user is logged in. It provides you with a way to  |
     |   kind of "program" web pages.                                         |
     |                                                                        |
     |------------------------------------------------------------------------|
     | Changing The Way Results Look:                                         |
     |------------------------------------------------------------------------|
     | The results partial is located in [public>partials>results.html]. It   |
     | uses ASP.NET to allow you to use programming to generate HTML. You'll  |
     | see what's called "directives" used all over the place. They typically |
     | look like "ng-[something]", but there's also one that's already in use |
     | called "checklist-model", but we'll discuss that in the next section.  |
     |   ng-repeat will allow you to iterate through arrays and do the same   |
     | thing with each element. ng-if will only write its contents to the     |
     | HTML if the code inside it returns true. ng-show will write its        |
     | contents to the file each time, but will hide them if the code inside  |
     | returns true. ng-hide will do the same, but only if the code inside    |
     | returns false. ng-bind will put its results between the inner and      |
     | outer tag. Google these for more information.                          |
     |   Sometimes you'll find yourself needing to just insert code directly  |
     | into places. In this case, just put that code between a pair of        |
     | brackets (like this: {{}} ). Don't do this for anything that can be    |
     | seen directly on-page because until the code resolves, the user will   |
     | see the code you've written, and that's bad style.                     |
     |   We also have something called "Best Bets". They're search results    |
     | that always appear for specified search terms. They come back in a     |
     | field called "ads". They're basically identical to regular results, so |
     | they use the same ng-repeat with a small bit of logic added to give    |
     | them a class called "best-bets". Searchblox returns them with all the  |
     | same fields that regular records have, but with an '@' appended to the |
     | front of each.                                                         |
     |                                                                        |
     |------------------------------------------------------------------------|
     | Changing How The Results Work:                                         |
     |------------------------------------------------------------------------|
     | The results.html logic exists inside the scope of the $scope variable. |
     | This means that you can access any of the methods or variables located |
     | inside of $scope without first having to write "$scope.".              |
     |   So, if you want to perform some complicated logic on something, make |
     | a function that does that and just call it within the file. Likewise,  |
     | if you want to access a variable that exists outside of $scope, you    |
     | can just drop it inside $scope. I've done this in searchBlox.js, for   |
     | example.                                                               |
     |   If you want to change how the facets work, you'll need to look in    |
     | [js>directives>checklist-model.js]. When checkboxes get checked, the   |
     | method "add" will be called. The parameter "arr" is an array           |
     | consisting of the values of every checked box. "item" is the value     |
     | that is about to be added. "comparator" is a function that can be used |
     | to compare elements in arr to item. Likewise, "remove" gets called     |
     | when a checkbox is unchecked.                                          |
     |   Additionally, the subfacets from the results are stored in           |
     | $scope.result.facetFields. The names of facets are stored in           |
     | $scope.result.facetKeys. This is because they get stripped out by the  |
     | ng-repeat that processes the facets. We have also decided to allow a   |
     | syntax for aliasing facet names. These aliases are stored in           |
     | searchBloxService.friendlyFacets. To make displaying both facets and   |
     | subfacets for the user easier, I have written a function called        |
     | specialName(str) inside $scope. This should not be used anywhere that  |
     | requires the original values, since it might not return that original  |
     | value.                                                                 |
     |   We have also hard-coded some fake facets into the page called        |
     | "Date Range". They don't actually use the facets feature in SearchBlox |
     | and instead use the startdate and enddate parameters. To change these  |
     | ranges, edit the variable "dateRanges" in $scope. This facet is also   |
     | only displayed if the user in the widget provided us with a name for   |
     | it. This functionality should be easy to find and change in the        |
     | results.html file.                                                     |
     |   The results' descriptions can sometimes be too long. To combat this, |
     | we are using a module called 'jquery.dotdotdot'. We could just use the |
     | same method that we use to shorten urls with css styling, but the use  |
     | of <mark> tags in the descriptions means that complications might      |
     | occur. So we use the extension instead. It would work better if we     |
     | didn't put the results on the page with ajax. To fix that problem, we  |
     | start a loop in javascript that occasionally uses the modele to update |
     | the descriptions.                                                      |
     |                                                                        |
     |------------------------------------------------------------------------|
     | Changing How The URL is Formatted:                                     |
     |------------------------------------------------------------------------|
     | There are two URLS that this section could be referring to. The first  |
     | is the URL that queries SearchBlox. The second is the URL that gets    |
     | put up on the URL bar on the page itself.                              |
     |   SearchBlox Query URL:                                                |
     |     Check out the method in this file called "getURL()". The results   |
     |     of this method get put directly into the AJAX call. It also calls  |
     |    "getFacets()", which also populates the "friendlyFacets" variable.  |
     |   Address Bar URL:                                                     |
     |     Check out the file [js>config>routes.js]. The $stateProvider.state |
     |     functions define how this is structured, in two cases, the first   |
     |     being when first navagated to, and the second being when a search  |
     |     has already been performed.                                        |
     |       You may also want to have the search box format this URL         |
     |     correctly. If so, you'll want to check out the file [App_Data>     |
     |     Sitefinity>WebsiteTemplates>MatrixBase>App_Themes>MatrixBase>js>   |
     |     search.js]. Look for a line that contains "location.href".         |
     |                                                                        |
     |------------------------------------------------------------------------|
     | How To Pass Information Between Services:                              |
     |------------------------------------------------------------------------|
     | You can set up functions that get and set a variable that you provide  |
     | in your services. That way, whenever another service wants to read or  |
     | write a variable, they can do it if they've been passed the service.   |
     |                                                                        |
     |------------------------------------------------------------------------|
     | How To Get Information From The Widget:                                |
     |------------------------------------------------------------------------|
     | This one is simple. Widget variables are stored in window.IAFCSearch.  |
     |                                                                        |
     |------------------------------------------------------------------------|
     | These instructions are annoying and in the way.                        |
     |------------------------------------------------------------------------|
     | Sorry about that! Just trying to be thorough. Please don't delete this |
     | altogether from the file. Just collapse it on the side :)              |
     `-----------------------------------------------------------------------*/

    /* If you want to have information from results.html affect what this file
     * does, you can add a field to a tag like ng-click="myFunction(param)" and
     * put myFunction in results.js as a method named myFunction. myFunction
     * can then call something from this file that does what you want.
     * i.e. $scope.setSortBy calls searchBloxService.setSortBy().
     */

	app.service('searchBloxService', [
		'$http', '$q', 'utilService',
		function($http, $q, utilService) {

		    var service = {
                descriptionUpdatesOn: false,
		        getFilterParamsQuery: getFilterParamsQuery,
				getResults: getResults,
				getSearchPhrase: getSearchPhrase,
				getURL: getURL,
				pageNumber: 1,
				setPageNumber: setPageNumber,
				sortBy: "relevance",
				setSortBy: setSortBy,
				friendlyFacets: [],
                getFriendlyFacets: getFriendlyFacets
			};

			return service;

		    ////////////

			/**
			 * Get the search results from SearchBlox.
			 *
			 * @param  {object} searchParams An object of all the search parameters (including filtering).
			 * @return {promise}
			 */
			function getResults(searchParams) {
				var deferred = $q.defer();
				var serviceUrl = service.getURL(searchParams);
				console.log("URL:" + serviceUrl);

				// NOTE: we're using the jQuery AJAX methods because they seem to be more
				// forgiving when it comes to the JSONP format
				$.ajaxSettings.traditional = true;
				$.ajax({
					url: serviceUrl,
                    //data: queryParams,
					timeout: 30000,
					success: function (data) {
					    //continued in controllers/results.js, in the function
                        //getResults(), and then handleSearchResponse()
						deferred.resolve(data);
					},
					error: function(data, status, error) {
						deferred.reject(error);
					},
					dataType: 'jsonp'
				});

				return deferred.promise;
			}

		    /* Get the URL that we will use to make the request to SearchBlox.
             * URL will follow this format:
             *   <hostname>/searchblox/servlet/SearchServlet?query=<params>
             *
             */
			function getURL(searchParams) {
			    var hostname = utilService.getApiPath();
			    hostname = hostname.trim();
			    //If we have a / at the end of the URL, remove it
			    while (hostname.slice(-1) === "/") {
			        hostname = hostname.slice(0, -1);
			    }

			    var serviceURL = hostname 
                    + '/searchblox/servlet/SearchServlet?query=' +
					service.getSearchPhrase(searchParams.q.replace(/&/g,"%26"),
                                            searchParams.filterParams,
                                            service.pageNumber,
                                            service.sortBy);
                
			    //Add in all the collections to be used with a simple map
			    getCollections().map(col => {
                    if(!isNaN(parseInt(col))){
                        serviceURL += "&col=" + col
                    }
			    });

			    //Add in all the facets
			    getFacets().map(facet => {
			        serviceURL +=
                        "&facet.field=" + facet
                        + "&f." + facet + ".size=1000";
			    });

			    //And now all the defaults
			    serviceURL += "&xsl=json" + "&facet=on";

			    //Convert illegal spaces to url-friendly %20s
			    serviceURL = serviceURL.replace(/\s+/g, "%20");

			    return serviceURL;
			}

			/**
			 * Generate and encode the search phrase that will be used in the 
             * URL parameter.
			 *
			 * @param  {string}  q            The search phrase.
			 * @param  {object}  filterParams Object of filters.
             * @param  {integer} pageNumber   A positive integer.
             * @param  {string}  sortBy       Dictates how results are sorted.
             *                                Acceptable values are:
             *                                "relevance" "new" "old" "alpha".
			 * @return {string}               The concatenated, encoded string.
			 */
			function getSearchPhrase(q, filterParams, pageNumber, sortBy) {
			    var paramsArray = [];
                
                //Handle user query
				if (q === '*' || q === '') {
					paramsArray.push('*%3A*');
				} else {
					paramsArray.push(q);
				}

                //Handle page
				paramsArray.push("&page=" + (pageNumber ? pageNumber : 1));

			    //Handle sort
				switch (sortBy) {
				    case "new":
				        paramsArray.push("&sort=date&sortdir=desc");
                        break;
				    case "old":
				        paramsArray.push("&sort=date&sortdir=asc");
				        break;
				    case "alpha":
				        paramsArray.push("&sort=alpha&sortdir=asc");
				        break;
				    default:
				        paramsArray.push("&sort=relevance");
				}

				var filterParamsString =
                    service.getFilterParamsQuery(filterParams, true);
				if (filterParamsString) {
					paramsArray.push(filterParamsString);
				}

				return paramsArray.join("");
			}

		    /* Returns an array of all the collections to use in this search.
             */
			function getCollections() {
			    var colsRAW = window.IAFCSearch.collectionId;
                //If something went wrong, just return an empty array
			    if (!colsRAW || colsRAW == "N/A") {
			        return [];
			    }

                //Otherwise return an array with each number, trimmed.
			    return colsRAW.split(",").map(elem => { return elem.trim();});
			}

		    /* Returns an array of facet names to use for this search.
             */
			function getFacets() {
                var availableFacets = [];
                var facetsRAW = window.IAFCSearch.facetNames;

			    //facetsRAW will be "N/A" when user left empty, so check that
                if (facetsRAW == "N/A") {
                    return [];
                }

			    /* facetsRAW is a list of facets separated by commas and/or
                 * newlines. We need to figure out where facets are delineated
                 * dynamically, so as we parse the string, we look for the next
                 * newline and next comma and see which is closer, then use
                 * that as the separator.
                 */
                while (facetsRAW && typeof facetsRAW == 'string' && facetsRAW != "") {
			        var iONextNL = facetsRAW.indexOf("\n");
			        var iONextComma = facetsRAW.indexOf(",");
			        var nextFacet = "";

                    //Base Case: Only one facet left
			        if (iONextNL == -1 && iONextComma == -1) {
			            nextFacet = facetsRAW;
			            facetsRAW = "";
                    }
                        //Check if comma is closer
			        else if (iONextNL == -1
                        || (iONextComma < iONextNL && iONextComma > -1))
			        {
			            nextFacet = facetsRAW.substring(0, iONextComma);
			            facetsRAW = facetsRAW.substring(iONextComma+1);
			        }
                        //Only possibility now is that newline is closer
			        else {
			            nextFacet = facetsRAW.substring(0, iONextNL);
			            facetsRAW = facetsRAW.substring(iONextNL+1);
			        }

			        nextFacet = nextFacet.trim();
                    if(nextFacet != "")
                        availableFacets.push(nextFacet);
                }

			    //As part of our syntax, we allow an =, the rvalue of which
			    //denotes the "friendly" version of the facet's name.
                
                availableFacets = availableFacets.map(fac => {
                    //If there was no assignment, make no change.
                    if (fac.indexOf("=") == -1) {
                        return fac;
                    }

                    //pieces = [lvalue, rvalue]
                    var pieces = fac.split("=");
                    var friendly = { "facet": pieces[0].trim(), "alias": pieces[1].trim() };
                    service.friendlyFacets.push(friendly);

                    //Just keep the lvalue in our array
                    return pieces[0].trim();
                })

                return availableFacets;
			}

		    /**
			 * Generate the encoded parameter string for the filters.
			 *
			 * @param  {object}  filterParams The filter parameters object
			 * @param  {boolean} asString     Determine if the returned value should be a string.
			 * @return {array|string}
			 */
			function getFilterParamsQuery(filterParams, asString) {
				var keys = _.keys(filterParams);
				var queryArray = [];

			    //For each subfacet, add it to the search url in the form of:
                //&f.MYFACET.filter=SUBFACET
				_.each(keys, function(key) {
					for (var i = 0; i < filterParams[key].length; i++) {
					    queryArray.push("&" + filterParams[key][i]);
					}
				});

				if (!queryArray.length) {
					return false;
				}

				if (asString) {
					return queryArray.join("");
				}

				return queryArray;
			}

		    /* Sets the page number to num.
             * 
             * @param {integer}  num  A positive integer.
             */
			function setPageNumber(num) {
			    if (Number.isInteger(num) && num > 0) {
                    service.pageNumber = num;
                }
			}

		    /* Does what it says on the tin.
             */
			function setSortBy(sort) {
			    service.sortBy = sort;
			}

			function getFriendlyFacets() {
			    return service.friendlyFacets;
			}
		}
	]);
};
