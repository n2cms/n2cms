/*
 * n2gallery 0.1 - Copyright (c) 2007 Cristian Libardo
 *
 * This is free software; you can redistribute it and/or modify it
 * under the terms of the GNU Lesser General Public License as
 * published by the Free Software Foundation; either version 2.1 of
 * the License, or (at your option) any later version.
 *
 * Usage example:
 *
 *	<script type="text/javascript" src="jquery-1.1.2.pack.js"></script>
 *	<script type="text/javascript" src="n2gallery.js"></script>
 * 	<script type="text/javascript">
 *		$(document).ready(function(){
 *          // displays the images linked by href in #large
 *			$("a.linksToImage").n2gallery("#large");
 *		});
 *	</script>
 * 	<script type="text/javascript">
 *		$(document).ready(function(){
 *          // show next-previous thumbnails in #prev and #next and display link title in #title
 *			$("a.prevNext").n2gallery("#navigator", 
 *              {navigate:true, prev:"#prev", next:"#next", title:"#title"});
 *		});
 *	</script>
*/

(function($){
    $.fn.n2gallery = function(target, options) {
        if(this.length < 1)
            return;

        // defaults
        self.options = {
            navigate: false,
            prev: null,
            next: null,
            title: null
        };
        if(options) {
            $.extend(self.options, options);
        }
        
        // target image
        var t = $(target);
        
        var imgHtml = function(src,alt){
            return "<img src='" + src + "' alt='" + alt + "'/>"
        };
        
        // display the larger image
        var swap = function(){
            var a = this;
            t.empty().append(imgHtml(a.src,a.title));
        };
        
        var init = function(a){
            a.src = a.href;
            a.href = "javascript:void(0);";
            a.target = "";
            $(a).click(swap);
        };
        
        // optional behaviour
        if(self.options.navigate){
            var addThumbnail = function(a, target) {
                $(a).clone().appendTo(target)
                    .click(swap).attr("title", a.title).each(function(){
                        this.src = a.src;
                        this.prev = a.prev;
                        this.next = a.next;
                    }).show();
            };
            swap = function(){
                var a = this;
                t.empty().append(imgHtml(a.src,a.title));
                $(self.options.prev).empty().each(function(){
                    if(a.prev){
                        addThumbnail(a.prev, this);
                    }
                });
                $(self.options.next).empty().each(function(){
                    if(a.next){
                        addThumbnail(a.next, this);
                    }
                });
                if(self.options.title) $(self.options.title).empty().append(a.title);
            };
            init = function(a){
                a.src = a.href;
                a.href = "javascript:void(0);";
                a.target = "";
                $(a).click(swap);
                $(a).hide();
            };
        }
    
        // attach
        for(var i=0; i<this.length; ++i){
            var a = this.get(i);
            init(a);
            if(i-1 >= 0)
                a.prev = this.get((i-1));
            if(i+1 < this.length)
                a.next = this.get(i+1);
        }
 
        $(this.get(0)).each(swap);
        
        if($.browser.msie){//preload images for ie6
            this.each(function(i){
                var a = this;
                if(i>0) t.append(imgHtml(a.src,a.title));
            });
            t.children().each(function(i){
                if(i>0) $(this).hide();
            });
        }
    };
})(jQuery);