namespace UCTrafficApp.Pages
{
    public partial class HomePage : ContentPage
    {
        private int activeRouteIndex = -1;

        public HomePage()
        {
            InitializeComponent();
            DetectCurrentLocation();
            LoadMap();

            // Navigate event
            MapWebView.Navigated += MapWebView_Navigated;

            // Start continuous location update
            StartLocationUpdates();
        }

        // Detect current location (with geolocation permissions)
        private async void DetectCurrentLocation()
        {
            try
            {
                // Check platform
                if (DeviceInfo.Platform == DevicePlatform.iOS || DeviceInfo.Platform == DevicePlatform.Android)
                {
                    // Request permission to access geolocation
                    var status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                    if (status != PermissionStatus.Granted)
                    {
                        await DisplayAlert("Permission Denied", "Location permission is required to access geolocation.", "OK");
                        return;
                    }

                    var request = new GeolocationRequest(GeolocationAccuracy.Best);
                    var location = await Geolocation.Default.GetLocationAsync(request);

                    if (location != null)
                    {
                        FromEntry.Text = $"{location.Latitude},{location.Longitude}";
                    }
                    else
                    {
                        await DisplayAlert("Location Error", "Unable to get current location. Enter manually.", "OK");
                    }
                }
                else
                {
                    // Geolocation is not supported on Windows or other platforms
                    await DisplayAlert("Geolocation Error", "Geolocation is not supported on this platform. Please enter location manually.", "OK");
                }
            }
            catch (Exception ex)
            {
                // Catch any exceptions related to geolocation and show an error message
                await DisplayAlert("Location Error", $"Unable to get current location. Error: {ex.Message}. Please enter manually.", "OK");
            }
        }

        // Load the Google Map in WebView with directions
        private void LoadMap()
        {
            string html = @"<!DOCTYPE html>
<html>
<head>
<meta name='viewport' content='initial-scale=1.0'/>
<meta charset='utf-8'>
<title>UC Traffic Map</title>
<style>
html,body {height:100%;margin:0;padding:0;}
#map {height:100%;width:100%;}
#directionsPanel {position:absolute;top:10px;right:10px;background:white;max-width:300px;max-height:90%;overflow:auto;padding:10px;font-family:Arial,sans-serif;font-size:14px;z-index:10;}
.route-option {cursor:pointer;margin-bottom:10px;padding:5px;border:1px solid #ccc;border-radius:5px;}
.route-option:hover {background-color:#f0f0f0;}
.route-selected {background-color:#FFD700;}
</style>
<script src='https://maps.googleapis.com/maps/api/js?key=AIzaSyCafYRF6gOBgozQy_77-0C4DZZhICCPAGk'></script>
<script>
let map, directionsService, directionsRenderers=[], userMarker, activeRouteIndex=-1;

function initMap() {
    map = new google.maps.Map(document.getElementById('map'), { zoom:13, center:{lat:39.1031,lng:-84.5120} });
    directionsService = new google.maps.DirectionsService();
    const trafficLayer = new google.maps.TrafficLayer();
    trafficLayer.setMap(map);

    if(navigator.geolocation){
        navigator.geolocation.watchPosition(pos=>{
            const latlng={lat:pos.coords.latitude,lng:pos.coords.longitude};
            if(!userMarker){
                userMarker = new google.maps.Marker({position:latlng,map:map,title:'You',icon:'http://maps.google.com/mapfiles/ms/icons/blue-dot.png'});
            } else { userMarker.setPosition(latlng); }

            if(activeRouteIndex>=0){
                const bounds = directionsRenderers[activeRouteIndex].getDirections().routes[0].bounds;
                bounds.extend(userMarker.getPosition());
                map.fitBounds(bounds);
            }
        }, err=>console.log(err), {enableHighAccuracy:true});
    }
}

function calculateRoute(fromAddr,toAddr){
    directionsRenderers.forEach(dr=>dr.setMap(null));
    directionsRenderers=[];
    activeRouteIndex=-1;

    directionsService.route({
        origin:fromAddr,
        destination:toAddr,
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
                leg.steps.forEach(step=>{panelHtml+=`<li>${step.instructions} (${step.distance.text})</li>`;});
                panelHtml+='</ol></div>';
            });
            document.getElementById('directionsPanel').innerHTML=panelHtml;
        } else { alert('Directions request failed: '+status); }
    });
}

function selectRoute(index){
    activeRouteIndex=index;
    directionsRenderers.forEach((renderer,i)=>{
        const color=(i===index)?'#FF0000':'#0000FF';
        renderer.setOptions({polylineOptions:{strokeColor:color,strokeOpacity:0.7,strokeWeight:5}});
        const panelDiv=document.getElementById('route-'+i);
        if(panelDiv) panelDiv.classList.toggle('route-selected', i===index);
    });

    const bounds=directionsRenderers[index].getDirections().routes[0].bounds;
    if(userMarker) bounds.extend(userMarker.getPosition());
    map.fitBounds(bounds);
}

window.initMap=initMap;
window.calculateRoute=calculateRoute;
window.selectRoute=selectRoute;
</script>
</head>
<body onload='initMap()'>
<div id='map'></div>
<div id='directionsPanel'></div>
</body>
</html>";

            MapWebView.Source = new HtmlWebViewSource { Html = html };
        }

        // WebView navigated event (handle click events on route options)
        private async void MapWebView_Navigated(object? sender, WebNavigatedEventArgs? e)
        {
            await MapWebView.EvaluateJavaScriptAsync(@"
                const panel = document.getElementById('directionsPanel');
                if(panel){
                    const observer = new MutationObserver(() => {
                        document.querySelectorAll('.route-option').forEach((div, i) => {
                            div.onclick = () => { selectRoute(i); };
                        });
                    });
                    observer.observe(panel, { childList: true, subtree: true });
                }
            ");
        }

        // Start continuous location updates
        private async void StartLocationUpdates()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        if (DeviceInfo.Platform == DevicePlatform.iOS || DeviceInfo.Platform == DevicePlatform.Android)
                        {
                            var request = new GeolocationRequest(GeolocationAccuracy.Best);
                            var loc = await Geolocation.Default.GetLocationAsync(request);
                            if (loc != null)
                            {
                                string js = $"if(userMarker) {{ userMarker.setPosition(new google.maps.LatLng({loc.Latitude},{loc.Longitude})); }}";
                                await MapWebView.EvaluateJavaScriptAsync(js);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error updating location: {ex.Message}");
                    }
                    await Task.Delay(1000); // update every second
                }
            });
        }

        // Handle the Go button click to calculate the route
        private async void OnGoClicked(object sender, EventArgs e)
        {
            string from = FromEntry.Text;
            string to = ToEntry.Text;

            if (string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(to))
            {
                await DisplayAlert("Input Error", "Please enter both From and To addresses.", "OK");
                return;
            }

            string js = $"calculateRoute('{from.Replace("'", "\\'")}','{to.Replace("'", "\\'")}')";
            await MapWebView.EvaluateJavaScriptAsync(js);
        }
    }
}
