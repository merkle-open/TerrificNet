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

		bind: function() {
			var mod = this,
				$ctx = mod.$ctx,
				$slider = $ctx.find('.js-slider');

			$slider.owlCarousel({
				navigation: false, // Show next and prev buttons
				slideSpeed: 700,
				paginationSpeed: 700,
				mouseDrag: false,
				autoPlay: 7000, //Set AutoPlay to 7 seconds
				items : 4,
				itemsDesktop : [1199, 4],
				itemsDesktopSmall : [959, 3],
				lazyLoad : true
			});
		},

		on: function(callback) {
			var mod = this;

			//mod.sandbox.subscribe('productState', mod);
			mod.bind();

			callback();
		},

		after: function() {
			var mod = this,
				$ctx = mod.$ctx;
		}

	});
}(Tc.$));
