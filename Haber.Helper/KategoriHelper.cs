using Haber.COM;
using Haber.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Haber.Helper
{
    public class KategoriHelper
    {
        HaberContext context;
        public KategoriHelper()
        {
            context = new HaberContext();
        }
        public KategoriHelper(HaberContext tContext)
        {
            context = tContext;
        }

        public List<Kategori> TumKategoriler()
        {
            var result = context.Kategoriler.ToList();
            return result;
        }
        public List<Kategori> KategoriAra(string id)
        {
            var result = context.Kategoriler.Where(x => x.KategoriAdi.Contains(id)).ToList();
            return result;
        }
        public Kategori KategoriGetir(int? kategoriID)
        {
            var kategori = context.Kategoriler.Find(kategoriID);
            return kategori;
        }
        public void KategoriKaydet(Kategori kategori)
        {
            context.Kategoriler.Add(kategori);
            context.SaveChanges();
        }
        public int KategoriSil(int id)
        {
            var kategori = KategoriGetir(id);

            var result = (from p in context.Haberler
                          where p.HaberKategori.KategoriID == id
                          select p).FirstOrDefault();

            if (result!=null)
            {
                int a = -1;
                return a;
            }
            else
            {
                context.Kategoriler.Remove(kategori);
                return context.SaveChanges();
            }

            
        }
    }
}
