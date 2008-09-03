$(document).ready(function(){
	$(".task h4").click(function(){
		$(this).siblings("p").toggle();
	}).siblings("p").hide();
	
	$("tbody h3").click(function(){
		$(this).siblings("div").toggle();
	});
});