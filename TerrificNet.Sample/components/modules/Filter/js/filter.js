(function($) {
	"use strict";
	/**
	 * Filter module implementation.
	 *
	 * @author Denis Skeledzic-Gemperli <denis.skeledzic@namics.com>
	 * @namespace Tc.Module
	 * @class Filter
	 * @extends Tc.Module
	 */
	Tc.Module.Filter = Tc.Module.extend({

		init: function($ctx, sandbox, modId) {
			this._super($ctx, sandbox, modId);
		},

		on: function(callback) {
			var mod = this,
				$ctx = mod.$ctx,
				$filterMobBtn = $ctx.find('.js-filter-mobile-btn'),
				$filterGroup = $ctx.find('.js-filter-group');

			$filterMobBtn.on('click', function () {

				var $this = $(this);
				$this.toggleClass('state-active');
				$filterGroup.toggleClass('state-open');
			});;



			callback();
		},

		after: function() {
			var mod = this,
				$ctx = mod.$ctx;
		}

	});
}(Tc.$));
