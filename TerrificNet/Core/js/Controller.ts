module Tcn {
	export class ControllerActivator {
		activate(name: string): Object {
			var ctor = eval(name);
			return new ctor();
		}
	}

	export class ControllerInjector {
		constructor(private activator: ControllerActivator) {
		}

		public lookup(element: JQuery): void {
			element.find("[data-tc-controller]").each((idx, elem) => this.register(elem));
			element.find("[data-tc-action]").each((idx, elem) => this.registerAction(elem));
		}

		private registerAction(element: Element) {
			var $elem = $(element);
			var action: string = $elem.data("tc-action");
			$elem.click(() => {
				var $container = $elem.parent("[data-tc-controller]");
				var controller = $container.data("tc-controller-inst");
				var result = controller[action]();

				ViewEngine.loadAndRenderAsync($container.data("tc-templateid"), result).then(html => {
					$container.html(html);
					this.lookup($container);
				});
			});
		}

		private register(element: Element) {
			var name: string = $(element).data("tc-controller");
			var controller = this.activator.activate(name);

			$(element).data("tc-controller-inst", controller);
		}
	}
} 