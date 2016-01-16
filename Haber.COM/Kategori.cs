using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Haber.COM
{
    public class Kategori
    {
        [Key]
        [Required]
        [Display(Name ="Kategori ID")]
        public int KategoriID { get; set; }
        [Required]
        [Display(Name ="Kategori Adı")]
        public string KategoriAdi { get; set; }
        [Display(Name ="Kategori Açıklama")]
        public string KategoriAciklama { get; set; }

        public virtual List<HaberCl> Haberler { get; set; }

    }
}
