var map; //define map as a global var

function adjustMapCard() {
  if ($('#currentConditionsCard').is(':visible')) {
    // if currentConditions card is visible, set mapCard to take half the width
    $('#mapCard').removeClass('col-12').addClass('col-md-6');
    $('#map').css('min-height', '500px');
  } else {
    // if currentConditions card is hidden, set mapCard to full width
    $('#mapCard').removeClass('col-md-6').addClass('col-12');
    $('#map').css('min-height', '600px');
  }
}

function initMap() {
  map = new google.maps.Map(document.getElementById('map'), {
    center: { lat: 40.251, lng: -95.489 },
    zoom: 3.5
  });

  //map click listener
  google.maps.event.addListener(map, 'click', function (event) {
    debugger;
    var lat = event.latLng.lat();
    var lng = event.latLng.lng();
    sendCoordinates(lat, lng); //call function to send coordinates to backend (HomeController)
  });
}

function sendCoordinates(lat, lng) {
  $.ajax({
    url: '/Home/GetWeatherDataAsync',
    type: 'POST',
    data: { latitude: lat, longitude: lng },
    success: function (response) {
      if (response.Status === "OK") {
        console.log(response);
        debugger;
        $('#currentConditionsContainer').html(response.HourlyViewHtml);
        $('#currentConditionsCard').show();
        adjustMapCard(); //resize map when 'current' card is visible
        $('#extendedForecastContainer').html(response.ExtendedViewHtml);
        $('#extendedForecastContainer').show();
        alertModal(response.Message, response.Status);
      } else {
        console.log(response);
        alertModal(response.Message, response.Status);
      }
    },
    error: function (xhr, status, error) {
      console.error('Error fetching weather data:', error);
      alertModal('An error occurred while fetching weather data, please try again.', 'ERR');
    }
  });
}

// call adjustMapCard when window is resized to handle dynamic changes
$(window).resize(adjustMapCard);

// initial call to set correct widths on page loads
$(document).ready(function () {
  adjustMapCard();
});