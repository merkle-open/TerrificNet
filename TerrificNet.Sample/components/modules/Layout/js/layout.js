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
				$ctx = mod.$ctx;



			callback();
		},

		after: function() {
			var mod = this,
				$ctx = mod.$ctx;


		}

	});
}(Tc.$));
