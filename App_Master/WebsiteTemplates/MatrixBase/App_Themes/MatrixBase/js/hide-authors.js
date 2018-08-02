// collapses Authors view on Resource page
module.exports = function ($) {
    $(function () {
		$(window).load(function() {
			var authors = $("#Authors").next().children().children().find("ul");
			authors.css("display", "none");
        });
    });
};
