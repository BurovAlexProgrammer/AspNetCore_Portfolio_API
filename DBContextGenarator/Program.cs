using System;
using System.Linq;
using WebAPI;
using WebDAL.Entities;

namespace DbContextGenerator
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
            using (var db = new AppDbContext())
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
            using (var db = new AppDbContext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
            }
        }

        public static void FillMockupData()
        {
            Console.WriteLine("Start");
            
            using (var db = new AppDbContext())
            {
                var account1 = new Account() { name = "Test", email = "mail@mail.ru", role = "Admin"};
                var account2 = new Account() { name = "Test4"};
                db.Accounts.Add(account1);
                db.Accounts.Add(account2);
                db.SaveChanges();
            }

            using (var db = new AppDbContext())
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