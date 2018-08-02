module.exports = function ($) {

    $(function () {
        $('.mg-images-list').find('img').each(function () {
            $(this).parent().attr('title', $(this).attr('alt'));
        });
    });

};
