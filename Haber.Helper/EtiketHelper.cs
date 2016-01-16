using Haber.COM;
using Haber.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Haber.Helper
{
    public class EtiketHelper
    {
        HaberContext context;
        public EtiketHelper()
        {
            context = new HaberContext();
        }
        public EtiketHelper(HaberContext tContext)
        {
            context = tContext;
        }

        public void EtiketKaydet(Etiket etiket)
        {

            context.Etiketler.Add(etiket);
            context.SaveChanges();

        }

        public bool EtiketKayitliMi(Etiket etiket)
        {
            
            bool kayitli = false;

            //if (context.Etiketler.Where(x => x.EtiketAdi.Contains(etiket.EtiketAdi)).FirstOrDefault() != null)
            //{
            //    kayitli = true;

            //}

            var result = (from p in context.Etiketler
                          where p.EtiketAdi == etiket.EtiketAdi
                          select p).FirstOrDefault();
            if (result!=null)
            {
                kayitli = true;
            }
            return kayitli;

        }
        public void EtiketeHaberEkle(Etiket etiket, HaberCl haber)
        {
            etiket.EtiketHaberleri.Add(haber);
        }

        public List<Etiket> TumEtiketleriListele()
        {
            var result = context.Etiketler.OrderBy(x=>x.EtiketID).ToList();
            return result;
        }
        public Etiket EtiketGetir(int? id)
        {
            var result = context.Etiketler.Find(id);
            return result;
        }
        public Etiket EtiketGetir(string etiketAdi)
        {
            var result = (from p in context.Etiketler
                          where p.EtiketAdi == etiketAdi
                          select p).FirstOrDefault();
            return result;
        }
        public void EtiketSil(Etiket etiket)
        {
            context.Etiketler.Remove(etiket);
            context.SaveChanges();
        }
    }
}
