function initMap() {
  let map = new google.maps.Map(document.getElementById('map'), {
    center: { lat: 40.251, lng: -95.489 },
    zoom: 3.5
  });

  google.maps.event.addListener(map, 'click', function (event) {
    debugger;
    var lat = event.latLng.lat();
    var lng = event.latLng.lng();
    sendCoordinates(lat, lng); //call function to send coordinates
  });
}

function sendCoordinates(lat, lng) {
  debugger;
  $.ajax({
    url: '/Home/GetWeatherDataAsync',
    type: 'POST',
    data: { latitude: lat, longitude: lng },
    success: function (response) {
      if (response.Status === "OK") {
        console.log(response);
        $('#currentConditionsContainer').html(response.RenderedPartialViewHtml);
        $('#currentConditionsCard').show(); 
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

  //function mapDataToViewModel(hourlyForecasts, officeData) {
  //  debugger;
  //  $.ajax({
  //    url: '/Home/MapDataToViewModel',
  //    type: 'POST',
  //    contentType: 'application/json',
  //    data: { hourlyForecasts: hourlyForecasts, officeData: officeData },
  //    success: function (viewModel) {
  //      debugger;
  //      console.log('returned hourlyForecasts, officeData to mapDataToViewModel ');
  //      // Update UI with the viewModel?
  //      $('#currentConditionsContainer').html(viewModel);
  //    },
  //    error: function (xhr, status, error) {
  //      console.error('Error mapping data to view model:', error);
  //    }
  //  });
  //}

  //function updateForecastCards(forecastData) {
  //  // Update Bootstrap cards with forecast data
  //  debugger;
  //  $('#currentConditions').text(forecastData.summary);
  //}
}