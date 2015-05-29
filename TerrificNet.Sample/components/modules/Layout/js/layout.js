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
				$offNav = $ctx.find('.js-l-off-nav'),
				$backTopBtn = $ctx.find('.js-btn-back-top');

			mod.sandbox.subscribe('layout', mod);

			// -----------------------------------------
			// Mobile Navigation Button
			// -----------------------------------------
			$navMobBtn.on('click', function (e) {
				var $this = $(this);
				$this.toggleClass('state-active');
				$offNav.toggleClass('state-open');
				$page.toggleClass('state-overflow');
			});

			$ctx.click('click', function () {
				// console.log(1);
			});

			// -----------------------------------------
			// Back Top Button
			// -----------------------------------------
			$backTopBtn.hide();

			$(function () {
				$(window).scroll(function () {
					if ($(this).scrollTop() > 100) {
						$backTopBtn.fadeIn('fast');
					} else {
						$backTopBtn.fadeOut('fast');
					}
				});

				$backTopBtn.on('click', function () {
					$('body, html').animate({
						scrollTop: 0
					}, 800);
					return false;
				});
			});

			callback();
		},

		after: function() {
			var mod = this,
				$ctx = mod.$ctx;
		}

	});
}(Tc.$));
