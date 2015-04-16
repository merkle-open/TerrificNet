(function ($) {
	Tc.Utils.Helper = {

		/**
		 * smooth-scrolling to an object
		 * @dependency jquery.scrollTo
		 *
		 * @param $obj {object} jQuery Object to scroll to
		 * @param duration {int}
		 * @param settings {object}
		 */
		scrollTo: function ($obj, duration, settings) {

			var options = {
				axis: 'y'
			};

			duration = duration || 500;
			settings = $.extend({}, options, settings);

			$.scrollTo($obj, duration, settings);
		},


		/**
		 * check if element is completly in viewport
		 *
		 * @param $obj
		 * @returns {boolean} is completly in viewport
		 */
		isElementInViewport: function ($obj) {
			var rect = $obj.get(0).getBoundingClientRect(),
				$window = $(window);

			return (
				rect.top >= 0 &&
				rect.left >= 0 &&
				rect.bottom <= $window.height() &&
				rect.right <=  $window.width()
			);
		},

		/**
		 * gets the .l-page top-margin as integer
		 *
		 * @returns {Number}
		 */
		getBodyOffsetTop: function () {
			var y = $('.js-l-page').css('margin-top');
			return !isNaN(parseInt(y, 10)) ? parseInt(y, 10) : 0;
		}
	};
})(Tc.$);
