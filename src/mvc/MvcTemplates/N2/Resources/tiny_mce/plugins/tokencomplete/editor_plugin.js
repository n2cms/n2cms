(function () {
    var downKey = 40;
    var upKey = 38;
    var escKey = 27;
    var enterKey = 13;
    var lastPressedKeys = [];
    var trigger = ['{', '{'];

    var list;

    tinymce.create('tinymce.plugins.TokenCompletePlugin', {
        init: function (ed, url) {
            var json = ed.getParam('tokens');

            list = $('<ul />').addClass('auto-list');

            $(json.tokens).each(function (i, e) {
                var elem = $('<li>' + e.name + '</li>')
            		.click(function () {
            		    insertFromDropdown($(this).text(), e.options);
            		});

                console.log(e);
                $(list).prepend(elem);
            });

            $('body').append(list);

            tinymce.DOM.loadCSS(url + '/css/tokencomplete.css');

            /* propagerar inte till editorn, körs i dropdownen */
            function keyDownEvent(ed, e) {
            }

            function keyPressEvent(ed, e) {
                lastPressedKeys.reverse();
                lastPressedKeys[1] = String.fromCharCode(e.charCode);

                if (!(lastPressedKeys < trigger || trigger < lastPressedKeys)) {
                    showDropdown(ed);
                }
                else {
                    hideDropdown();
                }
            }

            function keyUpEvent(ed, e) {
            }

            function clickEvent(ed, e) {
                hideDropdown();
                lastPressedKeys = [];
            }

            ed.onKeyUp.addToTop(keyUpEvent);
            ed.onKeyDown.addToTop(keyDownEvent);
            ed.onKeyPress.addToTop(keyPressEvent);
            ed.onClick.add(clickEvent);

            function showDropdown(ed) {
                var tinymcePosition = $(ed.getContainer()).position();
                var toolbarPosition = $(ed.getContainer()).find(".mceToolbar").first();
                var nodePosition = $(ed.selection.getNode()).position();
                var textareaTop = 0;
                var textareaLeft = 0;

                if (ed.selection.getRng().getClientRects().length > 0) {
                    textareaTop = ed.selection.getRng().getClientRects()[0].top + ed.selection.getRng().getClientRects()[0].height;
                    textareaLeft = ed.selection.getRng().getClientRects()[0].left;
                } else {
                    textareaTop = parseInt($(ed.selection.getNode()).css("font-size")) * 1.3 + nodePosition.top;
                    textareaLeft = nodePosition.left;
                }

                $(list).css("margin-top", tinymcePosition.top + toolbarPosition.innerHeight() + textareaTop);
                $(list).css("margin-left", tinymcePosition.left + textareaLeft);
                $(list).css("display", "block");

                list.show();
            }

            function hideDropdown() {
                list.hide();
            }

            function insertFromDropdown(text, options) {
                hideDropdown();
                lastPressedKeys = [];

                ed.selection.setContent(text);

                if (options != null && options.length > 0) {
                    $(options).each(function (i, o) {
                        ed.selection.setContent('|' + o.name);
                    });
                }

                ed.selection.setContent('}}');

                tinyMCE.activeEditor.focus();
            }
        },

        getInfo: function () {
            return {
                longname: 'N2 TokenComplete',
                author: 'Magnus von Wachenfeldt',
                authorurl: 'http://www.deckhand.se',
                infourl: 'http://www.deckhand.se',
                version: tinymce.majorVersion + "." + tinymce.minorVersion
            };
        }
    });

    tinymce.PluginManager.add('tokencomplete', tinymce.plugins.TokenCompletePlugin);
})();