(window.n2 || (window.n2 = {})).FileUpload = function (maxFileSize, ticket, selected, refreshFrames) {
	"use strict";

	function onFileUploaded(e) {
		var fileupload = this;
		setTimeout(function () {
			$(".complete", fileupload).parent().fadeOut();
			if ($(".error", fileupload).length > 0)
				if (!confirm("Failed to upload some files, confirm to refresh"))
					return;

			if ($(".cancel", fileupload).length == 0)
				onUploadComplete();
		}, 10);
	}
	function onUploadComplete() {
		$("#fileupload").removeClass("uploading");
		$("#uploadcontrols").slideDown();
		refreshFrames();
		window.location = window.location;
	}
	function onUploadFailed() {
		$("#fileupload").removeClass("uploading");
		$("#uploadcontrols").slideDown();
		$("#uploadcontrols").prepend("<div class='alert alert-error'><button type='button' class='close' data-dismiss='alert'>×</button>Failed uploading</div>");
	}
	function onUploadStart() {
		$("#uploadcontrols").slideUp();
		$("#fileupload").addClass("uploading");
	}

	if (typeof FileReader == "undefined") {
		$("#fileupload em").hide();
	}

	// Initialize the jQuery File Upload widget:
	$('#fileupload').fileupload({
		url: "UploadFile.ashx",
		maxFileSize: maxFileSize,
		previewMaxWidth: 48,
		previewMaxHeight: 48,
		autoUpload: true,
		sequentialUploads: true,
		formData: { ticket: ticket, selected: selected }
	})
    .bind('fileuploadstart', onUploadStart)
    .bind('fileuploaddone', onFileUploaded)
    .bind('fileuploadfail', onUploadFailed);


	//        // Load existing files:
	//        $.getJSON($('form').attr("enctype", "multipart/form-data").prop('action'), function (files) {
	//            var fu = $('#fileupload').data('fileupload');
	//            fu._adjustMaxNumberOfFiles(-files.length);
	//            fu._renderDownload(files)
	//            .appendTo($('#fileupload .files'))
	//            .fadeIn(function () {
	//                // Fix for IE7 and lower:
	//                $(this).show();
	//            });
	//        });

	//        // Open download dialogs via iframes,
	//        // to prevent aborting current uploads:
	//        $('#fileupload .files').delegate('a:not([target^=_blank])', 'click', function (e) {
	//            e.preventDefault();
	//            $('<iframe style="display:none;"></iframe>')
	//                .prop('src', this.href)
	//                .appendTo('body');
	//        });
};