function careerrss($){
	if (top.location.pathname === '/learn-and-develop') {
	    var loader = function() {
	        $.ajax({
	            url: "//www.careerwebsite.com/distrib/jobs/html.cfm?code=xy2m3nKBinUeIvPhV2GLphLo9Ep9Srkq",
	            type: "get", //send it through get method
	            success: function(response) {
	                populateData(response);
	            },
	            error: function(xhr) {
	                console.log('Career feed doesnt work ', xhr);
	            }
	        });
	    }

	    function populateData(respData) {
	        if (respData !== undefined) {
				$(".info-list-career").append(respData);
				 $(".jt_job_position").each(function (index){
					 $(this).replaceWith( "<h6>" + $(this).html() + "</h6>" );
				 });
				//$(".jt_job").wrap("<li></li>");
				$(".jt_alljobs a").addClass("all-link");
				$(".jt_alljobs a").text("all career postings");
	        }
	    }
	    loader();
	}
};
module.exports = careerrss;
