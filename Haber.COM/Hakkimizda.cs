using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Haber.COM
{
    public class Hakkimizda
    {
        [Key]
        [Required]
        [Display(Name ="Hakkımızda İçerik ID")]
        public int HakID { get; set; }
        [Required]
        [Display(Name ="Hakkımızda İçerik Başlığı")]
        public string HakBaslik { get; set; }
        [Required]
        [Display(Name ="Hakkımızda İçeriği")]
        public string HakIcerik { get; set; }
        [Required]
        [Display(Name ="Hakkımızda İçerik Eklenme Tarihi")]
        public DateTime HakEklenmeTarihi { get; set; }
        [Required]
        [Display(Name ="Hakkımızda Aktiflik Durumu")]
        public bool HakAktiflik { get; set; }
    }
}
