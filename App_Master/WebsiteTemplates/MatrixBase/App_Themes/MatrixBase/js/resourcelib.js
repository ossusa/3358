module.exports = exporting;

function exporting($) {
	require('./vendor/jquery.matrix.selecttree')($);

	$(function() {
		$('.controls:not(:has(.mtx-select-tree-container)) select.checkboxes')
			.removeClass('chzn-done')
			.siblings('.chzn-container').remove().end()
			.selectTree();

		$(document).on('click', '.reset', function() {
			var selector = $(this).data('select');
			$(selector).selectTree('uncheckAll');
			$(this).parent().find('input[type="submit"]').click();
		});
	});
}
