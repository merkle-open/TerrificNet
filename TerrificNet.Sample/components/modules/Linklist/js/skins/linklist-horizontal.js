(function ($) {
	"use strict";
	/**
	 * Horizontal skin implementation for the Linklist module.
	 *
	 * @namespace Tc.Module.{{pattern-js}}
	 * @class Horizontal
	 * @extends Tc.Module
	 */
	Tc.Module.Linklist.Horizontal = function (parent) {

		this.on = function (callback) {
			var mod = this,
				$ctx = mod.$ctx,
				$titel = $ctx.find('.js-title'),
				$list = $ctx.find('.js-linklist');

			enquire.register("(max-width: 659px)", {

				match: function () {

					$list.hide();

					$titel.on('click', function () {

						var $this = $(this);

						$this.toggleClass('state-active');
						$list.toggle(500);
					})
				},

				unmatch: function () {
					$list.show();
				}
			});

			parent.on(callback);
		};
	};
}(Tc.$));
