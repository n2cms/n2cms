<%@ Page Language="C#" AutoEventWireup="true" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
		Carry on, nothing to see here. Just learning som javascript concepts.
		<h4>Closures:</h4>
			<script type="text/javascript">
				var x = function() {
					var i = 0;
					return function() {
						return ++i;
					}
				};

				var f = x();
				document.write("first: " + f());
				document.write(", second: " + f());
				
			</script>
		
		<h4>Polimofism:</h4>
		
		<script type="text/javascript">
			var animal = function() {
				var legs = 4;
				var nipples = 2;
				this.getLegs = function() { return legs; };
				this.getNipples = function() { return nipples; };
			};
			var dog = function() {
				animal.call(this);
				this.getNipples = function() { return 8; };
			}
			var human = function() {
				animal.call(this);
				this.getLegs = function() { return 2; };
			}

			var lassie = new dog();
			var hubert = new human();

			document.write("dog: " + lassie.getLegs() + "/" + lassie.getNipples());
			document.write(", human: " + hubert.getLegs() + "/" + hubert.getNipples());
		</script>
		
		
		<h4>Call:</h4>
		
		
		<script type="text/javascript">
			var blog = function() {
				this.type = "blog";
				this.getType = function() {
					return this.type;
				}
			}
			var joshuasBlog = new blog();

			document.write(joshuasBlog.getType() + "->" + joshuasBlog.getType.call({ type: "hehehe" }));
		</script>
		
		<h4>Prototype</h4>
		
		<script type="text/javascript">
			var y = function(value) {
				this.superValue = value;
				this.getSuperValue = function() { return this.superValue; }
			}
			var x = function(value) {
				this.value = value;
				this.getValue = function() { return this.value; }
			}
			x.prototype = new y(123);
			var a = new x(1);
			var b = new x(2);
			document.write("a.value: " + a.getValue());
			document.write(", b.value: " + b.value);
			document.write(", a.superValue: " + a.getSuperValue());
			document.write(", b.superValue: " + b.superValue);
		
		</script>
		
		<h4>Windows</h4>
		
		<script type="text/javascript">

			document.write("parent title: " + window.parent.document.title);
			document.write("<br/>window.parent==window: " + (window.parent == window));
			document.write("<br/>parent name: " + window.parent.name);
		
		</script>
		
		<h4>Recursive frame prototype</h4>
		
		<script type="text/javascript">
		<%
			int depth = int.Parse(Request["depth"] ?? "2");
			if(depth > 0) { %>
			document.write("<iframe width='75%' id='recusiveFrame' src='<%= N2.Web.Url.Parse(Request.RawUrl).SetQueryParameter("depth", depth - 1) %>'></iframe>");
		<% } %>
			
			var init = function(w) {
				if (w.manager)
					return w.manager;

				try {
					if (w.name != "top" && w != w.parent) {
						var m = function() {
							this.ask2 = function() { alert(window.name) }
						};
						m.prototype = init(w.parent);
						w.manager = new m();
					} else {
						w.manager = {
							ask: function() { alert(w.name); } ,
							ask2: function() { alert(window.name);}
						};
					}
					return w.manager;
				} catch (e) {
					document.write(e.toString());
				}
			};
			init(window);
			document.write("<input onclick='window.manager.ask();' value='ask' type='button' /><input onclick='window.manager.ask2();' value='ask2' type='button' />");
			window.name = "Window " + <%= depth %>;
			document.write("(" + window.name + ")");
		</script>
			
    </div>
    </form>
</body>
</html>
