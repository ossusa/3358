/*SLIDER*/
$(document).ready(function () {
    /*Slider*/
    $('.resources__slider').slick({
        dots: false,
        infinite: true,
        speed: 300,
        slidesToShow: 2,
        slidesToScroll: 1,
        responsive: [{
            breakpoint: 1109,
            settings: {
                slidesToShow: 1,
                slidesToScroll: 1
            }
        }]
    });

    /*SEARCH IN HEADER*/
    var $search = '<li class="header__list header__list-search"><button class="header__search anticon anticon-search" type="submit"></button></li>';
    var $li = $(".header__list:contains('Account')");
    var $liLogout = $(".header__list:contains('SignIn')");
    var $liSearch = '<div class="mg-search-box hidden"><div class="relative"><label for="site-search" class="visuallyhidden">Search</label><input type="text" class="search-box" id="site-search" placeholder="Search"><button id="site-search-submit" class="hidden__search anticon anticon-search" type="submit"></button></div></div>';
    $($search).insertAfter($li);
    $($search).insertBefore($liLogout);
    $($liSearch).appendTo('.header__list-search');
    $(".header__search").click(function (e) {
        e.preventDefault();
        $('.mg-search-box').toggle();
        $('.header__sub_ul').hide();
    });
        $("#site-search-submit").click(function (e) {
            e.preventDefault();
            var q = $('#site-search').val();
            location.href = '/topics-and-tools/volunteer/vws/chiefs-a-rit/search-results/' + q;
        });
    /*Mob search*/
    $(".site-search-submit").click(function (e) {
        e.preventDefault();
        var qSecond = $('.site-search').val();
        location.href = '/topics-and-tools/volunteer/vws/chiefs-a-rit/search-results/' + qSecond;
    });

    $(document).ready(function () {
        $($liLogout).closest('.header__table-tr').addClass('header_unlogged');
    });

    /*LABEL COLORS*/
    $(document).ready(function() {
        var $breadcrumbLeadershipThird =  $(".selectric-community__title-select .label:contains('Leadership')");
        var $breadcrumbFinanceThird =  $(".selectric-community__title-select .label:contains('Finance')");
        var $breadcrumbPersonnelThird =  $(".selectric-community__title-select .label:contains('Personnel')");
        var $breadcrumbCommunity =  $(".community__breadcrumbs:contains('Community')");
        var $breadcrumbCommunitySecond = $(".resources-breadcrumbs").has("span:contains('Community')");
        var $breadcrumbLeadership =  $(".community__breadcrumbs:contains('Leadership')");
        var $breadcrumbLeadershipSecond = $(".resources-breadcrumbs").has("span:contains('Leadership')");
        var $breadcrumbFinance =  $(".community__breadcrumbs:contains('Finance')");
        var $breadcrumbFinanceSecond = $(".resources-breadcrumbs").has("span:contains('Finance')");
        var $breadcrumbPersonnel =  $(".community__breadcrumbs:contains('Personnel')");
        var $breadcrumbPersonnelSecond = $(".resources-breadcrumbs").has("span:contains('Personnel')");
        $($breadcrumbCommunity).next().addClass('label-community');
        $($breadcrumbCommunitySecond).next().addClass('label-community');
        $($breadcrumbLeadership).next().addClass('label-leadership');
        $($breadcrumbLeadershipSecond).next().addClass('label-leadership');
        $($breadcrumbFinance).next().addClass('label-finance');
        $($breadcrumbFinanceSecond).next().addClass('label-finance');
        $($breadcrumbPersonnel).next().addClass('label-personnel');
        $($breadcrumbPersonnelSecond).next().addClass('label-personnel');
        $($breadcrumbLeadershipThird).addClass('label-leadership');
        $($breadcrumbFinanceThird).addClass('label-finance');
        $($breadcrumbPersonnelThird).addClass('label-personnel');

        var $liLeadership =  $(".selectric-handbook_title_select .selectric-items li:contains('Leadership')");
        $($liLeadership).addClass('label-leadership');
        var $liCommunity =  $(".selectric-handbook_title_select .selectric-items li:contains('Community')");
        $($liCommunity).addClass('label-community');
        var $liFinance =  $(".selectric-handbook_title_select .selectric-items li:contains('Finance')");
        $($liFinance).addClass('label-finance');
        var $liPersonnel =  $(".selectric-handbook_title_select .selectric-items li:contains('Personnel')");
        $($liPersonnel).addClass('label-personnel');
    });

    /*OFF CANVAS MENU*/
    $(".header__mob-open").click(function () {
        $('.header__mob-nav').css({ width: "100vw" });
    });
    $(".header__mob-close").click(function () {
        $('.header__mob-nav').css({ width: "0" });
    });

    /*ACCORDION*/
    $(".accordion__title").on("click", function (e) {
        e.preventDefault();
        var $this = $(this);
        if (!$this.hasClass("accordion-active")) {
            $(".accordion__content").slideUp(400);
            $(".accordion__title").removeClass("accordion-active");
            $('.accordion__arrow').removeClass('accordion__rotate');
        }
        $this.toggleClass("accordion-active");
        $this.next().slideToggle();
        $('.accordion__arrow', this).toggleClass('accordion__rotate');
    });
    $(".accordion__sub-title").on("click", function (e) {
        e.preventDefault();
        var $this = $(this);
        if (!$this.hasClass("accordion-sub-active")) {
            $(".accordion__sub-content").slideUp(400);
            $(".accordion__sub-title").removeClass("accordion-sub-active");
            $('.accordion__sub-arrow').removeClass('accordion__rotate');
        }
        $this.toggleClass("accordion-sub-active");
        $this.next().slideToggle();
        $('.accordion__sub-arrow', this).toggleClass('accordion__rotate');
    });

    /*ARROW IN MENU LINK*/
    var $arrowRight = '<div class="anticon anticon-right"></div>';
    $(".header__nav > ul > li > div > ul > li > a").append($arrowRight);

    /*RESOURCES WIDGET*/
    $('.resources').prepend($(".resources__logged"));
    $('.community__head-btn').click(function () {
        $(this).addClass('community__head-btn-active');
    });


    $('.selectric-community__title-select .selectric').click(function (e) {
        e.stopPropagation();
        $(this).find('.category__arrow').toggleClass('rotated');
    });
    $('.selectric-community__category-select .selectric').click(function (e) {
        e.stopPropagation();
        $(this).find('.category__arrow').toggleClass('rotated');
    });
    $(document).on("click", function () {
        $(".rotated").toggleClass('rotated');
    });


    /*STICKY HEADER*/
    var headerText = $('#headerText');
    var wrapper = $('#wrapper');
    var stickyHeader = $('.box-mobile');
    $(window).scroll(function(){
        if ($(window).scrollTop() >= headerText.outerHeight()) {
            wrapper.css("margin-top", stickyHeader.height());
            stickyHeader.addClass('sticky');
        }
        else {
            stickyHeader.removeClass('sticky');
            wrapper.css("margin-top", 0);
        }
    });

    //image container height
    function setImgHeight() {
        var imgContainer = $('.img-container-js');
        var width = imgContainer.width();
        imgContainer.height(width/13*9 -2);
    };

    //set content's height to make footer always at the bottom
    function setWrapperHeight() {
        var windowHeight = $( window ).height();
        var headerHeight = $( '#header').outerHeight();
        var footerHeight = $( '.footer').outerHeight();
        var minHeight = windowHeight - headerHeight - footerHeight;
        $('#wrapper').css('min-height', minHeight);
    };


    setInterval(function() {
        setImgHeight();
        setWrapperHeight();
    }, 250);





    $('.markAsCompleteBtn').click(function () {
        $('.complete_box').addClass('anticon anticon-check');
    });
    $('.header__list:nth-of-type(1) > a').click(function (e) {
        e.preventDefault();
        $('.header__sub_ul').toggle();
        $('.mg-search-box').hide();
    });
    $('.header__sub_list:nth-of-type(1)').click(function() {
        $(".header__sub_list:nth-of-type(1) .header__second_ul").show();
        $('.header__sub_list .header__second_ul').not('.header__sub_list:nth-of-type(1) .header__second_ul').hide();
    });
    $('.header__sub_list:nth-of-type(2)').click(function() {
        $(".header__sub_list:nth-of-type(2) .header__second_ul").show();
        $('.header__sub_list .header__second_ul').not('.header__sub_list:nth-of-type(2) .header__second_ul').hide();
    });
    $('.header__sub_list:nth-of-type(3)').click(function() {
        $(".header__sub_list:nth-of-type(3) .header__second_ul").show();
        $('.header__sub_list .header__second_ul').not('.header__sub_list:nth-of-type(3) .header__second_ul').hide();
    });
    $('.header__sub_list:nth-of-type(4)').click(function() {
        $(".header__sub_list:nth-of-type(4) .header__second_ul").show();
        $('.header__sub_list .header__second_ul').not('.header__sub_list:nth-of-type(4) .header__second_ul').hide();
    });

    $(window).click(function() {
        $('.header__sub_ul').hide();
        $('.header__second_ul').hide();
        $(".header__list:nth-of-type(1) > .header__list-link").removeClass("colored-list");
        $(".header__sub_list:nth-of-type(1) .header__sub_list-link.colored").removeClass("colored");
        $(".header__sub_list:nth-of-type(2) .header__sub_list-link.colored").removeClass("colored");
        $(".header__sub_list:nth-of-type(3) .header__sub_list-link.colored").removeClass("colored");
        $(".header__sub_list:nth-of-type(4) .header__sub_list-link.colored").removeClass("colored");
        $('.mg-search-box').hide();
    });
    $('.header__list-link:nth-of-type(1), .header__sub_list, .header__sub_ul, .header__second_ul, .header__search, .mg-search-box').click(function(event){
        event.stopPropagation();
    });
    $('.header__sub_list-link').click(function(e){
        e.preventDefault();
    });
});
$(document).ready(function() {
    $(".header__sub_list-link").click(function(){
        $(".header__sub_list:nth-of-type(1) .header__sub_list-link.colored").removeClass("colored");
        $(this).addClass("colored");
    });
    $(".header__sub_list-link").click(function(){
        $(".header__sub_list:nth-of-type(2) .header__sub_list-link.colored").removeClass("colored");
        $(this).addClass("colored");
    });
    $(".header__sub_list-link").click(function(){
        $(".header__sub_list:nth-of-type(3) .header__sub_list-link.colored").removeClass("colored");
        $(this).addClass("colored");
    });
    $(".header__sub_list-link").click(function(){
        $(".header__sub_list:nth-of-type(4) .header__sub_list-link.colored").removeClass("colored");
        $(this).addClass("colored");
    });
    $(".header__list:nth-of-type(1) > a").click(function(){
        $(".header__list:nth-of-type(1) > a").addClass("colored-list");
    });
});

/*TEXT CUTTING*/
/*First title and desc*/
if($(window).width() < 767) {
    $('.resources__slide-desc').each(function() {
        var x = 87;
        var y = '...';
        if ($(this).text().length > (x - y.length)) {
            $(this).text($(this).text().substr(0, x-y.length) + y);
        }
    });
    $('.resources__slide-title').each(function() {
        var xTitle = 31;
        var yTitle = '...';
        if ($(this).text().length > (xTitle - yTitle.length)) {
            $(this).text($(this).text().substr(0, xTitle-yTitle.length) + yTitle);
        }
    });
} else {
    $('.resources__slide-desc').each(function() {
        var x = 144;
        var y = '...';
        if ($(this).text().length > (x - y.length)) {
            $(this).text($(this).text().substr(0, x-y.length) + y);
        }
    });
    $('.resources__slide-title').each(function() {
        var xTitle = 55;
        var yTitle = '...';
        if ($(this).text().length > (xTitle - yTitle.length)) {
            $(this).text($(this).text().substr(0, xTitle-yTitle.length) + yTitle);
        }
    });
}

/*Second title and  desc*/
if($(window).width() < 1109) {
    $('.complete__resources_slide .resources__slide-desc, .handbook__template .resources__slide-desc').each(function() {
        var xSecond = 63;
        var ySecond = '...';
        if ($(this).text().length > (xSecond - ySecond.length)) {
            $(this).text($(this).text().substr(0, xSecond-ySecond.length) + ySecond);
        }
    });
    $('.complete-box .resources__slide-title, .handbook__template .resources__slide-title').each(function() {
        var xSecondTitle = 47;
        var ySecondTitle = '...';
        if ($(this).text().length > (xSecondTitle - ySecondTitle.length)) {
            $(this).text($(this).text().substr(0, xSecondTitle-ySecondTitle.length) + ySecondTitle);
        }
    });
} else {
    $('.complete-box .resources__slide-desc, .handbook__template .resources__slide-desc').each(function() {
        var xSecond = 90;
        var ySecond = '...';
        if ($(this).text().length > (xSecond - ySecond.length)) {
            $(this).text($(this).text().substr(0, xSecond-ySecond.length) + ySecond);
        }
    });

}
/*Third*/
$('.topics_seperator').each(function() {
    var xThird = 106;
    var yThird = '...';
    if ($(this).text().length > (xThird - yThird.length)) {
        $(this).text($(this).text().substr(0, xThird-yThird.length) + yThird);
    }
});
/*Fourth*/
$('.topics__img-back-info').each(function() {
    var xFourth = 116;
    var yFourth = '...';
    if ($(this).text().length > (xFourth - yFourth.length)) {
        $(this).text($(this).text().substr(0, xFourth-yFourth.length) + yFourth);
    }
});
/*Fifth*/
function myFunction(x) {
    if (x.matches) { // If media query matches
        $('.resources_text-separate').each(function () {
            var xFifth = 24;
            var yFifth = '...';
            if ($(this).text().length > (xFifth - yFifth.length)) {
                $(this).text($(this).text().substr(0, xFifth - yFifth.length) + yFifth);
            }
        });
        $('.resources_desc-separate').each(function () {
            var xFifthTitle = 64;
            var yFifthTitle = '...';
            if ($(this).text().length > (xFifthTitle - yFifthTitle.length)) {
                $(this).text($(this).text().substr(0, xFifthTitle - yFifthTitle.length) + yFifthTitle);
            }
        });
    } else {
        $('.resources_text-separate').each(function () {
            var xFifth = 35;
            var yFifth = '...';
            if ($(this).text().length > (xFifth - yFifth.length)) {
                $(this).text($(this).text().substr(0, xFifth - yFifth.length) + yFifth);
            }
        });
    }
}

var x = window.matchMedia("(min-width: 767px) and (max-width: 1109px)")
myFunction(x) // Call listener function at run time
x.addListener(myFunction) // Attach listener function on state changes

$(".header__sub_list-link").on("click", function () {
    $('.header_unlogged .header__second_ul').toggleClass('toggle');
});
