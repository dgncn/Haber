using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Haber.COM;
using Haber.Helper;
using Haber.DAL;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.IO;

namespace Haber.Web.Controllers
{
    public class DashboardController : Controller
    {
        static HaberContext context = new HaberContext();
        HaberClHelper haberhelper = new HaberClHelper(context);
        YazarHelper yazarhelper = new YazarHelper(context);
        KategoriHelper kategorihelper = new KategoriHelper(context);
        EtiketHelper etikethelper = new EtiketHelper(context);
        YorumHelper yorumhelper = new YorumHelper(context);
        ResimHelper resimhelper = new ResimHelper(context);
        static List<Etiket> yenietiketListesi = new List<Etiket>();
        // GET: Dashboard
        public ActionResult Index()
        {
            return View();

        }

        #region Haber Sayfaları

        public ActionResult HaberEkle()
        {
            ViewBag.yazarlar = yazarhelper.TumYazarlariListele();
            ViewBag.kategoriler = kategorihelper.TumKategoriler();

            return View();
        }
        [HttpPost]
        public ActionResult HaberEkle(HaberCl haber, int yazarid, int kategoriID, IEnumerable<HttpPostedFileBase> files)
        {

            haber.HaberGirisTarihi = DateTime.Now;
            haber.HaberKategori = kategorihelper.KategoriGetir(kategoriID);
            haber.HaberYazari = yazarhelper.YazarGetir(yazarid);
            haber.HaberOkunmaSayisi = 0;
            int i1 = 1;
            foreach (var file in files)
            {
                if (file != null && file.ContentLength > 0)
                {

                    var resimAdi = Path.GetFileName(Guid.NewGuid().ToString() + file.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/Galeri"), resimAdi);
                    file.SaveAs(path);
                    // var result = resimhelper.ResimKaydet(resimAdi);
                    // Resim r = resimhelper.ResimGetir(result);


                    Resim r = new Resim { ResimAdi = resimAdi };
                    if (haber.HaberResimleri == null)
                    {
                        haber.HaberResimleri = new List<Resim> { r };
                    }
                    else
                    {
                        haber.HaberResimleri.Add(r);
                    }

                }
                i1++;

            }
            if (haber.HaberEtiketleri[0].EtiketAdi == null)
            {
                haber.HaberEtiketleri = null;
                haberhelper.HaberKaydet(haber);
            }
            else
            {


                var etiketler = etikethelper.TumEtiketleriListele();
                string haberEtiket = haber.HaberEtiketleri[0].EtiketAdi;
                var etiket = etikethelper.EtiketGetir(haberEtiket);
                bool etiketdbdeVarMi = etiketler.Contains(etiket);

                if (etiketdbdeVarMi)
                {

                    Etiket[] etik = new Etiket[haber.HaberEtiketleri.Count()];
                    for (int i = 0; i < haber.HaberEtiketleri.Count(); i++)
                    {
                        etik[i] = haber.HaberEtiketleri[i];
                    }
                    haber.HaberEtiketleri.Clear();
                    context.Haberler.Add(haber);
                    context.SaveChanges();

                    context.Entry(haber).State = EntityState.Unchanged;

                    foreach (var etiket4 in haber.HaberEtiketleri)
                    {
                        context.Entry(etiket4).State = EntityState.Unchanged;
                        EntityState a = new EntityState();
                        a = context.Entry(etiket4).State;
                    }

                    var originalHaber = context.Haberler.Find(haber.HaberID);

                    var updatedHaber = originalHaber;


                    context.Entry(updatedHaber).State = EntityState.Modified;
                    foreach (var eti in etik)
                    {
                        eti.EtiketAdi = eti.EtiketAdi.Trim();
                        updatedHaber.HaberEtiketleri.Add(eti);
                    }


                    context.SaveChanges();

                    #region yorum
                    ////List<Etiket> yenietiketListesi = new List<Etiket>();
                    //yenietiketListesi = haber.HaberEtiketleri;
                    //Etiket[] etik = new Etiket[haber.HaberEtiketleri.Count()];
                    //for (int i = 0; i < haber.HaberEtiketleri.Count(); i++)
                    //{
                    //    etik[i] = haber.HaberEtiketleri[i];
                    //}
                    ////foreach (var etiket2 in haber.HaberEtiketleri)
                    ////{
                    ////    Etiket et = etiket2;
                    ////    haber.HaberEtiketleri.Remove(etiket2);

                    ////    yenietiketListesi.Add(et);
                    ////}
                    //haber.HaberEtiketleri.Clear();
                    //haberhelper.HaberKaydet(haber);
                    //var h = haberhelper.HaberGetir(haber.HaberID);


                    //foreach (var eti in etik)
                    //{
                    //    haberhelper.HabereEtiketEkle(h, eti);
                    //}

                    //context.Entry(h).State = System.Data.Entity.EntityState.Modified;
                    //context.SaveChanges();
                    ////id - kategori - yazar
                    //return RedirectToAction("HaberDuzenle");

                    #endregion

                }
                else
                {
                    foreach (var etiket2 in haber.HaberEtiketleri)
                    {
                        etiket2.EtiketAdi = etiket2.EtiketAdi.Trim();
                    }
                    haberhelper.HaberKaydet(haber);
                }
            }

            return RedirectToAction("HaberDuzenle");




        }

        public ActionResult HaberListele()
        {
            var result = haberhelper.TumHaberleriListele();

            return View(result);

        }
        public ActionResult HaberDuzenle()
        {
            var result = haberhelper.TumHaberleriListele();
            return View(result);
        }
        public ActionResult HaberDuzenleme(int? id)
        {
            //if (id==null)
            //{
            //    id = -1;
            //}
            bool bosMu = string.IsNullOrEmpty(id.ToString());
            if (bosMu)
            {
                return RedirectToAction("HaberDuzenle");
            }
            else
            {
                var result = (from p in context.Haberler
                              where p.HaberID == id
                              select p).FirstOrDefault();
                if (result != null)
                {
                    var haber = haberhelper.HaberGetir(id);
                    ViewBag.yazarlar = yazarhelper.TumYazarlariListele();
                    ViewBag.kategoriler = kategorihelper.TumKategoriler();
                    return View(haber);
                }
                else
                {
                    return RedirectToAction("HaberDuzenle");
                }

            }

        }
        [HttpPost]
        public ActionResult HaberDuzenleme(HaberCl haber, int haberID, int kategoriID, int yazarID, string haberEtiketi, IEnumerable<HttpPostedFileBase> files, List<Resim> resimListe)
        {


            haber.HaberID = haberID;
            haber.HaberKategori = kategorihelper.KategoriGetir(kategoriID);
            haber.HaberYazari = yazarhelper.YazarGetir(yazarID);
            var haberResimleri = haberhelper.HaberResimleriniGetir(haberhelper.HaberGetir(haberID));
            haber.HaberResimleri = haberResimleri;

            if (haber.HaberEtiketleri != null && haber.HaberEtiketleri[0].EtiketAdi == haberEtiketi)
            {

            }
            else
            {
                if (string.IsNullOrWhiteSpace(haberEtiketi) || string.IsNullOrEmpty(haberEtiketi))
                {
                    haber.HaberEtiketleri = new List<Etiket>();
                }
                else
                {


                    var etiket = new Etiket { EtiketAdi = haberEtiketi };
                    if (haber.HaberEtiketleri == null)
                    {
                        haber.HaberEtiketleri = new List<Etiket>();
                        haber.HaberEtiketleri.Add(etiket);
                    }
                    else if (haber.HaberEtiketleri.Count == 0)
                    {
                        haber.HaberEtiketleri.Add(etiket);
                    }
                    
                }
            }
            int i1 = 1;
            foreach (var file in files)
            {
                if (file != null && file.ContentLength > 0)
                {

                    var resimAdi = Path.GetFileName(Guid.NewGuid().ToString() + file.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/Galeri"), resimAdi);
                    file.SaveAs(path);
                    // var result = resimhelper.ResimKaydet(resimAdi);
                    // Resim r = resimhelper.ResimGetir(result);


                    Resim r = new Resim { ResimAdi = resimAdi };
                    if (haber.HaberResimleri == null)
                    {
                        haber.HaberResimleri = new List<Resim> { r };
                    }
                    else
                    {
                        haber.HaberResimleri.Add(r);
                    }

                }
                i1++;

            }
            var h = haberhelper.HaberGetir(haber.HaberID);
            //context.Entry(haber).State = System.Data.Entity.EntityState.Unchanged;

            h.HaberBaslik = haber.HaberBaslik;
            h.HaberIcerik = haber.HaberIcerik;
            h.HaberDurumu = haber.HaberDurumu;
            h.HaberGirisTarihi = haber.HaberGirisTarihi;
            h.HaberKategori = haber.HaberKategori;
            h.HaberYazari = haber.HaberYazari;
            h.HaberEtiketleri = haber.HaberEtiketleri;
            h.HaberResimleri = haber.HaberResimleri;
            h.HaberOkunmaSayisi = haber.HaberOkunmaSayisi;
            context.Entry(h).State = System.Data.Entity.EntityState.Modified;
            context.SaveChanges();
            //id - kategori - yazar
            return RedirectToAction("HaberDuzenle");
        }

        public ActionResult HaberSil(int id)
        {
            haberhelper.HaberSil(id);
            return RedirectToAction("HaberDuzenle");

        }
        #endregion

        #region Kategori Sayfaları



        public ActionResult KategoriListele()
        {
            var result = kategorihelper.TumKategoriler();
            return View(result);

        }
        public ActionResult KategoriEkle()
        {
            return View();
        }
        [HttpPost]
        public ActionResult KategoriEkle(Kategori kategori)
        {
            try
            {
                kategorihelper.KategoriKaydet(kategori);
            }
            catch (Exception)
            {
                return View();
            }

            return RedirectToAction("KategoriListele");
        }
        public ActionResult KategoriDuzenle()
        {

            var result = kategorihelper.TumKategoriler();
            return View(result);
        }

        public ActionResult KategoriDuzenleme(int? id)
        {
            bool bosMu = string.IsNullOrEmpty(id.ToString());
            if (bosMu)
            {
                return RedirectToAction("KategoriDuzenle");
            }
            else
            {
                var result = (from p in kategorihelper.TumKategoriler()
                              where p.KategoriID == id
                              select p).FirstOrDefault();
                if (result != null)
                {
                    var kategori = kategorihelper.KategoriGetir(id);
                    return View(kategori);
                }
                else
                {
                    return RedirectToAction("KategoriDuzenle");
                }

            }
        }
        [HttpPost]
        public ActionResult KategoriDuzenleme(Kategori kategori, int kategoriID)
        {
            var k = kategorihelper.KategoriGetir(kategori.KategoriID);

            k.KategoriAdi = kategori.KategoriAdi;
            k.KategoriAciklama = kategori.KategoriAciklama;

            context.Entry(k).State = System.Data.Entity.EntityState.Modified;
            context.SaveChanges();

            return RedirectToAction("KategoriDuzenle");
        }

        public ActionResult KategoriSil(int id)
        {
            try
            {
                int a = kategorihelper.KategoriSil(id);
                var result = kategorihelper.TumKategoriler();
                if (a == -1)
                {
                    ViewBag.hata3 = "Silmek istediğiniz kategoride haberler yer almaktadır.Sadece kategoride haber yer almayan kategoriler silinebilir. ";

                    return View();
                }
                else
                {
                    ViewBag.hata3 = " Kategori Silindi.";
                    return View();
                }
            }
            catch (Exception)
            {

                return RedirectToAction("KategoriDuzenle");
            }

        }
        public ActionResult KategoriHaberleri()
        {
            ViewBag.kategoriler = kategorihelper.TumKategoriler();
            var result = haberhelper.KategoriyeGoreHaberler();
            return View(result);

        }
        [HttpPost]
        public ActionResult KategoriHaberleri(int kategoriID)
        {
            ViewBag.kategoriler = kategorihelper.TumKategoriler();
            var kategori = kategorihelper.KategoriGetir(kategoriID);
            var result = haberhelper.KategoriyeGoreHaberler(kategori);
            return View(result);

        }

        public ActionResult KategoriHaberleriYeni(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("HaberListele");
            }
            ViewBag.kategoriAd = kategorihelper.KategoriGetir(id).KategoriAdi;
            var result = haberhelper.KategoriyeGoreHaberler(kategorihelper.KategoriGetir(id));
            return View(result);
        }
        #endregion

        #region Yorum Sayfaları


        public ActionResult YorumListele()
        {
            var result = yorumhelper.TumYorumlariListele();
            return View(result);
        }
        public ActionResult YorumEkle()
        {
            ViewBag.haberlistesi = haberhelper.TumHaberleriListele();
            return View();
        }
        [HttpPost]
        public ActionResult YorumEkle(Yorum yorum, int HaberID)
        {
            yorum.YorumYazmaTarihi = DateTime.Now;
            yorum.YorumHaberi = haberhelper.HaberGetir(HaberID);
            try
            {
                yorumhelper.YorumEkle(yorum);
                return RedirectToAction("YorumListele");
            }
            catch (Exception)
            {
                return View();
            }
        }

        public ActionResult YorumDuzenle()
        {

            var result = yorumhelper.TumYorumlariListele();
            return View(result);
        }

        public ActionResult YorumDuzenleme(int? id)
        {
            bool bosMu = string.IsNullOrEmpty(id.ToString());
            if (bosMu)
            {
                return RedirectToAction("YorumDuzenle");
            }
            else
            {
                var result = (from p in yorumhelper.TumYorumlariListele()
                              where p.YorumID == id
                              select p).FirstOrDefault();
                if (result != null)
                {
                    var yorum = yorumhelper.YorumGetir(id);
                    return View(yorum);
                }
                else
                {
                    return RedirectToAction("YorumDuzenle");
                }

            }
        }
        [HttpPost]
        public ActionResult YorumDuzenleme(Yorum yorum, int yorumID)
        {
            var y = yorumhelper.YorumGetir(yorum.YorumID);

            y.YorumIcerik = yorum.YorumIcerik;
            y.YorumDurumu = yorum.YorumDurumu;

            context.Entry(y).State = System.Data.Entity.EntityState.Modified;
            context.SaveChanges();

            return RedirectToAction("YorumDuzenle");
        }

        public ActionResult YorumSil(int id)
        {
            try
            {
                int a = yorumhelper.YorumSil(id);
                var result = yorumhelper.TumYorumlariListele();
                if (a == -1)
                {
                    ViewBag.hata5 = "Silmek istediğiniz kategoride haberler yer almaktadır.Sadece kategoride haber yer almayan kategoriler silinebilir. ";

                    return View();
                }
                else
                {
                    ViewBag.hata5 = " Yorum Silindi.";
                    return View();
                }
            }
            catch (Exception)
            {

                return RedirectToAction("YorumDuzenle");
            }

        }
        #endregion

        #region Yazar Sayfaları

        public ActionResult YazarListele()
        {
            var result = yazarhelper.TumYazarlariListele();
            return View(result);
        }
        public ActionResult YazarEkle()
        {
            return View();
        }
        [HttpPost]
        public ActionResult YazarEkle(Yazar yazar)
        {
            yazarhelper.YazarKaydet(yazar);
            return RedirectToAction("YazarListele");
        }
        public ActionResult YazarDuzenle()
        {
            var result = yazarhelper.TumYazarlariListele();
            return View(result);
        }
        public ActionResult YazarDuzenleme(int? id)
        {
            bool bosMu = string.IsNullOrEmpty(id.ToString());
            if (bosMu)
            {
                return RedirectToAction("YazarDuzenle");
            }
            else
            {
                var result = (from p in yazarhelper.TumYazarlariListele()
                              where p.YazarID == id
                              select p).FirstOrDefault();
                if (result != null)
                {
                    var yazar = yazarhelper.YazarGetir(id);
                    return View(yazar);
                }
                else
                {
                    return RedirectToAction("YazarDuzenle");
                }

            }

        }
        [HttpPost]
        public ActionResult YazarDuzenleme(Yazar yazar, int yazarID)
        {
            var y = yazarhelper.YazarGetir(yazarID);
            y.YazarAdSoyad = yazar.YazarAdSoyad;

            context.Entry(y).State = System.Data.Entity.EntityState.Modified;
            context.SaveChanges();
            return RedirectToAction("YazarListele");

        }

        public ActionResult YazarSil(int id)
        {
            try
            {
                int a = yazarhelper.YazarSil(id);
                var result = yazarhelper.TumYazarlariListele();
                if (a == -1)
                {
                    ViewBag.hata4 = "Silmek istediğiniz yazarın haberleri vardır.Sadece haberi olmayan yazarlar silinebilir. ";

                    return View();
                }
                else
                {
                    ViewBag.hata4 = "Yazar Silindi.";
                    return View();
                }
            }
            catch (Exception)
            {

                return RedirectToAction("YazarDuzenle");
            }
        }

        public ActionResult YazarHaberleri()
        {
            ViewBag.yazarlar = yazarhelper.TumYazarlariListele();
            var result = haberhelper.YazaraGoreHaberler();
            return View(result);
        }

        [HttpPost]
        public ActionResult YazarHaberleri(int yazarID)
        {
            ViewBag.yazarlar = yazarhelper.TumYazarlariListele();
            var yazar = yazarhelper.YazarGetir(yazarID);
            var result = haberhelper.YazaraGoreHaberler(yazar);
            return View(result);

        }
        public ActionResult YazarHaberleriYeni(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("HaberListele");
            }
            ViewBag.yazarAd = yazarhelper.YazarGetir(id).YazarAdSoyad;
            var result = haberhelper.YazaraGoreHaberler(yazarhelper.YazarGetir(id));
            return View(result);
        }

        #endregion

        #region Etiket Sayfaları

        public ActionResult EtiketListele()
        {
            var result = etikethelper.TumEtiketleriListele();
            return View(result);
        }

        public ActionResult EtiketEkle()
        {
            ViewBag.hata = "";
            return View();
        }

        [HttpPost]
        public ActionResult EtiketEkle(Etiket etiket)
        {
            try
            {
                etiket.EtiketAdi = etiket.EtiketAdi.Trim();
                var kayitliMi = etikethelper.EtiketKayitliMi(etiket);

                if (kayitliMi)
                {
                    ViewBag.hata = "Aynı etiket adına sahip bir etiket zaten mevcuttur.";
                    return View();
                }

                etikethelper.EtiketKaydet(etiket);
                return RedirectToAction("EtiketListele");
            }
            catch (Exception)
            {

                return View();
            }
        }



        public ActionResult EtiketDuzenle()
        {
            var result = etikethelper.TumEtiketleriListele();
            return View(result);
        }
        public ActionResult EtiketDuzenleme(int? id)
        {
            bool bosMu = string.IsNullOrEmpty(id.ToString());


            if (bosMu)
            {
                return RedirectToAction("EtiketDuzenle");
            }
            else
            {
                Etiket etiket = etikethelper.EtiketGetir(id);
                ViewBag.etiGoreHaberler = haberhelper.EtiketeGoreHaberler(id);

                ViewBag.tumEtiketler = etikethelper.TumEtiketleriListele();

                return View(etiket);


            }

        }
        [HttpPost]
        public ActionResult EtiketDuzenleme(Etiket etiket, int etiketID)
        {
            var e = etikethelper.EtiketGetir(etiketID);
            e.EtiketAdi = etiket.EtiketAdi.Trim();

            context.Entry(e).State = System.Data.Entity.EntityState.Modified;
            context.SaveChanges();
            return RedirectToAction("EtiketListele");

        }

        public ActionResult EtiketSil(int? id)
        {
            try
            {
                if (id == null)
                {
                    return RedirectToAction("EtiketDuzenle");
                }
                else
                {
                    ViewBag.etiGoreHaberler = haberhelper.EtiketeGoreHaberler(id);

                    ViewBag.tumEtiketler = etikethelper.TumEtiketleriListele();
                    var etiket = etikethelper.EtiketGetir(id);

                    foreach (var haber in (List<HaberCl>)ViewBag.etiGoreHaberler)
                    {
                        haber.HaberEtiketleri.Remove(etiket);
                    }
                    etikethelper.EtiketSil(etiket);
                    return RedirectToAction("EtiketDuzenle");
                }


            }
            catch (Exception)
            {

                return RedirectToAction("EtiketDuzenle");
            }
        }

        public ActionResult EtiketHaberleri()
        {
            ViewBag.etiketlerTum = etikethelper.TumEtiketleriListele();

            var result = haberhelper.EtiketeGoreHaberler();
            return View(result);
        }

        [HttpPost]
        public ActionResult EtiketHaberleri(int etiketID)
        {
            ViewBag.etiketlerTum = etikethelper.TumEtiketleriListele();
            if (etiketID == 0)
            {
                var result = haberhelper.TumHaberleriListele();
                return View(result);
            }
            else
            {
                var etiket = etikethelper.EtiketGetir(etiketID);
                var result = haberhelper.EtiketeGoreHaberler(etiket.EtiketID);
                return View(result);
            }


        }

        public ActionResult EtiketHaberleriYeni(int? id)
        {
            if (string.IsNullOrEmpty(id.ToString()))
            {
                return RedirectToAction("EtiketHaberleri");
            }
            else
            {
                try
                {
                    var etiket = etikethelper.EtiketGetir(id);
                    ViewBag.etiketAdi2 = etiket.EtiketAdi;
                    string etAd = (string)ViewBag.etiketAdi2;

                    var result = (from p in context.Etiketler
                                  where p.EtiketAdi == etAd
                                  select p).ToList();
                    List<HaberCl> haberListesi = null;
                    List<HaberCl> haberListesi2 = null;

                    foreach (var etiket3 in result)
                    {

                        var haberEtiketleri = (from p in context.Etiketler
                                               where p.EtiketHaberleri.Count > 0
                                               select p).ToList();

                        //haberListesi = new List<HaberCl>();
                        var liste = haberEtiketleri.Where(x => x.EtiketID == etiket3.EtiketID);
                        foreach (var etiket4 in liste)
                        {
                            if (haberListesi == null)
                            {
                                haberListesi = haberhelper.EtiketeGoreHaberler(etiket4.EtiketID);
                            }
                            else
                            {
                                haberListesi2 = haberhelper.EtiketeGoreHaberler(etiket4.EtiketID);
                                haberListesi.AddRange(haberListesi2);
                            }
                        }
                    }
                    if (haberListesi == null)
                    {
                        haberListesi = new List<HaberCl>();
                        return View(haberListesi);
                    }
                    else
                    {
                        return View(haberListesi);
                    }
                }
                catch (Exception)
                {

                    return RedirectToAction("EtiketHaberleri");
                }
            }

        }

    }


    #endregion

}