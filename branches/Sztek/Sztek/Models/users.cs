using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sztek.Models
{
    public class users
    {
        [DisplayName("ID")]
        public int id { get; set; }
        
        [DisplayName("Felhasználónév")]
        [Required(ErrorMessage = "A felhasználónevet kötelező megadni!")]
        public string username { get; set; }

        [DisplayName("Játékok")]
        public virtual games game { get; set; }

        [DisplayName("Játékra vár")]
        [DefaultValue(false)]
        public bool? inLobby { get; set; }

        [DisplayName("Ország")]
        public string country { get; set; }

        [DisplayName("Leírás")]
        public string description { get; set; }


        public virtual ICollection<lobby> lobbyList { get; set; }
        public virtual ICollection<results> results { get; set; }
    }
}