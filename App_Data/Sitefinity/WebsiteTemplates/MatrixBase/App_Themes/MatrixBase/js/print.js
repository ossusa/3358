/**
 * Replace default copyright year with the current year
 *
 * Dependencies: jQuery
 */

var $ = require('jquery');

module.exports = function () {
    $(function () {
        $('.icon-envelop').click(function(){
            emailCurrentPage();
        });
        $('.icon-print-1').click(function(){
            printFunction();
        });
    }); // DOM ready
};

function printFunction() {
    window.print();
}
function emailCurrentPage(){
    window.location.href="mailto:?subject="+document.title+"&body="+escape(window.location.href);
}