# TerrificNet
===========
## Definitions
### Module
* One model
* Multiple skins
* Mutliple data variations
### Skin
Default Skin
### Template
Handlebar Syntax
Helpers

```handlebars
<p>{{hallo}}</p>
{{module template="templatepath"}}
{{placeholder key="phContent"}}
{{label "person/male"}}
```

### Datavariation
Default Datavariation
### Model
### Schema
## Roadmap
* Skins with shared model schemas
* Datavariation for modules
	* Placeholder definition
	* Queryparameter support (languages, etc.)
* Backend for placeholder configuration
* Documentation
	* Handlebar reference (incl. helpers)
	* Example project (neutral -> Denis)
* Controller fallback provider for sitecore. When no controller is registred in sitecore the default terrific controller will be used.
* Creation for modules, skins, datavariations (action)
* Improved error handling
	* Template failures (missing variables)
	* Datasource
* Backend for label service
* Helper refactoring
* Update for console host
* Fake model generator (eg. Lorem ipsum text)
* Shared model schemas, eg. use of modules
	* $ref parameter
* Frontify integration
