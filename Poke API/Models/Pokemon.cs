using System;
namespace Poke_API.Models
{
	public class Pokemon
	{
        public string Name { get; set; }
        public string Url { get; set; }
        public string ImageUrl { get; set; }
        public int numPokedex { get; set; }
        public List<string> Types { get; set; }

    }
}

