(function($) {
	'use strict';
	/**
	 * Breadcrumb module implementation.
	 *
	 * @author Christoph Buehler <christoph.buehler@namics.com>
	 * @namespace Tc.Module
	 * @class Breadcrumb
	 * @extends Tc.Module
	 */
	Tc.Module.Breadcrumb = Tc.Module.extend({

		init: function($ctx, sandbox, modId) {
			this._super($ctx, sandbox, modId);

		},

		on: function(callback) {
			var mod = this,
				$ctx = mod.$ctx;

			/*
			$ctx.on('click', function() {
				Tcn.ViewEngine.loadAndRenderAsync($ctx.data("templateid"), {links: [{ name: "entry1" }, { name: "test" + new Date().toTimeString() }]}).then(function(data) {
					$ctx.html(data);
				});
			});
			*/

			callback();
		},

		after: function() {
			var mod = this,
				$ctx = mod.$ctx;


		}
	});
}(Tc.$));
