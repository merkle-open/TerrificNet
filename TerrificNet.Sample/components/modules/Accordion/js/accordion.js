(function ($) {
	"use strict";
	/**
	 * Accordion module implementation.
	 *
	 * @author Denis Skeledzic-Gemperli <denis.skeledzic@namics.com>
	 * @namespace Tc.Module
	 * @class Accordion
	 * @extends Tc.Module
	 */
	Tc.Module.Accordion = Tc.Module.extend({

		init: function ($ctx, sandbox, modId) {
			this._super($ctx, sandbox, modId);
		},

		on: function (callback) {
			var mod = this,
				$ctx = mod.$ctx,
				$accordionPanel = $ctx.find('.js-accordion dt'),
				$accordionContainer = $ctx.find('.js-accordion dd'),
				animation = 200;


			// Handle click function
			$accordionContainer.hide();

			$accordionPanel.each(function() {
				if ($(this).hasClass('state-active')) {
					//this clicked panel
					var $this = $(this),
					$dl = $this.parents('dl');

					//console.log($this);

					$dl.find('dd').slideDown(animation).addClass('state-open');
				}
			});

			$accordionPanel.on('click', function () {

				//this clicked panel
				var $this = $(this),

					//the target panel content
					$target = $this.next();

				//Only toggle non-displayed
				if (!$this.hasClass('state-active')) {

					// console.log('no -> accordion-active class // click --> open');
					//add active class
					$this.addClass('state-active');

					//slide down target panel
					$target.slideDown(animation);
					$target.addClass('state-open');

				} else if ($this.hasClass('state-active')) {

					//slide up any open panels and remove active class
					$target.slideUp(animation);

					//remove any active class
					$this.removeClass('state-active');
				}

				return false;
			});

			callback();
		}
	});
}(Tc.$));
