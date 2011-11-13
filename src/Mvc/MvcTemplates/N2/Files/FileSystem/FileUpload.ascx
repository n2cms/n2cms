<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FileUpload.ascx.cs" Inherits="N2.Management.Files.FileSystem.FileUpload" %>
<%@ Import Namespace="N2.Edit" %>

<div id="fileupload" class="droparea">
    <p><strong><%= GetLocalResourceString("SelectFiles", "Select files to upload") %></strong></p>
    <p>
        <input id="fuAlternative" runat="server" type="file" name="files[]" multiple="multiple" />
        </p><p>
        <asp:Button runat="server" ID="btnAlternative" OnCommand="OnAlternativeCommand" Text="Upload" CssClass="upload" meta:resuorceKey="btnAlternative" />
        <em><%= GetLocalResourceString("DropFiles", "Or drop files here") %></em>
    </p>
    <div class="fileupload-content">
        <div class="fileupload-progressbar"></div>
        <table class="files"></table>
    </div>
</div>
        
<script src="jquery.iframe-transport.js" type="text/javascript"></script>
<script src="jquery.tmpl.js" type="text/javascript"></script>
<script src="jquery.fileupload.js" type="text/javascript"></script>
<script src="jquery.fileupload-ui.js" type="text/javascript"></script>
<script type="text/javascript">
    var maxFileSize = <%= maxFileSize %>;
    var ticket = '<%= FormsAuthentication.Encrypt(new FormsAuthenticationTicket("SecureUpload-" + Guid.NewGuid(), false, 60)) %>';
    var selected = '<%= Selection.SelectedItem.Path %>';
    function onUploadComplete() {
        $("input.upload").removeAttr("disabled");
        <%= Page.GetRefreshFramesScript(Selection.SelectedItem, N2.Edit.ToolbarArea.Navigation, true) %>;
        window.location = window.location;
    }
    function onUploadStart() { 
        $("input.upload").attr("disabled", "disabled");
    }

    $(function () {
        'use strict';

        // Initialize the jQuery File Upload widget:
        $('#fileupload').fileupload({ 
            url:"UploadFile.ashx", 
            maxFileSize: maxFileSize, 
            previewMaxWidth: 32, 
            previewMaxHeight: 32, 
            autoUpload:true, 
            formData: { ticket:ticket, selected:selected }
        })
            .bind('fileuploadstart', onUploadStart)
            .bind('fileuploaddone', onUploadComplete);

        // Load existing files:
        $.getJSON($('form').attr("enctype", "multipart/form-data").prop('action'), function (files) {
            var fu = $('#fileupload').data('fileupload');
            fu._adjustMaxNumberOfFiles(-files.length);
            fu._renderDownload(files)
            .appendTo($('#fileupload .files'))
            .fadeIn(function () {
                // Fix for IE7 and lower:
                $(this).show();
            });
        });

        // Open download dialogs via iframes,
        // to prevent aborting current uploads:
        $('#fileupload .files').delegate('a:not([target^=_blank])', 'click', function (e) {
            e.preventDefault();
            $('<iframe style="display:none;"></iframe>')
                .prop('src', this.href)
                .appendTo('body');
        });

    });
</script>

<script id="template-upload" type="text/x-jquery-tmpl">
    <tr class="template-upload{{if error}} ui-state-error{{/if}}">
        <td class="preview"></td>
        <td class="name">{{if name}}${name}{{else}}Untitled{{/if}}</td>
        <td class="size">${sizef}</td>
        {{if error}}
            <td class="error" colspan="2">Error:
                {{if error === 'maxFileSize'}}File is too big
                {{else error === 'minFileSize'}}File is too small
                {{else error === 'acceptFileTypes'}}Filetype not allowed
                {{else error === 'maxNumberOfFiles'}}Max number of files exceeded
                {{else}}${error}
                {{/if}}
            </td>
        {{else}}
            <td class="progress"><div></div></td>
            <td class="start"><button>Start</button></td>
        {{/if}}
        <td class="cancel"><button>Cancel</button></td>
    </tr>
</script>
<script id="template-download" type="text/x-jquery-tmpl">
    <tr class="template-download{{if error}} ui-state-error{{/if}}">
        {{if error}}
            <td></td>
            <td class="name">${name}</td>
            <td class="size">${sizef}</td>
            <td class="error" colspan="2">Error:
                {{if error === 1}}File exceeds upload_max_filesize (php.ini directive)
                {{else error === 2}}File exceeds MAX_FILE_SIZE (HTML form directive)
                {{else error === 3}}File was only partially uploaded
                {{else error === 4}}No File was uploaded
                {{else error === 5}}Missing a temporary folder
                {{else error === 6}}Failed to write file to disk
                {{else error === 7}}File upload stopped by extension
                {{else error === 'maxFileSize'}}File is too big
                {{else error === 'minFileSize'}}File is too small
                {{else error === 'acceptFileTypes'}}Filetype not allowed
                {{else error === 'maxNumberOfFiles'}}Max number of files exceeded
                {{else error === 'uploadedBytes'}}Uploaded bytes exceed file size
                {{else error === 'emptyResult'}}Empty file upload result
                {{else}}${error}
                {{/if}}
            </td>
        {{else}}
            <td class="preview">
                {{if thumbnail_url}}
                    <a href="${url}"><img src="${thumbnail_url}"></a>
                {{/if}}
            </td>
            <td class="name">
                <a href="${url}"{{if thumbnail_url}}{{/if}}>${name}</a>
            </td>
            <td class="size">${sizef}</td>
            <td colspan="2"></td>
        {{/if}}
    </tr>
</script>