document.addEventListener("DOMContentLoaded", function () {
    var timeoutId;

    // Maneja el evento de teclado en el campo de búsqueda
    document.getElementById("searchInput").addEventListener("input", function () {
        // Reinicia el temporizador cada vez que se presiona una tecla
        clearTimeout(timeoutId);

        // Establece un temporizador para realizar la búsqueda después de 500 ms
        timeoutId = setTimeout(function () {
            // Obtiene el valor actual del campo de búsqueda
            var searchTerm = document.getElementById("searchInput").value;
            // Llama a la función de búsqueda
            searchPokemon(searchTerm);
        }, 500);
    });

    // Función para realizar la búsqueda y actualizar la lista de Pokémon
    function searchPokemon(searchTerm) {
        // Realiza una solicitud Ajax al servidor para obtener los Pokémon filtrados
        var xhr = new XMLHttpRequest();
        xhr.open("GET", "/Pokemon/Search?searchTerm=" + searchTerm, true);

        xhr.onload = function () {
            if (xhr.status == 200) {
                // Actualiza la lista de Pokémon en la vista parcial "_PokemonList"
                document.getElementById("pokemonList").innerHTML = xhr.responseText;
            } else {
                console.error("Error en la búsqueda: " + xhr.statusText);
            }
        };

        xhr.onerror = function () {
            console.error("Error en la solicitud Ajax");
        };

        xhr.send();
    }
});


function addToTeam(pokemonName) {
    // Obtener el equipo del usuario desde el localStorage
    let userTeam = JSON.parse(localStorage.getItem('userTeam')) || [];

    // Verificar si el Pokémon ya está en el equipo
    if (!userTeam.includes(pokemonName)) {
        // Verificar si el equipo ya tiene 6 Pokémon
        if (userTeam.length < 6) {
            // Agregar el nuevo Pokémon al equipo
            alert("Pokemon: " + pokemonName + " ahora es parte de tu equipo!!!")

            userTeam.push(pokemonName);

            // Almacenar el equipo actualizado en el localStorage
            localStorage.setItem('userTeam', JSON.stringify(userTeam));

            // Actualizar la lista del equipo en la interfaz
            updateTeamList();
            
        } else {
            alert('El equipo ya tiene 6 Pokémon. Elimina uno antes de agregar otro.');
        }
    } else {
        alert('Este Pokémon ya está en tu equipo.');
    }
}

// Función para eliminar un Pokémon del equipo del usuario
function removeFromTeam(pokemonName) {
    let userTeam = JSON.parse(localStorage.getItem('userTeam')) || [];

    // Filtrar el Pokémon que se va a eliminar
    userTeam = userTeam.filter(pokemon => pokemon !== pokemonName);

    // Actualizar el equipo en el localStorage
    localStorage.setItem('userTeam', JSON.stringify(userTeam));

    // Actualizar la lista del equipo en la interfaz
    updateTeamList();
}

// Función para actualizar la lista del equipo en la interfaz
function updateTeamList() {
    let userTeam = JSON.parse(localStorage.getItem('userTeam')) || [];
    let userTeamList = document.getElementById('userTeamList');

    // Limpiar la lista actual
    userTeamList.innerHTML = '';

    // Agregar los Pokémon del equipo a la lista
    userTeam.forEach(pokemon => {
        let listItem = document.createElement('li');
        listItem.textContent = pokemon;
        listItem.className = 'list-group-item d-flex justify-content-between align-items-center';

        // Botón para eliminar el Pokémon del equipo
        let removeButton = document.createElement('button');
        removeButton.textContent = 'Eliminar';
        removeButton.className = 'btn btn-danger';
        removeButton.onclick = function () {
            removeFromTeam(pokemon);
        };

        // Agregar el botón de eliminar al elemento de la lista
        listItem.appendChild(removeButton);

        // Agregar el elemento a la lista
        userTeamList.appendChild(listItem);
    });
}

// Actualizar la lista del equipo en la interfaz al cargar la página
updateTeamList();


//fin de logica para agregar o liminar a el equipo pokemon

//inicio para filtrado