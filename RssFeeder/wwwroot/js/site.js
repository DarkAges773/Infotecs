$(document).ready(() => {
	$(".js-toggle-raw").each(function () {
		$(this).click(function () {
			$(this).children().toggle();
			let container = $(this).closest(".js-publication");
			$(container).children(".js-html-raw-view").toggle();
			$(container).children(".js-html-view").toggle();
		})
	})
})