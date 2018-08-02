var $ = require('jquery');

module.exports = function () {
    $(function () {

		var $li = '<li class="form-container mg-site-search"><button class="icon icon-search search-trigger" type="submit"></button><div class="mg-search-box hidden padded-less banded"><div class="relative"><label for="site-search" class="visuallyhidden">Search</label><input type="text" class="search-box block" placeholder="search" id="site-search"><button id="site-search-submit" class="icon icon-search mg-site-search__submit" type="submit"></button></div></div></li>';

        $('.main-nav .top-level > ul').append( $li );

		var $li2 = '<li class="form-container mg-site-search second-search"><button class="icon icon-search search-trigger" type="submit"></button><div class="mg-search-box hidden padded-less banded"><div class="relative"><label for="site-search" class="visuallyhidden">Search</label><input type="text" class="search-box block" placeholder="search" id="site-search2"><button id="site-search-submit-2" class="icon icon-search mg-site-search__submit" type="submit"></button></div></div></li>';

        $( $li2 ).insertAfter( '.main-nav' );

    	$(document).on('click', '.search-trigger', event => {
    		event.preventDefault();
    		$('.mg-search-box').toggleClass('hidden');
    	});

        $(document).on('click', '#site-search-submit', event => {
        	event.preventDefault();
        	var q = $('#site-search').val();

        	location.href = '/search-results/#/' + q + "/page=1";
        });

		$(document).on('click', '#site-search-submit-2', event => {
        	event.preventDefault();
        	var q = $('#site-search2').val();

        	location.href = '/search-results/#/' + q + "/page=1";
        });

    }); //DOM ready
};
