// Variable map globalmente
let map;
// Arreglo para Almacenar los marcadores
let markers = [];
//Almacenar las personas obtenidas de la api
let persons = [];

//Funcion para limpiar los marcadores existentes
function clearMarkers(){
    markers.forEach(marker => marker.setMap(null));
    markers = [];
}

// Funcion para anadir un marcador al mapa
function addMarker(position, title, map){
    const marker = new google.maps.Marker({
        position,
        map,
        title
    });
    markers.push(marker);
}

// Funcion para cargar las personas desde la api
async function loadPersons(){
    try{
        const response = await fetch('https://localhost:7062/api/Persona');
        if(!response.ok) throw new Error('Error al cargar las personas');
        personas = await response.json();

        //Llenar el select con las personas
        const select = document.getElementById('person-select');
        select.innerHTML = '<option-value="">-- Selecciona una persona --</option>';
        personas.forEach(persona =>{
            const option = document.createElement('option');
            option.value = persona.id;
            option.textContent = persona.pnombre + " " + persona.papellido;
            select.appendChild(option);

        });
    } catch (error){
        console.error("error", error)
        document.getElementById('person-select').innerHTML = '<option value ="">Error al cargar las personas</option>';
    }
    
}

// Funcion globalmente accesible para acceder al mapa
window.iniciarMapa = function(){
    const coord = { lat: 4.4279668, lng: -75.21346609999999 };

    map = new google.maps.Map(document.getElementById('map'), {
        zoom: 14,
        center: coord
    });

    //Marcador inicial
    addMarker({lat:4.4371816, lng:-75.16474629}, 'Default', map);

    //Cargar las personas al iniciar el mapa
    loadPersons();

};

// Manejar el boton de mostrar ubicacion actual
document.getElementById('show-current').addEventListener('click', async () => {
    try{
        const response = await fetch('https://localhost:7062/api/Ubicaciones/Actuales');
        if (!response.ok) throw new Error('Error al cargar las ubicaciones');
        const ubicaciones = await response.json();

        clearMarkers();
        ubicaciones.forEach(ubicacion => {
            if (ubicacion.ultimaUbicacion) {
                const position = { lat: ubicacion.ultimaUbicacion.latitud, lng: ubicacion.ultimaUbicacion.longitud };
                addMarker(position, ubicacion.nombre, map);
            }
        });

        if (ubicaciones.some(u => u.UltimaUbicacion)) {
            const bounds = new google.maps.LatLngBounds();
            ubicaciones.forEach(ubicacion => {
                if (ubicacion.ultimaUbicacion) {
                    bounds.extend({ lat: ubicacion.ultimaUbicacion.latitud, lng: ubicacion.ultimaUbicacion.longitud });
                }
            });
            map.fitBounds(bounds);
        }

        document.getElementById('info-panel').innerHTML = `
            <h3>Todas las ubicaciones</h3>
            <ul>
                ${ubicaciones
                    .filter(u => u.ultimaUbicacion)
                    .map(u => `<li>${u.nombre}: (${u.ultimaUbicacion.latitud}, ${u.ultimaUbicacion.longitud}) - ${u.ultimaUbicacion.direccion} - ${u.ultimaUbicacion.fecha}</li>`)
                    .join('')}
            </ul>
        `;

    }catch (error){
        console.error('Error:', error);
        document.getElementById('info-panel').innerHTML = `<p>Error al cargar las ubicaciones</p>`;
    }
});

// Nueva función para mostrar el historial de ubicaciones
document.getElementById('show-history').addEventListener('click', async () => {
    const select = document.getElementById('person-select');
    const personaId = select.value;

    if (!personaId) {
        document.getElementById('info-panel').innerHTML = `<p>Por favor selecciona una persona</p>`;
        return;
    }

    try {
        const response = await fetch(`https://localhost:7062/api/Ubicaciones/historial/${personaId}`);
        if (!response.ok) throw new Error('Error al cargar el historial de ubicaciones');
        const historial = await response.json();

        // Obtener el nombre de la persona seleccionada
        const persona = personas.find(p => p.id == personaId);
        const nombrePersona = persona ? `${persona.pnombre} ${persona.papellido}` : 'Persona';

        // Limpiar marcadores existentes
        clearMarkers();

        // Si no hay historial, mostrar un mensaje
        if (historial.length === 0) {
            document.getElementById('info-panel').innerHTML = `<p>No hay historial de ubicaciones para ${nombrePersona}</p>`;
            return;
        }

        // Agregar marcadores para cada ubicación en el historial
        historial.forEach((ubicacion, index) => {
            const position = { lat: ubicacion.latitud, lng: ubicacion.longitud };
            const markerTitle = `${nombrePersona} - Ubicación ${index + 1} (${ubicacion.fecha})`;
            addMarker(position, markerTitle, map);
        });

        // Ajustar el mapa para mostrar todos los marcadores
        const bounds = new google.maps.LatLngBounds();
        historial.forEach(ubicacion => {
            bounds.extend({ lat: ubicacion.latitud, lng: ubicacion.longitud });
        });
        map.fitBounds(bounds);

        // Mostrar el historial en el panel de información
        document.getElementById('info-panel').innerHTML = `
            <h3>Historial de ubicaciones de ${nombrePersona}</h3>
            <ul>
                ${historial
                    .map((ubicacion, index) => `<li>Ubicación ${index + 1}: (${ubicacion.latitud}, ${ubicacion.longitud}) - ${ubicacion.direccion} - ${ubicacion.fecha}</li>`)
                    .join('')}
            </ul>
        `;
    } catch (error) {
        console.error('Error al cargar el historial:', error);
        document.getElementById('info-panel').innerHTML = `<p>Error al cargar el historial de ubicaciones</p>`;
    }
});

// Función accesible globalmente
// window.iniciarMapa = function() {
//     var coord = {lat:4.4279668, lng:-75.21346609999999};
    
//     map = new google.maps.Map(document.getElementById('map'), {
//         zoom: 14,
//         center: coord
//     });
    
//     //La ubicación de mi casa JAJAJJAJAJ
//     var micasa = {lat:4.4371816, lng:-75.16474629};
//     // marcador para verificar que funciona
//     new google.maps.Marker({
        
//         position: micasa,
//         map: map,
//         title: 'Ubicación inicial'
//     });

//     //En este js falta la conexión con la API del backend
//     //para poder implementar los botones con las listas de personas y los marcadores de las ubicaciones
