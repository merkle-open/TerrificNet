var Tcn;
(function (Tcn) {
    var ControllerActivator = (function () {
        function ControllerActivator() {
        }
        ControllerActivator.prototype.activate = function (name) {
            var ctor = eval(name);
            return new ctor();
        };
        return ControllerActivator;
    })();
    Tcn.ControllerActivator = ControllerActivator;
    var ControllerInjector = (function () {
        function ControllerInjector(activator) {
            this.activator = activator;
        }
        ControllerInjector.prototype.lookup = function (element) {
            var _this = this;
            element.find("[data-tc-controller]").each(function (idx, elem) { return _this.register(elem); });
            element.find("[data-tc-action]").each(function (idx, elem) { return _this.registerAction(elem); });
        };
        ControllerInjector.prototype.registerAction = function (element) {
            var _this = this;
            var $elem = $(element);
            var action = $elem.data("tc-action");
            $elem.click(function () {
                var $container = $elem.parent("[data-tc-controller]");
                var controller = $container.data("tc-controller-inst");
                var result = controller[action]();
                Tcn.ViewEngine.loadAndRenderAsync($container.data("tc-templateid"), result).then(function (html) {
                    $container.html(html);
                    _this.lookup($container);
                });
            });
        };
        ControllerInjector.prototype.register = function (element) {
            var name = $(element).data("tc-controller");
            var controller = this.activator.activate(name);
            $(element).data("tc-controller-inst", controller);
        };
        return ControllerInjector;
    })();
    Tcn.ControllerInjector = ControllerInjector;
})(Tcn || (Tcn = {}));
//# sourceMappingURL=Controller.js.map