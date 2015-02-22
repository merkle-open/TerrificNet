/// <reference path="../../../../scripts/typings/jquery/jquery.d.ts" />

module Tc.Sample {
	export class TestModule {
		public reload() : JQueryPromise<Object> {
			return jQuery.get("/model/components/modules/TestModule").then(function(data) {
				data.test = new Date();
				return data;
			});
		}
	}
}