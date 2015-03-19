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

        console.log(domObject);
    }

    function Module($el) {
        this.id = $el.data('id');
        this.html = $el.find('.js-module-definition').html();
    }


    //variables
    var $editor = $('.page-editor'),
        dom = new JsonDom($('#siteDefinition', $editor).html()),
        modules = [];


    //functions


    //event binders
    $('.sidebar .module', $editor).on('dragstart', function (e) {
        e.originalEvent.dataTransfer.setData('id', $(this).data('id'));

    });

    $editor
        .on('dragover', '.plh', function (e) {
            e.preventDefault();
            $(this).addClass('drag-over');
        })
        .on('dragleave', '.plh', function () {
            $(this).removeClass('drag-over');
        })
        .on('drop', '.plh', function (e) {
            console.log(e.originalEvent.dataTransfer.getData('id'))
            $(this).removeClass('drag-over');
        });

    $('.plh.module .btn-delete', $editor).click(function () {
        var $this = $(this),
            $modStart = $this.parent(),
            plhId = $modStart.data('plh-id');

        if (!$modStart.nextAll('.plh.module').length) {
            throw new Error("no module end found.");
        }
        var $modEnd = $modStart.nextAll('.plh.module').first();
        var $between = $modStart.nextUntil($modEnd);

        //todo: remove from json dom

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