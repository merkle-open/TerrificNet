/// <reference path="../../../../scripts/typings/jquery/jquery.d.ts" />
var Tc;
(function (Tc) {
    var Sample;
    (function (Sample) {
        var TestModule = (function () {
            function TestModule() {
            }
            TestModule.prototype.reload = function () {
                return jQuery.get("/model/components/modules/TestModule").then(function (data) {
                    data.test = new Date();
                    return data;
                });
            };
            return TestModule;
        })();
        Sample.TestModule = TestModule;
    })(Sample = Tc.Sample || (Tc.Sample = {}));
})(Tc || (Tc = {}));
//# sourceMappingURL=TestModule.js.map