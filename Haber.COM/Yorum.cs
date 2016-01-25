using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Haber.COM
{
    public class Yorum
    {
        [Key]
        [Required]
        [Display(Name ="Yorum ID")]
        public int YorumID { get; set; }
        [Required]
        [Display(Name ="YorumYazari")]
        public string YorumYazari { get; set; }
        [Required]
        [Display(Name ="Yorum İçeriği")]
        public string YorumIcerik { get; set; }
        [Required]
        [Display(Name ="Yorum Yazma Tarihi")]
        public DateTime YorumYazmaTarihi { get; set; }
        [Required]
        [Display(Name ="Yorum Aktiflik Durumu")]
        public bool YorumDurumu { get; set; }

        public virtual HaberCl YorumHaberi { get; set; }

    }
}
