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
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Haber.Web.Models;
using System.Net;

namespace Haber.Web.Controllers
{
    //[Authorize(Roles ="Admin")]
    [Authorize(Roles =ClassOfUserRoles.SuperAdmin +","+ClassOfUserRoles.Admin + "," + ClassOfUserRoles.NewsWriter)]
    public class DashboardController : Controller
    {
        static HaberContext context = new HaberContext();
        HaberClHelper haberhelper = new HaberClHelper(context);
        YazarHelper yazarhelper = new YazarHelper(context);
        KategoriHelper kategorihelper = new KategoriHelper(context);
        EtiketHelper etikethelper = new EtiketHelper(context);
        YorumHelper yorumhelper = new YorumHelper(context);
        ResimHelper resimhelper = new ResimHelper(context);
        HakkindaHelper hakkindahelper = new HakkindaHelper(context);
        IletisimHelper iletisimhelper = new IletisimHelper(context);
        static List<Etiket> yenietiketListesi = new List<Etiket>();


        private UserManager<HaberUser> userManager;
        private RoleManager<HaberRole> roleManager;

        

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
            ViewBag.test = "HttpGet";

            return View();
        }


        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult HaberEkle(HaberCl haber, int yazarid, int kategoriID, IEnumerable<HttpPostedFileBase> files)
        {
            ViewBag.test = "HttpPost";
            if (haber.HaberEtiketleri[0].EtiketAdi==null)
            {
                haber.HaberEtiketleri.Clear();
            }
            if (ModelState.IsValid)
            {
                haber.HaberIcerik = System.Net.WebUtility.HtmlDecode(haber.HaberIcerik);
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
                    string etiketOrn = haber.HaberEtiketleri[0].EtiketAdi;
                    //birden fazla virgül ile ayrılan etiket ekleme
                    if (etiketOrn.Contains(','))
                    {
                        string[] etiketDizim = etiketOrn.Split(',');
                        haber.HaberEtiketleri.Clear();
                        foreach (var etiketE in etiketDizim)
                        {
                            haber.HaberEtiketleri.Add(new Etiket { EtiketAdi = etiketE.Trim() });
                        }
                        haberhelper.HaberKaydet(haber);
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
            ViewBag.yazarlar = yazarhelper.TumYazarlariListele();
            ViewBag.kategoriler = kategorihelper.TumKategoriler();
            return View(haber);
        }

        
        public ActionResult HaberListele()
        {
            var result = haberhelper.TumHaberleriListele().OrderByDescending(x => x.HaberGirisTarihi).ToList();

            return View(result);

        }
        
        public ActionResult HaberDuzenle()
        {
            //HaberClHelper haberhelper2 = new HaberClHelper(context);
            var result = haberhelper.TumHaberleriListele().OrderByDescending(x => x.HaberGirisTarihi).ToList();
            return View(result);
        }
        
        public ActionResult HaberDuzenleme(int? id)
        {
            //if (id==null)
            //{
            //    id = -1;
            //}
            ViewBag.test2 = "HttpGet";
            bool bosMu = string.IsNullOrEmpty(id.ToString());
            if (bosMu)
            {
                return RedirectToAction("HaberDuzenle");
            }
            else
            {
                ViewBag.haberOkSayi = haberhelper.HaberGetir(id).HaberOkunmaSayisi;
                var result = (from p in context.Haberler
                              where p.HaberID == id
                              select p).FirstOrDefault();
                if (result != null)
                {
                    var haber = haberhelper.HaberGetir(id);
                    ViewBag.yazarlar = yazarhelper.TumYazarlariListele();
                    ViewBag.kategoriler = kategorihelper.TumKategoriler();
                    haber.HaberOkunmaSayisi = (int)ViewBag.haberOkSayi;
                    return View(haber);
                }
                else
                {
                    return RedirectToAction("HaberDuzenle");
                }

            }

        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult HaberDuzenleme(HaberCl haber, int haberID, int kategoriID, int yazarID, string haberEtiketi, IEnumerable<HttpPostedFileBase> files, List<Resim> resimListe)
        {
            ViewBag.test2 = "HttpPost";
            if (ModelState.IsValid)
            {
                haber.HaberIcerik = System.Net.WebUtility.HtmlDecode(haber.HaberIcerik);
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
            else
            {
                ViewBag.haberOkSayi = haberhelper.HaberGetir(haber.HaberID).HaberOkunmaSayisi;
                ViewBag.yazarlar = yazarhelper.TumYazarlariListele();
                ViewBag.kategoriler = kategorihelper.TumKategoriler();
                
                return View(haber);
            }
        }

        
        public ActionResult HaberSil(int id)
        {
            haberhelper.HaberSil(id);
            return RedirectToAction("HaberDuzenle");

        }
        #endregion

        #region Kategori Sayfaları


        [Authorize(Roles = ClassOfUserRoles.SuperAdmin + "," + ClassOfUserRoles.Admin)]
        public ActionResult KategoriListele()
        {
            var result = kategorihelper.TumKategoriler();
            return View(result);

        }
        [Authorize(Roles = ClassOfUserRoles.SuperAdmin + "," + ClassOfUserRoles.Admin)]
        public ActionResult KategoriEkle()
        {
            return View();
        }
        [Authorize(Roles = ClassOfUserRoles.SuperAdmin + "," + ClassOfUserRoles.Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult KategoriEkle(Kategori kategori)
        {
            if (ModelState.IsValid)
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
            else
            {
                return View(kategori);
            }
        }
        [Authorize(Roles = ClassOfUserRoles.SuperAdmin + "," + ClassOfUserRoles.Admin)]
        public ActionResult KategoriDuzenle()
        {

            var result = kategorihelper.TumKategoriler();
            return View(result);
        }
        [Authorize(Roles = ClassOfUserRoles.SuperAdmin + "," + ClassOfUserRoles.Admin)]

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
        [Authorize(Roles = ClassOfUserRoles.SuperAdmin + "," + ClassOfUserRoles.Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult KategoriDuzenleme(Kategori kategori, int kategoriID)
        {
            var k = kategorihelper.KategoriGetir(kategori.KategoriID);
            if (ModelState.IsValid)
            {

                k.KategoriAdi = kategori.KategoriAdi;
                k.KategoriAciklama = kategori.KategoriAciklama;

                context.Entry(k).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();

                return RedirectToAction("KategoriDuzenle");
            }
            else
            {
                return View(kategori);
            }
        }
        [Authorize(Roles = ClassOfUserRoles.SuperAdmin + "," + ClassOfUserRoles.Admin)]

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
        [Authorize(Roles = ClassOfUserRoles.SuperAdmin + "," + ClassOfUserRoles.Admin)]
        public ActionResult KategoriHaberleri()
        {
            ViewBag.kategoriler = kategorihelper.TumKategoriler();
            var result = haberhelper.KategoriyeGoreHaberler().OrderByDescending(x=>x.HaberGirisTarihi).ToList();
            return View(result);

        }
        [Authorize(Roles = ClassOfUserRoles.SuperAdmin + "," + ClassOfUserRoles.Admin)]
        [HttpPost]
        public ActionResult KategoriHaberleri(int kategoriID)
        {
            ViewBag.kategoriler = kategorihelper.TumKategoriler();
            var kategori = kategorihelper.KategoriGetir(kategoriID);
            var result = haberhelper.KategoriyeGoreHaberler(kategori);
            return View(result);

        }
        [Authorize(Roles = ClassOfUserRoles.SuperAdmin + "," + ClassOfUserRoles.Admin)]

        public ActionResult KategoriHaberleriYeni(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("KategoriHaberleri");
            }
            ViewBag.kategoriAd = kategorihelper.KategoriGetir(id).KategoriAdi;
            var result = haberhelper.KategoriyeGoreHaberler(kategorihelper.KategoriGetir(id)).OrderByDescending(x => x.HaberGirisTarihi).ToList();
            return View(result);
        }
        #endregion

        #region Yorum Sayfaları

        [Authorize(Roles = ClassOfUserRoles.SuperAdmin + "," + ClassOfUserRoles.Admin)]
        public ActionResult YorumListele()
        {
            var result = yorumhelper.TumYorumlariListele().OrderByDescending(x => x.YorumYazmaTarihi).ToList();
            return View(result);
        }
        [Authorize(Roles = ClassOfUserRoles.SuperAdmin + "," + ClassOfUserRoles.Admin)]
        public ActionResult YorumEkle()
        {
            ViewBag.haberlistesi = haberhelper.TumHaberleriListele();
            return View();
        }
        [Authorize(Roles = ClassOfUserRoles.SuperAdmin + "," + ClassOfUserRoles.Admin)]
        [HttpPost]
        public ActionResult YorumEkle(Yorum yorum, int HaberID)
        {
            yorum.YorumYazari = User.Identity.Name;
            ViewBag.haberlistesi = haberhelper.TumHaberleriListele();
            yorum.YorumIcerik = WebUtility.HtmlDecode(yorum.YorumIcerik);
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
        [Authorize(Roles = ClassOfUserRoles.SuperAdmin + "," + ClassOfUserRoles.Admin)]

        public ActionResult YorumDuzenle()
        {

            var result = yorumhelper.TumYorumlariListele().OrderByDescending(x => x.YorumYazmaTarihi).ToList();
            return View(result);
        }
        [Authorize(Roles = ClassOfUserRoles.SuperAdmin + "," + ClassOfUserRoles.Admin)]

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
        [Authorize(Roles = ClassOfUserRoles.SuperAdmin + "," + ClassOfUserRoles.Admin)]
        [HttpPost]
        public ActionResult YorumDuzenleme(Yorum yorum, int yorumID)
        {
            yorum.YorumIcerik = WebUtility.HtmlDecode(yorum.YorumIcerik);
            var y = yorumhelper.YorumGetir(yorum.YorumID);

            y.YorumIcerik = yorum.YorumIcerik;
            y.YorumDurumu = yorum.YorumDurumu;

            context.Entry(y).State = System.Data.Entity.EntityState.Modified;
            context.SaveChanges();

            return RedirectToAction("YorumDuzenle");
        }
        [Authorize(Roles = ClassOfUserRoles.SuperAdmin + "," + ClassOfUserRoles.Admin)]

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
        [Authorize(Roles = ClassOfUserRoles.SuperAdmin + "," + ClassOfUserRoles.Admin)]
        public ActionResult YazarListele()
        {
            var result = yazarhelper.TumYazarlariListele();
            return View(result);
        }
        [Authorize(Roles = ClassOfUserRoles.SuperAdmin + "," + ClassOfUserRoles.Admin)]
        public ActionResult YazarEkle()
        {
            return View();
        }
        [Authorize(Roles = ClassOfUserRoles.SuperAdmin + "," + ClassOfUserRoles.Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult YazarEkle(Yazar yazar)
        {
            if (ModelState.IsValid)
            {
                yazarhelper.YazarKaydet(yazar);
                return RedirectToAction("YazarListele");
            }
            else
            {
                return View(yazar);
            }
        }
        [Authorize(Roles = ClassOfUserRoles.SuperAdmin + "," + ClassOfUserRoles.Admin)]
        public ActionResult YazarDuzenle()
        {
            var result = yazarhelper.TumYazarlariListele();
            return View(result);
        }
        [Authorize(Roles = ClassOfUserRoles.SuperAdmin + "," + ClassOfUserRoles.Admin)]
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
        [Authorize(Roles = ClassOfUserRoles.SuperAdmin + "," + ClassOfUserRoles.Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult YazarDuzenleme(Yazar yazar, int yazarID)
        {
            if (ModelState.IsValid)
            {
                var y = yazarhelper.YazarGetir(yazarID);
                y.YazarAdSoyad = yazar.YazarAdSoyad;

                context.Entry(y).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
                return RedirectToAction("YazarListele");
            }
            else
            {
                return View(yazar);
            }

        }
        [Authorize(Roles = ClassOfUserRoles.SuperAdmin + "," + ClassOfUserRoles.Admin)]

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
        [Authorize(Roles = ClassOfUserRoles.SuperAdmin + "," + ClassOfUserRoles.Admin)]

        public ActionResult YazarHaberleri()
        {
            ViewBag.yazarlar = yazarhelper.TumYazarlariListele();
            var result = haberhelper.YazaraGoreHaberler();
            return View(result);
        }
        [Authorize(Roles = ClassOfUserRoles.SuperAdmin + "," + ClassOfUserRoles.Admin)]

        [HttpPost]
        public ActionResult YazarHaberleri(int yazarID)
        {
            ViewBag.yazarlar = yazarhelper.TumYazarlariListele();
            var yazar = yazarhelper.YazarGetir(yazarID);
            var result = haberhelper.YazaraGoreHaberler(yazar);
            return View(result);

        }
        [Authorize(Roles = ClassOfUserRoles.SuperAdmin + "," + ClassOfUserRoles.Admin)]
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
        [Authorize(Roles = ClassOfUserRoles.SuperAdmin + "," + ClassOfUserRoles.Admin)]
        public ActionResult EtiketListele()
        {
            var result = etikethelper.TumEtiketleriListele();
            return View(result);
        }
        [Authorize(Roles = ClassOfUserRoles.SuperAdmin + "," + ClassOfUserRoles.Admin)]
        public ActionResult EtiketEkle()
        {
            ViewBag.hata = "";
            return View();
        }
        [Authorize(Roles = ClassOfUserRoles.SuperAdmin + "," + ClassOfUserRoles.Admin)]
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
        [Authorize(Roles = ClassOfUserRoles.SuperAdmin + "," + ClassOfUserRoles.Admin)]



        public ActionResult EtiketDuzenle()
        {
            var result = etikethelper.TumEtiketleriListele();
            return View(result);
        }
        [Authorize(Roles = ClassOfUserRoles.SuperAdmin + "," + ClassOfUserRoles.Admin)]
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
        [Authorize(Roles = ClassOfUserRoles.SuperAdmin + "," + ClassOfUserRoles.Admin)]
        [HttpPost]
        public ActionResult EtiketDuzenleme(Etiket etiket, int etiketID)
        {
            var e = etikethelper.EtiketGetir(etiketID);
            e.EtiketAdi = etiket.EtiketAdi.Trim();

            context.Entry(e).State = System.Data.Entity.EntityState.Modified;
            context.SaveChanges();
            return RedirectToAction("EtiketListele");

        }
        [Authorize(Roles = ClassOfUserRoles.SuperAdmin + "," + ClassOfUserRoles.Admin)]

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
        [Authorize(Roles = ClassOfUserRoles.SuperAdmin + "," + ClassOfUserRoles.Admin)]

        public ActionResult EtiketHaberleri()
        {
            ViewBag.etiketlerTum = etikethelper.TumEtiketleriListele();

            var result = haberhelper.EtiketeGoreHaberler();
            return View(result);
        }
        [Authorize(Roles = ClassOfUserRoles.SuperAdmin + "," + ClassOfUserRoles.Admin)]

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
        [Authorize(Roles = ClassOfUserRoles.SuperAdmin + "," + ClassOfUserRoles.Admin)]

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
        #endregion

        #region Hakkında Sayfaları
        [Authorize(Roles = ClassOfUserRoles.SuperAdmin + "," + ClassOfUserRoles.Admin)]
        public ActionResult HakkindaListele()
        {
            var result = hakkindahelper.TumHakkindaListesi();
            return View(result);
        }
        [Authorize(Roles = ClassOfUserRoles.SuperAdmin + "," + ClassOfUserRoles.Admin)]

        public ActionResult HakkindaEkle()
        {
            return View();
        }
        [Authorize(Roles = ClassOfUserRoles.SuperAdmin + "," + ClassOfUserRoles.Admin)]

        [HttpPost]
        public ActionResult HakkindaEkle(Hakkimizda hak)
        {
            hak.HakIcerik = System.Net.WebUtility.HtmlDecode(hak.HakIcerik);
            hak.HakEklenmeTarihi = DateTime.Now;
            hakkindahelper.HakkindaEkle(hak);
            return RedirectToAction("HakkindaListele");
            
        }
        [Authorize(Roles = ClassOfUserRoles.SuperAdmin + "," + ClassOfUserRoles.Admin)]
        public ActionResult HakkindaDuzenleme(int? id)
        {
            
            bool bosMu = string.IsNullOrEmpty(id.ToString());
            if (bosMu)
            {
                return RedirectToAction("HakkindaListele");
            }
            else
            {
                var result = (from p in context.Hakkimizda
                              where p.HakID == id
                              select p).FirstOrDefault();
                if (result != null)
                {
                    var hak= hakkindahelper.HakkindaGetir(id);
                   
                    return View(hak);
                }
                else
                {
                    return RedirectToAction("HakkindaListele");
                }

            }

        }
        [Authorize(Roles = ClassOfUserRoles.SuperAdmin + "," + ClassOfUserRoles.Admin)]
        [HttpPost]
        public ActionResult HakkindaDuzenleme(Hakkimizda hakkinda)
        {
            hakkinda.HakIcerik = WebUtility.HtmlDecode(hakkinda.HakIcerik);

            var hak = hakkindahelper.HakkindaGetir(hakkinda.HakID);
            //context.Entry(haber).State = System.Data.Entity.EntityState.Unchanged;

            hak.HakBaslik = hakkinda.HakBaslik;
            hak.HakIcerik = hakkinda.HakIcerik;
            hak.HakEklenmeTarihi = hakkinda.HakEklenmeTarihi;
            hak.HakAktiflik = hakkinda.HakAktiflik;
            context.Entry(hak).State = System.Data.Entity.EntityState.Modified;
            context.SaveChanges();
            
            return RedirectToAction("HakkindaListele");
        }
        [Authorize(Roles = ClassOfUserRoles.SuperAdmin + "," + ClassOfUserRoles.Admin)]

        public ActionResult HakkindaSil(int? id)
        {
            if (id==null || id==0)
            {
                return View("HakkindaListele");
            }
            else
            {
                var h = hakkindahelper.HakkindaGetir(id);
                hakkindahelper.HakkindaSil(h);
                return RedirectToAction("HakkindaListele");
            }
            

        }

        #endregion

        #region İletişim Sayfaları
        [Authorize(Roles = ClassOfUserRoles.SuperAdmin + "," + ClassOfUserRoles.Admin)]

        public ActionResult IletisimListele()
        {
            var result = iletisimhelper.TumIletisimListesi().OrderByDescending(x=>x.IltGondermeTarihi).ToList();
            return View(result);
        }
        [Authorize(Roles = ClassOfUserRoles.SuperAdmin + "," + ClassOfUserRoles.Admin)]
        public ActionResult IletisimDuzenleme(int? id)
        {
            bool bosMu = string.IsNullOrEmpty(id.ToString());
            if (bosMu)
            {
                return RedirectToAction("IletisimListele");
            }
            else
            {
                var result = (from p in context.Iletisim
                              where p.IletisimID == id
                              select p).FirstOrDefault();
                if (result != null)
                {
                    var ilt = iletisimhelper.IletisimGetir(id);

                    return View(ilt);
                }
                else
                {
                    return RedirectToAction("IletisimListele");
                }

            }

        }
        [Authorize(Roles = ClassOfUserRoles.SuperAdmin + "," + ClassOfUserRoles.Admin)]
        [HttpPost]
        public ActionResult IletisimDuzenleme(Iletisim iletisim)
        {

            iletisim.IltIcerik = System.Net.WebUtility.HtmlDecode(iletisim.IltIcerik);
            var ilt = iletisimhelper.IletisimGetir(iletisim.IletisimID);
            //context.Entry(haber).State = System.Data.Entity.EntityState.Unchanged;

            ilt.IltAdSoyad = iletisim.IltAdSoyad;
            ilt.email= iletisim.email;
           // ilt.IltGondermeTarihi = iletisim.IltGondermeTarihi;
            ilt.IltIcerik= iletisim.IltIcerik;
            ilt.IltKonu = iletisim.IltKonu;
            context.Entry(ilt).State = System.Data.Entity.EntityState.Modified;
            context.SaveChanges();

            return RedirectToAction("IletisimListele");
        }
        [Authorize(Roles = ClassOfUserRoles.SuperAdmin + "," + ClassOfUserRoles.Admin)]
        public ActionResult IletisimEkle()
        {
            return View();
        }
        [Authorize(Roles = ClassOfUserRoles.SuperAdmin + "," + ClassOfUserRoles.Admin)]

        [HttpPost]
        public ActionResult IletisimEkle(Iletisim ile)
        {
            ile.IltIcerik= System.Net.WebUtility.HtmlDecode(ile.IltIcerik);
            ile.IltGondermeTarihi = DateTime.Now;
            iletisimhelper.IletisimEkle(ile);
            return RedirectToAction("IletisimListele");

        }
        [Authorize(Roles = ClassOfUserRoles.SuperAdmin + "," + ClassOfUserRoles.Admin)]

        public ActionResult IletisimSil(int? id)
        {
            if (id == null || id == 0)
            {
                return View("IletisimListele");
            }
            else
            {
                var i = iletisimhelper.IletisimGetir(id);
                iletisimhelper.IletisimSil(i);
                return RedirectToAction("IletisimListele");
            }


        }
        
        #endregion
        [Authorize(Roles = ClassOfUserRoles.SuperAdmin)]
        public ActionResult KullaniciListesi()
        {
            UserStore<HaberUser> userStore = new UserStore<HaberUser>(context);
            userManager = new UserManager<HaberUser>(userStore);
            RoleStore<HaberRole> roleStore = new RoleStore<HaberRole>(context);
            roleManager = new RoleManager<HaberRole>(roleStore);

            ViewBag.users = userManager.Users.OrderByDescending(x => x.EklenmeTarihi).ToList();
            ViewBag.roles = roleManager.Roles.ToList();

            return View();
        }
        [Authorize(Roles = ClassOfUserRoles.SuperAdmin)]
        public ActionResult KullaniciDuzenleme(string id)
        {
            UserStore<HaberUser> userStore = new UserStore<HaberUser>(context);
            userManager = new UserManager<HaberUser>(userStore);
            RoleStore<HaberRole> roleStore = new RoleStore<HaberRole>(context);
            roleManager = new RoleManager<HaberRole>(roleStore);

            if (id!=null)
            {
                var result = userManager.FindByName(id);
                if (result!=null)
                {
                    ViewBag.roles = roleManager.Roles.ToList();
                    return View(result);
                }
                else
                {
                    return RedirectToAction("KullaniciListesi");
                }
            }
            else
            {
                return RedirectToAction("KullaniciListesi");
            }
        }
        [Authorize(Roles = ClassOfUserRoles.SuperAdmin)]
        [HttpPost]
        public ActionResult KullaniciDuzenleme(HaberUser user, string roleID)
        {
            UserStore<HaberUser> userStore = new UserStore<HaberUser>(context);
            userManager = new UserManager<HaberUser>(userStore);
            RoleStore<HaberRole> roleStore = new RoleStore<HaberRole>(context);
            roleManager = new RoleManager<HaberRole>(roleStore);
            if (user==null)
            {
                return RedirectToAction("KullaniciListesi");
            }
            HaberRole hrole = roleManager.FindById(roleID);
            //userManager.AddToRole(user.Id, hrole.Id);
            var huser = userManager.FindByName(user.Id);
            huser.Name = user.Name;
            huser.SurName = user.SurName;
            huser.Email = user.Email;
            userManager.AddToRole(huser.Id,hrole.Name);
            if (hrole.Name=="News Writer")
            {
                yazarhelper.YazarKaydet(new Yazar { YazarAdSoyad = huser.Name + " " + huser.SurName });
            }
            context.SaveChanges();
            return RedirectToAction("KullaniciListesi");

        }
        [Authorize(Roles = ClassOfUserRoles.SuperAdmin)]
        public ActionResult KullaniciDetay(string id)
        {
            UserStore<HaberUser> userStore = new UserStore<HaberUser>(context);
            userManager = new UserManager<HaberUser>(userStore);
            //RoleStore<HaberRole> roleStore = new RoleStore<HaberRole>(context);
            //roleManager = new RoleManager<HaberRole>(roleStore);

            if (id==null || id =="")
            {
                return RedirectToAction("KullaniciListesi");
            }
            else
            {
                HaberUser user = userManager.FindByName(id);
                if (user!=null)
                {
                    ViewBag.roller = (from p in user.Roles
                                      select p.RoleId).ToList();
                    var roleIDs = (List<string>)ViewBag.roller;
                    ViewBag.rolAdlari = userManager.GetRoles(user.Id);
                    return View(user);
                }
                else
                {
                    return RedirectToAction("KullaniciListesi");
                }
            }
        }
        [Authorize(Roles = ClassOfUserRoles.SuperAdmin)]
        public void KullaniciRolSilV(HaberUser user, string id)
        {
            UserStore<HaberUser> userStore = new UserStore<HaberUser>(context);
            userManager = new UserManager<HaberUser>(userStore);
            RoleStore<HaberRole> roleStore = new RoleStore<HaberRole>(context);
            roleManager = new RoleManager<HaberRole>(roleStore);

            userManager.RemoveFromRole(user.Id, roleManager.FindById(id).Name);
            context.SaveChanges();
        }
        [Authorize(Roles = ClassOfUserRoles.SuperAdmin)]
        [HttpGet]
        public ActionResult KullaniciRolSil(string id)
        {
            UserStore<HaberUser> userStore = new UserStore<HaberUser>(context);
            userManager = new UserManager<HaberUser>(userStore);
            if (id==null || id=="")
            {
                return RedirectToAction("KullaniciListesi");
            }
            else
            {

                HaberUser hu = userManager.FindByName(id);
                ViewBag.roles = userManager.GetRoles(hu.Id);
                ViewBag.roleIds = (from p in hu.Roles
                                   select p.RoleId).ToList();

                return View(hu);
            }
        }
        [Authorize(Roles = ClassOfUserRoles.SuperAdmin)]
        [HttpPost]
        public ActionResult KullaniciRolSil(string rID, string userID)
        {
            UserStore<HaberUser> userStore = new UserStore<HaberUser>(context);
            userManager = new UserManager<HaberUser>(userStore);
            var user = userManager.FindById(userID);
            if (string.IsNullOrEmpty(rID) || string.IsNullOrWhiteSpace(rID))
            {
                return RedirectToAction("KullaniciListesi");
            }
            if (user.Roles.Count==1)
            {
                return RedirectToAction("KullaniciListesi");
            }
            else
            {
                KullaniciRolSilV(user, rID);
                var yazar = "News Writer";
                var role = roleManager.FindByName(yazar);
                if (rID == role.Id)
                {
                    string yazarAD = user.Name + " " + user.SurName;
                    Yazar y = yazarhelper.YazarGetir(yazarAD);
                    yazarhelper.YazarSil(y.YazarID);
                }
                return RedirectToAction("KullaniciListesi");
            }
        }
        [Authorize(Roles = ClassOfUserRoles.SuperAdmin)]
        public ActionResult KullaniciSil(string id)
        {
            UserStore<HaberUser> userStore = new UserStore<HaberUser>(context);
            userManager = new UserManager<HaberUser>(userStore);
            if (id==null || id=="")
            {
                return RedirectToAction("KullaniciListesi");
            }
            else
            {
                var user = userManager.FindByName(id.Trim());
                if (user!=null)
                {
                    userManager.Delete(user);
                    context.SaveChanges();
                    
                }
                    return RedirectToAction("KullaniciListesi");
            }
        }

        [Authorize(Roles = ClassOfUserRoles.SuperAdmin)]
        public ActionResult KullaniciEkle()
        {
            UserStore<HaberUser> userStore = new UserStore<HaberUser>(context);
            userManager = new UserManager<HaberUser>(userStore);

            return View();
            
        }
        [Authorize(Roles = ClassOfUserRoles.SuperAdmin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult KullaniciEkle(RegisterModel model)
        {
            UserStore<HaberUser> userStore = new UserStore<HaberUser>(context);
            userManager = new UserManager<HaberUser>(userStore);

            if (ModelState.IsValid)
            {
                HaberUser user = new HaberUser();
                user.Email = model.Email;
                user.Name = model.Name;
                user.SurName = model.SurName;
                user.UserName = model.UserName;
                user.EklenmeTarihi = DateTime.Now;
                IdentityResult ir = userManager.Create(user, model.Password);
                if (ir.Succeeded)
                {
                    userManager.AddToRole(user.Id, "User");
                    return RedirectToAction("KullaniciListesi");
                }
                else
                {
                    ModelState.AddModelError("RegisterUser", "Kullanıcı ekleme işleminde hata.");
                }
            }
            return View(model);

        }
    }




}