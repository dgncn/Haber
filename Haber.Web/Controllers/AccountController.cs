using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.EntityFramework;
using Haber.COM;
using Haber.DAL;

namespace Haber.Web.Controllers
{
    public class AccountController : Controller
    {
        static HaberContext context = new HaberContext();
        private UserManager<HaberUser> userManager;
        private RoleManager<HaberRole> roleManager;


        public AccountController()
        {
            UserStore<HaberUser> userStore = new UserStore<HaberUser>(context);
            userManager = new UserManager<HaberUser>(userStore);

            RoleStore<HaberRole> roleStore = new RoleStore<HaberRole>(context);
            roleManager = new RoleManager<HaberRole>(roleStore);

        }







    }
    
}