(function($) {
	"use strict";
	/**
	 * DownloadList module implementation.
	 *
	 * @author Denis Skeledzic-Gemperli <denis.skeledzic@namics.com>
	 * @namespace Tc.Module
	 * @class DownloadList
	 * @extends Tc.Module
	 */
	Tc.Module.DownloadList = Tc.Module.extend({

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
