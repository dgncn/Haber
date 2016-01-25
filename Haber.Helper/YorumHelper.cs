using Haber.COM;
using Haber.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Haber.Helper
{
    public class YorumHelper
    {
        HaberContext context;
        public YorumHelper()
        {
            context = new HaberContext();
        }
        public YorumHelper(HaberContext tContext)
        {
            context = tContext;
        }
        public List<Yorum> TumYorumlariListele()
        {
            var result = context.Yorumlar.ToList();
            return result;
        }
        public void YorumEkle(Yorum yorum)
        {
            context.Yorumlar.Add(yorum);
            context.SaveChanges();
        }
        public Yorum YorumGetir(int? id)
        {
            var yorum = context.Yorumlar.Find(id);
            return yorum;
        }
       

        public int YorumSil(int id)
        {
            var yorum = YorumGetir(id);

           
                context.Yorumlar.Remove(yorum);
                return context.SaveChanges();

        }
    }
}
