(function($) {
	"use strict";
	/**
	 * NavService module implementation.
	 *
	 * @author Denis Skeledzic-Gemperli <denis.skeledzic@namics.com>
	 * @namespace Tc.Module
	 * @class NavService
	 * @extends Tc.Module
	 */
	Tc.Module.NavService = Tc.Module.extend({

		init: function($ctx, sandbox, modId) {
			this._super($ctx, sandbox, modId);

		},

		on: function(callback) {
			var mod = this,
				$ctx = mod.$ctx,
				$langLink = $ctx.find('.js-service-link-language-item'),
				$langList = $ctx.find('.js-service-link-language-list');

			/*
			$link.hover(
				function() {
					$('.glyph-arw-s-down').addClass('state-active');
				}, function() {
					$('.glyph-arw-s-down').removeClass('state-active');
				}
			);
			*/

			$langLink.click(function() {

				var $this = $(this);
				$this.toggleClass('state-active');
				$langList.toggleClass( 'state-open' );
			});

			callback();
		},

		after: function() {
			var mod = this,
				$ctx = mod.$ctx;

		}

	});
}(Tc.$));
