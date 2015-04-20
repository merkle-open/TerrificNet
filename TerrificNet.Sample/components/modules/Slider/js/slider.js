(function($) {
	"use strict";
	/**
	 * Slider module implementation.
	 *
	 * @author Denis Skeledzic-Gemperli <denis.skeledzic@namics.com>
	 * @namespace Tc.Module
	 * @class Slider
	 * @extends Tc.Module
	 */
	Tc.Module.Slider = Tc.Module.extend({

		init: function($ctx, sandbox, modId) {
			this._super($ctx, sandbox, modId);



		},

		on: function(callback) {
			var mod = this,
				$ctx = mod.$ctx;



			callback();
		},

		after: function() {
			var mod = this,
				$ctx = mod.$ctx;


		}

	});
}(Tc.$));
