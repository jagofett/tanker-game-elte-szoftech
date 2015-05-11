using System;
using System.Collections.Generic;

namespace Sztek.Models
{
    public class lobby
    {
        public int id { get; set; }
        public string lobbyName { get; set; }
        public bool isMainLobby { get; set; }
        public virtual games game { get; set; }

        //public virtual ICollection<users> userList { get; set; }
        //public bool status { get; set; }
    }
}