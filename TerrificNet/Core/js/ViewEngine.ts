/// <reference path="../../scripts/typings/jquery/jquery.d.ts" />

module Tcn {

    export interface ITemplateRepository {
		getAsync(id:string):JQueryPromise<IView>;
		register(id:string, view:IView):void;
    }

    export interface IView {
        render(model: Object): string;
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
            return promise.then(view => view.render(model));
        }

        public loadAndRenderAsync(templateId: string, modelPromise: JQueryPromise<Object>): JQueryPromise<string> {
            var templatePromise = this.templateRepository.getAsync(templateId);
            return jQuery.when(templatePromise, modelPromise)
				.then((view: IView, model: Object) => view.render(model));
        }
    }

	export var TemplateRepository: ITemplateRepository = new TemplateRepositoryInternal("/js/{id}?jsRepository=Tcn.TemplateRepository");
	export var ViewEngine: ViewEngineImplementation = new ViewEngineImplementation(TemplateRepository);

}