(function($) {
	"use strict";
	/**
	 * ProductCarousel module implementation.
	 *
	 * @namespace Tc.Module
	 * @class ProductCarousel
	 * @extends Tc.Module
	 */
	Tc.Module.ProductCarousel = Tc.Module.extend({

		dataTemplate: null,

		init: function($ctx, sandbox, modId) {
			this._super($ctx, sandbox, modId);

			this.dataTemplate = $ctx.data('template');
		},

		bind: function() {
			var mod = this,
				$ctx = mod.$ctx,
				$prodSlider = $ctx.find('.js-product-carousel'),
				$btnNavPrev = $ctx.find('.js-btn-nav-prev'),
				$btnNavNext = $ctx.find('.js-btn-nav-next');

			$prodSlider.owlCarousel({

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
