/**
 * Branding Area initialization
 *
 * Dependencies: jQuery, Branding Area plugin
 */

module.exports = function ($) {
    require('./vendor/jquery.matrix.branding')($);

    $(function () {
        $('.branding').matrixBranding({
            nextButtonText: '<span class="icon icon-angle-right"></span>',
            previousButtonText: '<span class="icon icon-angle-left"></span>'
        });
    });
};
