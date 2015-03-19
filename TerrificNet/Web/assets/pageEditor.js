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

            console.log(domObject);
        };

        this.addElementToPlaceholder = function(plhId, element, index){

        };

        this.addElementToPlaceholderStart = function(plhId, element){
            var plh = placeholders.get(function (e) {
                return e.name === plhId;
            });
            if (!plh || !element) throw new Error("Placeholder or Element not found!");
            plh.insertFirst(element);
        };

        this.addElementToPlaceholderEnd = function(plhId, element){
            var plh = placeholders.get(function (e) {
                return e.name === plhId;
            });
            if (!plh || !element) throw new Error("Placeholder or Element not found!");
            plh.insertLast(element);
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
            console.log(placeholders);
        }

        init();
    }

    function Placeholder(name, plhElements) {
        var elements = plhElements;
        this.blub = elements;
        this.name = name;

        this.elementExists = function (elementId, idx) {
            var el = elements[idx];
            return (el.module && el.module === elementId) || (el.template && el.template === elementId);
        };

        this.removeElement = function (idx) {
            elements.splice(idx, 1);
        };

        this.insertFirst = function(element){
            elements.unshift(element.elementJson);
        };

        this.insertLast = function(element){
            elements.push(element.elementJson);
        };

        this.insertAt = function(element, idx){

        };
    }

    function Element($el, elType) {
        var html = $el.find('.js-module-definition').html(),
            id = $el.data('id'),
            skin = $el.data('skin'),
            type = elType;

        Object.defineProperty(this, 'elementId', {
            get: function(){
                return id && skin ? id + '/' + skin : id;
            }
        });

        Object.defineProperty(this, 'skin', {
            get: function(){
                return skin;
            }
        });

        Object.defineProperty(this, 'elementJson', {
            get: function(){
                return {
                    _placeholder: null,
                    data_variation: null,
                    module: id,
                    skin: skin
                };
            }
        });

        this.render = function(plhId){
            var el = $('<div/>');
            if(type === 'module'){
                el.append($('<div/>', {
                    class: 'plh module start',
                    'data-module-id': id,
                    'data-plh-id': plhId,
                    'data-index': guid()
                }).text('Module "'+id+'" before')
                    .append($('<span/>', {
                        class: 'btn-delete',
                        'data-toggle': 'tooltip',
                        'data-placement': 'top',
                        'title': 'Delete module.'
                    }).append($('<i/>', {
                        class: 'glyphicon glyphicon-remove'
                    }))));
            } else {

            }
            el.append(html);
            if(type === 'module'){
                el.append($('<div/>', {
                    class: 'plh module end',
                    'data-plh-id': plhId
                }).text('Module '+id+' after'));
            } else {

            }
            return el.html();
        };
    }

    //variables
    var $editor = $('.page-editor'),
        jsonDom = new JsonDom($('#siteDefinition', $editor).html()),
        elements = [];


    //functions
    function guid() {
        function s4() {
            return Math.floor((1 + Math.random()) * 0x10000)
                .toString(16)
                .substring(1);
        }

        return s4() + s4() + '-' + s4() + '-' + s4() + '-' +
            s4() + '-' + s4() + s4() + s4();
    }

    //event binders
    $('.sidebar .module', $editor).on('dragstart', function (e) {
        var $this = $(this);
        var id = $this.data('id');
        if($this.data('skin')) id += '/' + $this.data('skin');
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
            var $modStart = $this.hasClass('start') ? $this : $this.prevAll('.start[data-plh-id="' + $this.data('plh-id') + '"]').first();

            if(!$this.hasClass('module') && !$this.hasClass('template')){
                //if element is a placeholder start or placeholder end DIV, adding is pretty easy.
                var plhId = $this.attr('id').replace('plh_', '');
                if(before){
                    jsonDom.addElementToPlaceholderStart(plhId, element);
                    $this.after(element.render(plhId));
                } else {
                    jsonDom.addElementToPlaceholderEnd(plhId, element);
                    $this.before(element.render(plhId));
                }
                $('[data-toggle="tooltip"]', $editor).tooltip({
                    container: '.page-editor'
                });
                return;
            }

            console.log(element, $modStart, before);
        });

    $editor.on('click', '.plh .btn-delete', function () {
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
        $('.plh.start[id="plh_' + plhId + '"]').nextUntil('.plh.end[id="plh_' + plhId + '"]', '.plh.start[data-plh-id="' + plhId + '"]').each(function (k, v) {
            if ($(v).data('index') === guid) {
                idx = k;
            }
        });
        if (idx === -1) throw new Error("Element / Template index could not be found!");

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
        elements.push(new Element($(v), 'module'));
    });
});