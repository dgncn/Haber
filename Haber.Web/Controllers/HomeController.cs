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

            return View(haberhelper.TumHaberleriListele().OrderByDescending(x=>x.HaberGirisTarihi).Take(10).ToList());
        }
    }
}