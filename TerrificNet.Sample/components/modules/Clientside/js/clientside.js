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
				
			var render = function(){
				var renderData = {
					'title': 'Clientside ' + new Date(),
					'input': $ctx.find('input').val(),
					'title2':'additional client side data'
				};
				
				Tcn.ViewEngine.loadAndRenderAsync(mod.dataTemplate, renderData).then(function (html) {
					var dd = new diffDOM();
					var newElement = document.createElement('div');
					newElement.innerHTML = html;
					var newDom = newElement.children[0]; 
					
					dd.apply($ctx[0], dd.diff($ctx[0], newDom));
					
//					$ctx.html(html);
//					mod.bind();
				});
			};
			
			$ctx.find('input').on('keyup', function(){
				render();
			});
			
			$ctx.on('click', function(){
				render();
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
