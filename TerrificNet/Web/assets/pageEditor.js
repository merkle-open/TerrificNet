$(document).ready(function() {
    //class definitions
    function SiteDom(json){
        var domObject = JSON.parse(json);

        console.log(domObject);
    }


    //variables
    var $editor = $('.page-editor'),
        dom = new SiteDom($('#siteDefinition', $editor).html());


    //functions


    //event binders
    $('.sidebar .module', $editor).on('dragstart', function(e){
        console.log(e);
    });

    $('.plh.module .btn-delete', $editor).click(function(){
        var $this = $(this);

        console.log($this)
    });

    $editor.on('click', 'a', function(){
        //prevent links from firing
        return false;
    });

    //initialize tooltips
    $('[data-toggle="tooltip"]', $editor).tooltip({
        container: '.page-editor'
    });
});