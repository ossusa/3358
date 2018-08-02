/**
 * Replace default copyright year with the current year
 *
 * Dependencies: jQuery
 */

module.exports = function ($) {
    $(function () {
        var currentYear = (new Date).getFullYear();
        $(".copy-year").text(currentYear);
    });
};
