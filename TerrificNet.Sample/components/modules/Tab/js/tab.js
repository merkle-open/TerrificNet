(function ($) {
	"use strict";
	/**
	 * Tab module implementation.
	 *
	 * @author Denis Skeledzic-Gemperli <denis.skeledzic@namics.com>
	 * @namespace Tc.Module
	 * @class Tab
	 * @extends Tc.Module
	 */
	Tc.Module.Tab = Tc.Module.extend({

		init: function ($ctx, sandbox, modId) {
			this._super($ctx, sandbox, modId);
		},

		on: function (callback) {
			var mod = this,
				$ctx = mod.$ctx,
				$tab = $ctx.find('.js-tab-list li'),
				$tabContent = $ctx.find('.js-tab-content');

			$tab.click(function () {
				var tab_id = $(this).attr('data-tab');

				$tab.removeClass('state-active');
				$tabContent.removeClass('state-active');

				$(this).addClass('state-active');
				$("#" + tab_id).addClass('state-active');
			});

			callback();
		}
	});
}(Tc.$));
