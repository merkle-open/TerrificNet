/// <reference path="../../scripts/typings/jquery/jquery.d.ts" />

module Tcn {

    export interface ITemplateRepository {
		getAsync(id:string):JQueryPromise<IView>;
		register(id:string, view:IView):void;
    }

    export interface IView {
        render(context: IRenderingContext, model: Object): string;
    }

	export interface IRenderingContext {
		write(val: string): void;
		writeEscape(val: string): void;
	}

	class TemplateRepositoryInternal implements ITemplateRepository
	{
		private views: { [id: string]: IView } = {};

		constructor(private url: string) {
		}

		public getAsync(id: string): JQueryPromise<IView> {
			if (this.views[id])
				return jQuery.Deferred<IView>().resolve(this.views[id]);

			return jQuery.getScript(this.url.replace("{id}", id))
				.then(() => this.views[id]);
		}

		public register(id: string, view: IView): void {
			this.views[id] = view;
		}
	}

    export class ViewEngineImplementation {
        private promises: { [id: string]: JQueryPromise<IView> };

        constructor(private templateRepository: ITemplateRepository) {
        }

        public renderAsync(templateId: string, model: Object): JQueryPromise<string> {
            var promise = this.templateRepository.getAsync(templateId);
            return promise.then(view => this.render(view, model));
        }

        public loadAndRenderAsync(templateId: string, modelPromise: JQueryPromise<Object>): JQueryPromise<string> {
            var templatePromise = this.templateRepository.getAsync(templateId);
            return jQuery.when(templatePromise, modelPromise)
				.then(this.render);
        }

		private render(view: IView, model: Object): string {
			var ctx = new StringRenderingContext();
			view.render(ctx, model);

			return ctx.out;
		}
    }

	export class StringRenderingContext implements IRenderingContext {
		public out:string = "";

		public write(val: string): void {
			this.out += val;
		}

		public writeEscape(val: string): void {
			this.out += Utils.escapeExpression(val);
		}
	}

	class Utils {
		static escapeMap : { [val:string]: string } = {
			"&": "&amp;",
			"<": "&lt;",
			">": "&gt;",
			'"': "&quot;",
			"'": "&#x27;",
			"`": "&#x60;"
		};

		static badChars: RegExp = /[&<>"'`]/g;
		static possible: RegExp = /[&<>"'`]/;

		private static escapeChar(chr:string): string {
			return Utils.escapeMap[chr];
		}

		public static escapeExpression(val:string): string {
			if (val == null) {
				return "";
			} else if (!val) {
				return val + '';
			}

			val = "" + val;

			if (!this.possible.test(val)) { return val; }
			return val.replace(this.badChars, this.escapeChar);
		}
	}

	export var TemplateRepository: ITemplateRepository = new TemplateRepositoryInternal("/js/{id}");
	export var ViewEngine: ViewEngineImplementation = new ViewEngineImplementation(TemplateRepository);

}