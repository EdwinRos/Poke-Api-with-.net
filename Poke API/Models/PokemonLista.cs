using System;
using System.Collections.Generic;
namespace Poke_API.Models
{
	public class PokemonLista
	{
        public int Count { get; set; }
        public List<Pokemon>? Results { get; set; }
    }
}

