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
            HaberContext context2 = new HaberContext();
            var list = context2.Haberler.ToList();
            return list;
        }

        public int HaberKaydet(HaberCl haber)
        {
            context.Haberler.Add(haber);

            return context.SaveChanges();

        }
        public List<HaberCl> EtiketeGoreHaberler(int? etiketID)
        {
            List<HaberCl> hbListe = null;
            if (etiketID == null)
            {
                var result = context.Haberler.ToList();
                return result;
            }
            else
            {
                
                var result = context.Etiketler.FirstOrDefault(x => x.EtiketID == etiketID).EtiketHaberleri;

                if (result.Count>0)
                {
                    hbListe = new List<HaberCl>();
                    foreach (var haber in result)
                    {
                        hbListe.Add(haber);
                    }
                }

                //var result2 = context.Haberler.Where(x => x.HaberEtiketleri[0].EtiketID == etiketID).ToList();
                //return result2;
            }
            return hbListe;

        }
        public List<HaberCl> EtiketeGoreHaberler()
        {

            var result = context.Haberler.ToList();
            return result;

        }


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
            var resimListe = (from y in context.Resimler
                              where y.ResimHaber.HaberID == haber.HaberID
                              select y).ToList();
            foreach (var yorum in yorumListe)
            {
                YorumHelper yorumhelper = new YorumHelper(context);
                yorumhelper.YorumSil(yorum.YorumID);
            }
            foreach (var resim in resimListe)
            {
                ResimHelper resimhelper = new ResimHelper(context);
                resimhelper.ResimSil(resim.ResimID);
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
        public List<HaberCl> KategoriyeGoreSonHaberler1(Kategori kategori,int? haberSayisi = null)
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
        public List<HaberCl> KategoriyeGoreSonHaberler(Kategori kategori,  int? haberBaslangicIndexi = null, int istenilenHaberSayisi = 0)
        {
            if (kategori == null && istenilenHaberSayisi == null && haberBaslangicIndexi == null)
            {
                var result = (from p in context.Haberler
                              select p).ToList();

                return result;
            }
            else
            {

                if (haberBaslangicIndexi == null || haberBaslangicIndexi==0)
                {
                    if (istenilenHaberSayisi == null || istenilenHaberSayisi == 0)
                    {
                        var result = (from p in context.Haberler
                                      where p.HaberKategori.KategoriID == kategori.KategoriID
                                      orderby p.HaberGirisTarihi descending
                                      select p).ToList();

                        return result;
                    }
                    else
                    {
                        int sayi = Convert.ToInt32(istenilenHaberSayisi);
                        var result = (from p in context.Haberler
                                      where p.HaberKategori.KategoriID == kategori.KategoriID
                                      orderby p.HaberGirisTarihi descending
                                      select p).Take(sayi).ToList();

                        return result;
                    }
                }
                else
                {
                    if (istenilenHaberSayisi == null || istenilenHaberSayisi == 0)
                    {
                        var result = (from p in context.Haberler
                                      where p.HaberKategori.KategoriID == kategori.KategoriID
                                      orderby p.HaberGirisTarihi descending
                                      select p).ToList();

                        return result;
                    }
                    else
                    {
                        int haberBaslangicIndex = Convert.ToInt32(haberBaslangicIndexi);
                        int sayi = Convert.ToInt32(istenilenHaberSayisi);
                        var result = (from p in context.Haberler
                                      where p.HaberKategori.KategoriID == kategori.KategoriID
                                      orderby p.HaberGirisTarihi descending
                                      select p).Skip(haberBaslangicIndex).Take(sayi).ToList();

                        return result;
                    }
                }

            }
        }

        public List<HaberCl> KategoriyeGoreHaberler()
        {
            var result = context.Haberler.ToList();
            return result;
        }
        public List<HaberCl> KategoriyeGoreHaberler(int? haberAdet)
        {
            if (haberAdet==null)
            {
                var result = context.Haberler.ToList();
                return result;
            }
            else
            {
                int hAdet = Convert.ToInt32(haberAdet);
                var result = context.Haberler.Take(hAdet).ToList();
                return result;
            }
            
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
        public List<HaberCl> BenzerHaberleriGetir(string icerik,HaberCl haber)
        {
            var benzerHaberler = (from p in context.Haberler
                                  where p.HaberBaslik.Contains(icerik) ||
                                  p.HaberIcerik.Contains(icerik)
                                  select p).ToList();

            if (benzerHaberler == null || benzerHaberler.Count == 0)
            {
                benzerHaberler = KategoriyeGoreSonHaberler(haber.HaberKategori, 3);
                return benzerHaberler;
            }
            else if(benzerHaberler.Count==1)
            {
                var haberListesi = KategoriyeGoreSonHaberler(haber.HaberKategori, benzerHaberler.Count,2);
                benzerHaberler.AddRange(haberListesi);
                return benzerHaberler;
            }
            else if (benzerHaberler.Count==2)
            {
                var haberListesi = KategoriyeGoreSonHaberler(haber.HaberKategori, benzerHaberler.Count, 1);
                benzerHaberler.AddRange(haberListesi);
                return benzerHaberler;
            }
            else if(benzerHaberler.Count==3)
            {
                return benzerHaberler;
            }
            else
            {
                benzerHaberler.RemoveRange(3, benzerHaberler.Count - 3);
                return benzerHaberler;
            }
            
        }

        public void HaberOkSayisiGuncelle(HaberCl guncelKaydedilecekHaber)
        {
            HaberCl orijinalGuncellenecekhaber;
            orijinalGuncellenecekhaber = HaberGetir(guncelKaydedilecekHaber.HaberID);
            orijinalGuncellenecekhaber.HaberOkunmaSayisi = guncelKaydedilecekHaber.HaberOkunmaSayisi;
            context.Entry(orijinalGuncellenecekhaber).State = System.Data.Entity.EntityState.Modified;
            context.SaveChanges();
        }
        public void HabereYorumEkle(HaberCl haber,Yorum yorum)
        {
            haber.Yorumlar.Add(yorum);
            var h = HaberGetir(haber.HaberID);
            h.Yorumlar = haber.Yorumlar;
            context.Entry(h).State = System.Data.Entity.EntityState.Modified;
            context.SaveChanges();
        }
        public List<HaberCl> HaberAra(string id)
        {

            var result = (from p in context.Haberler
                          where p.HaberBaslik.Contains(id.Trim()) ||
                          p.HaberIcerik.Contains(id.Trim()) ||
                          p.HaberKategori.KategoriAdi == id.Trim() ||
                          p.HaberYazari.YazarAdSoyad == id.Trim()
                          select p).ToList();
            return result;
        }
    }
}
