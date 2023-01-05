if ($('#datatable').length !== 0) {

    var tableId = '#datatable';

    //var tableHead = document.querySelector('.sticky-nav')
    //tableHead.addEventListener('sticky-change', function (e) {
    //    // when  thead becomes sticky, add is-stuck class to it (which adds a border-bottom to it)
    //    this.classList.toggle('is-stuck', e.detail.isSticky)
    //})

    var $_table;

    if (window.outerWidth > 1700) {
          $_table = $('#datatable').DataTable({
            "scrollY": "" + (window.outerHeight - 440) + "px",
            "scrollCollapse": true,
           // "sScrollX": '100%',
            responsive: true,
            colReorder: true,
            renderer: 'bootstrap',
            language: {
                search: '<i class="fa fa-search pos-abs mt-2 pt-3px ml-25 text-blue-m2"></i>',
                searchPlaceholder: " Search ...",
                processing: "Loading Data...",
                zeroRecords: "No matching records found"

            },
            lengthMenu: [[10, 25, 50, 100, -1], [10, 25, 50, 100, "All"]],
            processing: true,
            serverSide: true,
            orderCellsTop: true,
            searching: true,
            stateSave: true,
            // autoWidth: true,
            deferRender: true,
            "pageLength": 50,
            "lengthChange": true,
            //dom: '<"row"<"col-sm-12 col-md-6"B><"col-sm-12 col-md-6 text-right"l>><"row"<"col-sm-12"tr>><"row"<"col-sm-12 col-md-5"i><"col-sm-12 col-md-7"p>>',
            dom: '<"html5buttons"B>lTfgitp',
            ajax: {
                type: "POST",
                url: '/Order/LoadTable/',
                contentType: "application/json; charset=utf-8",
                async: true,
                data: function (data) {
                    let additionalValues = [];
                    additionalValues.push(($("#id-date-1").val()));
                    additionalValues.push(($("#id-date-2").val()));
                    additionalValues.push(($("#id-date-3").val()));
                    additionalValues.push(($("#id-date-4").val()));

                    data.AdditionalValues = additionalValues;
                    return JSON.stringify(data);
                },
                beforeSend: function () {
                    // Here, manually add the loading message.
                    $('#datatable > tbody').html(
                        '<tr class="odd">' +
                        '<td valign="top" colspan="10" class="dataTables_empty">Loading&hellip;</td>' +
                        '</tr>'
                    );
                }
            },
            "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
                if (aData.ScheduledOnNonWorkingDay.toString() === "true") {
                    $(nRow).addClass('nonWkDayOrder');
                }

            },
            columns: [

                {
                    //'className': 'details-control',
                    title: "Order No",
                    data: "OrderNo",
                    name: "co",

                },
                {
                    title: "Main Order No",
                    data: "ReoccurrenceOrderNo",
                    name: "co",
                    render: function (data, type, row) {
                        try {
                            if (row.ReOccurenceParentOrderId.toString() == "0") {
                                return "";
                            }
                            else {
                                return "#" + row.ReOccurenceParentOrderId.toString();
                            }
                        } catch (e) {
                            return "";
                        }


                    }
                },
                {
                    title: "Order Date",
                    data: "OrderDate",
                    name: "co",
                    render: function (data, type, row) {
                        return new moment(row.OrderDate).format("MM/DD/YYYY");

                    }
                },
                {
                    title: "Customer",
                    data: "CustomerName",
                    name: "co"

                },
                {
                    title: "Ship Date",
                    data: "ShipStartDate",
                    name: "co",
                    render: function (data, type, row) {
                        return new moment(row.ShipStartDate).format("dddd") + " ," +
                            new moment(row.ShipStartDate).format("MM/DD/YYYY hh:mm a");

                    }
                },
                {
                    title: "Amount($)",
                    data: "TotalAmount",
                    name: "co"

                },
                {
                    title: "Assigned To",
                    data: "EmployeeName",
                    name: "co"

                },
                {
                    title: "Ship Address",
                    data: "CustomerShipAddress",
                    name: "co"
                },
                {
                    title: "Status",
                    data: "IsActive",
                    searchable: false,
                    render: function (data, type, row) {
                        if (row.IsActive === 0) {
                            return '<span class="badge badge-danger mr-1">In Active</span>';
                        }
                        else {
                            return '<span class="badge badge-success mr-1"> Active</span>';
                        }
                    }
                },
                {
                    title: "Action",
                    data: "UserId",
                    searchable: false,
                    render: function (data, type, row) {
                        return '<span><a href="/Order/Edit/' + row.OrderId + '">Edit</a>|</span >' +
                            '<span><a href="#" onclick="deleteFun(' + row.OrderId + ')">Delete</a>|</span >' +
                            '<a href="#" onclick="openOrderNotePopup(' + row.OrderId + ')">Add Note</a>';
                    }
                }
            ],
            //colReorder: {
            //    //disable column reordering for first and last columns
            //    fixedColumnsLeft: 1,
            //    fixedColumnsRight: 1
            //},
            classes: {
                sLength: "dataTables_length text-left w-auto",
            },
            buttons: {
                dom: {
                    button: {
                        className: 'btn' //remove the default 'btn-secondary'
                    },
                    container: {
                        className: 'dt-buttons btn-group bgc-white-tp2 text-right w-auto'
                    }
                },

                buttons: [
                    {
                        extend: 'colvis',
                        columns: ':not(.noVis)',
                        text: 'Select Column',
                        "className": "btn-light-default btn-bgc-white btn-h-outline-primary btn-a-outline-primary"
                    },
                    {
                        "extend": "copy",
                        "text": "<i class='far fa-copy text-125 text-purple'></i> <span class='d-none'>Copy to clipboard</span>",
                        "className": "btn-light-default btn-bgc-white btn-h-outline-primary btn-a-outline-primary"
                    },

                    {
                        "extend": "csv",
                        "text": "<i class='fa fa-database text-125 text-success-m1'></i> <span class='d-none'>Export to CSV</span>",
                        "className": "btn-light-default btn-bgc-white btn-h-outline-primary btn-a-outline-primary"
                    },

                    {
                        "extend": "print",
                        "text": "<i class='fa fa-print text-125 text-orange-d1'></i> <span class='d-none'>Print</span>",
                        "className": "btn-light-default btn-bgc-white  btn-h-outline-primary btn-a-outline-primary",
                        autoPrint: false,
                        message: 'This print was produced using the Print button for DataTables'
                    }
                ]
            },
            "order": [[0, "desc"]],

        });
    }
    else {
          $_table = $('#datatable').DataTable({
            "scrollY": "" + (window.outerHeight - 440) + "px",
            "scrollCollapse": true,
            "sScrollX": '100%',
            responsive: true,
            colReorder: true,
            renderer: 'bootstrap',
            language: {
                search: '<i class="fa fa-search pos-abs mt-2 pt-3px ml-25 text-blue-m2"></i>',
                searchPlaceholder: " Search ...",
                processing: "Loading Data...",
                zeroRecords: "No matching records found"

            },
            lengthMenu: [[10, 25, 50, 100, -1], [10, 25, 50, 100, "All"]],
            processing: true,
            serverSide: true,
            orderCellsTop: true,
            searching: true,
            stateSave: true,
            // autoWidth: true,
            deferRender: true,
            "pageLength": 50,
            "lengthChange": true,
            //dom: '<"row"<"col-sm-12 col-md-6"B><"col-sm-12 col-md-6 text-right"l>><"row"<"col-sm-12"tr>><"row"<"col-sm-12 col-md-5"i><"col-sm-12 col-md-7"p>>',
            dom: '<"html5buttons"B>lTfgitp',
            ajax: {
                type: "POST",
                url: '/Order/LoadTable/',
                contentType: "application/json; charset=utf-8",
                async: true,
                data: function (data) {
                    let additionalValues = [];
                    additionalValues.push(($("#id-date-1").val()));
                    additionalValues.push(($("#id-date-2").val()));
                    additionalValues.push(($("#id-date-3").val()));
                    additionalValues.push(($("#id-date-4").val()));

                    data.AdditionalValues = additionalValues;
                    return JSON.stringify(data);
                },
                beforeSend: function () {
                    // Here, manually add the loading message.
                    $('#datatable > tbody').html(
                        '<tr class="odd">' +
                        '<td valign="top" colspan="10" class="dataTables_empty">Loading&hellip;</td>' +
                        '</tr>'
                    );
                }
            },
            "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
                if (aData.ScheduledOnNonWorkingDay.toString() === "true") {
                    $(nRow).addClass('nonWkDayOrder');
                }

            },
            columns: [

                {
                    //'className': 'details-control',
                    title: "Order No",
                    data: "OrderNo",
                    name: "co",

                },
                {
                    title: "Main Order No",
                    data: "ReoccurrenceOrderNo",
                    name: "co",
                    render: function (data, type, row) {
                        try {
                            if (row.ReOccurenceParentOrderId.toString() == "0") {
                                return "";
                            }
                            else {
                                return "#" + row.ReOccurenceParentOrderId.toString();
                            }
                        } catch (e) {
                            return "";
                        }


                    }
                },
                {
                    title: "Order Date",
                    data: "OrderDate",
                    name: "co",
                    render: function (data, type, row) {
                        return new moment(row.OrderDate).format("MM/DD/YYYY");

                    }
                },
                {
                    title: "Customer",
                    data: "CustomerName",
                    name: "co"

                },
                {
                    title: "Ship Date",
                    data: "ShipStartDate",
                    name: "co",
                    render: function (data, type, row) {
                        return new moment(row.ShipStartDate).format("dddd") + " ," +
                            new moment(row.ShipStartDate).format("MM/DD/YYYY hh:mm a");

                    }
                },
                {
                    title: "Amount($)",
                    data: "TotalAmount",
                    name: "co"

                },
                {
                    title: "Assigned To",
                    data: "EmployeeName",
                    name: "co"

                },
                {
                    title: "Ship Address",
                    data: "CustomerShipAddress",
                    name: "co"
                },
                {
                    title: "Status",
                    data: "IsActive",
                    searchable: false,
                    render: function (data, type, row) {
                        if (row.IsActive === 0) {
                            return '<span class="badge badge-danger mr-1">In Active</span>';
                        }
                        else {
                            return '<span class="badge badge-success mr-1"> Active</span>';
                        }
                    }
                },
                {
                    title: "Action",
                    data: "UserId",
                    searchable: false,
                    render: function (data, type, row) {
                        return '<span><a href="/Order/Edit/' + row.OrderId + '">Edit</a>|</span >' +
                            '<span><a href="#" onclick="deleteFun(' + row.OrderId + ')">Delete</a>|</span >' +
                            '<a href="#" onclick="openOrderNotePopup(' + row.OrderId + ')">Add Note</a>';
                    }
                }
            ],
            //colReorder: {
            //    //disable column reordering for first and last columns
            //    fixedColumnsLeft: 1,
            //    fixedColumnsRight: 1
            //},
            classes: {
                sLength: "dataTables_length text-left w-auto",
            },
            buttons: {
                dom: {
                    button: {
                        className: 'btn' //remove the default 'btn-secondary'
                    },
                    container: {
                        className: 'dt-buttons btn-group bgc-white-tp2 text-right w-auto'
                    }
                },

                buttons: [
                    {
                        extend: 'colvis',
                        columns: ':not(.noVis)',
                        text: 'Select Column',
                        "className": "btn-light-default btn-bgc-white btn-h-outline-primary btn-a-outline-primary"
                    },
                    {
                        "extend": "copy",
                        "text": "<i class='far fa-copy text-125 text-purple'></i> <span class='d-none'>Copy to clipboard</span>",
                        "className": "btn-light-default btn-bgc-white btn-h-outline-primary btn-a-outline-primary"
                    },

                    {
                        "extend": "csv",
                        "text": "<i class='fa fa-database text-125 text-success-m1'></i> <span class='d-none'>Export to CSV</span>",
                        "className": "btn-light-default btn-bgc-white btn-h-outline-primary btn-a-outline-primary"
                    },

                    {
                        "extend": "print",
                        "text": "<i class='fa fa-print text-125 text-orange-d1'></i> <span class='d-none'>Print</span>",
                        "className": "btn-light-default btn-bgc-white  btn-h-outline-primary btn-a-outline-primary",
                        autoPrint: false,
                        message: 'This print was produced using the Print button for DataTables'
                    }
                ]
            },
            "order": [[0, "desc"]],

        });
    }
   
    window.$_table = $_table;

    $_table.columns().every(function (index) {
        $('#fingers10 thead tr:last th:eq(' + index + ') input')
            .on('keyup',
                function (e) {
                    if (e.keyCode === 13) {
                        table.column($(this).parent().index() + ':visible').search(this.value).draw();
                    }
                });
    });


    $_table
        .on('select', function (e, dt, type, index) {
            if (type == 'row') {
                var row = $_table.row(index).node()
                _highlightSelectedRow(row)
            }
        })
        .on('deselect', function (e, dt, type, index) {
            if (type == 'row') {
                var row = $_table.row(index).node()
                _unhighlightDeselectedRow(row)
            }
        })

    // when clicking the checkbox in table header, select/deselect all rows
    $(tableId)
        .on('click', 'th input[type=checkbox]', function () {
            if (this.checked) {
                $_table.rows().select().every(function () {
                    _highlightSelectedRow(this.node())
                });
            }
            else {
                $_table.rows().deselect().every(function () {
                    _unhighlightDeselectedRow(this.node())
                })
            }
        })

    // don't select row if we click on the "show details" `plus` sign (td)
    $(tableId).on('click', 'td.dtr-control', function (e) {
        e.stopPropagation()
    })


    // add/remove bgc-h-* class to TH when soring columns
    var previousTh = null
    var toggleTH_highlight = function (th) {
        th.classList.toggle('bgc-yellow-l2')
        th.classList.toggle('bgc-h-yellow-l3')
        th.classList.toggle('text-blue-d2')
    }

    $(tableId)
        .on('click', 'th:not(.sorting_disabled)', function () {
            if (previousTh != null) toggleTH_highlight(previousTh)// unhighlight previous TH
            toggleTH_highlight(this)
            previousTh = this
        })

    // don't select row when clicking on the edit icon
    $('a[data-action=edit').on('click', function (e) {
        e.preventDefault()
        e.stopPropagation()// don't select
    })

    // add a dark border
    $('.dataTables_wrapper')
        .addClass('border-b-1 border-x-1 brc-dark-l2')

        // highlight DataTable header
        // also already done in CSS, but you can use custom colors here
        .find('.row:first-of-type')
        .addClass('border-b-1 brc-default-l3 bgc-blue-l4')
        .next().addClass('mt-n1px')// move the next `.row` up by 1px to go below header's border

    // highlight DataTable footer
    $('.dataTables_wrapper')
        .find('.row:last-of-type')
        .addClass('border-t-1 brc-default-l3 mt-n1px bgc-default-l4')


    // if the table has scrollbars, add ace styling to it
    $('.dataTables_scrollBody').aceScroll({ color: "grey" })

    $('#datatable tbody').on('click', 'td.details-control', function () {
        var tr = $(this).closest('tr');
        var row = $_table.row(tr);

        if (row.child.isShown()) {
            // This row is already open - close it
            row.child.hide();
            tr.removeClass('shown');
        } else {
            // Open this row
            row.child(format(row.data())).show();
            tr.addClass('shown');
        }
    });




    //enable tooltips
    setTimeout(function () {
        $('.dt-buttons button')
            .each(function () {
                var div = $(this).find('span').first()
                if (div.length == 1) $(this).tooltip({ container: 'body', title: div.parent().text() })
                else $(this).tooltip({ container: 'body', title: $(this).text() })
            })
        $('[data-rel=tooltip').tooltip({ container: 'body' })
    }, 0);

    setTimeout(function () {
        $("#datatable th").each(function (index, th) {
            $(th).addClass("border-0 bgc-white bgc-h-yellow-l3 shadow-sm");
        });
        $("#datatable thead").each(function (index, th) {
            $(th).addClass("sticky-nav text-secondary-m1 text-uppercase text-85");
        });
        $(".html5buttons").each(function (index, th) {
            $(th).addClass("table-tools-col");
        });

        $("#datatable_wrapper").prepend('<div class="row border-b-1 brc-default-l3 bgc-blue-l4">' +
            '<div class="col-12 col-sm-6" id="dtHeaderLeft"></div>' +
            '<div class="col-12 col-sm-6 text-right table-tools-col" id="dtHeaderRight"></div>' +

            '</div> ');
        $("#datatable_length").appendTo("#dtHeaderLeft");
        $(".html5buttons").appendTo("#dtHeaderRight");

        $("#datatable_wrapper").append('<div class="row border-t-1 brc-default-l3 mt-n1px bgc-default-l4">' +
            '<div class="col-12 col-md-5" id="dtFooterLeft"></div>' +
            '<div class="col-12 col-md-7" id="dtFooterRight"></div>' +

            '</div> ');

        $("#datatable_info").appendTo("#dtFooterLeft");
        $("#datatable_paginate").appendTo("#dtFooterRight");



        $('.dataTables_filter').appendTo('.page-tools').find('input').addClass('pl-45 radius-round').removeClass('form-control-sm')
            // and add a "+" button
            .end().append('<button data-rel="tooltip" id="btnAdd" onclick="add()" type="button" class="btn radius-round btn-outline-primary border-2 btn-sm ml-2" title="Add New"><i class="fa fa-plus"></i></button>')


        $($("thead tr")[0]).remove();


    }, 10);









};






