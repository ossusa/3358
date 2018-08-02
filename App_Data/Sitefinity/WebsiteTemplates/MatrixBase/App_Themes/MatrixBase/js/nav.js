/**
 * Replace default copyright year with the current year
 *
 * Dependencies: jQuery
 */

module.exports = function ($) {
    $(function () {

        $(document).on('click', '.toggle-main-nav', function(e){
            var $nav = $('.main-nav');
            $nav.toggleClass('showing');
        });

        // add login button
        var $utiltiyNav = $('.top-utility-nav .sfContentBlock'),
            $loginLogOut = $('.login-logout-button a').addClass('button');

        $utiltiyNav.append( $loginLogOut );

        //Responsive Menu
        responsiveMenu();

        sideNavHierarchy($);


    }); // DOM ready
};
function responsiveMenu(){
    //sub menus toggle on mobile
    $('.main-nav ul > li').each(function(){
        var $a = $(this).children('a'),
            $ul = $(this).children('ul');
        if( $ul.length ) {
            $a.append('<span class="toggle-item" />');
        }
    });
    var toggleClass=$('.toggle-item');
    $(document).on('click', '.main-nav ul .toggle-item', function(e){
        e.preventDefault();
        e.stopPropagation();
        var $ul = $(this).closest('li').find('ul');

        if( $ul.hasClass('opened') ){
            $(this).removeClass('menu-toggle');
            $(this).parent().removeClass('selected');
        } else {
            $(this).addClass('menu-toggle');
            $(this).parent().addClass('selected');
        }

        $ul.toggleClass('opened');
    });
}
function sideNavHierarchy($) {
    var $selected = $('.right-rail-navigation .rtSelected').eq(0);
    var $parentLIs = $selected.parentsUntil('.RadTreeView', '.rtLI');
    $parentLIs.each(function(){
        $(this).children('.rtTop, .rtBot, .rtMid').addClass('rtSelectedAncestor');
    });
}
