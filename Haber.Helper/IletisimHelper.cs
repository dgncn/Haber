using Haber.COM;
using Haber.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Haber.Helper
{
    public class IletisimHelper
    {
        HaberContext context;
        public IletisimHelper()
        {
            context = new HaberContext();
        }
        public IletisimHelper(HaberContext tContext)
        {
            context = tContext;
        }

        public List<Iletisim> TumIletisimListesi()
        {
            var result = context.Iletisim.ToList();
            return result;
        }
        public Iletisim IletisimGetir(int? id)
        {
            var result = context.Iletisim.Find(id);
            return result;
        }
        public void IletisimSil(Iletisim i)
        {
            context.Iletisim.Remove(i);
            context.SaveChanges();
        }
        public void IletisimEkle(Iletisim ile)
        {
            context.Iletisim.Add(ile);
            context.SaveChanges();
        }
    }
}
