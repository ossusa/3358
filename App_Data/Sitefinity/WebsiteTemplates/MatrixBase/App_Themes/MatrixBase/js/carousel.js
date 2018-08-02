var noUiSlider = require('./lib/nouislider');

function carousel($) {

    require('./lib/flipster')($);

    (function(){
        //quick fix reorder based on div.data.attr
        /*var items = $("li.mg-carousel__item"),
        lic=[],featureItem = $("li.mg-carousel__item").find("*[data-carousel-order]").parents('li');
        var md;
        $("li.mg-carousel__item").find("*[data-carousel-order]").parents('li').remove();
        items.each(function(idx,itm){
            var $this = $(this);
            if(idx ===2 ){
                //itm.remove();
                md=itm;
                lic.push(featureItem);
                //lic.push(itm);
            }else{
                lic.push(itm);
            }
        });
        lic.push(md);
        $('.sfitemsList').empty().append(lic);    */
        //end
        var viewportWidth = $("body").innerWidth();
        if(viewportWidth < 960){
            $('.slider-range').addClass('noslider');
            $('.carousel-section .featured-carousel-ul').slick({
                infinite: false,
                speed: 300,
                slidesToShow: 1,
                slidesToScroll: 1,
                autoplay: false,
				buttons: true
            });
        }
        else{
            $('.mg-carousel').each(function(){
                bindCarouselWithControls($, this)
            });
        }
    })();

    $('.mg-carousel').css({
        opacity: 1
    });

}

function bindCarouselWithControls($, carousel) {

    var $controls = $(carousel).next('.mg-carousel__controls');

    var $carousel = $(carousel).flipster({
        loop: false,
        style: 'coverflow',
        buttons: true,
        scrollwheel: false,
        touch: true,
        itemContainer: '.featured-carousel-ul',
        itemSelector: '.mg-carousel__item'
    });
    $controls.on('click', '.mg-carousel__control', function(){
        var action = $(this).data('action');
        $carousel.flipster(action, action === 'play' ? 5000 : null);

        if (action === 'play') {
            $controls.addClass('is-playing');
        } else if (action === 'pause') {
            $controls.removeClass('is-playing');
        }
    });

    //var rangeSlider = $(carousel).closest('.carousel-section').find('.slider-range__slider-range')[0],
    var rangeSlider = document.getElementById('custom-slider-range'),
        itemsMax = ($(carousel).find('.mg-carousel__item').length);
        //console.log('itemsMax=>>>>'+itemsMax);
    if( rangeSlider ) {
        if(itemsMax<2){
            $('.mg-carousel__item').addClass('nocarousel');
            $('.slider-range').addClass('noslider');
            $('.flipster__container').addClass('noflipster');
        }
        var max=itemsMax -1;
        
        var startCarousel= parseInt(itemsMax/2);
        noUiSlider.create(rangeSlider, {
            start: [ startCarousel ],
            step: 1,
            range: {
                'min': [ 0 ],
                'max': [ max ]
            }
        });

        rangeSlider.noUiSlider.on('update', function( values, handle ) {
            //console.log( parseInt(values[handle]));
            $carousel.flipster('jump', parseInt(values[handle]) );

        });
        /*$carousel.on('mousewheel.flipster', function(e){
            e.preventDefault();
            // Convert the string to a number.
            var value = Number( rangeSlider.noUiSlider.get() );
            if(e.originalEvent.wheelDelta /120 >0) {
                rangeSlider.noUiSlider.set( value - 1 );
            }
            else{
                rangeSlider.noUiSlider.set( value + 1 );
            }
        });*/
        $carousel.find('.mg-carousel__item').on('click.flipster', function(e){
            // e.preventDefault();
            var currentItem=($(this).index());
            rangeSlider.noUiSlider.set(currentItem);
        });
    }

}

module.exports = carousel;
