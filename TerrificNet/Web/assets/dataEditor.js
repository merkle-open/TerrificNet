$(document).ready(function () {
	// Set the default CSS theme and icon library globally
	JSONEditor.defaults.theme = 'bootstrap3';
	JSONEditor.defaults.iconlib = 'fontawesome4';

	var partials = window.partials;
	var editor = null;

	JSONEditor.defaults.editors.templateSelect = JSONEditor.defaults.editors.select.extend({
		preBuild: function () {
			this.enum_options = partials;
			this.enum_display = partials;
			this.enum_values = partials;
		},
		setValue: function (value, initial) {
			this._super(value, initial);
		},
		onInputChange: function () {
			this._super();

			var that = this;

			$.get("/schema/" + this.value, function (data) {
				var value = editor.getValue();
				data.properties["template"] = { "type": "string", "format": "template" };
				that.parent.schema = data;
				var topEditor = that;
				while (topEditor.parent) {
					topEditor = topEditor.parent;
				}
				var schema = topEditor.schema;

				editor.destroy();

				editor = new JSONEditor(document.getElementById('editor_holder'), {
					schema: schema
				});
				editor.setValue(value);

				//that.init();
				//that.parent.build();
			});
		}
	});

	JSONEditor.defaults.resolvers.unshift(function (schema) {
		if (schema.type === "string" && schema.format === "template") {
			return "templateSelect";
		}
	});


	var schemaUrl = window.schemaUrl;
	var dataUrl = window.dataUrl;

	$.get(schemaUrl, function (data) {
		editor = new JSONEditor(document.getElementById('editor_holder'), {
			schema: data
		});

		$.get(dataUrl, function (model) {
			editor.setValue(model);
		});
	});

	// Hook up the submit button to log to the console
	$('#' + window.saveactionid).click(function () {
		// Get the value from the editor
		console.log(editor.getValue());

		if (editor.validate()) {
			$.ajax({
				type: 'PUT',
				url: dataUrl,
				data: JSON.stringify(editor.getValue()),
				headers: { "Content-Type": "application/json" },
			});
		}

	});

});