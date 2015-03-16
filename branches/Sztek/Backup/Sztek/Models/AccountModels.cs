using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;

namespace Sztek.Models
{
    public class DatabaseEntities : DbContext
    {
        public DatabaseEntities()
            : base("DefaultConnection")
        {
        }

        //public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<users> Users { get; set; }
        public DbSet<results> Results { get; set; }
        public DbSet<games> Games { get; set; }
        public DbSet<lobby> Lobby { get; set; }
    }

    //[Table("UserProfile")]
    //public class UserProfile
    //{
    //    [Key]
    //    [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
    //    public int UserId { get; set; }
    //    public string UserName { get; set; }
    //}

    public class RegisterExternalLoginModel
    {
        [Required]
        [Display(Name = "Felhasználónév")]
        public string UserName { get; set; }

        public string ExternalLoginData { get; set; }
    }

    public class LocalPasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Jelenlegi jelszó")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "A {0} legalább {2} karakter hosszú kell, hogy legyen.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Új jelszó")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Új jelszó megerősítése")]
        [Compare("NewPassword", ErrorMessage = "A megadott két jelszó nem egyezik.")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginModel
    {
        [Required]
        [Display(Name = "Felhasználónév")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Jelszó")]
        public string Password { get; set; }

        [Display(Name = "Adatok megjegyzése")]
        public bool RememberMe { get; set; }
    }

    public class RegisterModel
    {
        [Required]
        [Display(Name = "Felhasználónév")]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "A {0} legalább {2} karakter hosszú kell, hogy legyen.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Jelszó")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Jelszó megerősítése")]
        [Compare("Password", ErrorMessage = "A megadott két jelszó nem egyezik.")]
        public string ConfirmPassword { get; set; }


        [Display(Name = "Ország")]
        public string Country { get; set; }

        [Display(Name = "Leírás")]
        public string Description { get; set; }


    }


}
