(function($) {
    var isDragging = false;

    window.DragDrop = function(dz, dp, di, urls, messages) {
        this.dropPoints = dp;
        this.dragItems = di;
        this.dropZones = dz;
        this.urls = urls || {
            move: 'move.n2.ashx',
            create: 'create.n2.ashx',
            edit: 'edit.n2.ashx',
            remove: 'delete.n2.ashx'
        };
        this.messages = messages || {
            deleting: 'Do you really want to delete?'
        };
        this.init(this, dz);
    }

    window.DragDrop.prototype = {
        dragHelper: function(ev) {
            isDragging = true;
            var $t = $(this);
            $(document.body).addClass("dragging");
            var shadow = document.createElement('div');
            $(shadow).addClass("dragShadow")
				.css({ height: $t.height(), width: $t.width() })
				.text("Drop on a highlighted area").appendTo("body");
            return shadow;
        },

        dragStop: function(ev) {
            $(this).removeClass("dragged").siblings(".adjacent").removeClass("adjacent");
            $(ev.originalTarget).effect("transfer", { to: this, className: 'ui-effects-transfer' });
            $(document.body).removeClass("dragging");
            setTimeout(function() { isDragging = false; }, 100);
        },

        init: function(t, dz) {
            $(document).ready(function() {
                t.definitionsDraggable(t);
                t.itemsDraggable(t);
                t.droppableZones(t, dz);
                $(document.body).addClass("dragDrop");
            });
        },

        definitionsDraggable: function(t) {
            $(".definition").draggable({
                helper: t.dragHelper,
                distance: 5,
                scroll: true,
                stop: t.dragStop,
                start: function() {
                    var s = this;
                    t.dropHandler = function(d, ctrl) {
                        t.createIn(s.id, d);
                    };
                }
            });
        },

        itemsDraggable: function(t) {
            $('.zoneItem').draggable({
                dragPrevention: 'a,input,textarea,select',
                helper: t.dragHelper,
                cursorAt: { top: 8, left: 8 },
                scroll: true,
                stop: t.dragStop,
                start: function() {
                    $(this).addClass("dragged").next(".dropPoint").addClass("adjacent").end().prev(".dropPoint").addClass("adjacent");
                    var s = this;
                    t.dropHandler = function(d, ctrl) {
                        if (ctrl)
                            return t.copyTo(s.id, d);
                        else
                            return t.moveTo(s.id, d);
                    }
                }
            }).each(function() {
                var zoneId = this.id;
                $(this).children(".titleBar").find("img").each(function() {
                    var $img = $(this);
                    if ($img.is(".edit")) {
                        $img.click(function() {
                            t.edit(zoneId);
                        });
                    }
                    if ($img.is(".delete")) {
                        $img.click(function() {
                            t.del(zoneId);
                        });
                    }
                });
            });
        },

        droppableZones: function(t, dz) {
            var currentlyOver = null;
            for (var i = 0; i < dz.length; ++i) {
                $(dz[i].selector).droppable({
                    accept: dz[i].accept,
                    activeClass: 'droppable-active',
                    hoverClass: 'droppable-hover',
                    tolerance: 'pointer',
                    drop: function(ev, ui) {
                        $(ui.draggable).html("");
                        $(this).append("<div class='dropping'/>");
                        if (isDragging) {
                            isDragging = false;
                            t.dropHandler(this.id, ev.ctrlKey);
                        }
                    },
                    over: function(e, ui) {
                        currentlyOver = this;
                        var $t = $(this);
                        $t.data("html", $t.html()).data("height", $t.height());
                        $t.html(ui.draggable.html()).css("height", "auto");
                        //ui.draggable.css("visibility", "hidden");
                        ui.helper.height($t.height()).width($t.width());
                    },
                    out: function(e, ui) {
                        if (currentlyOver === this) {
                            currentlyOver = null;
                            //ui.draggable.css("visibility", "visible");
                        }
                        var $t = $(this);
                        $t.html($t.data("html")).height($t.data("height"));
                    }
                });
            }
        },

        getDropPoint: function(destinationId) {
            for (var i = 0; i < this.dropPoints.length; ++i) {
                if (this.dropPoints[i].dropKey == destinationId) {
                    return this.dropPoints[i];
                }
            }
        },

        getDragItem: function(sourceId) {
            for (var i = 0; i < this.dragItems.length; ++i) {
                if (this.dragItems[i].dragKey == sourceId) {
                    return this.dragItems[i];
                }
            }
        },

        getReturnUrl: function() {
            return window.location.pathname + window.location.search;
        },

        open: function(url) {
            window.location = url;
        },

        moveTo: function(sourceId, destinationId) {
            this.putIn(sourceId, destinationId, { action: 'move' });
        },

        copyTo: function(sourceId, destinationId) {
            this.putIn(sourceId, destinationId, { action: 'copy' });
        },

        putIn: function(sourceId, destinationId, request) {
            var t = this;
            var from = this.getDragItem(sourceId);
            var to = this.getDropPoint(destinationId);
            $.extend(request, from, to);
            $.getJSON(this.urls.move, request, function(data) {
                if (data.error) {
                    alert(data.message);
                }

                window.location.reload();
            });
        },

        createIn: function(sourceId, destinationId) {
            var t = this;
            var from = t.getDragItem(sourceId);
            var to = t.getDropPoint(destinationId);
            var request = { discriminator: sourceId, action: 'create' };
            $.extend(request, from, to);
            request.returnUrl = this.getReturnUrl();
            $.getJSON(t.urls.create, request, function(data) {
                if (data.error) {
                    alert(data.message);
                } else {
                    t.open(data.url);
                }
            });
        },

        edit: function(dragItemId) {
            var t = this;
            var request = t.getDragItem(dragItemId);
            request.returnUrl = t.getReturnUrl();
            request.action = 'edit';
            $.getJSON(t.urls.edit, request, function(data) {
                if (data.error) {
                    alert(data.message);
                } else {
                    t.open(data.url);
                }
            });
        },

        del: function(dragItemId) {
            if (confirm(this.messages.deleting)) {
                var request = this.getDragItem(dragItemId);
                request.action = 'delete';
                $.getJSON(this.urls.remove, request, function(data) {
                    if (data.error) {
                        alert(data.message);
                    }
                    window.location.reload();
                });
            }
        }
    }

    var n2 = {
        setupToolbar: function() {
        },
        refreshPreview: function() {
            window.top.location.reload();
        },
        refresh: function() {
            window.top.location.reload();
        }
    };

    window.SlidingCurtain = function(selector, startsOpen) {
        var $sc = $(selector);
        var closedPos = { top: (33 - $sc.height()) + "px", left: (20 - $sc.width()) + "px" };
        var openPos = { top: "0px", left: "0px" };

        var curtain = {
            isOpen: function() {
                return $.cookie("sc_open") == "true";
            },
            open: function(e) {
                if (e) {
                    $sc.animate(openPos);
                } else {
                    $sc.css(openPos);
                }
                $sc.addClass("opened");
                $.cookie("sc_open", "true", { expires: 1 });
            },
            close: function(e) {
                if (e) {
                    $sc.animate(closedPos);
                } else {
                    $sc.css(closedPos);
                }
                $sc.removeClass("opened");
                $.cookie("sc_open", null);
            }
        };

        if (startsOpen) {
            $sc.animate(openPos).addClass("opened");
        } else if (curtain.isOpen()) {
            curtain.open();
        } else {
            curtain.close();
        }

        $sc.find(".close").click(curtain.close);
        $sc.find(".open").click(curtain.open);
    };

})(jQuery);
