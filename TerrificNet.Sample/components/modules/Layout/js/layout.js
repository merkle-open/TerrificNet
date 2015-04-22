(function($) {
	"use strict";
	/**
	 * Layout module implementation.
	 *
	 * @author Denis Skeledzic-Gemperli <denis.skeledzic@namics.com>
	 * @namespace Tc.Module
	 * @class Layout
	 * @extends Tc.Module
	 */
	Tc.Module.Layout = Tc.Module.extend({

		init: function($ctx, sandbox, modId) {
			this._super($ctx, sandbox, modId);

		},

		on: function(callback) {
			var mod = this,
				$ctx = mod.$ctx,
				$page = $ctx.find('.js-l-page'),
				$navMobBtn = $ctx.find('.js-btn-m-mobile'),
				$navMobClose = $ctx.find('.js-btn-m-mobile-close');

			// -----------------------------------------
			// Mobile Navigaiton Button
			// -----------------------------------------
			$navMobBtn.on('click', function (e) {
				var $this = $(this);
				$this.toggleClass('state-active');
				$('.js-l-off-nav').addClass('state-open');
				$page.addClass('state-dimmed');
				$ctx.css('overflow-y', 'hidden');
			});

			$navMobClose.on('click', function (e) {
				var $this = $(this);
				$this.toggleClass('state-active');
				$('.js-l-off-nav').removeClass('state-open');
				$page.removeClass('state-dimmed');
				$ctx.css('overflow-y', 'scroll');
			});


			callback();
		},

		after: function() {
			var mod = this,
				$ctx = mod.$ctx;


		}

	});
}(Tc.$));
