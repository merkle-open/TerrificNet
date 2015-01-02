(function($) {
	"use strict";
	/**
	 * Layout module implementation.
	 *
	 * @author Denis Skeledzic-Gemperli <denis.skeledzic@namics.com>
	 * @namespace Tc.Module
	 * @class Layout
	 * @extends Tc.Module
	 */
	Tc.Module.Layout = Tc.Module.extend({

		init: function($ctx, sandbox, modId) {
			this._super($ctx, sandbox, modId);
		},

		on: function(callback) {
			var mod = this,
				$ctx = mod.$ctx,
				$navMobBtn = $ctx.find('.js-btn-navmain-mobile'),

				$targets = $ctx.find( '[rel~=tooltip]' ),
				$target	= false,
				$tooltip = false,
				$tip = false;

			// -----------------------------------------
			// Mobile Navigaiton Button
			// -----------------------------------------
			$navMobBtn.on('click', function (e) {

				var $this = $(this);
				$this.toggleClass('state-active');
				$('.js-l-nav').toggleClass('state-open');
				$('.js-l-service').toggleClass('state-open');
			});

			// -----------------------------------------
			// Conditionally change meta viewport tag
			// -----------------------------------------
			var viewport = $('meta[name="viewport"]');

			if (screen.width >= 768) {
				viewport.attr("content", "width=1024, user-scalable=no");
			} else {
				viewport.attr("content", "width=device-width, initial-scale=1.0, maximum-scale=1.0");
			}

			// Change the viewport value based on screen.width
			var viewport_set = function() {

				if (screen.width >= 768) {
					viewport.attr("content", "width=1024, user-scalable=no");
				} else{
					viewport.attr("content", "width=device-width, initial-scale=1.0, maximum-scale=1.0");
				}
			};

			// Set the correct viewport value on page load
			viewport_set();

			// Set the correct viewport after device orientation change or resize
			window.onresize = function() {
				viewport_set();
			};

			// -----------------------------------------
			// Tooltip
			// -----------------------------------------
			$targets.bind( 'mouseenter', function()
			{
				var $this = $(this);
					$target	 = $this;
					$tip	 = $target.attr('title');
					$tooltip = $( '<div class="e-tip tooltip"></div>' );

				if( !$tip || $tip == '' )
					return false;

				$target.removeAttr( 'title' );
				$tooltip.css( 'opacity', 1 )
						.html( $tip )
						.appendTo( 'body' );

				var init_tooltip = function()
				{
					if ( $( window ).width() < $tooltip.outerWidth() * 1.5 )
						$tooltip.css( 'max-width', $( window ).width() / 2 );
					else
						$tooltip.css( 'max-width', 340 );

					var pos_left = $target.offset().left + ( $target.outerWidth() / 2 ) - ( $tooltip.outerWidth() / 2 ),
						pos_top	 = $target.offset().top - $tooltip.outerHeight() - 20;

					if ( pos_left < 0 )
					{
						pos_left = $target.offset().left + $target.outerWidth() / 2 - 20;
						$tooltip.addClass( 'left' );
					} else {
						$tooltip.removeClass('left');
					}

					if ( pos_left + $tooltip.outerWidth() > $( window ).width() )
					{
						pos_left = $target.offset().left - $tooltip.outerWidth() + $target.outerWidth() / 2 + 20;
						$tooltip.addClass( 'right' );
					} else {
						$tooltip.removeClass( 'right' );
					}

					if ( pos_top < 0 )
					{
						var pos_top	 = $target.offset().top + $target.outerHeight();
						$tooltip.addClass( 'top' );
					} else {
						$tooltip.removeClass('top');
					}

					$tooltip.css( { left: pos_left, top: pos_top } )
							.animate( { top: '+=10', opacity: 1 }, 50 );
				};

				init_tooltip();
				$( window ).resize( init_tooltip );

				var remove_tooltip = function()
				{
					$tooltip.animate( { top: '-=10', opacity: 0 }, 50, function()
					{
						$( this ).remove();
					});

					$target.attr( 'title', $tip );
				};

				$target.bind( 'mouseleave', remove_tooltip );
				$tooltip.bind( 'click', remove_tooltip );
			});

			//document.addEventListener("touchstart", function() {},false);

			callback();
		},

		after: function() {
			var mod = this,
				$ctx = mod.$ctx;
		}

	});
}(Tc.$));
