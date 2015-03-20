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
            if (plh === null || index === -1 || !plh.elementExists(elementId, index)) throw new Error("Element with id " + elementId + " @ index " + index + " does not exist!");
            plh.removeElement(index);
        };

        this.addElementToPlaceholder = function (plhId, element, index, before) {
            var plh = placeholders.get(function (e) {
                return e.name === plhId;
            });
            if (!plh || !element) throw new Error("Placeholder or Element not found!");
            plh.insertAt(element, before ? index : index + 1);
            init();
        };

        this.addElementToPlaceholderStart = function (plhId, element) {
            var plh = placeholders.get(function (e) {
                return e.name === plhId;
            });
            if (!plh || !element) throw new Error("Placeholder or Element not found!");
            plh.insertFirst(element);
            init();
        };

        this.addElementToPlaceholderEnd = function (plhId, element) {
            var plh = placeholders.get(function (e) {
                return e.name === plhId;
            });
            if (!plh || !element) throw new Error("Placeholder or Element not found!");
            plh.insertLast(element);
            init();
        };

        Object.defineProperty(this, 'dom', {
            get: function () {
                var tmp = JSON.parse(JSON.stringify(domObject));
                delete tmp.scripts;
                delete tmp.styles;
                delete tmp.root;
                return JSON.stringify(tmp);
            }
        });

        function readPlaceholders(parent, parentPlh) {
            if (parent._placeholder) {
                for (var plh in parent._placeholder) {
                    if (parent._placeholder.hasOwnProperty(plh)) {
                        var plhId = parentPlh;
                        if (parent.module) plhId += (/\/[^\/]+$/g).exec(parent.module)[0].substring(1) + '/';
                        if (parent.template && !parent.root) plhId += (/\/[^\/]+$/g).exec(parent.template)[0].substring(1) + '/';
                        plhId += plh;
                        placeholders.push(new Placeholder(plhId, parent._placeholder[plh]));
                        parent._placeholder[plh].forEach(function (e) {
                            readPlaceholders(e, plhId + '/');
                        });
                    }
                }
            }
        }

        function init() {
            domObject.root = true;
            placeholders = [];
            readPlaceholders(domObject, '');
        }

        init();
    }

    function Placeholder(name, plhElements) {
        var elements = plhElements;
        this.name = name;

        this.elementExists = function (elementId, idx) {
            var el = elements[idx];
            return (el.module && el.module === elementId) || (el.template && el.template === elementId);
        };

        this.removeElement = function (idx) {
            elements.splice(idx, 1);
        };

        this.insertFirst = function (element) {
            elements.unshift(element.elementJson);
        };

        this.insertLast = function (element) {
            elements.push(element.elementJson);
        };

        this.insertAt = function (element, idx) {
            if (idx == 0) {
                elements.unshift(element.elementJson);
            } else if (idx == elements.length) {
                elements.push(element.elementJson);
            } else {
                elements.splice(idx, 0, element.elementJson);
            }
        };
    }

    function Element($el, elType) {
        var placeholder = null,
            id = $el.data('id'),
            skin = $el.data('skin'),
            type = elType;

        Object.defineProperty(this, 'elementId', {
            get: function () {
                return id && skin ? id + '/' + skin : id;
            }
        });

        Object.defineProperty(this, 'skin', {
            get: function () {
                return skin;
            }
        });

        Object.defineProperty(this, 'elementJson', {
            get: function () {
                if(type === 'module'){
                    return {
                        _placeholder: placeholder,
                        data_variation: null,
                        module: id,
                        skin: skin
                    };
                }
                return {
                    _placeholder: placeholder,
                    template: id
                };
            }
        });

        this.render = function (plhId, renderFunction, callback) {
            var url = '/web/page_edit/element_info/' + type + '?id=' + id + '&parent=' + plhId;
            if (skin) url += '&skin=' + skin;
            $.get(url).then(function (response) {
                placeholder = response._placeholder;
                renderFunction(response.html);
                callback();
            }, function (err) {
                console.error(err);
            });
        };
    }

    //variables
    var $editor = $('.page-editor'),
        jsonDom = new JsonDom($('#siteDefinition', $editor).html()),
        elements = [];


    //functions
    function reloadTooltips() {
        $('[data-toggle="tooltip"]', $editor).tooltip({
            container: '.page-editor'
        });
    }

    //event binders
    $('.sidebar .module, .sidebar .layout', $editor).on('dragstart', function (e) {
        var $this = $(this);
        var id = $this.data('id');
        if ($this.data('skin')) id += '/' + $this.data('skin');
        e.originalEvent.dataTransfer.setData('id', id);
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
            var id = e.originalEvent.dataTransfer.getData('id'),
                $this = $(this);
            $this.removeClass('drag-over');
            var element = elements.get(function (e) {
                return e.elementId === id;
            });
            if (!element) throw new Error("Element not found");

            var before = $this.hasClass('start');

            var placeholder = '';
            if (!$this.hasClass('module') && !$this.hasClass('template')) {
                //if element is a placeholder start or placeholder end DIV, adding is pretty easy.
                placeholder = $this.attr('id').replace('plh_', '');
                element.render(placeholder, function (html) {
                    if (before) {
                        $this.after(html);
                        return;
                    }
                    $this.before(html);
                }, function () {
                    if (before) {
                        jsonDom.addElementToPlaceholderStart(placeholder, element);
                    } else {
                        jsonDom.addElementToPlaceholderEnd(placeholder, element);
                    }
                    reloadTooltips();
                });
            } else {
                var path = $this.data('path'),
                    self = $this.data('self'),
                    $elementStart = $this.hasClass('start') ? $this : $this.prevAll('.start[data-path="' + path + '"]').first(),
                    $elementEnd = $this.hasClass('end') ? $this : $this.nextAll('.end[data-path="' + path + '"]').first();

                placeholder = path.substring(0, path.lastIndexOf('/' + self));

                var idx = -1;
                var guid = $elementStart.data('index');
                $('.plh.start[id="plh_' + placeholder + '"]').nextUntil('.plh.end[id="plh_' + placeholder + '"]', '.plh.start[data-path^="' + placeholder + '"]').each(function (k, v) {
                    if ($(v).data('index') === guid) {
                        idx = k;
                    }
                });
                if (idx === -1) throw new Error("Element / Template index could not be found!");
                element.render(placeholder, function (html) {
                    if (before) {
                        $elementStart.before(html);
                        return;
                    }
                    $elementEnd.after(html);
                }, function () {
                    jsonDom.addElementToPlaceholder(placeholder, element, idx, before);
                });
            }
        });

    $editor.on('click', '.plh .btn-delete', function () {
        var $this = $(this),
            $elementStart = $this.parent(),
            path = $elementStart.data('path'),
            self = $elementStart.data('self'),
            placeholder = path.substring(0, path.lastIndexOf('/' + self)),
            guid = $elementStart.data('index'),
            endFinder = '',
            elementId = '';

        if ($elementStart.hasClass('module')) {
            endFinder = '.plh.module.end';
            elementId = $elementStart.data('module-id');
        } else {
            endFinder = '.plh.template.end';
            elementId = $elementStart.data('template-id');
        }

        if (!$elementStart.nextAll(endFinder).length) {
            throw new Error("no module / template end found.");
        }
        var $modEnd = $elementStart.nextAll(endFinder).first();
        var $between = $elementStart.nextUntil($modEnd);

        var idx = -1;
        $('.plh.start[id="plh_' + placeholder + '"]').nextUntil('.plh.end[id="plh_' + placeholder + '"]', '.plh.start[data-path^="' + placeholder + '"]').each(function (k, v) {
            if ($(v).data('index') === guid) {
                idx = k;
            }
        });
        if (idx === -1) throw new Error("Element / Template index could not be found!");

        jsonDom.removeElementFromPlaceholder(placeholder, elementId, idx);

        $this.tooltip('hide');
        [$elementStart, $between, $modEnd].forEach(function (e) {
            e.remove();
        });
    });

    $editor.on('click', 'a', function () {
        //prevent links from firing
        return false;
    });

    $('.js-save', $editor).click(function(){
        $.post('/web/page_edit?id=' + $editor.data('id') + '&app=' + $editor.data('app'), {definition: jsonDom.dom}).then(function(){
        }, function(err){
            console.error(err);
        });
    });

    $('.js-cancel', $editor).click(function(){
        window.history.back();
    });

    //initialize tooltips
    $('[data-toggle="tooltip"]', $editor).tooltip({
        container: '.page-editor'
    });

    //init.
    $('.sidebar .module', $editor).each(function (k, v) {
        elements.push(new Element($(v), 'module'));
    });
    $('.sidebar .layout', $editor).each(function (k, v) {
        elements.push(new Element($(v), 'layout'));
    });
});