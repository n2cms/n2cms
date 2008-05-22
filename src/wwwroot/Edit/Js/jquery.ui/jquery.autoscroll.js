/*
 * AutoScroll Plugin for jQuery
 *
 * Copyright (c) 2006 Jonathan Sharp (jdsharp.us)
 * Licensed under the GPL license.
 *
 * http://jdsharp.us/code/AutoScroll/
 *
 * Date: 2006-09-19
 * Rev: 001
 */

$.autoscroll = {
	settings: 	null,
	interval: 	0,
	event: 		null,

	init: function(opts) {
		$.autoscroll.settings = $.extend({step: 20, trigger: 40, interval: 100}, opts || {});
		document.onmousemove= $.autoscroll.setMouseEvent;
	},
	
	start: function(){
		if(!$.autoscroll.settings){
			$.autoscroll.init();
		}
		
		if ($.autoscroll.interval == 0) {
			$.autoscroll.interval = setInterval($.autoscroll.step, $.autoscroll.settings.interval);
		}
	},
	
	stop: function(){
		clearInterval($.autoscroll.interval); 
		$.autoscroll.interval = 0;
	},

	setMouseEvent: function(e) {
		var e	= e || window.event;
		var de	= document.documentElement;
		var b	= document.body;
		$.autoscroll.event = {
			cursor: {
				x: e.pageX || (e.clientX + (de.scrollLeft || b.scrollLeft) - (de.clientLeft || 0)),
				y: e.pageY || (e.clientY + (de.scrollTop || b.scrollTop) - (de.clientTop || 0))
			},
	
			win: {
				w: window.innerWidth  || (de.clientWidth && de.clientWidth != 0 ? de.clientWidth : b.offsetWidth),
				h: window.innerHeight || (de.clientHeight && de.clientWidth != 0 ? de.clientHeight : b.offsetHeight)
			},
	
			scroll: {
				x: (document.all ? 
						(!de.scrollLeft ? b.scrollLeft : de.scrollLeft)
						:
						(window.pageXOffset ? window.pageXOffset : window.scrollX)
						),
				y: (document.all ? 
						(!de.scrollTop ? b.scrollTop : de.scrollTop)
						:
						(window.pageYOffset ? window.pageYOffset : window.scrollY)
						)
			}
		};
	},
	
	step: function() {
		var e = $.autoscroll.event;
		if (!e) {
			return;
		}

		var hot_l 	= e.scroll.x;
		var hot_r 	= e.scroll.x + e.win.w;
		var x		= e.cursor.x;

		var hot_t	= e.scroll.y;
		var hot_b	= e.scroll.y + e.win.h;
		var y 		= e.cursor.y;
	
		if (hot_l <= x && x <= (hot_l + $.autoscroll.settings.trigger)) {
			var ratio 	= (1 - ((x - hot_l) / $.autoscroll.settings.trigger));
			var step	= Math.round(ratio * $.autoscroll.settings.step, 0);
			e.scroll.x += -step;
			e.cursor.x += -step;
		} else if ((hot_r - $.autoscroll.settings.trigger) <= x && x <= hot_r) {
			var ratio 	= (1 - ((hot_r - x) / $.autoscroll.settings.trigger));
			var step	= Math.round(ratio * $.autoscroll.settings.step, 0);
			e.scroll.x += step;
			e.cursor.x += step;
		}
	
		if (hot_t <= y && y <= (hot_t + $.autoscroll.settings.trigger)) {
			var ratio 	= (1 - ((y - hot_t) / $.autoscroll.settings.trigger));
			var step	= Math.round(ratio * $.autoscroll.settings.step, 0);
			e.scroll.y += -step;
			e.cursor.y += -step;
		} else if ((hot_b - $.autoscroll.settings.trigger) <= y && y <= hot_b) {
			var ratio 	= (1 - ((hot_b - y) / $.autoscroll.settings.trigger));
			var step	= Math.round(ratio * $.autoscroll.settings.step, 0);
			e.scroll.y += step;
			e.cursor.y += step;
		}
	
		if (e.scroll.x < 0) {
			e.scroll.x = 0;
			e.cursor.x = 0;
		}
		if (e.scroll.y < 0) {
			e.scroll.y = 0;
			e.cursor.y = 0;
		}

		window.scrollTo(e.scroll.x, e.scroll.y);
	}
};
