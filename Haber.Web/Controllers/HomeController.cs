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
        // GET: Home
        public ActionResult Index()
        {
            KategoriHelper kategorihelper = new KategoriHelper();
            HaberClHelper haberhelper = new HaberClHelper();
            YorumHelper yorumhelper = new YorumHelper();
            ResimHelper resimhelper = new ResimHelper();
            EtiketHelper etikethelper = new EtiketHelper();
            YazarHelper yazarhelper = new YazarHelper();

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
            var result2 = resultOriginal.OrderBy(x => Guid.NewGuid()).Take(10).ToList();
            ViewBag.etiketResult = result;
            ViewBag.etiketResult2 = result2;
            return View(haberhelper.TumHaberleriListele().OrderByDescending(x=>x.HaberGirisTarihi).Take(10).ToList());
        }
    }
}