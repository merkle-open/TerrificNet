(function($) {
	"use strict";
	/**
	 * Search module implementation.
	 *
	 * @author Denis Skeledzic-Gemperli <denis.skeledzic@namics.com>
	 * @namespace Tc.Module
	 * @class Search
	 * @extends Tc.Module
	 */
	Tc.Module.Search = Tc.Module.extend({

		init: function($ctx, sandbox, modId) {
			this._super($ctx, sandbox, modId);
		},

		on: function(callback) {
			var mod = this,
				$ctx = mod.$ctx;

			function search (text) {
				window.location = $ctx.data('url') + "#query=" + text;
			}

			$('.js-search', $ctx).on('keyup', function(e){
				if(e.keyCode === 13 && $(this).val().length >= 3){
					search($(this).val());
				}
			});


			$ctx.on('click', '.btn-search', function() {
				search($ctx.find('.js-search').val());
			});

			callback();
		},

		after: function() {
			var mod = this,
				$ctx = mod.$ctx;
		}

	});
}(Tc.$));
