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
    var $li = $(".header__list:contains('ACCOUNT')");
    var $liLogout = $(".header__list:contains('LOGIN')");
    var $liSearch = '<div class="mg-search-box hidden"><div class="relative"><label for="site-search" class="visuallyhidden">Search</label><input type="text" class="search-box" id="site-search" placeholder="Search"><button id="site-search-submit" class="hidden__search anticon anticon-search" type="submit"></button></div></div>';
    $($search).insertAfter($li);
    $($search).insertBefore($liLogout);
    $($liSearch).appendTo('.header__list-search');
    $(".header__search").click(function (e) {
        e.preventDefault();
        $('.mg-search-box').toggle();
        $('.header__sub_ul').hide();
    });

    $(function(){
        $('#site-search-submit').on('keypress click', function(e){
            e.preventDefault();
            var q = $('#site-search').val();
            if (e.which === 13 || e.type === 'click') {
                location.href = '/topics-and-tools/volunteer/vws/chiefs-a-rit/search-results/' + q;
            }
        });
    });
    $(function () {

        $('body').off('keypress.enter').on('keypress.enter', function (e) {
            if (e.which == 13 && e.target.type != 'textarea') {
                var $btn = $(e.target).closest('.header__list-search').find(".hidden__search, button");
                if ($btn.length) {
                    $btn.click();
                    return false;
                }
            }
        });
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
    /*FOR IE*/

    if( $(".header__list-link  > span").text().indexOf('LOGIN') >= 0) {
        $(".header__table-tr").addClass("header_unlogged");
    }



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
        $('.img-container-js').each(function(){
          $(this).height($(this).width()/13*9 -2);
        })
    };


    setInterval(function() {
        setImgHeight();
    }, 250);
    setImgHeight();




    $('.complete_box').on('click', function (event) {
        $(event.currentTarget).addClass('anticon anticon-check');
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
    /*LIKES COLORING*/
    $(".addLike").click(function() {
        $(this).toggleClass('clicked-like');
        $(this).closest('.single-post-statistic').find(".addDislike").removeClass('clicked-dislike');
    });
    $('.addDislike').click(function(){
        $(this).toggleClass('clicked-dislike');
        $(this).closest('.single-post-statistic').find(".addLike").removeClass('clicked-like');
    });
    $('.addCommentLike').click(function(){
        $(this).toggleClass('clicked-like');
        $(this).closest('.single-post-statistic').find(".addCommentDislike").removeClass('clicked-dislike');
    });
    $('.addCommentDislike').click(function(){
        $(this).toggleClass('clicked-dislike');
        $(this).closest('.single-post-statistic').find(".addCommentLike").removeClass('clicked-like');
    });
});

$(window).on('load', function() {
    //image positioning within container
    $( ".img-container-js > a > img" ).each(function() {
        if (this.naturalWidth/this.naturalHeight < 13/9){
            $(this).css({
                'width': '100%',
                'height': 'auto'
            });
        }
    });

});
$(document).ready(function () {
    function textTruncate(){
/*MAIN PAGE TITLE*/
if($(window).width() > 360) {
    $('.big_truncate_title').each(function () {
        var xOne = 39;
        var yOne = '...';
        if ($(this).text().length > (xOne - yOne.length)) {
            $(this).text($(this).text().substr(0, xOne - yOne.length) + yOne);
        }
    });
} else {
    $('.big_truncate_title').each(function () {
        var xOne = 25;
        var yOne = '...';
        if ($(this).text().length > (xOne - yOne.length)) {
            $(this).text($(this).text().substr(0, xOne - yOne.length) + yOne);
        }
    });
    $('.big_truncate_desc').each(function() {
        var xSecond = 71;
        var ySecond = '...';
        if ($(this).text().length > (xSecond - ySecond.length)) {
            $(this).text($(this).text().substr(0, xSecond-ySecond.length) + ySecond);
        }
    });
    $('.big_truncate_hover').each(function() {
        var xThird = 130;
        var yThird = '...';
        if ($(this).text().length > (xThird - yThird.length)) {
            $(this).text($(this).text().substr(0, xThird-yThird.length) + yThird);
        }
    });
    }


/*MAIN PAGE DESC*/
if($(window).width() > 767) {
    $('.big_truncate_desc').each(function() {
        var xTwo = 114;
        var yTwo = '...';
        if ($(this).text().length > (xTwo - yTwo.length)) {
            $(this).text($(this).text().substr(0, xTwo-yTwo.length) + yTwo);
        }
    });
} else {
    $('.big_truncate_desc').each(function() {
        var xTwo = 80;
        var yTwo = '...';
        if ($(this).text().length > (xTwo - yTwo.length)) {
            $(this).text($(this).text().substr(0, xTwo-yTwo.length) + yTwo);
        }
    });
}
/*MAIN PAGE HOVER*/
if($(window).width() > 1109) {
    $('.big_truncate_hover').each(function() {
        var xThree = 116;
        var yThree = '...';
        if ($(this).text().length > (xThree - yThree.length)) {
            $(this).text($(this).text().substr(0, xThree-yThree.length) + yThree);
        }
    });
} else {
    $('.big_truncate_hover').each(function() {
        var xThree = 155;
        var yThree = '...';
        if ($(this).text().length > (xThree - yThree.length)) {
            $(this).text($(this).text().substr(0, xThree-yThree.length) + yThree);
        }
    });
}
/*M&MEDIA PAGE TITLE*/
if($(window).width() > 1109) {
    $('.big_truncate_title_sec').each(function() {
        var xFour = 39;
        var yFour = '...';
        if ($(this).text().length > (xFour - yFour.length)) {
            $(this).text($(this).text().substr(0, xFour-yFour.length) + yFour);
        }
    });
} else {
    $('.big_truncate_title_sec').each(function() {
        var xFour = 25;
        var yFour = '...';
        if ($(this).text().length > (xFour - yFour.length)) {
            $(this).text($(this).text().substr(0, xFour-yFour.length) + yFour);
        }
    });
}
/*M&MEDIA PAGE DESC*/
if($(window).width() > 1109) {
    $('.big_truncate_desc_sec').each(function() {
        var xFive = 126;
        var yFive = '...';
        if ($(this).text().length > (xFive - yFive.length)) {
            $(this).text($(this).text().substr(0, xFive-yFive.length) + yFive);
        }
    });
} else {
    $('.big_truncate_desc_sec').each(function() {
        var xFive = 56;
        var yFive = '...';
        if ($(this).text().length > (xFive - yFive.length)) {
            $(this).text($(this).text().substr(0, xFive-yFive.length) + yFive);
        }
    });
}
/*M&MEDIA PAGE HOVER*/
$('.sm_truncate_hover').each(function() {
    var xSix = 91;
    var ySix = '...';
    if ($(this).text().length > (xSix - ySix.length)) {
        $(this).text($(this).text().substr(0, xSix-ySix.length) + ySix);
    }
});
/*MY HB M&MEDIA PAGE TITLE*/
$('.sm_truncate_title').each(function() {
    var xSeven = 28;
    var ySeven = '...';
    if ($(this).text().length > (xSeven - ySeven.length)) {
        $(this).text($(this).text().substr(0, xSeven-ySeven.length) + ySeven);
    }
});
/*MY HB M&MEDIA PAGE DESC*/
if($(window).width() > 1109) {
    $('.sm_truncate_desc').each(function() {
        var xEight = 55;
        var yEight = '...';
        if ($(this).text().length > (xEight - yEight.length)) {
            $(this).text($(this).text().substr(0, xEight-yEight.length) + yEight);
        }
    });
} else {
    $('.sm_truncate_desc').each(function() {
        var xEight = 51;
        var yEight = '...';
        if ($(this).text().length > (xEight - yEight.length)) {
            $(this).text($(this).text().substr(0, xEight-yEight.length) + yEight);
        }
    });
}
    }
    $(document).ready(function () {
        textTruncate();
    });

    $(window).resize(function () {
        textTruncate();
    });
    if($(window).width() < 325) {
        $('.sm_truncate_title').each(function() {
            var xSmallest = 23;
            var ySmallest = '...';
            if ($(this).text().length > (xSmallest - ySmallest.length)) {
                $(this).text($(this).text().substr(0, xSmallest-ySmallest.length) + ySmallest);
            }
        });
    }
});


