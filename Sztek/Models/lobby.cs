using System;

namespace Sztek.Models
{
    public class lobby
    {
        public int id { get; set; }
        public virtual users users { get; set; }
        public bool status { get; set; }


    }
}