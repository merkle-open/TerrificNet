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

				$link = $ctx.find('.js-nav-main li a'),
				$flyout = $ctx.find('.js-nav-main-flyout'),
				$closeBtn = $ctx.find('.js-nav-close-btn');

			mod.sandbox.subscribe('layout', mod);

			$link.on('click', function () {
				var $this = $(this);
				$this.toggleClass('state-active');
				$this.next($flyout).toggleClass('state-open');
				mod._close($this);
			});

			$closeBtn.on('click', function () {
				$link.removeClass('state-active');
				$flyout.removeClass('state-open');
			});

			callback();
		},

		_close: function($keep) {
			var mod = this,
				$ctx = mod.$ctx;

			$ctx.find('.state-active').not($keep).toggleClass('state-active').next().toggleClass('state-open');
		}
	});
}(Tc.$));
