(function ($) {
	"use strict";
	/**
	 * NavSidebar module implementation.
	 *
	 * @author Denis Skeledzic-Gemperli <denis.skeledzic@namics.com>
	 * @namespace Tc.Module
	 * @class NavSidebar
	 * @extends Tc.Module
	 */
	Tc.Module.NavSidebar = Tc.Module.extend({

		init: function ($ctx, sandbox, modId) {
			this._super($ctx, sandbox, modId);
		},

		on: function (callback) {
			var mod = this,
				$ctx = mod.$ctx,
				$title = $ctx.find('.js-nav-sidebar-mobile-title'),
				$targetList = $ctx.find('.js-state-selected');

			enquire.register('only screen and (max-width : 768px)', function () {

				$title.each(function () {
					if ($targetList.length) {
						if ($targetList.attr('class').split(' ')[0] === 'nav-sidebar-list-level-5') {

							// console.log(true);
							$title.css('display', 'none');
						} else {
							$title.css('display', 'block');
						}
					}
				});

			}, false);



			callback();
		}
	});
}(Tc.$));
