using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Haber.COM;
using System.Data.Entity.ModelConfiguration.Conventions;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Haber.DAL
{
    public class HaberContext : IdentityDbContext<HaberUser>
    {
        public HaberContext() : base("HaberDBConStr")
        { }
        public DbSet<HaberCl> Haberler { get; set; }
        public DbSet<Kategori> Kategoriler { get; set; }
        public DbSet<Resim> Resimler { get; set; }
        public DbSet<Yazar> Yazarlar { get; set; }
        public DbSet<Yorum> Yorumlar { get; set; }
        public DbSet<Etiket> Etiketler { get; set; }
        public DbSet<Hakkimizda> Hakkimizda { get; set; }
        public DbSet<Iletisim> Iletisim { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            base.OnModelCreating(modelBuilder);
        }

    }
}
