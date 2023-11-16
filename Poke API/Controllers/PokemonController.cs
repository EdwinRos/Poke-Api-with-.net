using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using Poke_API.Models;
using Newtonsoft.Json;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Poke_API.Controllers
{
    public class PokemonController : Controller
    {

        private const int numeroDeItems = 25;
        private readonly HttpClient _httpClient;
        private readonly HttpClient _onePokemon;

        public PokemonController()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://pokeapi.co/api/v2/")

            };
        }

        public async Task<IActionResult> Index(int page = 1)
        {

            int offset = (page - 1) * numeroDeItems; //Numero de items
            HttpResponseMessage response = await _httpClient.GetAsync($"pokemon?offset={offset}&limit={numeroDeItems}"); //paginacion de la api desde y el limite

            if (response.IsSuccessStatusCode) //si la respuesta es exitosa
            {
                using (var stream = await response.Content.ReadAsStreamAsync())
                using (var reader = new StreamReader(stream))
                using (var jsonReader = new JsonTextReader(reader))
                {
                    var serializer = new JsonSerializer(); //json serializeer

                    var pokemonList = serializer.Deserialize<PokemonLista>(jsonReader); //deserializa el objeto

                    // Modificar la lista de Pokémon para asignar la URL de la imagen
                    foreach (var pokemon in pokemonList.Results)
                    {
                        int pokemonId = ExtractPokemonIdFromUrl(pokemon.Url); // Implementa esta función según la estructura de la URL
                        pokemon.ImageUrl = $"https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/other/official-artwork/{pokemonId}.png";
                        pokemon.numPokedex = pokemonId;


                        // Obtener los nombres de los tipos
                        HttpResponseMessage typeResponse = await _httpClient.GetAsync($"pokemon-form/{pokemonId}");
                        if (typeResponse.IsSuccessStatusCode)
                        {
                            using (var typeStream = await typeResponse.Content.ReadAsStreamAsync())
                            using (var typeReader = new StreamReader(typeStream))
                            using (var typeJsonReader = new JsonTextReader(typeReader))
                            {
                                var typeData = serializer.Deserialize<PokemonTypeData>(typeJsonReader);
                                pokemon.Types = typeData.Types.Select(t => t.Type.Name).ToList();
                            }
                        }
                    }

                    ViewBag.CurrentPage = page;
                    ViewBag.TotalPages = (int)Math.Ceiling((double)pokemonList.Count / numeroDeItems);



                    return View(pokemonList);
                }
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        private int ExtractPokemonIdFromUrl(string url)
        {

            // Buscamos la cadena "/pokemon/" en la URL
            int pokemonIndex = url.IndexOf("/pokemon/");

            if (pokemonIndex != -1)
            {
                // Añadimos la longitud de "/pokemon/" para obtener el inicio del ID
                int startIndex = pokemonIndex + "/pokemon/".Length;

                // Buscamos la siguiente barra después de "/pokemon/"
                int endIndex = url.IndexOf("/", startIndex);

                if (endIndex != -1)
                {
                    // Extraemos la porción de la URL que contiene el ID del Pokémon
                    string pokemonIdSubstring = url.Substring(startIndex, endIndex - startIndex);

                    // Intentamos convertir la porción a un entero
                    if (int.TryParse(pokemonIdSubstring, out int pokemonId))
                    {
                        return pokemonId;
                    }
                }
            }
            return 0;
        }

        //obtener un pokeemon
        public async Task<IActionResult> GetPokemonDetail(int id)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"pokemon-form/{id}");

            if (response.IsSuccessStatusCode)
            {
                using (var stream = await response.Content.ReadAsStreamAsync())
                using (var reader = new StreamReader(stream))
                using (var jsonReader = new JsonTextReader(reader))
                {
                    var serializer = new JsonSerializer();

                    var pokemonDetail = serializer.Deserialize<PokemonDetails>(jsonReader);

                    //imagen
                    pokemonDetail.imagePokemon = $"https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/other/official-artwork/{id}.png";

                    return View(pokemonDetail);
                }
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Search(string searchTerm)
        {
            // Validar si el término de búsqueda está presente
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                // Si el término de búsqueda está vacío, redirige a la acción Index para mostrar todos los Pokémon
                return RedirectToAction("Index");
            }

            // Lógica para obtener Pokémon filtrados según el término de búsqueda
            int offset = 0; // Puedes ajustar esto según tus necesidades
            HttpResponseMessage response = await _httpClient.GetAsync($"pokemon?limit={int.MaxValue}");

            if (response.IsSuccessStatusCode)
            {
                using (var stream = await response.Content.ReadAsStreamAsync())
                using (var reader = new StreamReader(stream))
                using (var jsonReader = new JsonTextReader(reader))
                {
                    var serializer = new JsonSerializer();
                    var pokemonList = serializer.Deserialize<PokemonLista>(jsonReader);

                    // Filtrar la lista de Pokémon según el término de búsqueda
                    var filteredPokemonList = pokemonList.Results
                        .Where(p => p.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                        .ToList();

                    // Modificar la lista de Pokémon para asignar la URL de la imagen y otros detalles
                    foreach (var pokemon in filteredPokemonList)
                    {
                         int pokemonId = ExtractPokemonIdFromUrl(pokemon.Url); // Implementa esta función según la estructura de la URL
                        pokemon.ImageUrl = $"https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/other/official-artwork/{pokemonId}.png";
                        pokemon.numPokedex = pokemonId;

               
                       // Obtener los nombres de los tipos
                         HttpResponseMessage typeResponse = await _httpClient.GetAsync($"pokemon-form/{pokemonId}");
                        if (typeResponse.IsSuccessStatusCode)
                        {
                            using (var typeStream = await typeResponse.Content.ReadAsStreamAsync())
                            using (var typeReader = new StreamReader(typeStream))
                            using (var typeJsonReader = new JsonTextReader(typeReader))
                            {
                                var typeData = serializer.Deserialize<PokemonTypeData>(typeJsonReader);
                                pokemon.Types = typeData.Types.Select(t => t.Type.Name).ToList();
                            }
                        }
                    }

                    // Devolver la vista parcial "_PokemonList" con la lista filtrada
                    return PartialView("_PokemonList", filteredPokemonList);
                }
            }
            else
            {
                return RedirectToAction("Error");
            }
        }
    }
}


