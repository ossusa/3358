module.exports = function ($) {
    $(function () {

        $('body')
			.off('keypress.enter')
			.on('keypress.enter', function (e) {
			  if (e.which == 13 && e.target.type != 'textarea') {
			      var $btn = $(e.target).closest('.form-container').find("input[type='submit'], button");
			      if ($btn.length) {
			          $btn.click();
			          return false;
			      }
			  }
			});

    });
};
