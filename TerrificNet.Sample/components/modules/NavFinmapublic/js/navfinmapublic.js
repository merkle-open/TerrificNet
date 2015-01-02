(function($) {
	"use strict";
	/**
	 * NavFinmapublic module implementation.
	 *
	 * @author Denis Skeledzic-Gemperli <denis.skeledzic@namics.com>
	 * @namespace Tc.Module
	 * @class NavFinmapublic
	 * @extends Tc.Module
	 */
	Tc.Module.NavFinmapublic = Tc.Module.extend({

		init: function($ctx, sandbox, modId) {
			this._super($ctx, sandbox, modId);
		},

		on: function(callback) {
			var mod = this,
				$ctx = mod.$ctx,
				$finmaMobBtn = $ctx.find('.js-nav-finmapublic-m-btn'),
				$finmaFlyout = $ctx.find('.js-finmapublic-flyout');

			$finmaMobBtn.on('click', function () {
				var $this = $(this);
				$this.toggleClass('state-active');
				$finmaFlyout.toggleClass('state-open');
			});

			callback();
		}

	});
}(Tc.$));
