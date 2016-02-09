using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.EntityFramework;
using Haber.COM;
using Haber.DAL;
using Haber.Web.Models;
using Microsoft.Owin.Security;
using System.Security.Claims;
using Haber.Helper;

namespace Haber.Web.Controllers
{
    public class AccountController : Controller
    {
        static HaberContext context = new HaberContext();
        private UserManager<HaberUser> userManager;
        private RoleManager<HaberRole> roleManager;


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
            ViewBag.resimler = resimhelper.ResimListele();
            var haberList = (List<HaberCl>)ViewBag.haberler;
            var enCokOkunanlar = (from p in haberList
                                  orderby p.HaberOkunmaSayisi descending
                                  select p).Take(6).ToList();
            ViewBag.enCokOkunanlarListesi = enCokOkunanlar;
            var etiketListe = (List<Etiket>)ViewBag.etiketler;
            var yorumListe = (List<Yorum>)ViewBag.yorumlar;
            var yorumListe2 = (from p in yorumListe
                               orderby p.YorumYazmaTarihi descending
                               select p).ToList();
            ViewBag.yorumListe2 = yorumListe2;
            var resultOriginal = from p in etiketListe
                                 group p by new { id = p.EtiketAdi } into ctg
                                 select new EtiketSonuc { EtiketSonucAdi = ctg.Key.id, EticketSonucSayisi = ctg.Count() };
            var result = resultOriginal.OrderByDescending(x => x.EticketSonucSayisi).Take(10).ToList();
            var result2 = resultOriginal.OrderBy(x => Guid.NewGuid()).Take(40).ToList();
            ViewBag.etiketResult = result;
            ViewBag.etiketResult2 = result2;
        }

        public AccountController()
        {
            UserStore<HaberUser> userStore = new UserStore<HaberUser>(context);
            userManager = new UserManager<HaberUser>(userStore);

            RoleStore<HaberRole> roleStore = new RoleStore<HaberRole>(context);
            roleManager = new RoleManager<HaberRole>(roleStore);

        }

        public ActionResult Register()
        {
            ViewbagListesi();
            var haberList = (List<HaberCl>)ViewBag.haberler;
            var haberSonList = haberList.OrderByDescending(x => x.HaberGirisTarihi).ToList();
            ViewBag.haberSonList = haberSonList;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            ViewbagListesi();
            var haberList = (List<HaberCl>)ViewBag.haberler;
            var haberSonList = haberList.OrderByDescending(x => x.HaberGirisTarihi).ToList();
            ViewBag.haberSonList = haberSonList;
            if (ModelState.IsValid)
            {
                HaberUser user = new HaberUser();
                user.Name = model.Name;
                user.SurName = model.SurName;
                user.UserName = model.UserName;
                user.Email = model.Email;
                user.EklenmeTarihi = DateTime.Now;
                IdentityResult ir = userManager.Create(user, model.Password);
                if (ir.Succeeded)
                {
                    userManager.AddToRole(user.Id, "User");
                    return RedirectToAction("Login");
                }
                else
                {
                    ModelState.AddModelError("RegisterUser", "Kullanıcı ekleme işleminde hata.");
                }
            }
            return View(model);
        }

        public ActionResult Login()
        {
            ViewbagListesi();
            var haberList = (List<HaberCl>)ViewBag.haberler;
            var haberSonList = haberList.OrderByDescending(x => x.HaberGirisTarihi).ToList();
            ViewBag.haberSonList = haberSonList;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model)
        {
            ViewbagListesi();
            var haberList = (List<HaberCl>)ViewBag.haberler;
            var haberSonList = haberList.OrderByDescending(x => x.HaberGirisTarihi).ToList();
            ViewBag.haberSonList = haberSonList;
            if (ModelState.IsValid)
            {
                HaberUser user = userManager.Find(model.UserName, model.Password);

                if (user!=null)
                {
                    IAuthenticationManager authManager = HttpContext.GetOwinContext().Authentication;

                    ClaimsIdentity identity = userManager.CreateIdentity(user, "ApplicationCookie");
                    AuthenticationProperties authProps = new AuthenticationProperties();
                    authProps.IsPersistent = model.RememberMe;
                    authManager.SignIn(authProps, identity);

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("LoginUser", "Böyle bir kullanıcı bulunamadı.");
                }
            }
            return View(model);
        }
        [Authorize]
        public ActionResult Logout()
        {
            IAuthenticationManager authManager = HttpContext.GetOwinContext().Authentication;
            authManager.SignOut();
            return RedirectToAction("Index", "Home");
        }
        

    }
    public static class ClassOfUserRoles
    {
        public const string Admin = "Admin";
        public const string SuperAdmin = "SuperAdmin";
        public const string User = "User";
        public const string NewsWriter = "News Writer";
    }


}