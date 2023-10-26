using System;
using System.Linq;
using WebAPI;
using WebDAL.Entities;

namespace DBContextGenarator
{
    class Program
    {
        static void Main(string[] args)
        {
            RecreateDB();
            FillMockupData();
            LogAccounts();
        }

        static void LogAccounts()
        {
            using (var db = new PdbContext())
            {
                var accounts = db.Accounts.ToList();
            
                foreach (var account in accounts)
                {
                    Console.WriteLine(account.name + Environment.NewLine);
                }
            }
        }

        public static void RecreateDB()
        {
            using (var db = new PdbContext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
            }
        }

        public static void FillMockupData()
        {
            Console.WriteLine("Start");

            using (var db = new PdbContext())
            {
                var account1 = new Account() { name = "Test", password = "123", email = "mail@mail.ru"};
                var account2 = new Account() { name = "Test4", password = "Aaaa" };
                db.Accounts.Add(account1);
                db.Accounts.Add(account2);
                db.SaveChanges();
            }

            using (var db = new PdbContext())
            {
                // var dep3 = db.Departments.Single(x => x.Name == "Dep3");
                // dep3.Name = "Dep33";
                // db.Update(dep3);
                db.SaveChanges();
            }


            // await using (var db = new PostgresDbContext())
            // {
            //     var result = db.Guests.Include(x => x.Department);
            //
            //     foreach (var guest in result)
            //     {
            //         Console.WriteLine($"Guest: {guest.Name} department: {guest.Department.Name}");
            //     }
            // }
        }
    }
}