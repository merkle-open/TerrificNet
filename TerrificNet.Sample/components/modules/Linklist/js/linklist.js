(function($) {
	"use strict";
	/**
	 * Linklist module implementation.
	 *
	 * @namespace Tc.Module
	 * @class Linklist
	 * @extends Tc.Module
	 */
	Tc.Module.Linklist = Tc.Module.extend({

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
