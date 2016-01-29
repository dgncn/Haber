using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Haber.COM
{
    public class HaberUser:IdentityUser
    {
        public string Name { get; set; }
        public string SurName { get; set; }
        public DateTime EklenmeTarihi { get; set; }
    }
    public class HaberRole: IdentityRole
    {
        public string Description { get; set; }

        public HaberRole()
        {

        }
        public HaberRole(string roleName,string desc):base(roleName)
        {
            this.Description = desc;
        }
    }
}
