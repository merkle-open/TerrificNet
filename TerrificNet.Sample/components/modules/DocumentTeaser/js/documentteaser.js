(function($) {
	"use strict";
	/**
	 * DocumentTeaser module implementation.
	 *
	 * @author Denis Skeledzic-Gemperli <denis.skeledzic@namics.com>
	 * @namespace Tc.Module
	 * @class DocumentTeaser
	 * @extends Tc.Module
	 */

	Tc.Module.DocumentTeaser = Tc.Module.extend({

		init: function($ctx, sandbox, modId) {
			this._super($ctx, sandbox, modId);
		},

		on: function(callback) {

			var mod = this,
				$ctx = mod.$ctx,
				$navIcon = $ctx.find('.js-document-teaser-nav-mobile'),
				$teaserList = $ctx.find('.js-document-teaser-box-info'),
				$teaserText = $ctx.find('.js-document-teaser-box-text'),
				$teaserIconNotification = $ctx.find('.js-icon-notification'),
				$teaserIconDelete = $ctx.find('.js-icon-delete');

			$navIcon.on('click', function (e) {

				var $this = $(this);

				e.stopPropagation();

				$this.toggleClass('state-active');
				$this.parent().find($teaserList).toggleClass('state-open');
				$this.parent().find($teaserText).toggleClass('state-open');
				$this.parent().find($teaserIconNotification).toggleClass('state-open'),
				$this.parent().find($teaserIconDelete).toggleClass('state-open');
			});

			callback();
		}

	});
}(Tc.$));
