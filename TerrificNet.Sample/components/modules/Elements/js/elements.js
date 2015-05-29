(function ($) {
	"use strict";
	/**
	 * Elements module implementation.
	 *
	 * @author Denis Skeledzic-Gemperli <denis.skeledzic@namics.com>
	 * @namespace Tc.Module
	 * @class Elements
	 * @extends Tc.Module
	 */
	Tc.Module.Elements = Tc.Module.extend({

		init: function ($ctx, sandbox, modId) {
			this._super($ctx, sandbox, modId);
		},

		on: function (callback) {
			var mod = this,
				$ctx = mod.$ctx;

			fluidvids.init({
				selector: ['iframe'],
				players: ['www.youtube.com', 'player.vimeo.com']
			});


			callback();
		},

		after: function () {
			var mod = this,
				$ctx = mod.$ctx;


		}

	});
}(Tc.$));
