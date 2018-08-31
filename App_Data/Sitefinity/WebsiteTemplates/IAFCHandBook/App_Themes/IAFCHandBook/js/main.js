

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
    var $search = '<li class="k-item k-item-search"><button class="header__search anticon anticon-search" type="submit"></button></li>';
    var $li = $(".sfNavHorizontalDropDownWrp li:contains('Account')");
    var $liSearch = '<div class="mg-search-box hidden"><div class="relative"><label for="site-search" class="visuallyhidden">Search</label><input type="text" class="search-box" id="site-search" placeholder="Search"><button id="site-search-submit" class="hidden__search anticon anticon-search" type="submit"></button></div></div>';
    $($search).insertAfter($li);
    $($liSearch).appendTo('.k-item-search');
    $(".header__search").click(function (e) {
        e.preventDefault();
        $('.mg-search-box').toggleClass('hidden');
    });
    $("#site-search-submit").click(function (e) {
        e.preventDefault();
        var q = $('#site-search').val();
        location.href = '/search-results/#/' + q + "/page=1";
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
    $('.markAsCompleteBtn').click(function () {
        $('.complete_box').addClass('anticon anticon-check');
    });
    $('.header__list-link:nth-of-type(1)').click(function (e) {
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
    // $('body').not('.header__sub_ul').not('.header__list-link:nth-of-type(1)').click(function() {
    //     $('.header__sub_ul').hide();
    // });
});