using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Haber.DAL;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Haber.COM;
namespace Haber.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);


            HaberContext context = new HaberContext();
            RoleStore<HaberRole> roleStore = new RoleStore<HaberRole>(context);
            RoleManager<HaberRole> roleManager = new RoleManager<HaberRole>(roleStore);

            if (!roleManager.RoleExists("Admin"))
            {
                HaberRole adminRole = new HaberRole("Admin", "Yönetici");
                roleManager.Create(adminRole);
            }

            if (!roleManager.RoleExists("User"))
            {
                HaberRole userRole = new HaberRole("User", "Okuyucu, yorum yazan");
                roleManager.Create(userRole);
            }
        }
    }

    public static class SayiToplam
    {
        public static int haberSayisi()
        {
            HaberContext context = new HaberContext();
            int haberAdedi = context.Haberler.Count();
            return haberAdedi;
        }
        public static int yazarSayisi()
        {
            HaberContext context = new HaberContext();
            int yazarAdedi = context.Yazarlar.Count();
            return yazarAdedi;
        }
        public static int kategoriSayisi()
        {
            HaberContext context = new HaberContext();
            int kategoriAdedi = context.Kategoriler.Count();
            return kategoriAdedi;
        }
        public static int yorumSayisi()
        {
            HaberContext context = new HaberContext();
            int yorumSayisi = context.Yorumlar.Count();
            return yorumSayisi;
        }
        public static int etiketSayisi()
        {
            HaberContext context = new HaberContext();
            int etiketSayisi = context.Yorumlar.Count();
            return etiketSayisi;
        }    
    }


    
}
