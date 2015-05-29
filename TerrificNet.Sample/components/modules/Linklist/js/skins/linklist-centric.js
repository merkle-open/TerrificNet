(function($) {
	"use strict";
	/**
	 * centric skin implementation for the Linklist module.
	 *
	 * @namespace Tc.Module.{{pattern-js}}
	 * @class Centric
	 * @extends Tc.Module
	 */
	Tc.Module.Linklist.Centric = function(parent) {

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
