using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Haber.COM
{
    public class Iletisim
    {
        [Key]
        [Required]
        [Display(Name ="İletişim ID")]
        public int IletisimID { get; set; }
        [Required]
        [Display(Name ="Ad Soyad")]
        public string IltAdSoyad { get; set; }
        [Required]
        [Display(Name ="Email Adresi")]
        public string email { get; set; }
        [Required]
        [Display(Name ="Konu")]
        public string IltKonu { get; set; }
        [Required]
        [Display(Name ="İçerik")]
        public string IltIcerik { get; set; }
        [Required]
        [Display(Name ="Gönderilme Tarihi")]
        public DateTime IltGondermeTarihi { get; set; }
    }
}
