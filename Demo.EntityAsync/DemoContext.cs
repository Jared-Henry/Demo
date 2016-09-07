using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.EntityAsync
{
    public class DemoContext: DbContext
    {
        public DbSet<Thing> Things { get; set; }
    }

    [Table("dbo.Thing")]
    public class Thing
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        public string Name { get; set; }
    }

    public class DemoContextInitializer : DropCreateDatabaseAlways<DemoContext>
    {
        public int SeedCount { get; set; }
        public DemoContextInitializer(int seedCount)
        {
            this.SeedCount = seedCount;
        }
        protected override void Seed(DemoContext context)
        {
            base.Seed(context);
            for(var i = 0; i < SeedCount; i++)
            {
                context.Things.Add(new Thing()
                {
                    Name = "Thing " + i
                });
            }
            context.SaveChanges();
        }
    }

    public class DemoContextFactory : IDbContextFactory<DemoContext>
    {
        public DemoContext Create()
        {
            return new DemoContext();
        }
    }
}
