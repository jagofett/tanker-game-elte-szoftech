using System.Collections.Generic;

namespace Sztek.Models
{
    public class games
    {
        public int id { get; set; }
        public bool status { get; set; }
        public string game_name { get; set; }
        public int max_player { get; set; }

        public virtual ICollection<users> users { get; set; }
        public virtual ICollection<results> results { get; set; }
    }
}