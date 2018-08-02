// hides Featured Topics from list when fetching
// used in detail pages
module.exports = function ($) {
    $(window).ready(function () {
		$(".sfCategoriesList").find("span:contains('Featured')").each(function() {
		    $(this).parent().css("display", "none");
		});
    });
};
