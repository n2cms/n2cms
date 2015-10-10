n2autosave = {
	interval: 2000,
	dirty: false,
	destroy: function(){
		clearTimeout(this.intervalHandle);
	},
	init: function () {
		console.log("autosave init")

		var self = this;
		this.intervalHandle = setInterval(function () {
			var dirtbags = [];
			jQuery.each(self.adapters, function () {
				if (this.dirty()) {
					dirtbags.push(this);
				}
			});
			if (self.dirty) {
				//self.dirty = false;
				var items = {};
				jQuery.each(dirtbags, function () {
					var item = items[this.itemID] || (items[this.itemID] = {});
					item[this.name] = this.checkout();
				});

				console.log("saving", dirtbags, items);
				var prev = null;
				jQuery.each(items, function (id) {
					//var deferred = jQuery.post("/N2/Api/Content.ashx/autosave?n2item=" + id, this, "application/json");
					var deferred = $.ajax({
						url: "/N2/Api/Content.ashx/autosave?n2item=" + id,
						type: 'post',
						contentType: "application/json",
						success: function () { console.log("success", id); },
						data: JSON.stringify(this)
					});
					if (prev)
						prev = prev.then(deferred);
					else
						prev = deferred;
				})
				prev.then(function () {
					console.log("ALL DONE", arguments);
					self.dirty = false;
				})
			} else if (dirtbags.length) {
				console.log("wait", dirtbags);
				self.dirty = true;
			}
		}, this.interval);
	},
	register: function (editorID, name, adapter) {
		function resolve(parent, key) {
			var dotIndex = key.indexOf(".");
			if (dotIndex < 0) {
				return parent[key];
			} else {
				return resolve(parent[key.substr(0, dotIndex)], key.substr(dotIndex + 1));
			}
		}
		var itemID = parseInt($("#" + editorID).closest("[data-n2-item]").attr("data-n2-item"));
		var fn = resolve(window, adapter);
		fn.prototype = { editorID: editorID, itemID: itemID, name: name, dirty: function () { return undefined; }, checkout: function () { return undefined; } };
		this.adapters.push(new fn());
	},
	adapters: [],
	input: function () {
		var self = this;
		var dirty = false;
		$("#" + this.editorID).on("change", function () {
			dirty = true;
		})
		this.dirty = function () {
			return dirty;
		}
		this.checkout = function () {
			dirty = false;
			return $("#" + this.editorID).val();
		}
	},
	ckeditor: function () {
		this.dirty = function () {
			return CKEDITOR.instances[this.editorID].checkDirty();
		}
		this.checkout = function () {
			var editor = CKEDITOR.instances[this.editorID];
			editor.resetDirty();
			return editor.getData();
		}
	}
}

jQuery(function () {
	n2autosave.init();
});