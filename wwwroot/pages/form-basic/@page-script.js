jQuery(function ($) {



    // datepicker
    var TinyDatePicker = DateRangePicker.TinyDatePicker;
    TinyDatePicker('#id-date-1', {
        mode: 'dp-below',
        date: new Date()
    })
        .on('statechange', function (ev) {

        });



    ////////////////////////
    // Datetimepicker plugin
    $('#id-timepicker').datetimepicker({
        icons: {
            time: 'far fa-clock text-green-d1 text-120',
            date: 'far fa-calendar text-blue-d1 text-120',

            up: 'fa fa-chevron-up text-secondary',
            down: 'fa fa-chevron-down text-secondary',
            previous: 'fa fa-chevron-left text-secondary',
            next: 'fa fa-chevron-right text-secondary',

            today: 'far fa-calendar-check text-purple-d1 text-120',
            clear: 'fa fa-trash-alt text-orange-d2 text-120',
            close: 'fa fa-times text-danger text-120'
        },

        // sideBySide: true,

        toolbarPlacement: "top",

        allowInputToggle: true,
        // showClose: true,
        // showClear: true,
        showTodayButton: true,

        //"format": "HH:mm:ss"
    });

    //***** NOTE *******//
    // the above `date/time` picker plugin was designed for BS3.
    // To make it work with BS4, the following piece of code is required
    $('#id-timepicker')
        .on('dp.show', function () {
            $(this).find('.collapse.in').addClass('show')
            $(this).find('.table-condensed').addClass('table table-borderless')

            $(this).find('[data-action][title]').tooltip() // enable tooltip
        })
        .on('dp.change', function (event) {
            if ($("#ShipStartDate").val()) {
                var shipStartDate = new Date($("#ShipStartDate").val());
                var shipEndDate = shipStartDate;
                shipEndDate.setMinutes(shipEndDate.getMinutes() + 30);
                $("#ShipEndDate").val(new moment(shipEndDate).format("MM/DD/YYYY hh:mm a").toString().toUpperCase());
                window.reOccurenceStartDateSetBy = "System"; 
                $('#ReOccurenceStartDate').val(new moment(shipStartDate).format("MM/DD/YYYY"));
                var reOccurenceendDate = new moment($("#ReOccurenceStartDate").val()).add(12, 'M');
                $('#ReOccurenceEndDate').val(new moment(reOccurenceendDate).format("MM/DD/YYYY"));
            }
        });

    // now listen to the `.collapse` events inside this datetimepicker accordion (one `.collapse` is for timepicker, the other one is for datepicker)
    // then add or remove the old `in` BS3 class so the plugin works correctly
    $(document)
        .on('show.bs.collapse', '.bootstrap-datetimepicker-widget .collapse', function () {
            $(this).addClass('in')
        }).on('hide.bs.collapse', '.bootstrap-datetimepicker-widget .collapse', function () {
            $(this).removeClass('in')
        })



    //////////////////////////////////
    // the small time picker inside popover

    $('#id-popover-timepicker')
        .popover({
            title: 'Choose Time',
            sanitize: false,
            content: function () {
                return $('#id-popover-timepicker-html').html()
            },
            html: true,
            placement: 'auto',
            trigger: 'manual',
            container: 'body',

            template: '<div class="popover popover-timepicker brc-primary-m3 border-1 radius-1 shadow-sm" role="tooltip"><div class="arrow brc-primary"></div><h3 class="popover-header bgc-primary-l3 brc-primary-l1 text-dark-tp4 text-600 text-center pb-25"></h3><div class="popover-body text-grey-d2 p-4"></div></div>'
        })
        .on('click', function () {
            // show popover when button is clicked
            $(this).popover('toggle')
            $(this).toggleClass('active')

            // get a reference to it
            var popover = $(document.body).find('.popover-timepicker').last()

            // hide popover if clicked outside of it
            if (popover.length > 0 && popover.hasClass('show')) {
                var This = this
                $(document).on('click.popover-time', function (ev) {
                    if (popover.get(0).contains(ev.target) || ev.target == document.getElementById('id-popover-timepicker')) return
                    $(This).popover('hide')
                    $(This).removeClass('active')

                    $(document).off('.popover-time')
                })
            }
        })





})