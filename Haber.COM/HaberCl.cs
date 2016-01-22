using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Haber.COM
{
    [Table("Haber")]
    public class HaberCl
    {
        [Key]
        [Required]
        [Display(Name ="Haber ID")]
        public int HaberID { get; set; }
        [Required]
        [Display(Name ="Haber Başlığı")]
        public string HaberBaslik { get; set; }
        [Required]
        [Display(Name ="Haber İçeriği")]
        public string HaberIcerik { get; set; }
        [Required]
        [Display(Name ="Haber Eklenme Tarihi")]
        public DateTime HaberGirisTarihi { get; set; }
        [Required]
        [Display(Name ="Haber Aktiflik Durumu")]
        public bool HaberDurumu { get; set; }
        [Required]
        [Display(Name ="Haber Okunma Sayısı")]
        public int HaberOkunmaSayisi { get; set; }

        public virtual Kategori HaberKategori { get; set; }
        public virtual Yazar HaberYazari { get; set; }

        public virtual List<Yorum> Yorumlar { get; set; }

        public virtual List<Etiket> HaberEtiketleri { get; set; }
        public virtual List<Resim> HaberResimleri { get; set; }
    }
}
