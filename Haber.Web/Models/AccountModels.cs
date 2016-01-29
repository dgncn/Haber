using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Haber.Web.Models
{
    public class RegisterModel
    {
        [Required]
        [Display(Name = "Adınız")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Soyadınız")]
        public string SurName { get; set; }

        [Required]
        [Display(Name = "Kullanıcı adı")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "E-Mail")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Şifre")]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Şifre Tekrar")]
        [Compare("Password", ErrorMessage = "Şifre uyuşmuyor")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginModel
    {
        [Required]
        [Display(Name = "Kullanıcı adı")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "Şifre")]
        public string Password { get; set; }

        [Display(Name = "Beni Hatırla?")]
        public bool RememberMe { get; set; }
    }
}