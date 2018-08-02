/**
 *
 *  Matrix Group Branding Area jQuery Plugin
 *  Author: Roger Vandawalker <rvandawalker@matrixgroup.net>, @rjv
 *  Version: 1.1.2
 *
 *  Dependencies: jQuery 1.4+ (http://jquery.com)
 *  Supports: IE7+, Chrome, Safari, Firefox 3.6+
 *
 *  Copyright (c) 2012 Matrix Group International <http://matrixgroup.net>
 *  Released under the MIT license
 *  http://www.opensource.org/licenses/mit-license.php
 *
 */

var jQuery = require('jquery');

module.exports = function() {

    (function ($, window, document, undefined) {
        var Branding = {

            _classes: {
                slide: 'slide',
                element: 'mtx-branding',
                container: 'slides-container',
                indicators: 'slide-indicators',
                background: 'slide-background',
                thumbnails: 'slide-thumbnails',
                thumbnail: 'slide-thumbnail'
            },

            _init: function (options, elem) {
                var self = this;

                self.elem = elem;
                self.$elem = $(elem);

                self.options = $.extend({}, $.fn.matrixBranding.options, options);

                self.$elem.addClass(self._classes.element);
                self.wrapSlides();

                if (self.getSlideCount() > 1) { // only append slide navigation if there is more than one slide
                    self.appendSlideNavigation();
                }

                if (self.options.showSlideIndicators && self.getSlideCount() > 1) { // only append slide
                    self.appendSlideIndicators();
                }

                if (self.options.showSlideThumbnails) {
                    self.appendThumbnails();
                }

                self.selectInitialSlide();

                if (self.options.autoPlay && self.getSlideCount() > 1) {
                    self.showTimer = setTimeout(function () {
                        self.playSlideshow();
                    }, self.options.slideDuration);
                }
            },

            wrapSlides: function () {
                var self = this;

                self.$elem.children().addClass(self._classes.slide).wrapAll($('<div/>', {"class": self._classes.container}));
            },

            selectInitialSlide: function () {
                var self = this,
                    slideNumber = self.options.initialSlide - 1;

                if (self.options.startWithRandomSlide) {
                    slideNumber = Math.floor(Math.random() * self.getSlideCount());
                }

                self.showSlide(self.$elem.find("." + self._classes.slide).eq(slideNumber));
            },

            appendSlideNavigation: function () {
                var self = this,
                    $apn = self.options.assignPreviousNavigation,
                    $ann = self.options.assignNextNavigation;

                // assign or create the "previous slide" navigation button

                // if the previous button has been explicitly assigned with a jquery object,
                // add the "previous" click event to it
                if ($apn instanceof jQuery) {
                    $apn.bind("click", function (e) {
                        e.preventDefault();
                        if (self.options.pauseOnClick) {
                            self.pauseSlideshow();
                        }
                        self.showPreviousSlide();
                    });
                } else {
                    $("<a/>", {
                        href: 'javascript:void(0);',
                        html: self.options.previousButtonText,
                        title: "Previous Slide",
                        "class": self.options.previousButtonClass,
                        click: function (e) {
                            e.preventDefault();
                            if (self.options.pauseOnClick) {
                                self.pauseSlideshow();
                            }
                            self.showPreviousSlide();
                        }
                    }).appendTo(self.$elem);
                }

                // if the next button has been explicitly assigned with a jquery object,
                // add the "next" click event to it
                if ($ann instanceof jQuery) {
                    $ann.bind("click", function (e) {
                        e.preventDefault();
                        if (self.options.pauseOnClick) {
                            self.pauseSlideshow();
                        }
                        self.showNextSlide();
                    });
                } else {
                    $("<a/>", {
                        href: 'javascript:void(0);',
                        html: self.options.nextButtonText,
                        title: "Next Slide",
                        "class": self.options.nextButtonClass,
                        click: function (e) {
                            e.preventDefault();
                            if (self.options.pauseOnClick) {
                                self.pauseSlideshow();
                            }
                            self.showNextSlide();
                        }
                    }).appendTo(self.$elem);
                }
            },

            appendSlideIndicators: function () {
                var self = this,
                    $indicators = $('<ul/>', {"class": self._classes.indicators}),
                    $slides = self.$elem.find("." + self._classes.slide);

                $slides.each(function (i) {
                    $('<a>',
                        {
                            text: (i + 1),
                            href: 'javascript:void(0);',
                            click: function () {
                                var $li = $(this).parent();

                                if (self.options.pauseOnClick) {
                                    self.pauseSlideshow();
                                }
                                self.showSlide($slides.eq($li.index()));
                            }
                        }
                    ).appendTo($indicators);
                });

                $indicators.children().wrap('<li/>');
                $indicators.appendTo(self.$elem);
            },

            appendThumbnails: function () {
                var self = this,
                    $thumbnails = self.options.assignThumbnailsContainer != null
                        ? self.options.assignThumbnailsContainer.addClass(self._classes.thumbnails)
                        : $('<div/>', {"class": self._classes.thumbnails}),
                    $slides = self.$elem.find("." + self._classes.slide);


                // recalculate the container width after all the images have loaded
                $(window).load(function () {
                    $(window).trigger("resize");
                }).resize(function () {
                    // if ($thumbnails.data('thumbs') !== undefined) { return; }
                    $thumbnails.data("thumbs", {"width": 0, "count": 0}); // set the default width to 0
                    $thumbnails.find(".thumb").each(function () {

                        $thumbnails.data('thumbs', {
                            "width": $thumbnails.data("thumbs").width + $(this).outerWidth() + Number($(this).css("margin-left").replace("px", "")),
                            "count": $thumbnails.data("thumbs").count + 1
                        });

                    });

                    var newWidth = Math.ceil($thumbnails.data('thumbs').width) + 1;
                    // alert(newWidth);

                    $thumbnails.children("div").css({
                        // round up the width and add 1px in order to account for improper subpixel rendering in IE<9
                        width: newWidth
                    });
                });

                $slides.each(function (i) {

                    // check to see if the slide has a pre-set thumbnail image
                    // if not, use the slide background image

                    $theThumbnail = $(this).find("." + self._classes.thumbnail).length === 1
                        ? $(this).find("." + self._classes.thumbnail).eq(0)
                        : $(this).find("img." + self._classes.background);

                    var $newThumb = $("<span/>",
                        {
                            "class": "thumb",
                            click: function () {
                                if (self.options.pauseOnClick) {
                                    self.pauseSlideshow();
                                }
                                self.showSlide($slides.eq($(this).index()));
                            }
                        });

                    $theThumbnail.clone().removeClass("slide-background").removeAttr("id").appendTo($newThumb);

                    $newThumb.appendTo($thumbnails);

                });

                $thumbnails.children().wrapAll('<div/>');

                if (self.options.assignThumbnailsContainer === null) {
                    $thumbnails.insertAfter(self.$elem);
                }

                self.options.assignThumbnailsContainer = $thumbnails;
            },

            showNextSlide: function () {
                var self = this,
                    $slides = self.$elem.find("." + self._classes.slide),
                    $current = $slides.filter("." + self.options.selectedSlideClass);

                if (($current.index() + 1) % $slides.length === 0) {
                    // the last slide has been reached; jump to first slide
                    self.showSlide($slides.first());
                } else {
                    self.showSlide($current.next("." + self._classes.slide));
                }
            },

            showPreviousSlide: function () {
                var self = this,
                    $slides = self.$elem.find("." + self._classes.slide),
                    $current = $slides.filter("." + self.options.selectedSlideClass);

                if ($current.index() === 0) {
                    // we're at the first slide; jump to last slide
                    self.showSlide($slides.last());
                } else {
                    self.showSlide($current.prev("." + self._classes.slide));
                }
            },

            showSlide: function ($slide) {
                var self = this,
                    index = $slide.index();

                $slide.addClass(self.options.selectedSlideClass).fadeIn(self.options.transitionDuration)
                    .siblings().removeClass(self.options.selectedSlideClass).fadeOut(self.options.transitionDuration);

                if (self.options.showSlideIndicators) {
                    self.showSlideIndicator($slide);
                }

                if (self.options.showSlideThumbnails) {
                    self.showSlideThumbnail($slide);
                }
            },

            showSlideIndicator: function ($slide) {
                var self = this,
                    index = $slide.index();

                if (self.options.showSlideIndicators) {
                    self.$elem.find("." + self._classes.indicators).children().eq(index).addClass(self.options.selectedSlideClass)
                        .siblings().removeClass(self.options.selectedSlideClass);
                }
            },

            showSlideThumbnail: function ($slide) {
                var self = this,
                    index = $slide.index(),
                    $thumbContainer = self.options.assignThumbnailsContainer, // self.$elem.next("."+self._classes.thumbnails),
                    $thumb = $thumbContainer.find(".thumb").eq(index);

                if (self.options.showSlideThumbnails) {
                    $thumb.addClass(self.options.selectedSlideClass)
                        .siblings().removeClass(self.options.selectedSlideClass);

                    self.slideToThumbnail($thumbContainer, $thumb);
                }
            },

            slideToThumbnail: function ($thumbContainer, $thumb) {
                var self = this,
                    leftScrollTo = -1,
                    topScrollTo = -1,
                    thumbPos = $thumb.position(),
                    thumbWidth = $thumb.outerWidth(),
                    thumbHeight = $thumb.outerHeight(),
                    containerWidth = $thumbContainer.outerWidth(),
                    containerHeight = $thumbContainer.outerHeight();

                if (((thumbWidth + thumbPos.left) > (containerWidth + $thumbContainer.scrollLeft()))
                    || ($thumbContainer.scrollLeft() > thumbPos.left)) {
                    leftScrollTo = thumbPos.left;
                }

                if (((thumbHeight + thumbPos.top) > (containerHeight + $thumbContainer.scrollTop()))
                    || ($thumbContainer.scrollTop() > thumbPos.top)) {
                    topScrollTo = thumbPos.top;
                }

                if (leftScrollTo > -1 || topScrollTo > -1) {
                    $thumbContainer.animate({
                        scrollLeft: leftScrollTo < 0 ? 0 : leftScrollTo,
                        scrollTop: topScrollTo < 0 ? 0 : topScrollTo
                    }, self.options.transitionDuration);
                }
            },

            playSlideshow: function () {
                var self = this;

                self.showNextSlide();

                self.showTimer = setTimeout(function () {
                    self.playSlideshow();
                }, self.options.slideDuration);
            },

            pauseSlideshow: function () {
                var self = this;
                clearTimeout(self.showTimer);
            },

            getSlideCount: function () {
                var self = this,
                    $slides = self.$elem.find("." + self._classes.slide);

                return $slides.length;
            }

        };

        $.fn.matrixBranding = function (options) {
            return this.each(function () {

                var branding = Object.create(Branding);
                branding._init(options, this);

            });
        };

        $.fn.matrixBranding.options = {
            nextButtonClass: 'next',        // class name applied to the "next" button
            previousButtonClass: 'prev',    // class name applied to the "previous" button
            nextButtonText: '&rarr;',       // text for the auto-generated "next" button
            previousButtonText: '&larr;',   // text for the auto-generated "previous" button
            selectedSlideClass: 'current',  // class name applied to the visible slide
            transitionDuration: 800,        // number in ms for how long a slide transition should be
            slideDuration: 7000,            // number in ms for how long a slide should show
            autoPlay: true,                 // set to false to prevent an automatic slideshow
            pauseOnClick: true,             // set to false to prevent pausing the slideshow on clicking slide navigation
            showSlideIndicators: false,     // set to true to automatically add a slide navigation list
            showSlideThumbnails: false,
            assignThumbnailsContainer: null,
            initialSlide: 1,                // explictly set a slide to show first
            startWithRandomSlide: false,    // set to true for a random initial slide
            assignNextNavigation: null,       // a jquery object to be binded as slide navigation
            assignPreviousNavigation: null    // a jquery object to be binded as slide navigation
        };
    })(jQuery, window, document);


// Polyfill for Object.create method
    if (typeof Object.create !== 'function') {
        Object.create = function (obj) {
            function F() {
            };
            F.prototype = obj;
            return new F();
        };
    }

};
