using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sztek.Models
{
    public class EditUserModel
    {
        public LocalPasswordModel PasswordModel { get; set; }
        public users UserModel { get; set; }
    }
}