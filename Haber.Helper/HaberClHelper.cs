using Haber.COM;
using Haber.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Haber.Helper
{
    public class HaberClHelper
    {
        HaberContext context;

        public HaberClHelper()
        {
            context = new HaberContext();
        }

        public HaberClHelper(HaberContext tContext)
        {
            context = tContext;
        }

        public List<HaberCl> TumHaberleriListele()
        {
            return context.Haberler.ToList();
        }

        public int HaberKaydet(HaberCl haber)
        {
            context.Haberler.Add(haber);

            return context.SaveChanges();

            //try
            //{

            //}
            //catch (Exception ex)
            //{
            //    context.Haberler.Attach(haber);
            //    context.Entry(haber).Property(x => x.HaberEtiketleri).IsModified = true;
            //    return context.SaveChanges();

            //}

        }
        public List<HaberCl> EtiketeGoreHaberler(int? etiketID)
        {
            if (etiketID == null)
            {
                var result = context.Haberler.ToList();
                return result;
            }
            else
            {
                var result = context.Etiketler.FirstOrDefault(x => x.EtiketID == etiketID).EtiketHaberleri;
                return result;

                //var result2 = context.Haberler.Where(x => x.HaberEtiketleri[0].EtiketID == etiketID).ToList();
                //return result2;
            }

        }
        public List<HaberCl> EtiketeGoreHaberler()
        {

            var result = context.Haberler.ToList();
            return result;

        }

        //public void HaberAttach(HaberCl haber)
        //{
        //    context.Haberler.Attach(haber);
        //    HaberCl h= context.Entry(haber).s;
        //    context.Entry(haber).Property(x => x.HaberEtiketleri[0]).IsModified = true;
        //    context.SaveChanges();
        //}

        public HaberCl HaberGetir(int? id)
        {
            var haber = context.Haberler.Find(id);
            return haber;
        }
        public void HaberSil(int id)
        {
            var haber = context.Haberler.Find(id);
            var yorumListe = (from x in context.Yorumlar
                              where x.YorumHaberi.HaberID == haber.HaberID
                              select x).ToList();
            foreach (var yorum in yorumListe)
            {
                YorumHelper yorumhelper = new YorumHelper(context);
                yorumhelper.YorumSil(yorum.YorumID);
            }
            context.Haberler.Remove(haber);
            context.SaveChanges();
        }
        public List<HaberCl> KategoriyeGoreHaberler(Kategori kategori)
        {
            if (kategori == null)
            {
                var result = (from p in context.Haberler
                              select p).ToList();

                return result;
            }
            else
            {


                var result = (from p in context.Haberler
                              where p.HaberKategori.KategoriID == kategori.KategoriID
                              select p).ToList();

                return result;
            }
        }
        public List<HaberCl> KategoriyeGoreSonHaberler(Kategori kategori,int? haberSayisi = null)
        {
            if (kategori == null && haberSayisi==null)
            {
                var result = (from p in context.Haberler
                              select p).ToList();

                return result;
            }
            else
            {

                if (haberSayisi==null)
                {
                    var result = (from p in context.Haberler
                                  where p.HaberKategori.KategoriID == kategori.KategoriID
                                  orderby p.HaberGirisTarihi descending
                                  select p).ToList();

                    return result;
                }
                else
                {
                    int sayi = Convert.ToInt32(haberSayisi);
                    var result = (from p in context.Haberler
                                  where p.HaberKategori.KategoriID == kategori.KategoriID
                                  orderby p.HaberGirisTarihi descending
                                  select p).Take(sayi).ToList();

                    return result;
                }
                
            }
        }

        public List<HaberCl> KategoriyeGoreHaberler()
        {
            var result = context.Haberler.ToList();
            return result;
        }
        public List<HaberCl> KategoriyeGoreHaberler(int haberAdet)
        {
            var result = context.Haberler.Take(haberAdet).ToList();
            return result;
        }
        public List<HaberCl> YazaraGoreHaberler()
        {
            var result = context.Haberler.ToList();
            return result;
        }
        public List<HaberCl> YazaraGoreHaberler(Yazar yazar)
        {
            if (yazar == null)
            {
                var result = (from p in context.Haberler
                              select p).ToList();

                return result;
            }
            else
            {


                var result = (from p in context.Haberler
                              where p.HaberYazari.YazarID == yazar.YazarID
                              select p).ToList();

                return result;
            }
        }

        public void HabereEtiketEkle(HaberCl haber, Etiket etiket)
        {
            haber.HaberEtiketleri.Add(etiket);

        }
        public List<Resim> HaberResimleriniGetir(HaberCl haber)
        {
            var result = (from p in context.Resimler
                          where p.ResimHaber.HaberID == haber.HaberID
                          select p).ToList();
            return result;
        }
        
    }
}
