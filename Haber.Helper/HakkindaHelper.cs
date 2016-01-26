using Haber.COM;
using Haber.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Haber.Helper
{
    public class HakkindaHelper
    {
        HaberContext context;
        public HakkindaHelper()
        {
            context = new HaberContext();
        }
        public HakkindaHelper(HaberContext tContext)
        {
            context = tContext;
        }

        public void HakkindaEkle(Hakkimizda hakkinda)
        {
            context.Hakkimizda.Add(hakkinda);
            context.SaveChanges();
        }
        public List<Hakkimizda> TumHakkindaListesi()
        {
            return context.Hakkimizda.ToList();
        }
        public Hakkimizda HakkindaGetir(int? id)
        {
            Hakkimizda h = context.Hakkimizda.Find(id);
            return h;
        }
        public void HakkindaSil(Hakkimizda h)
        {
            context.Hakkimizda.Remove(h);
            context.SaveChanges();
        }
    }
}
