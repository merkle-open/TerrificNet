(function ($) {
	"use strict";
	/**
	 * NavMain module implementation.
	 *
	 * @author Denis Skeledzic-Gemperli <denis.skeledzic@namics.com>
	 * @namespace Tc.Module
	 * @class NavMain
	 * @extends Tc.Module
	 */
	Tc.Module.NavMain = Tc.Module.extend({

		init: function ($ctx, sandbox, modId) {
			this._super($ctx, sandbox, modId);
		},

		on: function (callback) {
			var mod = this,
				$ctx = mod.$ctx,
				$nav = $ctx.find('.js-nav-main-list'),
				$item = $ctx.find('.js-main-list-item'),
				$link = $ctx.find('.js-main-list-link'),
				$menu = $ctx.find('.js-main-flyout'),

				delay = 300,
				timer = null;

			// Desktop
			enquire.register('only screen and (min-width : 768px)', function () {

				$item.on('mouseenter', function () {

					var $this = $(this);

					//if (timer)
					//	clearTimeout(timer);
					//timer = setTimeout(function () {

						$this.find($link).addClass('state-hover');
						$this.find($menu).addClass('state-open');

					//}, delay);
				})
				.on('mouseleave click', function () {

					// clearTimeout(timer);
					var $this = $(this);

					$this.find($link).removeClass('state-hover');
					$this.find($menu).removeClass('state-open');
				});

				$item.keyup(function(e) {

					var $this = $(this);

					if (e.keyCode == 13) {

						// console.log($this);

						$this.find($link).addClass('state-hover');
						$this.find($menu).addClass('state-open');
					}

					if (e.keyCode == 27) {

						// console.log($this);

						$this.find($link).removeClass('state-hover');
						$this.find($menu).removeClass('state-open');
					}
				});

			}, false);

			// Mobile
			enquire.register('only screen and (max-width : 767px)', function () {

				$link.on('click', function (e) {

					// console.log('click on nav in mobile')
					var $this = $(this);

					e.stopPropagation();

					if ($this.hasClass('state-hover')) {
						$this.removeClass('state-hover');
						$this.next().removeClass('state-open');
					} else {
						$link.not($this).removeClass('state-hover');
						$this.toggleClass('state-hover');

						$menu.not($this).removeClass('state-open');
						$this.next().toggleClass('state-open');
					}

					$('html,body').animate({scrollTop: $(this).offset().top}, 300);
				});


			}, false);

			callback();
		}
	});
}(Tc.$));
