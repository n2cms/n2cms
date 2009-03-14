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
		
    </div>
    </form>
</body>
</html>
