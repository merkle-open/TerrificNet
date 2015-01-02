(function ($) {
	"use strict";
	/**
	 * Stage module implementation.
	 *
	 * @author Denis Skeledzic-Gemperli <denis.skeledzic@namics.com>
	 * @namespace Tc.Module
	 * @class Stage
	 * @extends Tc.Module
	 */
	Tc.Module.Stage = Tc.Module.extend({

		init: function ($ctx, sandbox, modId) {
			this._super($ctx, sandbox, modId);
		},

		on: function (callback) {
			var mod = this,
				$ctx = mod.$ctx,
				$stage = $ctx.find('.js-stage').height(),
				$stageClaimTitleHeight = $ctx.find('.js-stage-claim-title').height();

				// console.log('$stageClaimTitleHeight : ' + $stageClaimTitleHeight);

				// $stage.css({'height': (($stageClaimTitleHeight) + 80)+'px'});
				// console.log($stage);

				// bgImages = $ctx.find('.js-stage-background-container img').length,
				// randomNumber = Math.floor(Math.random() * (bgImages) + 1);

			//console.log('bgImages : ' + bgImages);
			//console.log('randomNumber : ' + randomNumber);

			//$(".js-stage-background-container :nth-child(" +randomNumber+ ")").css('display', 'block');

			callback();
		}
	});
}(Tc.$));
