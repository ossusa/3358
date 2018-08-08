/* Adding the script tag to the head as suggested before */

var head = document.getElementsByTagName('head')[0];
var script = document.createElement('script');
script.type = 'text/javascript';
script.src = "https://code.jquery.com/jquery-2.2.1.min.js";

// Then bind the event to the callback function.
// There are several events for cross browser compatibility.
script.onreadystatechange = handler;
script.onload = handler;

// Fire the loading
head.appendChild(script);

function handler(){
    console.log('jquery added :)');
}

$( document ).ready(function() {
    $('.hb-jumbo__search').appendTo(".hb-jumbo");
});

$(document).ready(function() {
    $(".k-input").attr("placeholder", "Search");
});

$(document).ready(function() {
    $('.resources__slider').slick({
        dots: false,
        infinite: true,
        speed: 300,
        slidesToShow: 2,
        slidesToScroll: 1,
        responsive: [
            {
                breakpoint: 1109,
                settings: {
                    slidesToShow: 1,
                    slidesToScroll: 1
                }
            }
        ]
    });
});
$( document ).ready(function() {
  
  
});
$(function () {
    var $search = '<li class="k-item k-item-search"><button class="header__search anticon anticon-search" type="submit"></button></li>';
    var $li = $( ".sfNavHorizontalDropDownWrp li:contains('Account')" );
    var $liSearch = '<div class="mg-search-box hidden"><div class="relative"><label for="site-search" class="visuallyhidden">Search</label><input type="text" class="search-box" id="site-search" placeholder="Search"><button id="site-search-submit" class="hidden__search anticon anticon-search" type="submit"></button></div></div>';
    $($search).insertAfter($li);
    $( $liSearch ).appendTo('.k-item-search');
    $(document).on('click', '.header__search', event => {
    	event.preventDefault();
    	$('.mg-search-box').toggleClass('hidden');
    });
$(document).on('click', '#site-search-submit', event => {
        event.preventDefault();
        var q = $('#site-search').val();
        location.href = '/search-results/#/' + q + "/page=1";
   });

});
$(".header__mob-open").click(function(){
    $('.header__mob-nav').css({ width: "100vw" });
    // $('#main').css({ marginRight: "100vw" });
});
$(".header__mob-close").click(function(){
    $('.header__mob-nav').css({ width: "0" });
    // $('#main').css({ marginRight: "0" });
});
$(function() {

    //BEGIN
    $(".accordion__title").on("click", function(e) {

        e.preventDefault();
        var $this = $(this);

        if (!$this.hasClass("accordion-active")) {
            $(".accordion__content").slideUp(400);
            $(".accordion__title").removeClass("accordion-active");
            $('.accordion__arrow').removeClass('accordion__rotate');
        }

        $this.toggleClass("accordion-active");
        $this.next().slideToggle();
        $('.accordion__arrow',this).toggleClass('accordion__rotate');
    });
    //END
    $(".accordion__sub-title").on("click", function(e) {

        e.preventDefault();
        var $this = $(this);

        if (!$this.hasClass("accordion-sub-active")) {
            $(".accordion__sub-content").slideUp(400);
            $(".accordion__sub-title").removeClass("accordion-sub-active");
            $('.accordion__sub-arrow').removeClass('accordion__rotate');
        }

        $this.toggleClass("accordion-sub-active");
        $this.next().slideToggle();
        $('.accordion__sub-arrow',this).toggleClass('accordion__rotate');
    });
});
$(document).ready(function() {
    $arrowRight = '<div class="anticon anticon-right"></div>';
    $(".header__nav > ul > li > div > ul > li > a").append( $arrowRight );
    /*Select*/
    $('.community__title-select').selectric();

    $('.community__title-select').click(function(){
        $('.button').toggleClass('rotated').toggle('community__category-select');
    });
    $('.community__category-select').selectric();
    $('.community__category-select').find('.button').addClass('community__arrow');
    $('.community__category-select').click(function(){
        $('.button').toggleClass('rotated');
    });
});

