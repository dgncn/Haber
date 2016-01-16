using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Haber.COM
{
    public class Resim
    {
        [Key]
        [Required]
        [Display(Name ="Resim ID")]
        public int ResimID { get; set; }
        [Required]
        [Display(Name ="Resim Dosya Adı")]
        public string ResimAdi { get; set; }

        public virtual HaberCl ResimHaber { get; set; }
        
    }
}
