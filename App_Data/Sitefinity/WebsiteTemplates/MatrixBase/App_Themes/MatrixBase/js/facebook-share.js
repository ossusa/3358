module.exports = function ($) {
    $(function () {
        $(document).on('click', '.icon-facebook', event => {
            event.preventDefault();
            window.open("https://www.facebook.com/sharer/sharer.php?u=" + escape(window.location.href) + "&t=" + document.title, '', 'menubar=no,toolbar=no,resizable=yes,scrollbars=yes,height=300,width=600');
            return false;
        });
    });
};
