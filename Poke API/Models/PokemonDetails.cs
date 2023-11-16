using System;
namespace Poke_API.Models
{
	public class PokemonDetails
	{
		public string name { get; set; }

		public string id { get; set; }

		public string imagePokemon { get; set; }

        public List<PokemonDetailsType> Types { get; set; }
    }
}

