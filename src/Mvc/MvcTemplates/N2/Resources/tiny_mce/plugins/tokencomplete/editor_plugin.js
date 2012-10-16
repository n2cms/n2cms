(function () {
    var key = {
        down: 40,
        up: 38,
        esc: 27,
        enter: 13
    };
    var lastPressedKeys = [];
    var trigger = ['{', '{'];

    var list;

    tinymce.create('tinymce.plugins.TokenCompletePlugin', {
        init: function (ed, url) {
            var options = ed.getParam('tokencomplete_settings');

            list = $('<ul />').addClass('auto-list');

            $(options.tokens).each(function (i, token) {
                var elem = $('<li>' + (token.Description ? token.Name + " " + token.Description : token.Name) + '</li>')
                    .attr("data-name", token.Name)
                    .data("options", token.Options)
            		.click(function () {
            		    insertFromDropdown(this);
            		});

                $(list).prepend(elem);
            });

            $('body').append(list);

            tinymce.DOM.loadCSS(url + '/css/tokencomplete.css');

            /* propagerar inte till editorn, körs i dropdownen */

            function isVisible() {
                return list.is(":visible");
            }

            function keyPressEvent(ed, e) {
                lastPressedKeys.reverse();
                lastPressedKeys[1] = String.fromCharCode(e.charCode);
                if (!(lastPressedKeys < trigger || trigger < lastPressedKeys)) {
                    showDropdown(ed);
                }
            }

            function clickEvent(ed, e) {
                hideDropdown();
                lastPressedKeys = [];
            }

            function keyDownEvent(ed, e) {
                if (isVisible()) {
                    if (e.keyCode == key.enter) {
                        list.children("li.selected").each(function () {
                            insertFromDropdown(this);
                        });
                    } else if (e.keyCode == key.up) {
                        list.children("li.selected:not(:first)").removeClass("selected").prev().addClass("selected");
                    } else if (e.keyCode == key.down) {
                        list.children("li.selected:not(:last)").removeClass("selected").next().addClass("selected");
                    } else if (e.keyCode == key.esc) {
                        hideDropdown();
                    } else {
                        hideDropdown();
                        return;
                    }
                    e.stopPropagation();
                    e.preventDefault();
                }
            }
            //            ed.onKeyUp.addToTop(keyUpEvent);
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
                list.children("li.selected").removeClass("selected");
                list.children("li:first").addClass("selected");
            }

            function hideDropdown() {
                list.hide();
            }

            function insertFromDropdown(el) {
                var text = $(el).attr("data-name");
                var options = $(el).data("options");
                hideDropdown();
                lastPressedKeys = [];
                console.log(text, options);
                ed.selection.setContent(text);
                if (options && options.length > 0) {
                    $(options).each(function (i, o) {
                        ed.selection.setContent('|' + o.Name);
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