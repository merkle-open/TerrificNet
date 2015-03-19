$(document).ready(function () {
    //prototype extensions
    if (!Array.prototype.get) {
        Array.prototype.get = function (predicate) {
            var found = null;
            this.forEach(function (e) {
                if (!found && predicate(e)) {
                    found = e;
                }
            });
            return found;
        };
    }

    if (!Array.prototype.forEach) {
        Array.prototype.forEach = function (func) {
            for (var x = 0; x < this.length; x++)
                func(this[x], x);
        };
    }

    //class definitions
    function JsonDom(json) {
        var domObject = JSON.parse(json);
        var placeholders = [];

        this.removeElementFromPlaceholder = function (plhId, elementId, index) {
            var plh = placeholders.get(function (e) {
                return e.name === plhId;
            });
            if(plh === null || index === -1 || !plh.elementExists(elementId, index)) throw new Error("Element with id " + elementId + " @ index " + index + " does not exist!");
            plh.removeElement(index);

            console.log(domObject);
        };

        function readPlaceholders(parent, parentPlh) {
            if (parent._placeholder) {
                for (var plh in parent._placeholder) {
                    if (parent._placeholder.hasOwnProperty(plh)) {
                        placeholders.push(new Placeholder(parentPlh + plh, parent._placeholder[plh]));
                        parent._placeholder[plh].forEach(function (e) {
                            readPlaceholders(e, parentPlh + plh + '/');
                        });
                    }
                }
            }
        }

        function init() {
            readPlaceholders(domObject, '');
        }

        init();
    }

    function Placeholder(name, plhElements) {
        var elements = plhElements;
        this.name = name;
        this.els = elements;

        this.elementExists = function(elementId, idx){
            var el = elements[idx];
            return (el.module && el.module === elementId) || (el.template && el.template === elementId);
        };

        this.removeElement = function(idx){
            elements.splice(idx, 1);
        };
    }

    function Module($el) {
        this.id = $el.data('id');
        this.html = $el.find('.js-module-definition').html();
    }

    function Template($el) {

    }


    //variables
    var $editor = $('.page-editor'),
        jsonDom = new JsonDom($('#siteDefinition', $editor).html()),
        modules = [];


    //functions


    //event binders
    $('.sidebar .module', $editor).on('dragstart', function (e) {
        e.originalEvent.dataTransfer.setData('id', $(this).data('id'));
    });

    $editor
        .on('dragover', '.plh', function (e) {
            e.preventDefault();
            return false;
        })
        .on('dragenter', '.plh', function (e) {
            e.preventDefault();
            $(this).addClass('drag-over');
        })
        .on('dragleave', '.plh', function () {
            $(this).removeClass('drag-over');
            return false;
        })
        .on('drop', '.plh', function (e) {
            console.log(e.originalEvent.dataTransfer.getData('id'))
            $(this).removeClass('drag-over');
        });

    $('.plh .btn-delete', $editor).click(function () {
        var $this = $(this),
            $modStart = $this.parent(),
            plhId = $modStart.data('plh-id'),
            guid = $modStart.data('index'),
            endFinder = '',
            elementId = '';

        if ($modStart.hasClass('module')) {
            endFinder = '.plh.module.end';
            elementId = $modStart.data('module-id');
        } else {
            endFinder = '.plh.template.end';
            elementId = $modStart.data('template-id');
        }

        if (!$modStart.nextAll(endFinder).length) {
            throw new Error("no module / template end found.");
        }
        var $modEnd = $modStart.nextAll(endFinder).first();
        var $between = $modStart.nextUntil($modEnd);

        var idx = -1;
        $('.plh.start[id="plh_' + plhId + '"]').nextUntil('.plh.end[id="plh_' + plhId + '"]', '[data-plh-id="' + plhId + '"]').each(function (k, v) {
            if ($(v).data('index') === guid) {
                idx = k;
            }
        });
        if (idx === -1) throw new Error("Module / Template index could not be found!");

        jsonDom.removeElementFromPlaceholder(plhId, elementId, idx);

        $this.tooltip('hide');
        [$modStart, $between, $modEnd].forEach(function (e) {
            e.remove();
        });
    });

    $editor.on('click', 'a', function () {
        //prevent links from firing
        return false;
    });

    //initialize tooltips
    $('[data-toggle="tooltip"]', $editor).tooltip({
        container: '.page-editor'
    });

    //init.
    $('.sidebar .module', $editor).each(function (k, v) {
        modules.push(new Module($(v)));
    });
});