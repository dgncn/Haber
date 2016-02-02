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

            UserStore<HaberUser> userStore = new UserStore<HaberUser>(context);
            UserManager<HaberUser> userManager = new UserManager<HaberUser>(userStore);

            if (!roleManager.RoleExists("SuperAdmin"))
            {
                HaberRole suRole = new HaberRole("SuperAdmin", "Sistem Ana Yöneticisi");
                roleManager.Create(suRole);

            }
            var user = userManager.FindByName("SuperUser");
            if (user == null)
            {
                HaberUser huser = new HaberUser
                {
                    Name = "Super",
                    SurName = "User",
                    UserName = "SuperUser",
                    EklenmeTarihi = DateTime.Now
                };

                IdentityResult ir = userManager.Create(huser, "123456");
                if (ir.Succeeded)
                {
                    userManager.AddToRole(huser.Id, "SuperAdmin");
                }


            }
            else
            {
                if (!userManager.IsInRole(user.Id, "SuperAdmin"))
                {
                    userManager.AddToRole(user.Id, "SuperAdmin");
                }
            }

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
            if (!roleManager.RoleExists("News Writer"))
            {
                HaberRole newsRole = new HaberRole("News Writer", "Haber yazarı");
                roleManager.Create(newsRole);
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
            int etiketSayisi = context.Etiketler.Count();
            return etiketSayisi;
        }

    }

    public static class UserInfo
    {
        

        public  static HaberUser kullaniciBilgiAl(string userId)
        {
            HaberContext context = new HaberContext();
            RoleStore<HaberRole> roleStore = new RoleStore<HaberRole>(context);
            RoleManager<HaberRole> roleManager = new RoleManager<HaberRole>(roleStore);

            UserStore<HaberUser> userStore = new UserStore<HaberUser>(context);
            UserManager<HaberUser> userManager = new UserManager<HaberUser>(userStore);
            var kullanici = userManager.FindById(userId);
            if (kullanici!=null)
            {
                return kullanici;
            }
            else
            {
                return new HaberUser { };
            }
        }
        public static string rolBilgisiAl(string rolAdi)
        {
            HaberContext context = new HaberContext();
            RoleStore<HaberRole> roleStore = new RoleStore<HaberRole>(context);
            RoleManager<HaberRole> roleManager = new RoleManager<HaberRole>(roleStore);

            UserStore<HaberUser> userStore = new UserStore<HaberUser>(context);
            UserManager<HaberUser> userManager = new UserManager<HaberUser>(userStore);
            var role = roleManager.FindByName(rolAdi);
            if (role!=null)
            {
                return role.Id;
            }
            else
            {
                return "";
            }
        }
        
    }
    
}
