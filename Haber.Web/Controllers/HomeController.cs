using Haber.COM;
using Haber.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList;
namespace Haber.Web.Controllers
{
    public class HomeController : Controller
    {
        KategoriHelper kategorihelper = new KategoriHelper();
        HaberClHelper haberhelper = new HaberClHelper();
        YorumHelper yorumhelper = new YorumHelper();
        ResimHelper resimhelper = new ResimHelper();
        EtiketHelper etikethelper = new EtiketHelper();
        YazarHelper yazarhelper = new YazarHelper();
        HakkindaHelper hakkindahelper = new HakkindaHelper();
        IletisimHelper iletisimhelper = new IletisimHelper();
        // GET: Home
        public void ViewbagListesi()
        {
            ViewBag.kategoriler = kategorihelper.TumKategoriler();
            ViewBag.haberler = haberhelper.TumHaberleriListele();
            ViewBag.yazarlar = yazarhelper.TumYazarlariListele();
            ViewBag.yorumlar = yorumhelper.TumYorumlariListele();
            ViewBag.etiketler = etikethelper.TumEtiketleriListele();
            ViewBag.resimler = resimhelper.ResimListele().Where(x=>x.ResimHaber != null).ToList();
            var haberList = (List<HaberCl>)ViewBag.haberler;
            var enCokOkunanlar = (from p in haberList
                                  where p.HaberDurumu == true
                                  orderby p.HaberOkunmaSayisi descending
                                  select p).Take(6).ToList();
            ViewBag.enCokOkunanlarListesi = enCokOkunanlar;
            var etiketListe = (List<Etiket>)ViewBag.etiketler;
            var yorumListe = (List<Yorum>)ViewBag.yorumlar;
            var yorumListe2 = (from p in yorumListe
                               where p.YorumHaberi.HaberDurumu == true && p.YorumDurumu==true
                               orderby p.YorumYazmaTarihi descending
                               select p).Take(15).ToList();
            ViewBag.yorumListe2 = yorumListe2;
            var resultOriginal = from p in etiketListe
                                 group p by new { id = p.EtiketAdi } into ctg
                                 select new EtiketSonuc { EtiketSonucAdi = ctg.Key.id, EticketSonucSayisi = ctg.Count() };
            var result = resultOriginal.OrderByDescending(x => x.EticketSonucSayisi).Take(10).ToList();
            var result2 = resultOriginal.OrderBy(x => Guid.NewGuid()).Take(40).ToList();
            ViewBag.etiketResult = result;
            ViewBag.etiketResult2 = result2;
        }

        public ActionResult Index()
        {


            ViewbagListesi();
            var liste = haberhelper.TumHaberleriListele().Where(x=>x.HaberDurumu==true).OrderByDescending(x => x.HaberGirisTarihi).Take(10).ToList();

            return View(liste);
        }
        public ActionResult Ara(string aramaDegeri, int? page)
        {
            ViewbagListesi();
            if (string.IsNullOrEmpty(aramaDegeri)  || string.IsNullOrWhiteSpace(aramaDegeri))
            {
                return RedirectToAction("Index");
            }
            else
            {
                List<HaberCl> haberListesi = haberhelper.HaberAra(aramaDegeri.Trim());
                if (haberListesi.Count<1)
                {
                    var haberList = (List<HaberCl>)ViewBag.haberler;
                    var haberSonList = haberList.OrderByDescending(x => x.HaberGirisTarihi).ToList();
                    ViewBag.haberSonList = haberSonList;
                    return View("Hata");
                }
                int pageSize = 15;
                int pageNumber = (page ?? 1);
                ViewBag.aramaDegeri = aramaDegeri;
                return View(haberListesi.ToPagedList(pageNumber,pageSize));
            }
        }
        public ActionResult Kategori(int? id,int? page)
        {
            ViewbagListesi();

            if (id == null)
            {
                var result1 = haberhelper.KategoriyeGoreHaberler().Where(x => x.HaberDurumu == true).ToList();
                int pageSize = 15;
                int pageNumber = (page ?? 1);
                return View(result1.ToPagedList(pageNumber,pageSize));
            }
            else
            {
                var result1 = haberhelper.KategoriyeGoreSonHaberler1(kategorihelper.KategoriGetir(id)).Where(x => x.HaberDurumu == true).ToList();
                int pageSize = 15;
                int pageNumber = (page ?? 1);
                ViewBag.kategori = kategorihelper.KategoriGetir(id).KategoriAdi;
                return View(result1.ToPagedList(pageNumber,pageSize));
            }

        }



        public ActionResult HaberDetay(int? id)
        {
            ViewbagListesi();




            var haberListesi = (List<HaberCl>)ViewBag.haberler;
            var HaberVarMi = (from p in haberListesi
                              where p.HaberID == id
                              select p).FirstOrDefault();

            if (HaberVarMi != null)
            {
                if (id == null)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    List<HaberCl> benzerHaberListesi = new List<HaberCl>();
                    foreach (var etiket in HaberVarMi.HaberEtiketleri)
                    {
                        List<HaberCl> benzerhbrListesi = haberhelper.BenzerHaberleriGetir(etiket.EtiketAdi, HaberVarMi);
                        if (benzerhbrListesi != null && benzerhbrListesi.Count > 0)
                        {

                            benzerHaberListesi.AddRange(benzerhbrListesi);
                        }
                    }
                    HaberVarMi.HaberOkunmaSayisi++;
                    haberhelper.HaberOkSayisiGuncelle(HaberVarMi);



                    ViewBag.benzerhaberler = benzerHaberListesi;
                    var haber = haberhelper.HaberGetir(id);

                    return View(haber);
                }
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        [HttpPost]
        public ActionResult HaberDetay(string txtYorum, int id, string txtName)
        {
            if (txtName == null)
            {
                txtName = "Misafir";
            }
            HaberCl haber = haberhelper.HaberGetir(id);
            Yorum yorum = new Yorum();
            yorum.YorumDurumu = false;
            if (User.IsInRole("SuperAdmin"))
            {
                yorum.YorumDurumu = true;
            }
            yorum.YorumIcerik = txtYorum;
            yorum.YorumYazari = txtName;
            yorum.YorumYazmaTarihi = DateTime.Now;
            haberhelper.HabereYorumEkle(haber, yorum);
            return RedirectToAction("HaberDetay");
        }

        public ActionResult Etiket(string id)
        {
            ViewbagListesi();
            var etiketList = (List<Etiket>)ViewBag.etiketler;
            var etiketVarMi = (from p in etiketList
                               where p.EtiketAdi == id
                               select p).FirstOrDefault();
            var etiketListesi = (from p in etiketList
                                 where p.EtiketAdi == id
                                 select p).ToList();
            List<HaberCl> hbrList = new List<HaberCl>();
            if (etiketVarMi != null)
            {
                if (id == null || id == "")
                {
                    return RedirectToAction("Index");
                }
                else
                {

                    foreach (var etiket in etiketListesi)
                    {
                        var haberListe = haberhelper.EtiketeGoreHaberler(etiket.EtiketID);
                        if (haberListe != null && haberListe.Count > 0)
                        {

                            hbrList.AddRange(haberListe);
                        }
                        ViewBag.etiketAdi = etiket.EtiketAdi;
                    }

                }
            }
            else
            {
                return RedirectToAction("Index");
            }
            var EtiketHaberListesi = hbrList.OrderByDescending(x => x.HaberGirisTarihi).ToList();
            
            return View(EtiketHaberListesi);
        }

        public ActionResult Hakkimizda()
        {
            ViewbagListesi();
            var haberList = (List<HaberCl>)ViewBag.haberler;
            var haberSonList = haberList.OrderByDescending(x => x.HaberGirisTarihi).ToList();
            ViewBag.haberSonList = haberSonList;
            var result = hakkindahelper.TumHakkindaListesi();
            var hakkinda = (from p in result
                            where p.HakAktiflik == true
                            select p).ToList();
            if (hakkinda.Count > 1)
            {
                var hak = hakkinda[0];
                return View(hak);
            }
            else
            {
                if (hakkinda == null || hakkinda.Count == 0)
                {
                    var n = new Hakkimizda { HakBaslik = "", HakIcerik = "", HakEklenmeTarihi = DateTime.Now, HakAktiflik = false };
                    return View(n);
                }
                else
                {
                    var hak = hakkinda[0];
                    return View(hak);
                }
            }
        }

        public ActionResult Iletisim()
        {
            ViewbagListesi();
            var haberList = (List<HaberCl>)ViewBag.haberler;
            var haberSonList = haberList.OrderByDescending(x => x.HaberGirisTarihi).ToList();
            ViewBag.haberSonList = haberSonList;
            return View();
        }
        [HttpPost]
        public ActionResult Iletisim(Iletisim iletisim, int deger)
        {
            ViewbagListesi();
            var haberList = (List<HaberCl>)ViewBag.haberler;
            var haberSonList = haberList.OrderByDescending(x => x.HaberGirisTarihi).ToList();
            ViewBag.haberSonList = haberSonList;
            try
            {
                iletisim.IltGondermeTarihi = DateTime.Now;
                if (deger == 7)
                {
                    iletisimhelper.IletisimEkle(iletisim);
                    ViewBag.mesaj = "Mesajınız başarıyla alındı. Teşekkürler, kısa zamanda sizinle iletişime geçeceğiz.";
                }
                else
                {
                    return RedirectToAction("Iletisim");
                }

            }
            catch (Exception ex)
            {
                ViewBag.mesaj = "Bir hata gerçekleşti. Daha sonra lütfen tekrar deneyin.";
            }
            return View("IletisimTamamlandi");
        }
        public ActionResult IletisimTamamlandi()
        {
            ViewbagListesi();
            return View();
        }
        public ActionResult Yazar(int? id, int? page)
        {
            ViewbagListesi();
            if (id != null || id != 0)
            {
                var yazar = yazarhelper.YazarGetir(id);
                if (yazar != null)
                {
                    var liste = haberhelper.YazaraGoreHaberler(yazar).OrderByDescending(x => x.HaberGirisTarihi).ToList();
                    int pageSize = 15;
                    int pageNumber = (page ?? 1);
                    return View(liste.ToPagedList(pageNumber,pageSize));
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return RedirectToAction("Index");
            }


        }
    }
}