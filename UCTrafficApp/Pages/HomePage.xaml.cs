using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Dispatching;
using System.Diagnostics;

namespace UCTrafficApp.Pages;

public partial class HomePage : ContentPage
{
    // C# Fields to manage location state
    private Location? lastKnownLocation = null;
    private const string CurrentLocationPlaceholder = "My Current Location (Auto-Set)";

    // NOTE: For development/emulators, ensure the emulator or simulator is set to a Cincinnati location (e.g., Lat: 39.1310, Lng: -84.5160)
    // The Geolocation API relies on the device's reported location.

    public HomePage()
    {
        InitializeComponent();
        RequestLocationAndInit();
    }

    private async void RequestLocationAndInit()
    {
        try
        {
            // Only works on Android/iOS
            if (DeviceInfo.Platform == DevicePlatform.Android || DeviceInfo.Platform == DevicePlatform.iOS)
            {
                // Request LocationWhenInUse permission
                var status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                if (status != PermissionStatus.Granted)
                {
                    await DisplayAlert("Permission Denied", "Location permission is required for the map to work.", "OK");
                    return;
                }
            }

            // Get initial location and set the 'From' field
            await SetInitialLocation();

            LoadMap();
            StartLocationUpdates();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to initialize map: {ex.Message}", "OK");
        }
    }

    private async Task SetInitialLocation()
    {
        try
        {
            // Get a single, medium-accuracy fix for initial setup
            var loc = await Geolocation.Default.GetLocationAsync(new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10)));
            if (loc != null)
            {
                lastKnownLocation = loc;
                FromEntry.Text = CurrentLocationPlaceholder;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to get initial location: {ex.Message}");
            FromEntry.Placeholder = "From (e.g., Cincinnati)";
        }
    }


    private void LoadMap()
    {
        // Using C# Raw String Literal ("""...""") to correctly embed multi-line HTML/JavaScript
        string html = """
            <!DOCTYPE html>
            <html>
            <head>
            <meta name='viewport' content='width=device-width, initial-scale=1.0'/>
            <meta charset='utf-8'>
            <title>UC Traffic Map</title>
            <style>
            html,body {height:100%;margin:0;padding:0;}
            #map {height:100%;width:100%;}
            
            /* Route Panel Styling */
            #directionsPanel {
                position:absolute; top:70px; right:10px; 
                max-width: 80%; 
                width: 280px; 
                max-height: 50vh; 
                overflow-y: auto; 
                padding:12px; 
                background:white; 
                font-family:Arial,sans-serif; 
                font-size:14px; 
                z-index:10; 
                border-radius:12px; 
                box-shadow:0 4px 10px rgba(0,0,0,0.4);
            }
            .route-option {
                cursor:pointer;
                margin-bottom:8px;
                padding:10px;
                border:1px solid #ccc;
                border-radius:8px;
                transition: background-color 0.2s;
            }
            .route-option:hover {
                background-color:#f5f5f5;
            }
            .route-selected {
                background-color:#ffe6a3; 
                border-color: #FF4500;
                font-weight: bold;
            }
            .route-option ol {
                display: none;
            }
            .route-selected ol {
                display: block; 
                padding-left: 20px;
                margin-top: 10px;
                font-weight: normal;
                border-top: 1px solid #eee;
                padding-top: 5px;
            }
            
            /* Navigation Display Style */
            #navigationDisplay {
                position:absolute; top:0; left:0; right:0; background:rgb(255, 255, 255); 
                padding:15px; font-family:Arial,sans-serif; z-index:11; 
                border-bottom: 2px solid #FF4500; text-align:center;
            }
            
            /* Toggle Button Style */
            #togglePanelButton {
                position:absolute; 
                top: 70px; /* Adjusted position to be lower */
                right: 10px; 
                padding: 10px 12px;
                background-color: #FF4500;
                color: white;
                border: none;
                border-radius: 8px;
                box-shadow: 0 2px 5px rgba(0,0,0,0.3);
                cursor: pointer;
                font-size: 16px;
                z-index: 12;
                display: none; /* Hidden until a route is selected */
            }
            #togglePanelButton:hover {
                background-color: #e63e00;
            }
            </style>
            <script src='https://maps.googleapis.com/maps/api/js?key=AIzaSyCafYRF6gOBgozQy_77-0C4DZZhICCPAGk&libraries=geometry'></script>
            <script>
            let map, directionsService, directionsRenderers=[], userMarker, activeRouteIndex=-1, currentStepIndex = -1, geocoder;
            
            // Define the approximate geographical bounds for the Cincinnati area
            const CINCINNATI_BOUNDS = {
                north: 39.55,  // Increased from 39.25 to include Middletown, OH
                south: 38.95,  // South of Florence, KY
                east: -84.25,  // East of Batavia, OH
                west: -84.80,  // West of Lawrenceburg, IN
            };

            function initMap() {
                // Centering closer to UC
                map = new google.maps.Map(document.getElementById('map'), { zoom:13, center:{lat:39.1310,lng:-84.5160} }); 
                directionsService = new google.maps.DirectionsService();
                geocoder = new google.maps.Geocoder(); // Initialize Geocoder
                new google.maps.TrafficLayer().setMap(map);
            }

            // Function to check if a LatLng is within the Cincinnati bounds
            function isWithinCincinnati(latlng) {
                const lat = latlng.lat();
                const lng = latlng.lng();
                return lat <= CINCINNATI_BOUNDS.north &&
                       lat >= CINCINNATI_BOUNDS.south &&
                       lng <= CINCINNATI_BOUNDS.east &&
                       lng >= CINCINNATI_BOUNDS.west;
            }
            
            // Helper function to geocode and check validity
            function geocodeAndCheck(address, callback) {
                // If the address is already coordinates (from "My Current Location"), use them directly
                if (address.includes(',')) {
                    const parts = address.split(',');
                    const lat = parseFloat(parts[0]);
                    const lng = parseFloat(parts[1]);
                    const latLng = new google.maps.LatLng(lat, lng);
                    
                    if (isWithinCincinnati(latLng)) {
                        callback({ location: latLng, address: 'Current Location' }, true);
                    } else {
                        callback(null, false, "Your current location is outside the Cincinnati area.");
                    }
                    return;
                }

                // Geocode the address string
                // We use the bounds as a hint, but still must verify the result
                geocoder.geocode({ address: address, bounds: CINCINNATI_BOUNDS }, (results, status) => {
                    if (status === 'OK' && results[0]) {
                        const latLng = results[0].geometry.location;
                        if (isWithinCincinnati(latLng)) {
                            callback({ location: latLng, address: results[0].formatted_address }, true);
                        } else {
                            callback(null, false, "The address '"+address+"' is outside the Cincinnati area.");
                        }
                    } else {
                        callback(null, false, "Could not locate the address: " + address);
                    }
                });
            }

            function toggleDirectionsPanel() {
                const panel = document.getElementById('directionsPanel');
                if (panel.style.display === 'none' || panel.style.display === '') {
                    panel.style.display = 'block';
                    document.getElementById('togglePanelButton').innerHTML = '&#9776; Hide Routes'; 
                } else {
                    panel.style.display = 'none';
                    document.getElementById('togglePanelButton').innerHTML = '&#9776; Show Routes';
                }
            }
            
            function updateUserLocationAndFollow(lat, lng) {
                const latlng = new google.maps.LatLng(lat, lng); 
                
                if (!userMarker) {
                    userMarker = new google.maps.Marker({
                        position: latlng,
                        map: map,
                        title: 'You',
                        icon: 'http://maps.google.com/mapfiles/ms/icons/blue-dot.png'
                    });
                } else {
                    userMarker.setPosition(latlng);
                }
                
                map.setCenter(latlng);
                
                const navDisplay = document.getElementById('navigationDisplay');
                
                if (activeRouteIndex >= 0 && directionsRenderers[activeRouteIndex] && currentStepIndex >= 0) {
                    
                    if (map.getZoom() < 16) {
                        map.setZoom(16);
                    }
                    
                    const directions = directionsRenderers[activeRouteIndex].getDirections();
                    if (directions && directions.routes.length > activeRouteIndex) {
                        const steps = directions.routes[activeRouteIndex].legs[0].steps;
                        
                        if (currentStepIndex >= steps.length) {
                            navDisplay.innerHTML = '<b><span style="color:green;">You have arrived at your destination!</span></b>';
                            document.getElementById('togglePanelButton').style.display = 'none';
                            return;
                        }

                        let nextStep = steps[currentStepIndex];
                        const distanceToStepEnd = google.maps.geometry.spherical.computeDistanceBetween(latlng, nextStep.end_location);

                        if (distanceToStepEnd < 20) {
                            currentStepIndex++;
                            if (currentStepIndex >= steps.length) {
                                navDisplay.innerHTML = '<b><span style="color:green;">You have arrived at your destination!</span></b>';
                                document.getElementById('togglePanelButton').style.display = 'none';
                                return;
                            }
                            nextStep = steps[currentStepIndex]; 
                        }
                        
                        const instructionDiv = document.createElement('div');
                        instructionDiv.innerHTML = nextStep.instructions;
                        
                        navDisplay.innerHTML = `
                            <div style="font-size:18px; font-weight:bold;">${instructionDiv.innerText}</div>
                            <div style="font-size:14px; color:#444;">${nextStep.distance.text} to next step</div>
                        `;
                        
                    }
                } else {
                     navDisplay.innerHTML = '<i>Enter a route and press "Go" to begin navigation.</i>';
                }
            }


            function calculateRoute(fromAddr,toAddr){
                directionsRenderers.forEach(dr=>dr.setMap(null));
                directionsRenderers=[];
                activeRouteIndex=-1;
                currentStepIndex=-1; 
                
                document.getElementById('directionsPanel').style.display = 'none'; 
                document.getElementById('togglePanelButton').style.display = 'none'; 
                document.getElementById('navigationDisplay').innerHTML = '<i>Checking addresses for Cincinnati bounds...</i>';

                // Geocode and check Origin
                geocodeAndCheck(fromAddr, (originResult, originSuccess, originError) => {
                    if (!originSuccess) {
                        document.getElementById('navigationDisplay').innerHTML = `<b style="color:red;">Origin Error:</b> ${originError}`;
                        return;
                    }

                    // Geocode and check Destination
                    geocodeAndCheck(toAddr, (destResult, destSuccess, destError) => {
                        if (!destSuccess) {
                            document.getElementById('navigationDisplay').innerHTML = `<b style="color:red;">Destination Error:</b> ${destError}`;
                            return;
                        }
                        
                        // Proceed with routing only if both are successful and within bounds
                        document.getElementById('navigationDisplay').innerHTML = '<i>Calculating route...</i>';
                        document.getElementById('directionsPanel').style.display = 'block';

                        directionsService.route({
                            origin: originResult.location, // Use the LatLng object
                            destination: destResult.location, // Use the LatLng object
                            travelMode:'DRIVING',
                            provideRouteAlternatives:true
                        }, function(response,status){
                            if(status==='OK'){
                                let panelHtml='';
                                response.routes.forEach((route,index)=>{
                                    let renderer=new google.maps.DirectionsRenderer({
                                        map:map,
                                        routeIndex:index,
                                        suppressMarkers:false,
                                        polylineOptions:{strokeColor:'#0000FF',strokeOpacity:0.7,strokeWeight:5}
                                    });
                                    renderer.setDirections(response);
                                    directionsRenderers.push(renderer);

                                    const leg=route.legs[0];
                                    panelHtml+=`<div class='route-option' id='route-${index}'>`;
                                    panelHtml+=`<b>Route ${index+1}:</b> ${leg.distance.text}, ETA: ${leg.duration.text}<br/><ol>`;
                                    // Safely insert step instructions
                                    leg.steps.forEach(step=>{
                                        const instructionsDiv = document.createElement('div');
                                        instructionsDiv.innerHTML = step.instructions;
                                        panelHtml+=`<li>${instructionsDiv.innerText} (${step.distance.text})</li>`;
                                    });
                                    panelHtml+='</ol></div>';
                                });
                                document.getElementById('directionsPanel').innerHTML=panelHtml;

                                // Add click handlers
                                document.querySelectorAll('.route-option').forEach((div,i)=>{
                                    div.onclick=()=>selectRoute(i);
                                });

                                // Auto select first route, which initializes currentStepIndex = 0
                                if(response.routes.length>0) selectRoute(0);
                            } else { 
                                console.error('Directions request failed: '+status); 
                                document.getElementById('navigationDisplay').innerHTML = '<i>Route not found.</i>';
                            }
                        });
                    });
                });
            }

            function selectRoute(index){
                activeRouteIndex=index;
                currentStepIndex=0; // Start navigation from the first step
                
                directionsRenderers.forEach((renderer,i)=>{
                    const color=(i===index)?'#FF0000':'#0000FF';
                    renderer.setOptions({polylineOptions:{strokeColor:color,strokeOpacity:0.7,strokeWeight:5}});
                    const panelDiv=document.getElementById('route-'+i);
                    if(panelDiv) panelDiv.classList.toggle('route-selected', i===index);
                });
                
                // Set initial bounds when a route is selected (use fitBounds only on selection)
                if(userMarker && directionsRenderers[index]){
                    const directions = directionsRenderers[index].getDirections();
                    if (directions && directions.routes.length > index) {
                        const bounds=directions.routes[index].bounds;
                        bounds.extend(userMarker.getPosition());
                        map.fitBounds(bounds);
                    }
                }
                
                // HIDE: Hide the panel once a route is confirmed for clear map view
                document.getElementById('directionsPanel').style.display = 'none';
                
                // SHOW: Display the toggle button
                document.getElementById('togglePanelButton').style.display = 'block';
                document.getElementById('togglePanelButton').innerHTML = '&#9776; Show Routes';
            }

            window.initMap=initMap;
            window.calculateRoute=calculateRoute;
            window.selectRoute=selectRoute;
            window.updateUserLocationAndFollow=updateUserLocationAndFollow; // Exposed to C#
            window.toggleDirectionsPanel=toggleDirectionsPanel; // Exposed for button click
            </script>
            </head>
            <body onload='initMap()'>
            <div id='navigationDisplay'><i>Enter a route and press "Go" to begin navigation.</i></div>
            <button id='togglePanelButton' onclick='toggleDirectionsPanel()'>&#9776; Show Routes</button>
            <div id='map'></div>
            <div id='directionsPanel'></div>
            </body>
            </html>
            """;

        MapWebView.Source = new HtmlWebViewSource { Html = html };
    }

    private async void StartLocationUpdates()
    {
        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            while (true)
            {
                try
                {
                    // Check only for platforms that support Geolocation
                    if (DeviceInfo.Platform == DevicePlatform.Android || DeviceInfo.Platform == DevicePlatform.iOS)
                    {
                        var request = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(1));
                        var loc = await Geolocation.Default.GetLocationAsync(request);
                        if (loc != null)
                        {
                            // Update last known location for OnGoClicked
                            lastKnownLocation = loc;

                            // Call the dedicated JS function with lat/lng
                            string js = $"updateUserLocationAndFollow({loc.Latitude},{loc.Longitude});";
                            await MapWebView.EvaluateJavaScriptAsync(js);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log errors but keep the loop running
                    Debug.WriteLine($"Location update error: {ex.Message}");
                }
                await Task.Delay(2000); // Update every 2 seconds
            }
        });
    }

    private async void OnGoClicked(object sender, EventArgs e)
    {
        string from = FromEntry.Text;
        string to = ToEntry.Text;

        if (string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(to))
        {
            await DisplayAlert("Input Error", "Please enter both From and To addresses.", "OK");
            return;
        }

        string originQuery;

        // Check if the user is using the "Current Location" placeholder
        if (from == CurrentLocationPlaceholder && lastKnownLocation != null)
        {
            // Use precise coordinates for the origin, but format it as a string of coordinates
            originQuery = $"{lastKnownLocation.Latitude},{lastKnownLocation.Longitude}";
        }
        else
        {
            // Use the text input for address search, ensuring single quotes are escaped
            originQuery = from.Replace("'", "\\'");
        }

        // JS injection to calculate the route
        // The JS function now handles geocoding and bounds checking.
        string js = $"calculateRoute('{originQuery}','{to.Replace("'", "\\'")}')";
        await MapWebView.EvaluateJavaScriptAsync(js);
    }
}