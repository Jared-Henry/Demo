using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.SqlServer;
using System.Data.Entity;

namespace Demo.EntityAsync
{
    class Program
    {
        static void Main(string[] args)
        {
            Database.SetInitializer<DemoContext>(new DemoContextInitializer(100));
            var factory = new DemoContextFactory();
            Console.WriteLine("Initializing database...");
            using (var db = factory.Create())
            {
                db.Database.Initialize(false);
            }
            Console.WriteLine("Beginning work...");
            var work = GetThingResult(factory);
            Console.WriteLine("Waiting on work...");
            work.Wait();
            Console.WriteLine("Work done.");
            Console.WriteLine("Set1: {0}", work.Result.Set1.Length);
            Console.WriteLine("Set2: {0}", work.Result.Set2.Length);
            Console.WriteLine("Set3: {0}", work.Result.Set3.Length);
            Console.Write("Hit enter to exit...");
            Console.ReadLine();
        }

        public class ThingResult
        {
            public Thing[] Set1 { get; set; }
            public Thing[] Set2 { get; set; }
            public Thing[] Set3 { get; set; }
        }

        public static async Task<ThingResult> GetThingResult(IDbContextFactory<DemoContext> factory)
        {
            var set1 = NamesLike(factory, "%1%");
            var set2 = NamesLike(factory, "%2%");
            var set3 = NamesLike(factory, "%3%");

            return new ThingResult()
            {
                Set1 = await set1,
                Set2 = await set2,
                Set3 = await set3
            };
        }

        private static async Task<Thing[]> NamesLike(IDbContextFactory<DemoContext> factory, string like)
        {
            using (var db = factory.Create())
            {
                return await db.Things.Where(t => SqlFunctions.PatIndex(like, t.Name).HasValue).ToArrayAsync();
            }
        }
    }
}
