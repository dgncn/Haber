using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Haber.COM
{
    public class Etiket
    {
        [Key]
        [Required]
        [Display(Name ="Etiket ID")]
        public int EtiketID { get; set; }
        [Required]
        [StringLength(100)]
        [Display(Name ="Haber Etiket Adı")]
        public string EtiketAdi { get; set; }

        
        public virtual List<HaberCl> EtiketHaberleri { get; set; }



    }
    public class EtiketSonuc
    {
        public int EtiketSonucID { get; set; }
        public string EtiketSonucAdi { get; set; }
        public int EticketSonucSayisi { get; set; }

    }
}
