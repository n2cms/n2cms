jQuery(document).ready(function () {
	var dragMemory = null;
	var onDrop = function (e, ui) {
		var action = e.ctrlKey ? "copy" : "move";
		var to = this.rel;
		var from = dragMemory;
		parent.preview.location = "../paste.aspx?action=" + action
								+ "&memory=" + encodeURIComponent(from)
								+ "&selected=" + encodeURIComponent(to);
	};
	var onStart = function (e, ui) {
		dragMemory = this.rel;
	};

	var toDraggable = function (container) {
		jQuery("a", container).draggable({
			delay: 100,
			cursorAt: { top: 8, left: 8 },
			start: onStart,
			helper: 'clone'
		}).droppable({
			accept: '#nav li li a',
			hoverClass: 'droppable-hover',
			tolerance: 'pointer',
			drop: onDrop
		});
	}

	jQuery("#nav").SimpleTree({
		success: function (el) {
			toDraggable(el);
		}
	});

	jQuery("#nav").click(function (e) {
		var $a = $(e.target);
		if (!$a.is("a"))
			$a = $a.closest("a");

		if (!$a.is("a") || $a.is(".toggler"))
			return;

		var handler = n2nav.handlers[$a.attr("data-type")] || n2nav.handlers["fallback"];
		handler.call($a[0], e);

		document.body.className = document.body.className.replace(/\w+Selected ?/g, $a.attr("data-type") + "Selected");
	});

	window.onNavigating = function (options) {
		if (options.force)
			return;
		$("a[data-path=" + options.path + "]", this.document).each(function () {
			$(this).trigger("click");
			options.showNavigation = function () { };
		});
	};

	toDraggable(jQuery("#nav li li"));

	$(".tree a.selected").each(function () { document.body.className += " " + $(this).attr("data-type") + "Selected"; });

	$(".focusGroup a:not(.toggler):visible").n2keyboard({
		left: function (e, ctx) {
			var el = ctx.focused().closest(".folder-open")
							.children(".toggler").click()
							.siblings("a:not(.toggler)");
			ctx.focus(el);
		},
		right: function (e, ctx) {
			ctx.focused().siblings(".folder-close > .toggler").click();
		},
		esc: function () { $("#contextMenu").n2hide(); },
		del: function () { $("#contextMenu a.delete").n2trigger(); },
		c: function () { $("#contextMenu a.copy").n2trigger(); },
		n: function () { $("#contextMenu a.new").n2trigger(); },
		v: function () { $("#contextMenu a.paste").n2trigger(); },
		x: function () { $("#contextMenu a.move").n2trigger(); },
		enter: function () { ctx.focused().click(); }
	}, ".selected");
});
