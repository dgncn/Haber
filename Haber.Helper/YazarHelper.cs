using Haber.COM;
using Haber.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Haber.Helper
{
    public class YazarHelper
    {
        HaberContext context;

        public YazarHelper()
        {
           context = new HaberContext();
        }
        public YazarHelper(HaberContext tContext)
        {
            context = tContext;
        }

        public List<Yazar> TumYazarlariListele()
        {
            var result = context.Yazarlar.ToList();
            return result;
        }
        public Yazar YazarGetir(int? yazarID)
        {

            var yazar = context.Yazarlar.Find(yazarID);
            return yazar;
        }
        public Yazar YazarGetir(string yazarAdi)
        {

            var yazar = context.Yazarlar.Where(x => x.YazarAdSoyad == yazarAdi.Trim()).SingleOrDefault();
            return yazar;
        }
        public void YazarKaydet(Yazar yazar)
        {
            context.Yazarlar.Add(yazar);
            context.SaveChanges();
        }
        public int YazarSil(int id)
        {
            var yazar = YazarGetir(id);

            var result = (from p in context.Haberler
                          where p.HaberYazari.YazarID == id
                          select p).FirstOrDefault();

            if (result != null)
            {
                int a = -1;
                return a;
            }
            else
            {
                context.Yazarlar.Remove(yazar);
                return context.SaveChanges();
            }


        }
        
    }
    
}
