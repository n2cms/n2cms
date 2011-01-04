(function ($) {
	var isDragging = false;
	var dialog = null;
	var dialogOptions = {
		modal: true,
		width: 800,
		height: 600,
		closeOnEscape: true
	};

	window.n2DragDrop = function (urls, messages) {
		this.urls = $.extend({
			copy: 'copy.n2.ashx',
			move: 'move.n2.ashx',
			remove: 'delete.n2.ashx',
			create: 'create.n2.ashx'
		}, urls);
		this.messages = $.extend({
			deleting: 'Do you really want to delete?',
			helper: "Drop on a highlighted area"
		}, messages);
		this.init();
	}

	window.n2DragDrop.prototype = {

		init: function () {
			var self = this;
			this.makeDraggable();
			$(document.body).addClass("dragDrop");
			$('.titleBar a.command').live('click', function (e) {
				e.preventDefault();
				self.showDialog($(this).attr('href'));
			});
		},

		showDialog: function (href) {
			window.scrollTop = 0;
			if (dialog) dialog.remove();
			dialog = $('<div id="dialog" />').hide();
			$(document).append(dialog);
			var iframe = document.createElement('iframe');
			dialog.append(iframe);
			iframe.src = href;
			$(iframe).load(function () {
				var doc = $(iframe.contentWindow.document);
				doc.find('#toolbar a.cancel').click(function () {
					dialog.dialog('close');
				});
			});
			dialog.dialog(dialogOptions);
		},

		makeDraggable: function () {
			var $draggables = $('.zoneItem,.definition').draggable({
				handle: "> .titleBar",
				dragPrevention: 'a,input,textarea,select,img',
				helper: this.makeDragHelper,
				cursorAt: { top: 8, left: 8 },
				scroll: true,
				stop: this.stopDragging,
				start: this.startDragging
			})
			$draggables.data("handler", this);
		},

		makeDragHelper: function (e) {
			isDragging = true;
			var $t = $(this);
			var handler = $t.data("handler");
			$(document.body).addClass("dragging");
			var shadow = document.createElement('div');
			$(shadow).addClass("dragShadow")
				.css({ height: $t.height(), width: $t.width() })
				.text(handler.messages.helper).appendTo("body");
			return shadow;
		},

		makeDropPoints: function (dragged) {
			var type = $(dragged).addClass("dragged").attr("data-type");

			$(".dropZone").each(function () {
				var zone = this;
				var allowed = $(zone).attr("data-allowed") + ",";
				var title = $(zone).attr("title");
				if (allowed.indexOf(type + ",") >= 0) {
					$(zone).append("<div class='dropPoint below'/>");
					$(".zoneItem", zone).before("<div class='dropPoint before'/>");
				}
				$(".dropPoint", zone).html("<div class='description'>" + title + "</div>");
			});
			$(dragged).next(".dropPoint").remove();
			$(dragged).prev(".dropPoint").remove();
		},

		makeDroppable: function () {
			$(".dropPoint").droppable({
				activeClass: 'droppable-active',
				hoverClass: 'droppable-hover',
				tolerance: 'pointer',
				drop: this.onDrop,
				over: function (e, ui) {
					currentlyOver = this;
					var $t = $(this);
					$t.data("html", $t.html()).data("height", $t.height());
					$t.html(ui.draggable.html()).css("height", "auto");
					ui.helper.height($t.height()).width($t.width());
				},
				out: function (e, ui) {
					if (currentlyOver === this) {
						currentlyOver = null;
					}
					var $t = $(this);
					$t.html($t.data("html")).height($t.data("height"));
				}
			});
		},

		onDrop: function (e, ui) {
			if (isDragging) {
				isDragging = false;

				var $droppable = $(this);
				var $draggable = $(ui.draggable);

				var handler = $draggable.data("handler");
				$draggable.html("");
				$droppable.append("<div class='dropping'/>");

				var data = {
					ctrlKey: e.ctrlKey,
					item: $draggable.attr("data-item"),
					discriminator: $draggable.attr("data-type"),
					before: $droppable.filter(".before").next().attr("data-item") || "",
					below: $droppable.closest(".dropZone").attr("data-item"),
					zone: $droppable.closest(".dropZone").attr("data-zone"),
					returnUrl: window.location.href,
					dropped: true
				};

				handler.process(data);
			}
		},

		stopDragging: function (e) {
			$(this).removeClass("dragged");
			$(".dropPoint").remove();
			$(document.body).removeClass("dragging");
			setTimeout(function () { isDragging = false; }, 100);
		},

		startDragging: function (e) {
			var dragged = this;
			var handler = $(dragged).data("handler");
			handler.makeDropPoints(dragged);
			handler.makeDroppable();

			dragged.dropHandler = function (ctrl) {
				var id = $(this).attr("data-item");
				if (!id)
					t.createIn(s.id, d);
				else if (ctrl)
					return t.copyTo(s.id, dragged);
				else
					return t.moveTo(s.id, dragged);
			}
		},

		format: function (f, values) {
			for (var key in values) {
				var keyIndex = url.indexOf("{" + key + "}", 0);
				if (keyIndex >= 0)
					f = f.substring(0, keyIndex) + values[key] + f.substring(2 + keyIndex + formatKey.length);
			}
			return f;
		},

		process: function (command) {
			var self = this;
			if (command.item)
				command.action = command.ctrlKey ? "copy" : "move";
			else
				command.action = "create";
			command.random = Math.random();

			var url = self.urls[command.action];

			var reloaded = false;
			$.getJSON(url, command, function (data) {
				reloaded = true;
				if (data.redirect && command.action == "create" && data.dialog !== "no")
					self.showDialog(data.redirect);
				else if (data.redirect)
					window.location = data.redirect;
				else
					window.location.reload();
			});

			// hack: why no success??
			setTimeout(function () {
				if (!reloaded)
					window.location.reload();
			}, 15000);
		}
	};

	var n2 = {
		setupToolbar: function () {
		},
		refreshPreview: function () {
			window.top.location.reload();
		},
		refresh: function () {
			window.top.location.reload();
		}
	};

	n2SlidingCurtain = {
		selector: ".sc",
		closedPos: { top: "0px", left: "0px" },
		openPos: { top: "0px", left: "0px" },

		recalculate: function () {
			var $sc = $(this.selector)
			this.closedPos = { top: (30 - $sc.height()) + "px", left: (20 - $sc.width()) + "px" };
			if (!this.isOpen()) $sc.css(this.closedPos);
		},

		isOpen: function () {
			return $.cookie("sc_open") == "true";
		},

		init: function (selector, startsOpen) {
			this.selector = selector;
			var $sc = $(selector);
			this.recalculate();
			var self = this;

			var curtain = {
				open: function (e) {
					if (e) {
						$sc.animate(self.openPos);
					} else {
						$sc.css(self.openPos);
					}
					$sc.addClass("opened");
					$.cookie("sc_open", "true", { expires: 1 });
				},
				close: function (e) {
					if (e) {
						$sc.animate(self.closedPos);
					} else {
						$sc.css(self.closedPos);
					}
					$sc.removeClass("opened");
					$.cookie("sc_open", null);
				}
			};

			if (startsOpen) {
				$sc.animate(self.openPos).addClass("opened");
			} else if (this.isOpen()) {
				curtain.open();
			} else {
				curtain.close();
			}

			$sc.find(".close").click(curtain.close);
			$sc.find(".open").click(curtain.open);
		}
	};
})(jQuery);
