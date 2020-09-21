jQuery(function ($) {
    if (!window.Intl) {
        console.log("Calendar can't be displayed because your browser doesn's support `Intl`. You may use a polyfill!");
        return;
    }


    //hide/show relevant alert messages according to device
    if ('ontouchstart' in window) {
        $('#alert-1').removeClass('d-none')
        $('#alert-2').addClass('d-none')
    }


    // initialize the external events
    new FullCalendar.Draggable(document.getElementById('external-events'), {
        itemSelector: '.fc-event',
        longPressDelay: 50,
        eventData: function (eventEl) {
            return {
                title: eventEl.innerText,
                classNames: eventEl.getAttribute('data-class').split(' ')
            }
        },
        zIndex: 999,
        revert: true,      // will cause the event to go back to its
        revertDuration: 0
    })



    // change styling options and icons
    FullCalendar.BootstrapTheme.prototype.classes = {
        root: 'fc-theme-bootstrap',
        table: 'table-bordered table-bordered brc-default-l2 text-secondary-d1 h-95',
        tableCellShaded: 'bgc-secondary-l3',
        buttonGroup: 'btn-group',
        button: 'btn btn-white btn-h-lighter-blue btn-a-blue',
        buttonActive: 'active',
        popover: 'card card-primary',
        popoverHeader: 'card-header',
        popoverContent: 'card-body',
    };
    FullCalendar.BootstrapTheme.prototype.baseIconClass = 'fa';
    FullCalendar.BootstrapTheme.prototype.iconClasses = {
        close: 'fa-times',
        prev: 'fa-chevron-left',
        next: 'fa-chevron-right',
        prevYear: 'fa-angle-double-left',
        nextYear: 'fa-angle-double-right'
    };
    FullCalendar.BootstrapTheme.prototype.iconOverrideOption = 'FontAwesome';
    FullCalendar.BootstrapTheme.prototype.iconOverrideCustomButtonOption = 'FontAwesome';
    FullCalendar.BootstrapTheme.prototype.iconOverridePrefix = 'fa-';



    //for some random events to be added
    var date = new Date();
    var m = date.getMonth();
    var y = date.getFullYear();

    var day1 = Math.random() * 20 + 2;
    var day2 = Math.random() * 25 + 1;

    // initialize the calendar
    window.calendar = new FullCalendar.Calendar(document.getElementById('calendar'), {
        schedulerLicenseKey: 'CC-Attribution-NonCommercial-NoDerivatives',
        themeSystem: 'bootstrap',
        height: 650,
        // initialView: 'resourceTimeGridDay',
        //resourceGroupField: 'groupdId',
        resources: getResource(),
        headerToolbar: {
            start: 'prev,next today',
            center: 'title',
            end: 'dayGridMonth,timeGridWeek,resourceTimeGridDay'
        },
        events: getEvents(),
        eventDidMount: function (info) {
            info.el.querySelector('.fc-event-title').innerHTML = info.el.querySelector('.fc-event-title').innerText;
        },
        selectable: true,
        selectLongPressDelay: 200,
        editable: true,
        droppable: true,
        drop: function (info) {
                updateOrderDate(info.draggedEl.children[0].innerHTML, info.date, null,
                function (data) {
                    window.calendar.addEvent({
                        id: data['OrderId'],
                        resourceId: data['EmployeeId'],
                        title: data.CustomerName + '<br/>' + data['EmployeeName'],
                        start: new Date(data['ShipStartDate']),
                        end: data['ShipEndDate'] == null ? null : new Date(data['ShipEndDate']),
                        allDay: false,
                        extendedProps: data,
                        className: 'bgc-info text-white text-75'
                    });
               });
            info.draggedEl.parentNode.removeChild(info.draggedEl);
            if (info) {
                info.event.remove();
            }

        },
        eventDrop: function (info) {
            updateOrderDate(info.event.id, info.event.start, info.event.end, function () {

            });
        },
        slotDuration: '00:15:00',
        eventResize: function (info) {
            updateOrderDate(info.event.id, info.event.start, info.event.end, function (data) {
            });
        },
        eventDragStop: function (info) {
            var event = info.event;
            var jsEvent = info.jsEvent;
            if (isEventOverDiv(jsEvent.clientX, jsEvent.clientY)) {
                var el = $('<div style="cursor:pointer;margin-right:7px" onclick="showOrderDetail(' + info.event._def.extendedProps.OrderId + ')" ' +
                    ' oid="' + info.event._def.extendedProps.OrderId + '" class= "fc-event badge bgc-blue-d1 text-white border-0 py-2 text-90 mb-1 radius-2px"' +
                    'data-class="bgc-blue-d1 text-white text-95" >' +
                    'Order #<span>' + info.event._def.extendedProps.OrderId + '</span> <br />' +
                    '' + info.event._def.extendedProps.EmployeeName + '' +
                    '</div >').appendTo('#external-events-listing');

                new FullCalendar.Draggable(document.getElementById('external-events'), {
                    itemSelector: '.fc-event',
                    longPressDelay: 50,
                    eventData: function (eventEl) {
                        return {
                            title: eventEl.innerText,
                            classNames: eventEl.getAttribute('data-class').split(' ')
                        }
                    },
                    zIndex: 999,
                    revert: true,     
                    revertDuration: 0
                });

                info.event.remove();
                updateOrderDate(info.event._def.extendedProps.OrderId, null, null, function () { });
            }
        },

        eventClick: function (info) {
            var inf = info;
            var orderId = info.event._def.extendedProps.OrderId;
            if (!orderId) {
                orderId = info.el.getAttribute('oid');
            }

            $.ajax({
                url: "/Home/GetOrderPopup/" + orderId,
                type: "POST",
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (response) {
                    var modal = response;
                    modal = $(modal).appendTo('body');
                    modal.find('form').on('submit', function (ev) {
                        ev.preventDefault();
                        if (inf) {
                            inf.event.remove();
                        }
                        window.calendar.addEvent({
                            id: inf.event._def.extendedProps['OrderId'],
                            resourceId: $("#EmployeeId").val(),
                            title: inf.event._def.extendedProps.CustomerName + '<br/>' + $("#EmployeeId option:selected").text(),
                            start: new Date(inf.event._def.extendedProps['ShipStartDate']),
                            end: inf.event._def.extendedProps['ShipEndDate'] == null ? null : new Date(inf.event._def.extendedProps['ShipEndDate']),
                            allDay: false,
                            extendedProps: inf.event._def.extendedProps,
                            className: 'bgc-info text-white text-75'
                        });

                        modal.modal("hide");
                    });
                    modal.find('button[data-action=delete]').on('click', function () {
                        modal.modal("hide");
                    });
                    modal.modal('show').on('hidden.bs.modal', function () {
                        modal.remove();
                    });
                }
            });


        }



    });



    //
    window.calendar.render();

    var isEventOverDiv = function (x, y) {

        var external_events = $('#external-events');
        var offset = external_events.offset();
        offset.right = external_events.width() + offset.left;
        offset.bottom = external_events.height() + offset.top;

        // Compare
        if (x >= offset.left
            && y >= offset.top
            && x <= offset.right
            && y <= offset.bottom) { return true; }
        return false;

    }

});


