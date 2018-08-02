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
        coverImage('.home-article-area');
        coverImage('.overlay-article');
        coverImage('.topic-world');
		coverImage('.topic-about');
        boxMaxHeight('.height-box');
        searchTerm(".search-terms");


    }); // DOM ready
};

function printFunction() {
    window.print();
}
function emailCurrentPage(){
    window.location.href="mailto:?subject="+document.title+"&body="+escape(window.location.href);
}
function coverImage(cimg){
    var coverArea=$(cimg);
    var imgLocation=coverArea.find('.sfimageWrp').find('img');
    var img_src=imgLocation.attr('src');
    coverArea.css({"background-image":"url('"+img_src+"')","background-size":"cover", "background-position":"center"})
    imgLocation.remove();
}
function boxMaxHeight(heightelement){
    var maxHeight = Math.max.apply(null, $(heightelement).map(function ()
    {
        return $(this).height();
    }).get());
    $(heightelement).height(maxHeight);
}
function searchTerm(searchterm){
    $(searchterm).each(function(){
        var searchitem = $(this).text();
        var hyperlink = "/search-results/#/" + searchitem.trim() + "/page=1";
        $(this).attr("href",hyperlink);

    });
}
