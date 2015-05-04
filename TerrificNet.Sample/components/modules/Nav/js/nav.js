(function ($) {
	"use strict";
	/**
	 * Nav module implementation.
	 *
	 * @author Denis Skeledzic-Gemperli <denis.skeledzic@namics.com>
	 * @namespace Tc.Module
	 * @class Nav
	 * @extends Tc.Module
	 */
	Tc.Module.Nav = Tc.Module.extend({

		init: function ($ctx, sandbox, modId) {
			this._super($ctx, sandbox, modId);
		},

		on: function (callback) {
			var mod = this,
				$ctx = mod.$ctx,

				$link = $ctx.find('.js-nav-main-link'),
				$flyout = $ctx.find('.js-nav-main-flyout'),
				$closeBtn = $ctx.find('.js-nav-close-btn');

			mod.sandbox.subscribe('layout', mod);

			$link.on('click', function (e) {
				var $this = $(this);
				$this.toggleClass('state-active');
				$this.next($flyout).toggleClass('state-open');
				mod._close($this);

				if ($('.l-content-wrapper').data('blockUI.isBlocked') !== 1 || $link.hasClass('state-active'))  {
					$('.l-content-wrapper').block({
						message: null,
						overlayCSS: {
							backgroundColor: '#fff',
							opacity: 0.7,
							cursor: 'default',
							baseZ: 100
						},
						fadeIn: 400,
						fadeOut: 500
					});
				} else {
					$('.l-content-wrapper').unblock();
				}

				return false; // no link but we never know ;-)
			});

			$closeBtn.on('click', function () {
				$link.removeClass('state-active');
				$flyout.removeClass('state-open');
				$('.l-content-wrapper').unblock();
			});

			callback();
		},

		_close: function ($keep) {
			var mod = this,
				$ctx = mod.$ctx;

			$ctx.find('.state-active').not($keep).toggleClass('state-active').next().toggleClass('state-open');

			// $('.l-content-wrapper').unblock();
		}
	});
}(Tc.$));
