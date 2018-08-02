var noUiSlider = require('./lib/nouislider');

function flatCarousel($) {

    require('./lib/flipster')($);

    $(function(){

        $('.mg-carousel--flat').each(function(){
            bindCarouselWithControls($, this)
        });

        var firstTradeshowItem = $('.carousel-section--tradeshow .flipster__item--current').data('post-id');
        getDescription($, firstTradeshowItem);

    }); //DOM Ready

    $('.mg-carousel--flat').css({
        opacity: 1
    });

}

function getDescription($, currentItemID) {
  $.ajax({
    url: '/wp-json/wp/v2/tradeshow-items/' + currentItemID,
    dataType: 'json',
    success: function(data){
      $('.company-desc').html(data.content.rendered);
    }
  });
}

function bindCarouselWithControls($, carousel) {

    var $carouselContainer = $(carousel).closest('.carousel-section'),
        $letterNav = $carouselContainer.find('.letter-nav'),
        $carouselDropdown = $carouselContainer.find('.select-nav');

    var $carousel = $(carousel).flipster({
        loop: true,
        style: 'flat',
        start: 0,
        buttons: true,
        scrollwheel: false,
        autoplay: false,
        onItemSwitch: function(currentItem, previousItem){

          if( $carouselContainer.hasClass('carousel-section--tradeshow') ){
            var currentItemID = $(currentItem).data('post-id');
            getDescription($, currentItemID);
          }

        }
    });
    $carouselDropdown.on('change', function(e){
        e.preventDefault();

        var value = $(this).val(),
            $item = $(carousel).find('[data-post-id="'+value+'"]').eq(0);

        if( $item.length ){
            $carousel.flipster('jump', $item);
        }
    });
    $letterNav.on('click', 'a', function(e){
        e.preventDefault();

        var letter = $(this).text(),
            $item = $(carousel).find('[data-letter="'+letter+'"]').eq(0);

        if( $item.length ){
            $carousel.flipster('jump', $item);
            $(this).closest('.letter-nav').find('a').removeClass('active');
            $(this).addClass('active');
        }
    });

    var rangeSlider = $(carousel).closest('.carousel-section').find('.slider-range__slider-range')[0],
        itemsMax = ($(carousel).find('.mg-carousel__item').length) - 1;

    noUiSlider.create(rangeSlider, {
        start: [ 0 ],
        range: {
            'min': [ 0 ],
            'max': [ itemsMax ]
        }
    });

    rangeSlider.noUiSlider.on('update', function( values, handle ) {
        $carousel.flipster('jump', parseInt(values[handle]) );
    });

}
function clearForm(oForm, doSubmit) {
  var elements = oForm.elements;
  oForm.reset();
  for(i=0; i<elements.length; i++) {
    field_type = elements[i].type.toLowerCase();
    switch(field_type) {
      case "text":
      case "password":
      case "textarea":
      case "hidden":

        elements[i].value = "";
        break;

      case "radio":
      case "checkbox":
        if (elements[i].checked) {
          elements[i].checked = false;
        }
        break;

      case "select-one":
      case "select-multi":
        elements[i].selectedIndex = -1;
        break;

      default:
        break;
      }
  }
  if (doSubmit) {
    oForm.submit();
  }
}

module.exports = flatCarousel;
