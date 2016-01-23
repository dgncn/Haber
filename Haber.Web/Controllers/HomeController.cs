using Haber.COM;
using Haber.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
        // GET: Home
        public void ViewbagListesi()
        {
            ViewBag.kategoriler = kategorihelper.TumKategoriler();
            ViewBag.haberler = haberhelper.TumHaberleriListele();
            ViewBag.yazarlar = yazarhelper.TumYazarlariListele();
            ViewBag.yorumlar = yorumhelper.TumYorumlariListele();
            ViewBag.etiketler = etikethelper.TumEtiketleriListele();
            ViewBag.resimler = resimhelper.ResimListele();

            var etiketListe = (List<Etiket>)ViewBag.etiketler;
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

            
            return View(haberhelper.TumHaberleriListele().OrderByDescending(x => x.HaberGirisTarihi).Take(10).ToList());
        }
        public ActionResult Kategori(int? id)
        {
            ViewbagListesi();
            
            if (id == null)
            {
                var result1 = haberhelper.KategoriyeGoreHaberler(10);
                return View(result1);
            }
            else
            {
                var result1 = haberhelper.KategoriyeGoreSonHaberler(kategorihelper.KategoriGetir(id));
                return View(result1);
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
                    var haber = haberhelper.HaberGetir(id);
                    return View(haber);
                }
            }
            else
            {
                return RedirectToAction("Index");
            }
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
                if (id == null || id=="")
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    
                    foreach (var etiket in etiketListesi)
                    {
                        var haberListe = haberhelper.EtiketeGoreHaberler(etiket.EtiketID);
                        if (haberListe!=null && haberListe.Count>0)
                        {

                            hbrList.AddRange(haberListe);
                        }
                    }
                    
                }
            }
            else
            {
                return RedirectToAction("Index");
            }
            var EtiketHaberListesi = hbrList.OrderBy(x => x.HaberGirisTarihi).ToList();
            return View(EtiketHaberListesi);
        }
    }
}