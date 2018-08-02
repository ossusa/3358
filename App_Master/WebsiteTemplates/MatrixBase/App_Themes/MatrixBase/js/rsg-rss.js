function rss($){
	if($('.rss-container').length > 0){
    var loader = function () {
        $.ajax({
            url: "https://www.iafcams.org/components/api.cfc"
            , type: "get", //send it through get method
            data: {
                'method': 'gettop3'
            }
            , success: function (response) {
                // console.log('data is', populateData(response));
				populateData(response);
            }
            , error: function (xhr) {
                console.log('err', xhr);
            }
        });
    }

    function populateData(respData) {
        if (respData !== undefined) {
            var vols, careers;

            // $.each(respData.vol.DATA, function (idx, item){
            //     console.log("index is " + idx);
            //         $(".gold ul").append('<li>'+ "Volunteer Department" + '</li><li class="rsg-red">' + respData.vol.DATA[idx][1] + '</li>')
            //             .append('<li class="rsg-gray">' + respData.careercombo.DATA[idx][4] + '</li>');
            //     });
                $(".gold ul").append('<li>'+ "Volunteer Department" + '</li><li class="rsg-red">' + respData.vol.DATA[0][1] + '</li>')
                    .append('<li class="rsg-gray">' + respData.vol.DATA[0][4] + '</li>');
                $(".gold ul").append('<li class="Gold-Level">Gold-Level</li><li>'+ "Career / Combination Department" + '</li><li class="rsg-red">' + respData.careercombo.DATA[0][1] + '</li>')
                    .append('<li class="rsg-gray">' + respData.careercombo.DATA[0][4] + '</li>');
                $(".silver ul").append('<li>'+ "Volunteer Department" + '</li><li class="rsg-red">' + respData.vol.DATA[1][1] + '</li>')
                    .append('<li class="rsg-gray">' + respData.vol.DATA[1][4] + '</li>');
                $(".silver ul").append('<li class="Silver-Level">Silver-Level</li><li>'+ "Career / Combination Department" + '</li><li class="rsg-red">' + respData.careercombo.DATA[1][1] + '</li>')
                    .append('<li class="rsg-gray">' + respData.careercombo.DATA[1][4] + '</li>');
                $(".bronze ul").append('<li>'+ "Volunteer Department" + '</li><li class="rsg-red">' + respData.vol.DATA[2][1] + '</li>')
                    .append('<li class="rsg-gray">' + respData.vol.DATA[2][4] + '</li>');
                $(".bronze ul").append('<li class="Bronze-Level">Bronze-Level</li><li>'+ "Career / Combination Department" + '</li><li class="rsg-red">' + respData.careercombo.DATA[2][1] + '</li>')
                    .append('<li class="rsg-gray">' + respData.careercombo.DATA[2][4] + '</li>');
                $('.rss-container').slick({
                    dots: true,
                    infinite: true,
                    autoplay: true,
                    autoplaySpeed: 3000,
                    speed: 300,
                    adaptiveHeight: false
                });

        }
    }
    loader();
	}
};
module.exports = rss;
