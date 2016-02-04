using Haber.COM;
using Haber.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Haber.Helper
{
    public class ResimHelper
    {
        HaberContext context;
        public ResimHelper()
        {
            context = new HaberContext();
        }
        public ResimHelper(HaberContext tContext)
        {
            context = tContext;
        }
        public int ResimKaydet(string resimAd)
        {
            Resim r = new Resim();
            r.ResimAdi = resimAd;
            context.Resimler.Add(r);
            context.SaveChanges();
            return r.ResimID;
        }
        public Resim ResimGetir(int? id)
        {
            var resim = context.Resimler.Find(id);
            return resim;
        }
        public List<Resim> ResimListele()
        {
            var result = context.Resimler.ToList();
            return result;
        }
        public HaberCl ResimHaberiniGetir(Resim resim)
        {
            var result = context.Haberler.Where(x => x.HaberResimleri[0].ResimID == resim.ResimID).FirstOrDefault();
            return result;
        }
        public int ResimSil(int id)
        {
            var resim = ResimGetir(id);


            context.Resimler.Remove(resim);
            return context.SaveChanges();

        }
    }
}
