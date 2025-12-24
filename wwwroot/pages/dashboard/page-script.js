jQuery(function ($) {

    // show tooltips
    if (!('ontouchstart' in window)) $('[data-toggle="tooltip"]').tooltip({
        template: '<div class="tooltip" role="tooltip"><div class="brc-secondary arrow"></div><div class="bgc-secondary-d2 tooltip-inner text-600 text-110 pt-15 pb-2"></div></div>',
        placement: 'left',
        container: 'body'
    })


    // Welcome message
    if (localStorage.getItem('welcome.ace') !== 'displayed') {
        $.aceToaster.add({
            placement: 'tc',
            body: "\
          <div class='position-tl w-100 h-100 bgc-black-tp7'></div>\
          <div class='position-tl w-100 h-100 bgc-primary-tp4 opacity-4'></div>\
          <div class='p-25 d-flex pos-rel'>\
						<span class='d-inline-block text-center mb-3 py-3 px-1'>\
							<i class='fa fa-leaf fa-2x w-6 text-white ml-0 mr-25'></i>\
						</span>\
						<div>\
						  <h3 class='text-125'>Welcome to Ace <small>(v3.0)</small></h3>\
						  A lightweight, feature-rich, customizable and easy to use admin template!\
            </div>\
          </div>\
					<div>\
            <button data-dismiss='toast' class='btn btn-sm btn-outline-white btn-tp border-0 radius-round position-tr mt-15 mr-1'>\
              <i class='fa fa-times px-1px'></i>\
            </button>\
					</div>",

            width: 420,
            delay: 8,

            close: false,

            progress: 'position-bl bgc-white-tp3 pt-1px pb-2px m-1px',
            progressReverse: true,

            className: 'bgc-green-tp3 text-white shadow overflow-hidden border-0 p-1 radius-3px',

            bodyClass: 'border-0 text-white text-120 p-0 radius-1',
            headerClass: 'd-none',
        })

        localStorage.setItem('welcome.ace', 'displayed')// save so that we don't show it again
    }

    //////////////////////////////


    // Handle conversation box scrollbars  
    $('#conversations')
        .on('expanded.ace.widget', function () {
            // add `.mh-100` (max-height: 100%) when fullscreened
            $(this).find('[class*="ace-scroll"]').addClass('mh-100')
        })
        .on('restore.ace.widget', function () {
            // remove `.mh-100` when restored
            $(this).find('[class*="ace-scroll"]').removeClass('mh-100')
        });

    //update max height according to available space
    setTimeout(function () {
        var _scroller = document.querySelector('#conversations div[class*="ace-scroll"]')
        if (_scroller) _scroller.style.maxHeight = _scroller.parentNode.clientHeight + 'px'
    }, 10)


    //////////// tasks ... add a `line through` when checked
    $('#tasks input[type=checkbox]')
        .on('change', function () {
            $(this).closest('label').find('.task-title').toggleClass('line-through', this.checked);
        });




    ////////////////////////////////



});

