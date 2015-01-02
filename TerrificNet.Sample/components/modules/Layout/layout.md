// -----------------------------------------
// Layout
// -----------------------------------------

## Maincontainer

.mod-layout {

	//-- page --//
	.l-page {

		.l-head {
			.l-wrap {
				.l-service {}
				.l-logo    {}
				.l-nav     {}
			}
		}

		.l-body {
			.l-background { -> Stage-Module }

			.l-content-wrapper {
				.l-content {

					&.l-sub { // sub-pages prefix

					// l-left--right
					.l-main       {}
					.l-sidebar    {}
					================
					// l-right--left
					.l-sidebar    {}
					.l-main       {}

					}
				}
			}
		}

		.l-footer-wrapper {
			.l-footer .l-wrap {
				.l-footer-top     {}
				.l-footer-list    {}
				.l-footer-bottom  {}
			}
		}
	}
}
