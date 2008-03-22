n2dragging = false;

function DragDrop(dz,dp,di,urls,messages){
	this.dropPoints = dp;
	this.dragItems = di;
	this.urls = urls || {
		move: 'move.n2.ashx',
		create: 'create.n2.ashx',
		edit: 'edit.n2.ashx',
		remove: 'delete.n2.ashx'
	};
	this.messages = messages || {
		deleting: 'Do you really want to delete?'
	};
	this.init(this,dz);
}

DragDrop.prototype = {
	dragHelper: function(ev){
		n2dragging = true;
		$(document.body).addClass("dragging");
		var myHelper = document.createElement("div");
		$(myHelper).css({
			background: 'lightslategray',
			width: this.offsetWidth + 'px',
			height: this.offsetHeight + 'px',
			opacity: 0.5,
			position: 'absolute'
		});
		return myHelper;
	},

	dragStop: function(ev){
		$(document.body).removeClass("dragging");
		setTimeout(function(){n2dragging = false;},100);
	},

	init: function(t,dz){
		$(document).ready(function(){
			t.definitionsDraggable(t);
			t.itemsDraggable(t);
			t.droppableZones(t,dz);
			$(document.body).addClass("dragDrop");
		});
	},

	definitionsDraggable: function(t){
		$(".definition").draggable({
			helper: t.dragHelper,
			stop: t.dragStop,
			start: function(){
				var s = this;
				t.dropHandler = function(d,ctrl){
					t.createIn(s.id, d);
				};
			}
		});
	},

	itemsDraggable: function(t){
		$('.zoneItem').draggable({
			dragPrevention: 'a,input,textarea,select',
			helper: t.dragHelper,
			cursorAt: {top:8, left:8},
			stop: t.dragStop,
			start: function(){
				var s = this;
				t.dropHandler = function(d,ctrl){
					if(ctrl)
						return t.copyTo(s.id,d);
					else
						return t.moveTo(s.id,d);
				}
			}
		}).each(function(){
			var zoneId = this.id;
			$(this).children(".titleBar").find("img").each(function(){
				var $img = $(this);
				if($img.is(".edit")){
					$img.click(function(){
						t.edit(zoneId);
					});
				}
				if($img.is(".delete")){
					$img.click(function(){
						t.del(zoneId);
					});	
				}			
			});
		});
	},
	
	droppableZones: function(t,dz){
		for(var i=0; i<dz.length; ++i){
			$(dz[i].selector).droppable({
				accept: dz[i].accept,
				activeClass: 'droppable-active',
				hoverClass: 'droppable-hover',
				tolerance: 'pointer',
				drop: function(ev, ui) {
					if(n2dragging){
						n2dragging = false;
						t.dropHandler(this.id,ev.ctrlKey);
					}
				}
			});
		}
	},

	getDropPoint: function(destinationId){
		for(var i=0; i<this.dropPoints.length; ++i){
			if(this.dropPoints[i].dropKey == destinationId){
				return this.dropPoints[i];
			}
		}
	},

	getDragItem: function(sourceId){
		for(var i=0; i<this.dragItems.length; ++i){
			if(this.dragItems[i].dragKey == sourceId){
				return this.dragItems[i];
			}
		}
	},

	getReturnUrl: function(){
		return window.location.pathname + window.location.search;
	},

	open: function(url){
		if(window.top.location == window.location) {
			$(document.body).addClass("masked").prepend("<div class='popup'><iframe src='" + url + "&cancel=reloadTop" + "'></iframe></div><div class='mask'></div>");
		} else {
			window.location = url;
		}
		
	},

	moveTo: function(sourceId,destinationId){
		this.putIn(sourceId,destinationId,{action:'move'});
	},
	
	copyTo: function(sourceId,destinationId){
		this.putIn(sourceId,destinationId,{action:'copy'});
	},

	putIn: function(sourceId,destinationId,request){
		var from = this.getDragItem(sourceId);
		var to = this.getDropPoint(destinationId);
		$.extend(request, from, to);
		$.getJSON(this.urls.move, request, function(data){
			if(data.error){
				alert(data.message);
			}
			window.location.reload();
		});
	},

	createIn: function(sourceId,destinationId){
		var t = this;
		var from = t.getDragItem(sourceId);
		var to = t.getDropPoint(destinationId);
		var request = {discriminator: sourceId, action:'create'};
		$.extend(request, from, to);
		request.returnUrl = this.getReturnUrl();
		$.getJSON(t.urls.create, request, function(data){
			if(data.error){
				alert(data.message);
			} else {
				t.open(data.url);
			}
		});
	},

	edit: function(dragItemId){
		var t = this;
		var request = t.getDragItem(dragItemId);
		request.returnUrl = t.getReturnUrl();
		request.action = 'edit';
		$.getJSON(t.urls.edit, request, function(data){
			if(data.error){
				alert(data.message);
			} else {
				t.open(data.url);
			}
		});
	},

	del: function(dragItemId){
		if(confirm(this.messages.deleting)){
			var request = this.getDragItem(dragItemId);
			request.action = 'delete';
			$.getJSON(this.urls.remove, request, function(data){
				if(data.error){
					alert(data.message);
				}
				window.location.reload();
			});
		}
	}
}

var n2 = {
	setupToolbar: function(){
	},
	refreshPreview: function(){
		window.top.location.reload();
	},
	refresh: function(){
		window.top.location.reload();
	}
};





//http://www.quirksmode.org/js/cookies.html

var cookie = {
	create: function(name,value,days) {
		if (days) {
			var date = new Date();
			date.setTime(date.getTime()+(days*24*60*60*1000));
			var expires = "; expires="+date.toGMTString();
		}
		else var expires = "";
		document.cookie = name+"="+value+expires+"; path=/";
	},

	read: function(name) {
		var nameEQ = name + "=";
		var ca = document.cookie.split(';');
		for(var i=0;i < ca.length;i++) {
			var c = ca[i];
			while (c.charAt(0)==' ') c = c.substring(1,c.length);
			if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length,c.length);
		}
		return null;
	},

	erase: function(name) {
		cookie.create(name,"",-1);
	}
};




var SlidingCurtain = function(selector,startsOpen){
	var $sc = $(selector);
	var closedPos = {top: (18-$sc.height()) + "px", left: "-156px"};
	var openPos = {top: "0px", left: "0px"};
	
	var curtain = {
		isOpen: function(){
			return cookie.read("sc_open") == "true";
		},
		open: function(e){
			if(e){
				$sc.animate(openPos);
			}else{
				$sc.css(openPos);
			}
			$sc.addClass("opened");
			cookie.create("sc_open", "true", 1);
		},
		close: function(e){
			if(e){
				$sc.animate(closedPos);
			}else{
				$sc.css(closedPos);
			}
			$sc.removeClass("opened");
			cookie.erase("sc_open");
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