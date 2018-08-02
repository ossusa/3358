function pdf($){
	$( document ).ready(function() {
		$('a[href$=".pdf"]').attr("target","_blank");
		$('a[href$=".pdf"]').attr("rel","noopener noreferrer");
		$('a').filter(function() {
			return this.hostname && this.hostname !== location.hostname;
		}).attr("target","_blank");
		$('a').filter(function() {
			return this.hostname && this.hostname !== location.hostname;
		}).attr("rel","noopener noreferrer");
	});
};
module.exports = pdf;
