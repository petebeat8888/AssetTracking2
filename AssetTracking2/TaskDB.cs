namespace AppEFDBClasses
{
    internal class TaskDB
    {
        public static void FillDBdata()
        {

            Console.WriteLine("Filling default data to database");

            MyDbContext ContextPrep = new MyDbContext();

            // Add laptops
            ComputerDB NewComp1 = new()
            {
                ModelName = "HP",
                PurchaseDate = DateTime.Parse("2022-06-12"),
                Price = 1000,
                Office = "sweden"
            };
            ContextPrep.Computers.Add(NewComp1);


            ComputerDB NewComp2 = new()
            {
                ModelName = "Lenovo",
                PurchaseDate = DateTime.Parse("2022-05-12"),
                Price = 900,
                Office = "usa"
            };
            ContextPrep.Computers.Add(NewComp2);


            ComputerDB NewComp3 = new()
            {
                ModelName = "Asus",
                PurchaseDate = DateTime.Parse("2021-10-12"),
                Price = 1200,
                Office = "spain"
            };
            ContextPrep.Computers.Add(NewComp3);


            // Add phones
            PhoneDB NewPhone1 = new()
            {
                ModelName = "Iphone 13",
                PurchaseDate = DateTime.Parse("2022-09-22"),
                Price = 1200,
                Office = "spain"
            };
            ContextPrep.Phones.Add(NewPhone1);


            PhoneDB NewPhone2 = new()
            {
                ModelName = "Samsung 22",
                PurchaseDate = DateTime.Parse("2022-09-10"),
                Price = 1200,
                Office = "usa"
            };
            ContextPrep.Phones.Add(NewPhone2);


            PhoneDB NewPhone3 = new()
            {
                ModelName = "Nokia",
                PurchaseDate = DateTime.Parse("2020-03-03"),
                Price = 400,
                Office = "sweden"
            };
            ContextPrep.Phones.Add(NewPhone3);


            ContextPrep.SaveChanges();

        }


       


    }
}
