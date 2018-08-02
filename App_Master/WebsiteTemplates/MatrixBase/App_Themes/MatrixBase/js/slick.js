/**
 *  Slick carousel
 *
 * Dependencies: jQuery, slick library
 */

function slider($) {

    $('.vendor-carousel').slick({
        infinite: true,
        speed: 300,
        slidesToShow: 5,
        slidesToScroll: 5,
        autoplay: true,
        autoplaySpeed: 3000,
        responsive: [{
            breakpoint: 960,
            settings: {
                slidesToShow: 2,
                slidesToScroll: 2
            }
        },
            {
            breakpoint: 768,
            settings: {
                slidesToShow: 1,
                slidesToScroll: 1
            }
        }, {
            breakpoint: 480,
            settings: {
                slidesToShow: 1,
                slidesToScroll: 1
            }
        }]
    });
	$('.slick-container').slick({
		dots: true,
		infinite: true,
		autoplay: true,
		autoplaySpeed: 3000,
		speed: 300,
		adaptiveHeight: false
	});
}
module.exports = slider;
