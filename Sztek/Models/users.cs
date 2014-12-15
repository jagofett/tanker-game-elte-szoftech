using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Sztek.Models
{
    public class users
    {
        public int id { get; set; }
        public string username { get; set; }
        public virtual games game { get; set; }
        [DefaultValue(false)]
        public bool? in_lobby { get; set; }
        public string country { get; set; }
        public string description { get; set; }


        public virtual ICollection<lobby> lobby { get; set; }
        public virtual ICollection<results> results { get; set; }
    }
}