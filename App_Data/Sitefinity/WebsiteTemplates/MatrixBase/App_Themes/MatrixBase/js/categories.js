module.exports = function ($) {

	$(function(){

		// loop through categories list
        // if 'Protected content' exists - hide it
        $('.sfCategoriesList > li').each(function(i){
        	var category = $(this).text();
        	if( $.trim(category) == 'Protected content' ){
                $(this).addClass('hidden');
                var $articleDetail = $(this).closest('.article-detail'),
                	$resourceButton = $articleDetail.find('.btn-resource-access');
                if( $articleDetail ){
                	$resourceButton
                		.prepend('<span class="icon icon-key" style="margin-right: 5px;"></span>');
                }
            }
        });

        //generic hide Protected
        //looks inside any element with class 'searchProtected' for "Protected content" and adds a key in front of class 'title'
            $(".searchProtected:contains('Protected content')").find('.title a').prepend('<span class="icon icon-key" style="margin-right: 5px;"></span>');

	}); // DOM ready

	// Turns Topics into links for search
    $(window).ready(function () {
        var children = $("#searchTerm > div > .sfCategoriesList").children();
        $(children).each(function(){
            var searchterm = $(this).text();
            var hyperlink = "/search-results/#/" + searchterm.trim() + "/page=1";
            $(this).children().wrap("<a href=" + "'" +  hyperlink + "'"  +"></a>");
        });
    });
};