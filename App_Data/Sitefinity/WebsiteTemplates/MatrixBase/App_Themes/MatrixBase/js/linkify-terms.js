// Turns Topics into links for search
module.exports = function ($) {
    $(window).ready(function () {
        var children = $("#searchTerm > div > .sfCategoriesList").children();
        $(children).each(function(){
            var searchterm = $(this).text();
            var hyperlink = "/search-results/#/" + searchterm.trim() + "/page=1";
            $(this).children().wrap("<a href=" + "'" +  hyperlink + "'"  +"></a>");
        });

    });
};
