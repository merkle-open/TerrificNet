(function($) {
	"use strict";
	/**
	 * Carousel module implementation.
	 *
	 * @namespace Tc.Module
	 * @class Carousel
	 * @extends Tc.Module
	 */
	Tc.Module.Clientside = Tc.Module.extend({

		dataTemplate: null,

		init: function($ctx, sandbox, modId) {
			this._super($ctx, sandbox, modId);

			this.dataTemplate = $ctx.data('template');
		},

		bind: function() {
			var mod = this,
				$ctx = mod.$ctx;
			
			$ctx.on('click', function(){
				var renderData = {
					'title': 'Clientside ' + new Date(),
					'title2':'additional client side data'
				};
				
				Tcn.ViewEngine.loadAndRenderAsync(mod.dataTemplate, renderData).then(function (html) {
					$ctx.html(html);
					mod.bind();
				});

			});
		},

		on: function(callback) {
			var mod = this;

			mod.bind();

			callback();
		},

		after: function() {
			var mod = this,
				$ctx = mod.$ctx;
		}

	});
}(Tc.$));
