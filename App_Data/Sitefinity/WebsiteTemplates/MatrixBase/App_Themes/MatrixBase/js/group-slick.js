module.exports = function ($) {

    $('.slick-container > ul').each(function (i) {
        if (i % 2 == 0) {
            $(this).nextAll().andSelf().slice(0, 2).wrapAll('<div class="rsg"></div>');
        }
    });
};
