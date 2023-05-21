function createMap() {
    map = L.map('map').setView([53.677839, 23.829529], 13);
    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '&copy; <a href="https://www.openstreetmap.org/">OpenStreetMap</a> contributors',
        maxZoom:18,
        //minZoom:12
    }).addTo(map);
}

function addMarker(lat, lng, title) {
    L.marker([lat, lng]).addTo(map).bindPopup(title);
}
var popupContent = '<div><button class="marker-button">Button</button></div>';
function test(){
    return xer;
}
