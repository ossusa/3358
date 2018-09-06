

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

    /*Search in header*/
    var $search = '<li class="header__list header__list-search"><button class="header__search anticon anticon-search" type="submit"></button></li>';
    var $li = $(".header__list:contains('Account')");
    var $liLogout = $(".header__list:contains('Signin')");
    var $liSearch = '<div class="mg-search-box hidden"><div class="relative"><label for="site-search" class="visuallyhidden">Search</label><input type="text" class="search-box" id="site-search" placeholder="Search"><button id="site-search-submit" class="hidden__search anticon anticon-search" type="submit"></button></div></div>';
    $($search).insertAfter($li);
    $($search).insertBefore($liLogout);
    $($liSearch).appendTo('.header__list-search');
    $(".header__search").click(function (e) {
        e.preventDefault();
        $('.mg-search-box').toggleClass('hidden');
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
    /*Label colors*/
    $(document).ready(function() {
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
    });

    /*Off canvas menu*/
    $(".header__mob-open").click(function () {
        $('.header__mob-nav').css({ width: "100vw" });
    });
    $(".header__mob-close").click(function () {
        $('.header__mob-nav').css({ width: "0" });
    });

    /*Accordion*/
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

    /*Arrow*/
    var $arrowRight = '<div class="anticon anticon-right"></div>';
    $(".header__nav > ul > li > div > ul > li > a").append($arrowRight);
    /*Resources Widget*/
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


    //sticky header
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
        imgContainer.height(width/13*9 -1);
    }
    setInterval(function() {
        setImgHeight();
    }, 250);


    $('.markAsCompleteBtn').click(function () {
        $('.complete_box').addClass('anticon anticon-check');
    });
    $('.header__list:nth-of-type(1) > a').click(function (e) {
        e.preventDefault();
        $('.header__sub_ul').toggle();
    });
    $('.header__sub_list:nth-of-type(1)').click(function() {
        $(".header__second_ul:nth-of-type(1)").show();
        $('.header__second_ul').not(':nth-of-type(1)').hide();
    });
    $('.header__sub_list:nth-of-type(2)').click(function() {
        $(".header__second_ul:nth-of-type(2)").show();
        $('.header__second_ul').not(':nth-of-type(2)').hide();
    });
    $('.header__sub_list:nth-of-type(3)').click(function() {
        $(".header__second_ul:nth-of-type(3)").show();
        $('.header__second_ul').not(':nth-of-type(3)').hide();
    });
    $('.header__sub_list:nth-of-type(4)').click(function() {
        $(".header__second_ul:nth-of-type(4)").show();
        $('.header__second_ul').not(':nth-of-type(4)').hide();
    });
    $(window).click(function() {
        $('.header__sub_ul').hide();
        $('.header__second_ul').hide();
        $(".header__list:nth-of-type(1) > .header__list-link").removeClass("colored-list");
        $(".header__sub_list:nth-of-type(1) .header__sub_list-link.colored").removeClass("colored");
        $(".header__sub_list:nth-of-type(2) .header__sub_list-link.colored").removeClass("colored");
        $(".header__sub_list:nth-of-type(3) .header__sub_list-link.colored").removeClass("colored");
        $(".header__sub_list:nth-of-type(4) .header__sub_list-link.colored").removeClass("colored");
    });
    $('.header__list-link:nth-of-type(1), .header__sub_list, .header__sub_ul, .header__second_ul').click(function(event){
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
/*First desc*/
if($(window).width() < 767) {
    $('.resources__slide-desc').each(function() {
        var maxchars = 87;
        var seperator = '...';

        if ($(this).text().length > (maxchars - seperator.length)) {
            $(this).text($(this).text().substr(0, maxchars-seperator.length) + seperator);
        }
    });
} else {
    $('.resources__slide-desc').each(function() {
        var maxchars = 144;
        var seperator = '...';

        if ($(this).text().length > (maxchars - seperator.length)) {
            $(this).text($(this).text().substr(0, maxchars-seperator.length) + seperator);
        }
    });
}
/*First title*/
if($(window).width() < 767) {
    $('.resources__slide-title').each(function() {
        var maxcharacters = 31;
        var separate = '...';

        if ($(this).text().length > (maxcharacters - separate.length)) {
            $(this).text($(this).text().substr(0, maxcharacters-separate.length) + separate);
        }
    });
} else {
    $('.resources__slide-title').each(function() {
        var maxcharacters = 55;
        var separate = '...';

        if ($(this).text().length > (maxcharacters - separate.length)) {
            $(this).text($(this).text().substr(0, maxcharacters-separate.length) + separate);
        }
    });
}
/*Second desc*/
if($(window).width() < 1109) {
    $('.complete__resources_slide .resources__slide-desc, .handbook__template .resources__slide-desc').each(function() {
        var maxcharacters = 63;
        var separate = '...';

        if ($(this).text().length > (maxcharacters - separate.length)) {
            $(this).text($(this).text().substr(0, maxcharacters-separate.length) + separate);
        }
    });
} else {
    $('.complete-box .resources__slide-desc, .handbook__template .resources__slide-desc').each(function() {
        var mch = 90;
        var separ = '...';

        if ($(this).text().length > (mch - separ.length)) {
            $(this).text($(this).text().substr(0, mch-separ.length) + separ);
        }
    });
}
/*Second title*/
if($(window).width() < 1109) {
    $('.complete-box .resources__slide-title, .handbook__template .resources__slide-title').each(function() {
        var mch = 47;
        var separ = '...';

        if ($(this).text().length > (mch - separ.length)) {
            $(this).text($(this).text().substr(0, mch-separ.length) + separ);
        }

    });
}
/*Third*/
$('.topics_seperator').each(function() {
    var maxch = 106;
    var sep = '...';

    if ($(this).text().length > (maxch - sep.length)) {
        $(this).text($(this).text().substr(0, maxch-sep.length) + sep);
    }
});
// $(document).ready(function() {
//     $('.active-link').removeClass('active-link');
//     var currurl = window.location.pathname;
//     var val=$('.header__list-link:has([href="'+currurl+'"])').addClass('active-link');
// });
