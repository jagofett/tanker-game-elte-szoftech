using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sztek.Models
{
    public class newLobby
    {
        public int id { get; set; }
        public string lobbyName { get; set; }
        public bool isMainLobby { get; set; }
        public virtual games game { get; set; }

        public virtual List<users> UserList { get; set; } 


    }
}