(function($) {
	"use strict";
	/**
	 * News skin implementation for the Teaser module.
	 *
	 * @author Denis Skeledzic-Gemperli <denis.skeledzic@namics.com>
	 * @namespace Tc.Module.{{pattern-js}}
	 * @class News
	 * @extends Tc.Module
	 */
	Tc.Module.Teaser.News = function(parent) {

		this.on = function(callback) {
			var mod = this,
				$ctx = mod.$ctx;

			parent.on(callback);
		};

		this.after = function() {
			var mod = this,
				$ctx = mod.$ctx;


			parent.after();
		};

	};
}(Tc.$));
