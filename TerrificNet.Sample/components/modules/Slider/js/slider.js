(function($) {
	"use strict";
	/**
	 * Slider module implementation.
	 *
	 * @author Denis Skeledzic-Gemperli <denis.skeledzic@namics.com>
	 * @namespace Tc.Module
	 * @class Slider
	 * @extends Tc.Module
	 */
	Tc.Module.Slider = Tc.Module.extend({

		init: function($ctx, sandbox, modId) {
			this._super($ctx, sandbox, modId);
		},

		on: function(callback) {
			var mod = this,
				$ctx = mod.$ctx,

				sliderTimer = null,
				$slider = $ctx.find('.js-slider'),
				$slidebox = $ctx.find('.slider-item'),
				$navigation = $ctx.find('.slidesjs-navigation'),
				sWidth = $slider.data('width') || 640,
				sHeight = $slider.data('height') || 380,
				autoplay = $slider.data('autoplay') || false,
				interval = $slider.data('interval') || 7,
				pause = $slider.data('pause') || 10,
				restartDelay = $slider.data('restartdelay') || 7;

			$slider.hover(
				function() {
					$('.slidesjs-navigation').addClass('state-hover');
				}, function() {
					$('.slidesjs-navigation').removeClass('state-hover');
				}
			);

			if ($slider.find('li').length > 1) {

				$slider.slidesjs({

					width:  sWidth,
					height: sHeight,

					slide: {
						// Slide effect settings.
						speed: 200
						// [number] Speed in milliseconds of the slide animation.
					},

					play: {
						interval:     (interval * 1000), // [number] Time spent on each slide in seconds.
						effect:       'slide', // [string] Can be either "slide" or "fade".
						auto:         autoplay, // [boolean] Start playing the slideshow on load.
						swap:         true, // [boolean] show/hide stop and play buttons
						pauseOnHover: true, // [boolean] pause a playing slideshow on hover
						pause:        (pause * 1000), // [number]
						restartDelay: (restartDelay * 1000) // [number] restart delay on inactive slideshow
					},

					pagination: {
						effect: 'slide'
					},

					navigation: {
						// [boolean] Generates next and previous buttons.
						// You can set to false and use your own buttons.
						// User defined buttons must have the following:
						// previous button: class="slidesjs-previous slidesjs-navigation"
						// next button: class="slidesjs-next slidesjs-navigation"
						active: false,
						effect: 'slide' // [string] Can be either "slide" or "fade".
					}
				});
			}
			else {
				$slider.css('display', 'block');
				$navigation.css('display', 'none');
			}


			callback();
		},

		after: function() {
			var mod = this,
				$ctx = mod.$ctx;


		}

	});
}(Tc.$));
