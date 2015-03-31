(function($) {
	"use strict";
	/**
	 * Carousel module implementation.
	 *
	 * @namespace Tc.Module
	 * @class Carousel
	 * @extends Tc.Module
	 */
	Tc.Module.Carousel = Tc.Module.extend({

		dataTemplate: null,

		init: function($ctx, sandbox, modId) {
			this._super($ctx, sandbox, modId);

			this.dataTemplate = $ctx.data('template');
		},

		bind: function() {
			var mod = this,
				$ctx = mod.$ctx,
				$slider = $ctx.find('.js-carousel'),
				$btnNavPrev = $ctx.find('.js-btn-nav-prev'),
				$btnNavNext = $ctx.find('.js-btn-nav-next');

			$slider.owlCarousel({

				navigation: false, // Show next and prev buttons
				slideSpeed: 500,
				paginationSpeed: 500,
				mouseDrag: false,
				singleItem: true
			});

			// Custom Navigation Events
			$btnNavPrev.click(function () {
				$prodSlider.trigger('owl.prev');
			});

			$btnNavNext.click(function () {
				$prodSlider.trigger('owl.next');
			});
		},

		on: function(callback) {
			var mod = this;

			mod.sandbox.subscribe('productState', mod);
			mod.bind();

			callback();
		},

		after: function() {
			var mod = this,
				$ctx = mod.$ctx;
		}

	});
}(Tc.$));
