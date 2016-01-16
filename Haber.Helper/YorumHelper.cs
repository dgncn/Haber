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

            //var result = (from p in context.Haberler
            //              where p.Yorumlar[0].YorumID == id
            //              select p).FirstOrDefault();

            //if (result != null)
            //{
            //    int a = -1;
            //    return a;
            //}
            //else
            //{
                context.Yorumlar.Remove(yorum);
                return context.SaveChanges();
            //}


        }
    }
}
