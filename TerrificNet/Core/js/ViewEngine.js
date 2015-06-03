/// <reference path="../../scripts/typings/jquery/jquery.d.ts" />
var Tcn;
(function (Tcn) {
    var TemplateRepositoryInternal = (function () {
        function TemplateRepositoryInternal(url) {
            this.url = url;
            this.views = {};
        }
        TemplateRepositoryInternal.prototype.getAsync = function (id) {
            var _this = this;
            if (this.views[id])
                return jQuery.Deferred().resolve(this.views[id]);
            return jQuery.getScript(this.url.replace("{id}", id))
                .then(function () { return _this.views[id]; });
        };
        TemplateRepositoryInternal.prototype.register = function (id, view) {
            this.views[id] = view;
        };
        return TemplateRepositoryInternal;
    })();
    var ViewEngineImplementation = (function () {
        function ViewEngineImplementation(templateRepository) {
            this.templateRepository = templateRepository;
        }
        ViewEngineImplementation.prototype.renderAsync = function (templateId, model) {
            var _this = this;
            var promise = this.templateRepository.getAsync(templateId);
            return promise.then(function (view) { return _this.render(view, model); });
        };
        ViewEngineImplementation.prototype.loadAndRenderAsync = function (templateId, modelPromise) {
            var templatePromise = this.templateRepository.getAsync(templateId);
            return jQuery.when(templatePromise, modelPromise)
                .then(this.render);
        };
        ViewEngineImplementation.prototype.render = function (view, model) {
            var ctx = new StringRenderingContext();
            view.render(ctx, model);
            return ctx.out;
        };
        return ViewEngineImplementation;
    })();
    Tcn.ViewEngineImplementation = ViewEngineImplementation;
    var StringRenderingContext = (function () {
        function StringRenderingContext() {
            this.out = "";
        }
        StringRenderingContext.prototype.write = function (val) {
            this.out += val;
        };
        StringRenderingContext.prototype.writeEscape = function (val) {
            this.out += Utils.escapeExpression(val);
        };
        return StringRenderingContext;
    })();
    Tcn.StringRenderingContext = StringRenderingContext;
    var Utils = (function () {
        function Utils() {
        }
        Utils.escapeChar = function (chr) {
            return Utils.escapeMap[chr];
        };
        Utils.escapeExpression = function (val) {
            if (val == null) {
                return "";
            }
            else if (!val) {
                return val + '';
            }
            val = "" + val;
            if (!this.possible.test(val)) {
                return val;
            }
            return val.replace(this.badChars, this.escapeChar);
        };
        Utils.escapeMap = {
            "&": "&amp;",
            "<": "&lt;",
            ">": "&gt;",
            '"': "&quot;",
            "'": "&#x27;",
            "`": "&#x60;"
        };
        Utils.badChars = /[&<>"'`]/g;
        Utils.possible = /[&<>"'`]/;
        return Utils;
    })();
    Tcn.TemplateRepository = new TemplateRepositoryInternal("/js/{id}");
    Tcn.ViewEngine = new ViewEngineImplementation(Tcn.TemplateRepository);
})(Tcn || (Tcn = {}));
//# sourceMappingURL=ViewEngine.js.map