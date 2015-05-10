using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sztek.Models
{
    public class userGame
    {
        public int id { get; set; }
        public virtual games game { get; set; }
        public virtual users user { get; set; }
        public  int team { get; set; }


    }
}