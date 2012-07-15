/*

jQuery Tags Input Plugin 1.2.5
	
Copyright (c) 2011 XOXCO, Inc
	
Documentation for this plugin lives here:
http://xoxco.com/clickable/jquery-tags-input
	
Licensed under the MIT license:
http://www.opensource.org/licenses/mit-license.php

ben@xoxco.com

*/

(function(a){var b=new Array;var c=new Array;a.fn.doAutosize=function(b){var c=a(this).data("minwidth"),d=a(this).data("maxwidth"),e="",f=a(this),g=a("#"+a(this).data("tester_id"));if(e===(e=f.val())){return}var h=e.replace(/&/g,"&").replace(/\s/g," ").replace(/</g,"<").replace(/>/g,">");g.html(h);var i=g.width(),j=i+b.comfortZone>=c?i+b.comfortZone:c,k=f.width(),l=j<k&&j>=c||j>c&&j<d;if(l){f.width(j)}};a.fn.resetAutosize=function(b){var c=a(this).data("minwidth")||b.minInputWidth||a(this).width(),d=a(this).data("maxwidth")||b.maxInputWidth||a(this).closest(".tagsinput").width()-b.inputPadding,e="",f=a(this),g=a("<tester/>").css({position:"absolute",top:-9999,left:-9999,width:"auto",fontSize:f.css("fontSize"),fontFamily:f.css("fontFamily"),fontWeight:f.css("fontWeight"),letterSpacing:f.css("letterSpacing"),whiteSpace:"nowrap"}),h=a(this).attr("id")+"_autosize_tester";if(!a("#"+h).length>0){g.attr("id",h);g.appendTo("body")}f.data("minwidth",c);f.data("maxwidth",d);f.data("tester_id",h);f.css("width",c)};a.fn.addTag=function(d,e){e=jQuery.extend({focus:false,callback:true},e);this.each(function(){var f=a(this).attr("id");var g=a(this).val().split(b[f]);if(g[0]==""){g=new Array}d=jQuery.trim(d);if(e.unique){var h=a(g).tagExist(d);if(h==true){a("#"+f+"_tag").addClass("not_valid")}}else{var h=false}if(d!=""&&h!=true){a("<span>").addClass("tag").append(a("<span>").text(d).append("  "),a("<a>",{href:"#",title:"Removing tag",text:"x"}).click(function(){return a("#"+f).removeTag(escape(d))})).insertBefore("#"+f+"_addTag");g.push(d);a("#"+f+"_tag").val("");if(e.focus){a("#"+f+"_tag").focus()}else{a("#"+f+"_tag").blur()}a.fn.tagsInput.updateTagsField(this,g);if(e.callback&&c[f]&&c[f]["onAddTag"]){var i=c[f]["onAddTag"];i.call(this,d)}if(c[f]&&c[f]["onChange"]){var j=g.length;var i=c[f]["onChange"];i.call(this,a(this),g[j-1])}}});return false};a.fn.removeTag=function(d){d=unescape(d);this.each(function(){var e=a(this).attr("id");var f=a(this).val().split(b[e]);a("#"+e+"_tagsinput .tag").remove();str="";for(i=0;i<f.length;i++){if(f[i]!=d){str=str+b[e]+f[i]}}a.fn.tagsInput.importTags(this,str);if(c[e]&&c[e]["onRemoveTag"]){var g=c[e]["onRemoveTag"];g.call(this,d)}});return false};a.fn.tagExist=function(b){return jQuery.inArray(b,a(this))>=0};a.fn.importTags=function(b){id=a(this).attr("id");a("#"+id+"_tagsinput .tag").remove();a.fn.tagsInput.importTags(this,b)};a.fn.tagsInput=function(d){var e=jQuery.extend({interactive:true,defaultText:"add a tag",minChars:0,width:"300px",height:"100px",autocomplete:{selectFirst:false},hide:true,delimiter:",",unique:true,removeWithBackspace:true,placeholderColor:"#666666",autosize:true,comfortZone:20,inputPadding:6*2},d);this.each(function(){if(e.hide){a(this).hide()}var d=a(this).attr("id");if(!d||b[a(this).attr("id")]){d=a(this).attr("id","tags"+(new Date).getTime()).attr("id")}var f=jQuery.extend({pid:d,real_input:"#"+d,holder:"#"+d+"_tagsinput",input_wrapper:"#"+d+"_addTag",fake_input:"#"+d+"_tag"},e);b[d]=f.delimiter;if(e.onAddTag||e.onRemoveTag||e.onChange){c[d]=new Array;c[d]["onAddTag"]=e.onAddTag;c[d]["onRemoveTag"]=e.onRemoveTag;c[d]["onChange"]=e.onChange}var g='<div id="'+d+'_tagsinput" class="tagsinput"><div id="'+d+'_addTag">';if(e.interactive){g=g+'<input id="'+d+'_tag" value="" data-default="'+e.defaultText+'" />'}g=g+'</div><div class="tags_clear"></div></div>';a(g).insertAfter(this);a(f.holder).css("width",e.width);a(f.holder).css("height",e.height);if(a(f.real_input).val()!=""){a.fn.tagsInput.importTags(a(f.real_input),a(f.real_input).val())}if(e.interactive){a(f.fake_input).val(a(f.fake_input).attr("data-default"));a(f.fake_input).css("color",e.placeholderColor);a(f.fake_input).resetAutosize(e);a(f.holder).bind("click",f,function(b){a(b.data.fake_input).focus()});a(f.fake_input).bind("focus",f,function(b){if(a(b.data.fake_input).val()==a(b.data.fake_input).attr("data-default")){a(b.data.fake_input).val("")}a(b.data.fake_input).css("color","#000000")});if(e.autocomplete_url!=undefined){autocomplete_options={source:e.autocomplete_url};for(attrname in e.autocomplete){autocomplete_options[attrname]=e.autocomplete[attrname]}if(jQuery.Autocompleter!==undefined){a(f.fake_input).autocomplete(e.autocomplete_url,e.autocomplete);a(f.fake_input).bind("result",f,function(b,c,f){if(c){a("#"+d).addTag(c[0]+"",{focus:true,unique:e.unique})}})}else if(jQuery.ui.autocomplete!==undefined){a(f.fake_input).autocomplete(autocomplete_options);a(f.fake_input).bind("autocompleteselect",f,function(b,c){a(b.data.real_input).addTag(c.item.value,{focus:true,unique:e.unique});return false})}}else{a(f.fake_input).bind("blur",f,function(b){var c=a(this).attr("data-default");if(a(b.data.fake_input).val()!=""&&a(b.data.fake_input).val()!=c){if(b.data.minChars<=a(b.data.fake_input).val().length&&(!b.data.maxChars||b.data.maxChars>=a(b.data.fake_input).val().length))a(b.data.real_input).addTag(a(b.data.fake_input).val(),{focus:true,unique:e.unique})}else{a(b.data.fake_input).val(a(b.data.fake_input).attr("data-default"));a(b.data.fake_input).css("color",e.placeholderColor)}return false})}a(f.fake_input).bind("keypress",f,function(b){if(b.which==b.data.delimiter.charCodeAt(0)||b.which==13){b.preventDefault();if(b.data.minChars<=a(b.data.fake_input).val().length&&(!b.data.maxChars||b.data.maxChars>=a(b.data.fake_input).val().length))a(b.data.real_input).addTag(a(b.data.fake_input).val(),{focus:true,unique:e.unique});a(b.data.fake_input).resetAutosize(e);return false}else if(b.data.autosize){a(b.data.fake_input).doAutosize(e)}});f.removeWithBackspace&&a(f.fake_input).bind("keydown",function(b){if(b.keyCode==8&&a(this).val()==""){b.preventDefault();var c=a(this).closest(".tagsinput").find(".tag:last").text();var d=a(this).attr("id").replace(/_tag$/,"");c=c.replace(/[\s]+x$/,"");a("#"+d).removeTag(escape(c));a(this).trigger("focus")}});a(f.fake_input).blur();if(f.unique){a(f.fake_input).keydown(function(b){if(b.keyCode==8||String.fromCharCode(b.which).match(/\w+|[·ÈÌÛ˙¡…Õ”⁄Ò—,/]+/)){a(this).removeClass("not_valid")}})}}});return this};a.fn.tagsInput.updateTagsField=function(c,d){var e=a(c).attr("id");a(c).val(d.join(b[e]))};a.fn.tagsInput.importTags=function(d,e){a(d).val("");var f=a(d).attr("id");var g=e.split(b[f]);for(i=0;i<g.length;i++){a(d).addTag(g[i],{focus:false,callback:false})}if(c[f]&&c[f]["onChange"]){var h=c[f]["onChange"];h.call(d,d,g[i])}}})(jQuery);

//(function ($) {

//	var delimiter = new Array();
//	var tags_callbacks = new Array();

//	$.fn.addTag = function (value, options) {
//		var options = jQuery.extend({ focus: false, callback: true }, options);
//		this.each(function () {
//			id = $(this).attr('id');

//			var tagslist = $(this).val().split(delimiter[id]);
//			if (tagslist[0] == '') {
//				tagslist = new Array();
//			}

//			value = jQuery.trim(value);

//			if (options.unique) {
//				skipTag = $(tagslist).tagExist(value);
//			} else {
//				skipTag = false;
//			}

//			if (value != '' && skipTag != true) {
//				$('<span>').addClass('tag').append(
//                        $('<span>').text(value).append('&nbsp;&nbsp;'),
//                        $('<a>', {
//                        	href: '#',
//                        	title: 'Removing tag',
//                        	text: 'x'
//                        }).click(function () {
//                        	return $('#' + id).removeTag(escape(value));
//                        })
//                    ).insertBefore('#' + id + '_addTag');

//				tagslist.push(value);

//				$('#' + id + '_tag').val('');
//				if (options.focus) {
//					$('#' + id + '_tag').focus();
//				} else {
//					$('#' + id + '_tag').blur();
//				}

//				if (options.callback && tags_callbacks[id] && tags_callbacks[id]['onAddTag']) {
//					var f = tags_callbacks[id]['onAddTag'];
//					f(value);
//				}
//				if (tags_callbacks[id] && tags_callbacks[id]['onChange']) {
//					var i = tagslist.length;
//					var f = tags_callbacks[id]['onChange'];
//					f($(this), tagslist[i]);
//				}
//			}
//			$.fn.tagsInput.updateTagsField(this, tagslist);

//		});

//		return false;
//	};

//	$.fn.removeTag = function (value) {
//		value = unescape(value);
//		this.each(function () {
//			id = $(this).attr('id');

//			var old = $(this).val().split(delimiter[id]);


//			$('#' + id + '_tagsinput .tag').remove();
//			str = '';
//			for (i = 0; i < old.length; i++) {
//				if (old[i] != value) {
//					str = str + delimiter[id] + old[i];
//				}
//			}

//			$.fn.tagsInput.importTags(this, str);

//			if (tags_callbacks[id] && tags_callbacks[id]['onRemoveTag']) {
//				var f = tags_callbacks[id]['onRemoveTag'];
//				f(value);
//			}
//		});

//		return false;
//	};

//	$.fn.tagExist = function (val) {
//		if (jQuery.inArray(val, $(this)) == -1) {
//			return false; /* Cannot find value in array */
//		} else {
//			return true; /* Value found */
//		}
//	};

//	// clear all existing tags and import new ones from a string
//	$.fn.importTags = function (str) {
//		$('#' + id + '_tagsinput .tag').remove();
//		$.fn.tagsInput.importTags(this, str);
//	}

//	$.fn.tagsInput = function (options) {
//		var settings = jQuery.extend({ interactive: true, defaultText: 'add a tag', minChars: 0, width: '300px', height: '100px', 'hide': true, 'delimiter': ',', autocomplete: { selectFirst: false }, 'unique': true, removeWithBackspace: true }, options);

//		this.each(function () {
//			if (settings.hide) {
//				$(this).hide();
//			}

//			id = $(this).attr('id')

//			data = jQuery.extend({
//				pid: id,
//				real_input: '#' + id,
//				holder: '#' + id + '_tagsinput',
//				input_wrapper: '#' + id + '_addTag',
//				fake_input: '#' + id + '_tag'
//			}, settings);


//			delimiter[id] = data.delimiter;

//			if (settings.onAddTag || settings.onRemoveTag || settings.onChange) {
//				tags_callbacks[id] = new Array();
//				tags_callbacks[id]['onAddTag'] = settings.onAddTag;
//				tags_callbacks[id]['onRemoveTag'] = settings.onRemoveTag;
//				tags_callbacks[id]['onChange'] = settings.onChange;
//			}

//			var markup = '<div id="' + id + '_tagsinput" class="tagsinput"><div id="' + id + '_addTag">';

//			if (settings.interactive) {
//				markup = markup + '<input id="' + id + '_tag" value="" data-default="' + settings.defaultText + '" />';
//			}

//			markup = markup + '</div><div class="tags_clear"></div></div>';

//			$(markup).insertAfter(this);

//			$(data.holder).css('width', settings.width);
//			$(data.holder).css('height', settings.height);

//			if ($(data.real_input).val() != '') {
//				$.fn.tagsInput.importTags($(data.real_input), $(data.real_input).val());
//			}
//			if (settings.interactive) {
//				$(data.fake_input).val($(data.fake_input).attr('data-default'));
//				$(data.fake_input).css('color', '#666666');

//				$(data.holder).bind('click', data, function (event) {
//					$(event.data.fake_input).focus();
//				});

//				$(data.fake_input).bind('focus', data, function (event) {
//					if ($(event.data.fake_input).val() == $(event.data.fake_input).attr('data-default')) {
//						$(event.data.fake_input).val('');
//					}
//					$(event.data.fake_input).css('color', '#000000');
//				});

//				if (settings.autocomplete_url != undefined) {
//					console.log(data, settings.autocomplete_url);
//					$(data.fake_input).autocomplete(settings.autocomplete_url, settings.autocomplete).bind('result', data, function (event, data, formatted) {
//						console.log("data");
//						if (data) {
//							$(event.data.real_input).addTag(formatted, { focus: true, unique: (settings.unique) });
//						}
//					});

//					$(data.fake_input).bind('blur', data, function (event) {
//						if ($('.ac_results').is(':visible')) return false;
//						if ($(event.data.fake_input).val() != $(event.data.fake_input).attr('data-default')) {
//							if ((event.data.minChars <= $(event.data.fake_input).val().length) && (!event.data.maxChars || (event.data.maxChars >= $(event.data.fake_input).val().length)))
//								$(event.data.real_input).addTag($(event.data.fake_input).val(), { focus: false, unique: (settings.unique) });
//						}

//						$(event.data.fake_input).val($(event.data.fake_input).attr('data-default'));
//						$(event.data.fake_input).css('color', '#666666');
//						return false;
//					});

//				} else {
//					// if a user tabs out of the field, create a new tag
//					// this is only available if autocomplete is not used.
//					$(data.fake_input).bind('blur', data, function (event) {
//						var d = $(this).attr('data-default');
//						if ($(event.data.fake_input).val() != '' && $(event.data.fake_input).val() != d) {
//							if ((event.data.minChars <= $(event.data.fake_input).val().length) && (!event.data.maxChars || (event.data.maxChars >= $(event.data.fake_input).val().length)))
//								$(event.data.real_input).addTag($(event.data.fake_input).val(), { focus: true, unique: (settings.unique) });
//						} else {
//							$(event.data.fake_input).val($(event.data.fake_input).attr('data-default'));
//							$(event.data.fake_input).css('color', '#666666');
//						}
//						return false;
//					});

//				}
//				// if user types a comma, create a new tag
//				$(data.fake_input).bind('keypress', data, function (event) {
//					if (event.which == event.data.delimiter.charCodeAt(0) || event.which == 13) {
//						if ((event.data.minChars <= $(event.data.fake_input).val().length) && (!event.data.maxChars || (event.data.maxChars >= $(event.data.fake_input).val().length)))
//							$(event.data.real_input).addTag($(event.data.fake_input).val(), { focus: true, unique: (settings.unique) });

//						return false;
//					}
//				});
//				//Delete last tag on backspace
//				data.removeWithBackspace && $(data.fake_input).bind('keyup', function (event) {
//					if (event.keyCode == 8 && $(this).val() == '') {
//						var last_tag = $(this).closest('.tagsinput').find('.tag:last').text();
//						var id = $(this).attr('id').replace(/_tag$/, '');
//						last_tag = last_tag.replace(/[\s]+x$/, '');
//						$('#' + id).removeTag(escape(last_tag));
//						$(this).trigger('focus');
//					};
//				});
//				$(data.fake_input).blur();
//			} // if settings.interactive
//			return false;
//		});

//		return this;

//	};

//	$.fn.tagsInput.updateTagsField = function (obj, tagslist) {
//		id = $(obj).attr('id');
//		$(obj).val(tagslist.join(delimiter[id]));
//	};

//	$.fn.tagsInput.importTags = function (obj, val) {
//		$(obj).val('');
//		id = $(obj).attr('id');
//		var tags = val.split(delimiter[id]);
//		for (i = 0; i < tags.length; i++) {
//			$(obj).addTag(tags[i], { focus: false, callback: false });
//		}
//		if (tags_callbacks[id] && tags_callbacks[id]['onChange']) {
//			var f = tags_callbacks[id]['onChange'];
//			f(obj, tags[i]);
//		}
//	};

//})(jQuery);



$(document).ready(function () {
	$(".tagsEditor").each(function () {
		var $tags = $(this);
		$tags.tagsInput({
			width: null,
			height: null,
			autocomplete_url: $(this).attr("data-autocomplete-url"),
			autocomplete: { selectFirst: true, width: '100px', autoFill: true }
		});
//		$.post(
//			/*url*/$tags.attr("data-autocomplete-url"),
//			/*data*/{selected: $tags.attr("data-selected") },
//			/*success*/function(data){
//				$tags.importTags(data.join(","))
//			},
//			/*dataType*/"json"
//		);
	});
});