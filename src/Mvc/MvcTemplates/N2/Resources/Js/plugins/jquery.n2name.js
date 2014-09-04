/**
* n2name 0.2
*/

(function($) {
    function getName(titleid, whitespace, tolower, replacements){
        var titleBox=document.getElementById(titleid);
		if (!titleBox) return null;
	    var name;

        $.ajax({
        	type: 'POST',
        	async: false,
        	cache: false,
        	url: '/sluggenerator.n2.ashx',
        	data: { action: "sluggenerator", title: titleBox.value },
        	success: function (result) { name = result; },
        	error: function () {
        		name = titleBox.value.replace(/[.]+/g, '-')
        		    .replace(/[%?&/+:<>]/g, '')
        		    .replace(/\s+/g, whitespace)
        		    .replace(/[-]+/g, '-')
        		    .replace(/[-]+$/g, '');
        		if(tolower) name = name.toLowerCase();
        		for (var i in replacements){
        			name = name.replace(replacements[i].pattern, replacements[i].value);
        		}        		
        	}
        });

        return name;
    };
    
    function updateName(titleid, nameid, whitespace, tolower, replacements, checkboxid){
        var name = getName(titleid, whitespace, tolower, replacements);
        if(checkboxid && document.getElementById(checkboxid).checked)
	        document.getElementById(nameid).value = name;
    };

    function checkboxHandler(){
        var checked = $(this).find('input').attr('checked');
        if (checked)
			$(this).removeClass('unchecked')
				.siblings('input').removeClass('disabled').removeAttr('readonly');
        else
			$(this).addClass('unchecked')
				.siblings('input').addClass('disabled').attr('readonly', true);
    };
    
    function toggleChecked($cb){
        if($cb.attr("checked"))
            $cb.removeAttr("checked");
        else
            $cb.attr("checked", true);
    };
    
    $.fn.n2name = function(options) {
        var invokeUpdateName = function(){
	        updateName(options.titleId, options.nameId, options.whitespaceReplacement, options.toLower, options.replacements, options.keepUpdatedBoxId);
        };
        if(options.keepUpdatedBoxId){
            var $ku = $(this).siblings(".keepUpdated");
            
            $ku.click(function(e,stop) {
                toggleChecked($(this).find('input'));
                checkboxHandler.call(this);
            });
            
            $("#" + options.titleId).keyup(invokeUpdateName).blur(invokeUpdateName);

            var expected = getName(options.titleId, options.whitespaceReplacement, options.toLower, options.replacements);
            var actual = $("#" + options.nameId).attr("value");
            if(!expected || !actual || (expected == actual))
                $ku.each(checkboxHandler)
            else
                $ku.trigger('click');
        }
    };
})(jQuery);;
