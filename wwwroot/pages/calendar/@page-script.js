 

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

    if (window.calenderHourId == '0') {
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
                var tooltip = new Tooltip(info.el, {
                    title: info.event.extendedProps.description,
                    placement: 'top',
                    trigger: 'hover',
                    container: 'body'
                });
            },
            selectable: true,
            selectLongPressDelay: 200,
            editable: true,
            droppable: true,
            drop: function (info, event, a, b) {
                if (info.draggedEl.parentNode) {
                    updateOrderDate(info.draggedEl.children[0].innerHTML, info.date, null,
                        function (data) {

                            window.calendar.addEvent({
                                id: data['OrderId'],
                                resourceId: data['EmployeeId'],
                                title: data.CustomerShipAddress + '<br/>' + data['ItemName'],
                                description: data['ItemName'],
                                start: new Date(data['ShipStartDate']),
                                end: data['ShipEndDate'] == null ? null : new Date(data['ShipEndDate']),
                                allDay: false,
                                extendedProps: data,
                                className: 'text-75 ',
                                color: data['Color'],   // a non-ajax option
                                textColor: 'white'
                            });
                        });
                    info.draggedEl.parentNode.removeChild(info.draggedEl);
                    return false;
                }

                //if (info) {
                //    if (info.event) {
                //        info.event.remove();
                //    }

                //}

            },
            eventDrop: function (info) {
                if (info.view.type == "resourceTimeGridDay") {
                    updateOrderAssignee(info.event.id, info.event.start, info.event.end, info.event._def.resourceIds.toString(),
                        function (data) {
                            window.calendar.addEvent({
                                id: data['OrderId'],
                                resourceId: data['EmployeeId'],
                                title: data.CustomerShipAddress + '<br/>' + data['ItemName'],
                                description: data['ItemName'],
                                start: new Date(data['ShipStartDate']),
                                end: data['ShipEndDate'] == null ? null : new Date(data['ShipEndDate']),
                                allDay: false,
                                extendedProps: data,
                              //  className: 'bgc-info text-white text-75'
                                className: 'text-75 ',
                                color: data['Color'],   // a non-ajax option
                                textColor: 'white'
                            });

                        });
                    if (info.event) {
                        info.event.remove();
                    }

                }
                else {
                    updateOrderDate(info.event.id, info.event.start, info.event.end, function () {

                    });
                }

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
                                title: inf.event._def.extendedProps.CustomerName + '<br/>' + inf.event._def.extendedProps.ItemName,
                                description: inf.event._def.extendedProps.ItemName,
                                start: new Date(inf.event._def.extendedProps['ShipStartDate']),
                                end: inf.event._def.extendedProps['ShipEndDate'] == null ? null : new Date(inf.event._def.extendedProps['ShipEndDate']),
                                allDay: false,
                                extendedProps: inf.event._def.extendedProps,
                                className: 'text-75 ',
                                color: window.assingEmployeeColor,   // a non-ajax option
                                textColor: 'white'
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
    }
    else {
        
        window.calendar = new FullCalendar.Calendar(document.getElementById('calendar'), {
            schedulerLicenseKey: 'CC-Attribution-NonCommercial-NoDerivatives',
            themeSystem: 'bootstrap',
            height: 1100,
            initialView: (window.roleName === 'Admin' ? 'resourceTimeGridDay' : 'timeGridDay' ),
            //resourceGroupField: 'groupdId',
            resources: getResource(),
            headerToolbar: {
                start: 'prev,next today',
                center: 'title',
                end: (window.roleName === 'Admin' ? 'dayGridMonth,timeGridWeek,resourceTimeGridDay' : 'dayGridMonth,timeGridWeek,timeGridDay') //resourceTimeGridDay
            },
            events: getEvents(),
            eventDidMount: function (info, element, view) {
                
                //ReOccurenceParentOrderId
                try {

                     
                    if (info.event._def.extendedProps.ReOccurenceParentOrderId.toString() != "0") {
                        $(info.el).find('.fc-daygrid-event-dot').html('').html("<i class='fa fa-asterisk'></i>");
                    }
                } catch (e) {

                }
                

               

                if (info.event._def.extendedProps.Color) {
                    // debugger;
                    if (info.view.type == 'dayGridMonth') {
                        $(info.el).css('background-color', info.event._def.extendedProps.Color);
                    }
                    else {
                        
                        $(info.el).find('.fc-event-time').prepend(" <i style='margin-right:5px;font-size:10px;margin-top:1px;' class='fa fa-asterisk'></i>");
                        //
                    }
                    //  $(info.el).css('background-color', info.event._def.extendedProps.Color);
                }
                info.el.querySelector('.fc-event-title').innerHTML = info.el.querySelector('.fc-event-title').innerText;

                var tooltip = new Tooltip(info.el, {
                    title: "dfshfjsafhs",
                    placement: 'top',
                    trigger: 'hover',
                    container: 'body'
                });
            },
            selectable: true,
            selectLongPressDelay: 200,
            editable: true,
            droppable: true,
            drop: function (info, event, a, b) {
                if (info.draggedEl.parentNode) {
                    updateOrderDate(info.draggedEl.children[0].innerHTML, info.date, null,
                        function (data) {

                            window.calendar.addEvent({
                                id: data['OrderId'],
                                resourceId: data['EmployeeId'],
                                title: data.CustomerShipAddress + '<br/>' + data['ItemName'],
                                description: + data['ItemName'],
                                start: new Date(data['ShipStartDate']),
                                end: data['ShipEndDate'] == null ? null : new Date(data['ShipEndDate']),
                                allDay: false,
                                extendedProps: data,
                                className: 'text-75 ',
                                color: data['Color'],   // a non-ajax option
                                textColor: 'white'
                            });
                        });
                    info.draggedEl.parentNode.removeChild(info.draggedEl);
                    return false;
                }

                //if (info) {
                //    if (info.event) {
                //        info.event.remove();
                //    }

                //}

            },
            eventDrop: function (info) {
                if (info.view.type == "resourceTimeGridDay") {
                    updateOrderAssignee(info.event.id, info.event.start, info.event.end, info.event._def.resourceIds.toString(),
                        function (data) {
                            window.calendar.addEvent({
                                id: data['OrderId'],
                                resourceId: data['EmployeeId'],
                                title: data.CustomerShipAddress + '<br/>' + data['ItemName'],
                                description: data['ItemName'],
                                start: new Date(data['ShipStartDate']),
                                end: data['ShipEndDate'] == null ? null : new Date(data['ShipEndDate']),
                                allDay: false,
                                extendedProps: data,
                                className: 'text-75',
                                color: data['Color'],   // a non-ajax option
                                textColor: 'white'
                                
                            });

                        });
                    if (info.event) {
                        info.event.remove();
                    }

                }
                else {
                    updateOrderDate(info.event.id, info.event.start, info.event.end, function () {

                    });
                }

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
                            
                            var props = {};
                            Object.assign(props, inf.event._def.extendedProps);
                            props.Color = window.assingEmployeeColor;
                            window.calendar.addEvent({
                               
                                id: inf.event._def.extendedProps['OrderId'],
                                resourceId: $("#EmployeeId").val(),
                                title: inf.event._def.extendedProps.CustomerName + '<br/>' + inf.event._def.extendedProps.ItemName,
                                description: inf.event._def.extendedProps.ItemName,
                                start: new Date(inf.event._def.extendedProps['ShipStartDate']),
                                end: inf.event._def.extendedProps['ShipEndDate'] == null ? null : new Date(inf.event._def.extendedProps['ShipEndDate']),
                                allDay: false,
                                extendedProps: props,
                                className: 'text-75 ',
                                color: window.assingEmployeeColor,   // a non-ajax option
                                textColor: 'white'
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

            },
            scrollTime: window.startTime,
            viewDidMount: function (args) {
               
                if (args.view.type == "resourceTimeGridDay" || args.view.type=="timeGridDay") {
                   
                    $("#goToDateCalendar").show();
                    if ($($($(".fc-toolbar-chunk")[1]).children().eq(0)).children().length>0) {
                        $($($(".fc-toolbar-chunk")[1]).children().eq(0)).children().eq(0).remove();
                    }
                    $($($(".fc-toolbar-chunk")[1]).children().eq(0)).append('<span> (' + new moment(window.calendar.getDate()).format('dddd') + ')</span>');
                    
                }
                else {
                    if ($($($(".fc-toolbar-chunk")[1]).children().eq(0)).children().length > 0) {
                        $($($(".fc-toolbar-chunk")[1]).children().eq(0)).children().eq(0).remove();
                    }
                    $("#goToDateCalendar").hide();
                }
                getDayTimeDetail(window.calendar.getDate().getDay() + 1, function () {

                    setTimeSlotColor();
                });
            },
            eventRender: function (event, element, view) {
                
                $(info.el).find('.fc-daygrid-event-dot').html('').html("<i class='fa fa-asterisk'></i>");
                //element.find(".fc-title").prepend("<i class='fa fa-asterisk'></i>");
                //if (info.event._def.extendedProps.Color) {
                //    $(info.el).css('background-color', info.event._def.extendedProps.Color);
                //}
            },
            dateClick: function (info) {
                openAddOrder(info.dateStr);
            }
            //slotMinTime: window.startTime,
            //slotMaxTime: window.endTime

        });

    }


    window.calendar.render();
   
     
    $($(".fc-toolbar-chunk")[1]).append('<input type="text" id="goToDateCalendar" placeholder="MM/DD/YYYY" style="display:none;margin-top: 8px;margin-left: 25%;width: 50%;" class="form-control"  />');

    setTimeout(function () {

        $("#goToDateCalendar").val(new moment().format("MM/DD/YYYY"));

        var TinyDatePicker3 = DateRangePicker.TinyDatePicker;
        TinyDatePicker3('#goToDateCalendar', {
            mode: 'dp-below',
            date: new Date()
        })
            .on('statechange', function (ev) {
                if ($("#goToDateCalendar").val()) {
                   // console.log('f');
                   // window.calendar.gotoDate('2020-01-12');
                    if (new moment($("#goToDateCalendar").val())._d.toString() != "Invalid Date") {
                        
                        /*if ($("#goToDateCalendar").val().toString().length == "10" || $("#goToDateCalendar").val().toString().length == "9") {*/
                        window.calendar.gotoDate(new moment($("#goToDateCalendar").val()).format("YYYY-MM-DD"));
                        
                    /*}*/
                        $("#calendar .fc-toolbar-title span").text("("+new moment($("#goToDateCalendar").val()).format("dddd")+")");
                    }
                }
            });

        $("#goToDateCalendar").show();
       

    }, 500);

    $('.fc-prev-button').click(function () {

        
        if (window.calendar.view.type =="resourceTimeGridDay") {
            if ($($($(".fc-toolbar-chunk")[1]).children().eq(0)).children().length > 0) {
                $($($(".fc-toolbar-chunk")[1]).children().eq(0)).children().eq(0).remove();
            }
            $($($(".fc-toolbar-chunk")[1]).children().eq(0)).append('<span> (' + new moment(window.calendar.getDate()).format('dddd') + ')</span>');

        }

        getDayTimeDetail(window.calendar.getDate().getDay() + 1, function () {
            setTimeSlotColor();
        });
    });

    $('.fc-next-button').click(function () {
        if (window.calendar.view.type == "resourceTimeGridDay") {
            if ($($($(".fc-toolbar-chunk")[1]).children().eq(0)).children().length > 0) {
                $($($(".fc-toolbar-chunk")[1]).children().eq(0)).children().eq(0).remove();
            }
            $($($(".fc-toolbar-chunk")[1]).children().eq(0)).append('<span> (' + new moment(window.calendar.getDate()).format('dddd') + ')</span>');

        }

        getDayTimeDetail(window.calendar.getDate().getDay() + 1, function () {
            setTimeSlotColor();
        });
    });

    $('.fc-today-button').click(function () {
        if (window.calendar.view.type == "resourceTimeGridDay") {
            if ($($($(".fc-toolbar-chunk")[1]).children().eq(0)).children().length > 0) {
                $($($(".fc-toolbar-chunk")[1]).children().eq(0)).children().eq(0).remove();
            }
            $($($(".fc-toolbar-chunk")[1]).children().eq(0)).append('<span> (' + new moment(window.calendar.getDate()).format('dddd') + ')</span>');

        }

        getDayTimeDetail(window.calendar.getDate().getDay() + 1, function () {
            setTimeSlotColor();
        });
    });

    //fc-today-button

    function getDayTimeDetail(day, callback) {
        if (day == 0) {
            day = 7;
        }
        var data = {
            'day': day
        };
        console.log(data);
        $.ajax({
            url: "/Home/GetDayTimeDetail",
            type: "POST",
            dataType: "json",
            data: data,
            success: function (data) {


                window.startTime = data.StartTimeFormatted;
                window.endTime = data.EndTimeFormatted;
                callback();
            },
            error: function (data) {


            }
        })
    }

    function setTimeSlotColor() {

        removeTimeSlotColor();

        $(".fc-timegrid-slot").css({ 'background-color': '#fceed7' });

        if (!window.startTime) {
            return;
        }
        var startHr = Number(window.startTime.split(":")[0]);
        var startMin = Number(window.startTime.split(":")[1]);

        var endHr = Number(window.endTime.split(":")[0]);
        var endMin = Number(window.endTime.split(":")[1]);

        for (var i = startHr; i < (endHr + 1); i++) {
            if (i < 10) {
                if (i < endHr) {
                    if (i == startHr) {
                        if (startMin == 30) {
                            $("td[data-time='0" + i + ":30:00']").css({ 'background-color': 'white' });
                            $("td[data-time='0" + i + ":45:00']").css({ 'background-color': 'white' });
                        }
                        else {
                            $("td[data-time='0" + i + ":00:00']").css({ 'background-color': 'white' });
                            $("td[data-time='0" + i + ":15:00']").css({ 'background-color': 'white' });
                            $("td[data-time='0" + i + ":30:00']").css({ 'background-color': 'white' });
                            $("td[data-time='0" + i + ":45:00']").css({ 'background-color': 'white' });
                        }
                    }
                    else {
                        $("td[data-time='0" + i + ":00:00']").css({ 'background-color': 'white' });
                        $("td[data-time='0" + i + ":15:00']").css({ 'background-color': 'white' });
                        $("td[data-time='0" + i + ":30:00']").css({ 'background-color': 'white' });
                        $("td[data-time='0" + i + ":45:00']").css({ 'background-color': 'white' });
                    }
                }
                else {
                    if (endMin == 30) {
                        $("td[data-time='0" + i + ":00:00']").css({ 'background-color': 'white' });
                        $("td[data-time='0" + i + ":15:00']").css({ 'background-color': 'white' });
                        $("td[data-time='0" + i + ":30:00']").css({ 'background-color': 'white' });

                    }
                    else {
                        $("td[data-time='0" + i + ":00:00']").css({ 'background-color': 'white' });

                    }
                }
            }
            else {
                if (i < endHr) {
                    if (i == startHr) {
                        if (startMin == 30) {
                            $("td[data-time='" + i + ":30:00']").css({ 'background-color': 'white' });
                            $("td[data-time='" + i + ":45:00']").css({ 'background-color': 'white' });
                        }
                        else {
                            $("td[data-time='" + i + ":00:00']").css({ 'background-color': 'white' });
                            $("td[data-time='" + i + ":15:00']").css({ 'background-color': 'white' });
                            $("td[data-time='" + i + ":30:00']").css({ 'background-color': 'white' });
                            $("td[data-time='" + i + ":45:00']").css({ 'background-color': 'white' });
                        }
                    }
                    else {
                        $("td[data-time='" + i + ":00:00']").css({ 'background-color': 'white' });
                        $("td[data-time='" + i + ":15:00']").css({ 'background-color': 'white' });
                        $("td[data-time='" + i + ":30:00']").css({ 'background-color': 'white' });
                        $("td[data-time='" + i + ":45:00']").css({ 'background-color': 'white' });
                    }
                }
                else {
                    if (endMin == 30) {
                        $("td[data-time='" + i + ":00:00']").css({ 'background-color': 'white' });
                        $("td[data-time='" + i + ":15:00']").css({ 'background-color': 'white' });
                        $("td[data-time='" + i + ":30:00']").css({ 'background-color': 'white' });

                    }
                    else {
                        $("td[data-time='" + i + ":00:00']").css({ 'background-color': 'white' });

                    }
                }
            }

        }


    }

    function removeTimeSlotColor() {
        if (!window.startTime) {
            return;
        }
        $(".fc-timegrid-slot").each(function (index, ele) {
            $(this).css({ 'background-color': '' });

        });


    }
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

    //

});


