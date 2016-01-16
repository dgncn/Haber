using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Haber.COM
{
    public class Yazar
    {
        [Key]
        [Required]
        [Display(Name ="Yazar ID")]
        public int YazarID { get; set; }
        [Required]
        [Display(Name ="Yazar Adı Soyadı")]
        public string YazarAdSoyad { get; set; }

        public virtual List<HaberCl> YazarHaberListesi { get; set; }

    }
}
