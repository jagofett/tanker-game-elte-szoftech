using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sztek.Models
{
    public class toplistModel
    {
        public int Id { get; set; }

        [DisplayName("Név")]
        public String Name { get; set; }
        [DisplayName("Város")]
        public String Country { get; set; }

        [DisplayName("Pontszám")]
        public int Score { get; set; }
        [DisplayName("No.")]
        public int Place { get; set; }
    }
}