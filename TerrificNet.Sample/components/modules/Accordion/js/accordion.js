(function ($) {
	"use strict";
	/**
	 * Accordion module implementation.
	 *
	 * @author
	 * @namespace Tc.Module
	 * @class Accordion
	 * @extends Tc.Module
	 */
	Tc.Module.Accordion = Tc.Module.extend({

		on: function (callback) {
			var mod = this,
				$ctx = mod.$ctx;

			$ctx.on('click', '.js-accordion-toggle', function (e) {
				e.preventDefault();
				var $this = $(this),
					doClose = $this.hasClass('state-open');

				$ctx.find('.state-open').not($this).toggleClass('state-open').next().slideToggle(300);
				$this.toggleClass('state-open').next().slideToggle(400, function () {
					if (!doClose && !Tc.Utils.Helper.isElementInViewport($this)) {
						var offsetTop = Tc.Utils.Helper.getBodyOffsetTop() + 5;
						Tc.Utils.Helper.scrollTo($this, 400, {offset: -offsetTop});
					}
				});
			});

			callback();
		}

	});
}(Tc.$));
