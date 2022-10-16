/*
 *  Kod för att genera  Asset-Tracking listor
 *  Som sorteras med hjälp av LINQ
 * 
 *  Återanvänder en hel del kod från min Asset-tracking.cs och todolist
 * 
 * 
 */

using AppEFDBClasses;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;




// fill prepared data into tables only at first run
TaskDB.FillDBdata();

//break for when making migrations or FillDBdata
Console.WriteLine("<DEBUG> break");
Console.ReadLine();






// ----------------- program start


// Create all input for tasks regarding the Product Assets
while (true)
{

    // Start() returnerar 1-4 (show products, add new products, edit products(update or remove), and exit
    int waschosen = Start();

    int ProdNR = 0;
    string thisModel = "";


    // add Product
    if (waschosen == 1 || waschosen == 2)
    {
        (string Ret, string model, DateTime dt, int price, string office) = ProductInput(waschosen, "ADD");

        // reset color and wait for data
        if (Ret != "ended")
        {
            Console.ResetColor();
            Console.WriteLine("Please standby... Data being prepared.");
            Console.WriteLine("");
        }

        if (Ret == "comp")
        {
            //add new laptop to database
            MyDbContext ContextAdd = new MyDbContext();

            ComputerDB NewProd = new()
            {
                ModelName = model,
                PurchaseDate = dt,
                Price = price,
                Office = office
            };
            ContextAdd.Computers.Add(NewProd);

            ContextAdd.SaveChanges();

            Console.WriteLine("New Laptop was added.");
            // Keep the console window open.
            Console.WriteLine("-------------------------------");
            Console.WriteLine("Press any key to continue.");
            Console.ReadKey();
            continue;

        }
        else if (Ret == "phone")
        {
            //add new Phone to database
            MyDbContext ContextAdd = new MyDbContext();

            PhoneDB NewProd = new()
            {
                ModelName = model,
                PurchaseDate = dt,
                Price = price,
                Office = office
            };
            ContextAdd.Phones.Add(NewProd);

            ContextAdd.SaveChanges();

            Console.WriteLine("New Phone was added.");
            // Keep the console window open.
            Console.WriteLine("-------------------------------");
            Console.WriteLine("Press any key to continue.");
            Console.ReadKey();
            continue;
        }
        else if (Ret == "ended")
        {
            Console.WriteLine("Database was unaltered.");
            Console.WriteLine("Exiting!");
            break;
        }

    }
    else if (waschosen == 3)
    {
        int sorting = choseSorting();

        SortedOutput(sorting);

        // Keep the console window open.
        Console.WriteLine("-------------------------------");
        Console.WriteLine("Press any key to continue.");
        Console.ReadKey();
        continue;

    }
    else if (waschosen == 4)
    {
        // edit or remove
        int editing = editMode();

        var theIds = new List<KeyValuePair<int, int>>();

        if (editing == 1 || editing == 2)
        {
            // edit one of the products
            (ProdNR, thisModel, theIds) = ProductEdit(editing);

            if (ProdNR > 0)
            {
                var kvpPair = theIds.SingleOrDefault(kvp => kvp.Key == ProdNR);
                var pmKey = kvpPair.Value;

                // edit the line
                (string Ret, string model, DateTime dt, int price, string office) = ProductInput(editing, "EDIT");

                if (Ret == "comp")
                {

                    if (editThis("EDITCOMP", model, dt, price, office, pmKey))
                    {
                        Console.WriteLine("Edit of " + thisModel + " was a success");
                    }
                    else
                    {
                        Console.WriteLine("Product List was NOT altered");
                    }
                }
                else if (Ret == "phone")
                {
                    if (editThis("EDITPHONE", model, dt, price, office, pmKey))
                    {
                        Console.WriteLine("Edit of " + thisModel + " was a success");
                    }
                    else
                    {
                        Console.WriteLine("Product List was NOT altered");
                        continue;
                    }
                }
                else
                {
                    Console.WriteLine("Database was NOT altered");
                    continue;
                }
            }
            else
            {
                Console.WriteLine("Database was NOT altered");
                continue;
            }
        }
        else if (editing == 3 || editing == 4)
        {
            // remove one of the products
            (ProdNR, thisModel, theIds) = ProductEdit(editing);

            if (ProdNR > 0) {
 
                var kvpPair = theIds.SingleOrDefault(kvp => kvp.Key == ProdNR);
                var pmKey = kvpPair.Value;

                var dbToRemFrom = "";

                if (editing == 3)
                {
                    dbToRemFrom = "REMCOMP";
                }
                else if (editing == 4)
                {
                    dbToRemFrom = "REMPHONE";
                }

                if (editThis(dbToRemFrom, "", DateTime.Now, 0, "", pmKey))
                {
                    Console.WriteLine("Removal of " + thisModel + " was a success");
                }
                else
                {
                    Console.WriteLine("Product List was NOT altered");
                }
            }
            else
            {
                Console.WriteLine("Product List was NOT altered");
            }

        }
        // Keep the console window open.
        Console.WriteLine("-------------------------------");
        Console.WriteLine("Press any key to continue.");
        Console.ReadKey();
        continue;

    }
    else
    {
        Console.WriteLine("Product list Exit.");
        break;
    }


}


// ----------------- end program



// Methods

static int Start()
{

    string firstinput = "";
    bool Success = false;
    int category = 0;

    while (true)
    {

        Console.ResetColor();
        Console.WriteLine("-------------------------------------");
        Console.WriteLine("Product list with Laptops and Phones.");
        Console.WriteLine("-------------------------------------");
        Console.WriteLine("[1] Add new Laptop.");
        Console.WriteLine("[2] Add new Phone.");
        Console.WriteLine("[3] Show Product lists.");
        Console.WriteLine("[4] Edit Product(or Remove)");
        Console.WriteLine("or End with q.");
        Console.WriteLine("");
        Console.Write("Enter [1-4]: ");

        try
        {
            firstinput = Console.ReadLine();

            if (firstinput.ToLower().Trim() == "q")
            {
                Console.WriteLine("Exit.");
                break;
            }

            Success = int.TryParse(firstinput, out category);

        }
        catch (Exception e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Wrong Input. Error: " + e);
            Console.WriteLine("Try again.");
            continue;
        }
        if (Success)
        {
            if (category > 0 && category < 5)
            {
                break;
            }
            else
            {
                Console.WriteLine("Must be between 1-4. Try again.");
                continue;
            }
        }
    }
    if (Success)
    {
        return category;
    }
    else
    {
        return -1;
    }

}


static int choseSorting()
{

    int newsort = 0;
    bool Success = false;
    var firstinput = "";

    Console.ResetColor();
    Console.WriteLine("-----------------------------");
    Console.WriteLine("Chose Sorting");
    Console.WriteLine("-----------------------------");
    Console.WriteLine("[1] Sort by Office.");
    Console.WriteLine("[2] Sort by Purchase Date.");
    Console.Write("Enter Choice [1-2]: ");

    while (true)
    {

        try
        {
            firstinput = Console.ReadLine();

            if (firstinput.ToLower().Trim() == "q")
            {
                Console.WriteLine("Exit.");
                break;
            }

            Success = int.TryParse(firstinput, out newsort);

        }
        catch (Exception e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Wrong Input. Error: " + e);
            Console.WriteLine("Try again.");
            continue;
        }
        if (Success)
        {
            if (newsort > 0 && newsort < 3)
            {
                break;
            }
            else
            {
                Console.WriteLine("Must be [1-2]. Try again.");
                continue;
            }
        }
    }
    if (Success)
    {
        return newsort;
    }
    else
    {
        return 0;
    }

}


static int editMode()
{
    int choice = 0;
    bool Success = false;
    var firstinput = "";

    Console.ResetColor();
    Console.WriteLine("-----------------------------");
    Console.WriteLine("Choose Edit/Remove");
    Console.WriteLine("-----------------------------");
    Console.WriteLine("[1] Edit a Laptop.");
    Console.WriteLine("[2] Edit a Phone.");
    Console.WriteLine("[3] Remove a Laptop.");
    Console.WriteLine("[4] Remove a Phone.");

    Console.Write("Enter Choice[1-4]: ");

    while (true)
    {

        try
        {
            firstinput = Console.ReadLine();

            if (firstinput.ToLower().Trim() == "q")
            {
                Console.WriteLine("Exit");
                break;
            }

            Success = int.TryParse(firstinput, out choice);

        }
        catch (Exception e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Wrong Input. Error: " + e);
            Console.WriteLine("Try again.");
            continue;
        }
        if (Success)
        {
            if (choice > 0 && choice < 5)
            {
                break;
            }
            else
            {
                Console.WriteLine("Must be [1-4]. Try again.");
                continue;
            }
        }
    }
    if (Success)
    {
        return choice;
    }
    else
    {
        return 0;
    }

}


static void SortedOutput(int sortstyle)
{

    // reset color and wait for data
    Console.ResetColor();
    Console.WriteLine("Please standby... Data being prepared.");
    Console.WriteLine("");

    // init list
    List<Computer> ProductListComp = new();
    List<Phone> ProductListPhone = new();

    // get Database data
    MyDbContext ContextSel = new MyDbContext();

    // for this to work I would have to make the ComputerDB amd PhoneDB also have an interface like the old classes have
    //List<ComputerDB> ProductListComp = ContextSel.Computers.ToList();
    //List<PhoneDB> ProductListPhone = ContextSel.Phones.ToList();

    // get all data from DB into the old classes to be able to use Concat() via the interface
    var productsComp = ContextSel.Computers; // define query
    foreach (var row in productsComp) // query executed and data obtained from database
    {
        //Console.WriteLine(row.ModelName + " " + row.PurchaseDate + " " + row.Price + " " + row.Office);
        ProductListComp.Add(new Computer(row.ModelName, row.PurchaseDate, row.Price, row.Office));
    }

    var productsPhone = ContextSel.Phones; // define query
    foreach (var row in productsPhone) // query executed and data obtained from database
    {
        ProductListPhone.Add(new Phone(row.ModelName, row.PurchaseDate, row.Price, row.Office));
    }


    if (sortstyle == 1)
    {
        // Sort by Office

        //Due date how long ago?
        DateTime now = DateTime.Now;
        DateTime months6away = now.AddMonths(-30);
        DateTime months3away = now.AddMonths(-33);
        DateTime threeyearsago = now.AddMonths(-36);

        Console.ResetColor();
        Console.WriteLine("----------------------------------------");
        Console.WriteLine("Products sorted by Office.");
        Console.WriteLine("----------------------------------------");
        Console.WriteLine("Type".PadRight(15) + "Brand+Model".PadRight(15) + "Office".PadRight(15) + "Purchase Date".PadRight(15) + "Price in USD".PadRight(15) + "Currency".PadRight(15) + "Local price today");


        //  order by Office
        // concanate both Lists and order by Office
        var orderedResultOffice = ProductListComp.Concat<IProducts>(ProductListPhone).OrderBy(q => q.Office);

        foreach (var obj in orderedResultOffice)
        {
            if (obj.PurchaseDate > threeyearsago)
            {
                if (obj.PurchaseDate < months3away)
                {
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.BackgroundColor = ConsoleColor.Red;
                    obj.ShowProduct();
                    Console.ResetColor();
                }
                else if (obj.PurchaseDate < months6away)
                {
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.BackgroundColor = ConsoleColor.Yellow;
                    obj.ShowProduct();
                    Console.ResetColor();
                }
                else
                {
                    Console.ResetColor();
                    obj.ShowProduct();
                }
            }
            Console.ResetColor();
        }
    }
    else if (sortstyle == 2)
    {
        // Sort by Purchase date

        Console.ResetColor();
        Console.WriteLine("----------------------------------------");
        Console.WriteLine("Products sorted by Purchase date.");
        Console.WriteLine("----------------------------------------");
        Console.WriteLine("Type".PadRight(15) + "Brand+Model".PadRight(15) + "Office".PadRight(15) + "Purchase Date".PadRight(15) + "Price in USD".PadRight(15) + "Currency".PadRight(15) + "Local price today");


        // order by Purchase Date
        // concanate both Lists and order by Date
        var orderedResultDate = ProductListComp.Concat<IProducts>(ProductListPhone).OrderBy(q => q.PurchaseDate);

        //Due date how long ago?
        DateTime now = DateTime.Now;
        DateTime months6away = now.AddMonths(-30);
        DateTime months3away = now.AddMonths(-33);
        DateTime threeyearsago = now.AddMonths(-36);

        foreach (var obj in orderedResultDate)
        {

            if (obj.PurchaseDate > threeyearsago)
            {
                if (obj.PurchaseDate < months3away)
                {
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.BackgroundColor = ConsoleColor.Red;
                    obj.ShowProduct();
                    Console.ResetColor();
                }
                else if (obj.PurchaseDate < months6away)
                {
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.BackgroundColor = ConsoleColor.Yellow;
                    obj.ShowProduct();
                    Console.ResetColor();
                }
                else
                {
                    Console.ResetColor();
                    obj.ShowProduct();
                }
            }
            Console.ResetColor();
        }
    }

}


static (int, string, List<KeyValuePair<int,int>>) ProductEdit(int editing)
{

    string input = "";
    int choice = 0;
    bool Success = false;
    string thisModel = "";

    // reset color and wait for data
    Console.ResetColor();
    Console.WriteLine("-----------------------------");
    Console.WriteLine("Productlist");
    Console.WriteLine("-----------------------------");
    Console.WriteLine("Please standby... Data being prepared.");
    Console.WriteLine("");

    // init primary key list
    var theIds = new List<KeyValuePair<int,int>>();

    // get Database data
    MyDbContext ContextSel = new MyDbContext();

    if (editing == 1 || editing == 2)
    {
        int iter = 0;
        var productsComp = ContextSel.Computers; // define query
        foreach (var row in productsComp) // query executed and data obtained from database
        {
            //Console.WriteLine(row.ModelName + " " + row.PurchaseDate + " " + row.Price + " " + row.Office); // debug
            iter++;
            Console.WriteLine("[" + iter + "] Edit: " + row.ModelName + " - in Office: " + row.Office);
            theIds.Add(new KeyValuePair<int, int>(iter, row.Id));
            thisModel = row.ModelName;
        }

        Console.Write("Chose Product to Edit: ");
        input = Console.ReadLine();
        Success = int.TryParse(input, out choice);

    }
    else if (editing == 3 || editing == 4)
    {
        int iter = 0;
        var productsPhone = ContextSel.Phones; // define query
        foreach (var row in productsPhone) // query executed and data obtained from database
        {
            iter++;
            Console.WriteLine("[" + iter + "] Remove: " + row.ModelName + " - in Office: " + row.Office);
            theIds.Add(new KeyValuePair<int, int>(iter, row.Id));
            thisModel = row.ModelName;
        }

        Console.Write("Chose Product to Remove: ");
        input = Console.ReadLine();
        Success = int.TryParse(input, out choice);

    }

    

    if (Success && choice > 0)
    {
        return (choice, thisModel, theIds);
    }
    else
    {
        return (-1, thisModel, theIds);
    }

}


static bool editThis(string action, string modelname, DateTime dt, int price, string office, int pmKey)
{
    bool success = false;

    //Console.WriteLine("id: " + pmKey); // debug
    // reset color and wait for data
    Console.ResetColor();
    Console.WriteLine("Please standby... Data being prepared.");
    Console.WriteLine("");

    if (action == "EDITCOMP")
    {
        MyDbContext ContextUpdate = new MyDbContext();

        var ComputerEdit = ContextUpdate.Computers.SingleOrDefault(x => x.Id == pmKey);

        ComputerEdit.ModelName = modelname;
        ComputerEdit.PurchaseDate = dt;
        ComputerEdit.Price = price;
        ComputerEdit.Office = office;
        ContextUpdate.SaveChanges();
        success = true;
    }
    else if (action == "EDITPHONE")
    {
        MyDbContext ContextUpdate = new MyDbContext();

        var PhoneEdit = ContextUpdate.Computers.SingleOrDefault(x => x.Id == pmKey);

        PhoneEdit.ModelName = modelname;
        PhoneEdit.PurchaseDate = dt;
        PhoneEdit.Price = price;
        PhoneEdit.Office = office;
        ContextUpdate.SaveChanges();
        success = true;
    }
    else if (action == "REMCOMP")
    {
        MyDbContext ContextUpdate = new MyDbContext();

        var ComputersRow = ContextUpdate.Computers.SingleOrDefault(x => x.Id == pmKey);

        ContextUpdate.Computers.Remove(ComputersRow);
        ContextUpdate.SaveChanges();
        success = true;
    }
    else if (action == "REMPHONE")
    {
        MyDbContext ContextUpdate = new MyDbContext();

        var PhonesRow = ContextUpdate.Phones.SingleOrDefault(x => x.Id == pmKey);

        ContextUpdate.Phones.Remove(PhonesRow);
        ContextUpdate.SaveChanges();
        success = true;
    }

   
    if (success)
    {
        return true;
    }
    else
    {
        return false;
    }

}




// add or edit Products via user input
static (string, string, DateTime, int, string) ProductInput(int chosen, string action)
{

    // declare empty strings
    string priceInput = "";
    string modelInput = "";
    string dateInput = "";
    string officeInput = "";

    DateTime truedate = DateTime.Now;
    int intPrice = 0;


    // reset color
    Console.ResetColor();
    Console.WriteLine("");
    if (chosen == 1)
    {
        if (action == "ADD")
        {
            Console.WriteLine("Add new Laptop!");
        }
        else
        {
            Console.WriteLine("Edit a Laptop!");
        }
    }
    else if (chosen == 2)
    {
        if (action == "ADD")
        {
            Console.WriteLine("Add new Phone!");
        }
        else
        {
            Console.WriteLine("Edit a Phone!");
        }
    }

    Console.WriteLine("Exit with q");
    Console.WriteLine("");


    // input modelname
    while (true)
    {

        try
        {
            Console.ResetColor();
            Console.Write("Enter product modelname: ");
            modelInput = Console.ReadLine();

            if (modelInput.ToLower().Trim() == "q")
            {
                Console.WriteLine("Exit.");
                break;
            }

        }
        catch (Exception e)
        {
            Console.ForegroundColor = ConsoleColor.Red;

            Console.WriteLine("Wrong Input. Error: " + e);
            Console.WriteLine("Try Again.");
            continue;

        }

        if (modelInput.Trim() == "")
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Wrong input! Seems to be empty. Try again.");
            continue;
        }
        else
        {
            break;
        }
    }
    if (modelInput.ToLower().Trim() == "q")
    {
        return ("ended", "none", DateTime.Now, 0, "");
    }



    // input Product purchase date
    while (true)
    {

        try
        {
            Console.ResetColor();
            Console.Write("Enter the product Purchase date (like this 2022-06-06): ");
            dateInput = Console.ReadLine();

            if (dateInput.ToLower().Trim() == "q")
            {
                Console.WriteLine("Exit.");
                break;
            }

            truedate = DateTime.Parse(dateInput);

        }
        catch (Exception e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Wrong input. Error: " + e);
            Console.WriteLine("Try again.");
            continue;

        }

        if (dateInput.Trim() == "")
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Wrong input. Seems to be empty. Try again.");
            continue;
        }
        else
        {
            break;
        }

    }
    if (dateInput.ToLower().Trim() == "q")
    {
        return ("ended", "none", DateTime.Now, 0, "");
    }



    // input Product Price
    while (true)
    {

        bool Success = false;

        try
        {
            Console.ResetColor();
            Console.Write("Enter product price in USD: ");
            priceInput = Console.ReadLine();

            if (priceInput.ToLower().Trim() == "q")
            {
                Console.WriteLine("Exit.");
                break;
            }

            Success = int.TryParse(priceInput, out intPrice);

        }
        catch (Exception e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Wrong input. Error: " + e);
            Console.WriteLine("Try again.");
            continue;

        }

        if (Success == false)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Wrong price value! Try again (only numbers!)");
            continue;
        }
        else
        {
            break;
        }

    }
    if (priceInput.ToLower().Trim() == "q")
    {
        return ("ended", "none", DateTime.Now, 0, "");
    }



    // input office
    string myOffice = "";
    while (true)
    {

        try
        {
            Console.ResetColor();
            Console.Write("Enter Office: ");
            officeInput = Console.ReadLine();

            if (officeInput.ToLower().Trim() == "q")
            {
                Console.WriteLine("Exit.");
                break;
            }

        }
        catch (Exception e)
        {
            Console.ForegroundColor = ConsoleColor.Red;

            Console.WriteLine("Wrong input  Error: " + e);
            Console.WriteLine("Try again.");
            continue;

        }

        if (officeInput.Trim() == "")
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Wrong input! Seems to be empty. Try again.");
            continue;
        }

        myOffice = officeInput.ToLower().Trim();
        if (myOffice == "usa" || myOffice == "spain" || myOffice == "sweden")
        {
            break;
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Wrong Office! Can only be (USA, Spain , Sweden).");
            Console.WriteLine("Try again.");
            continue;
        }
    }
    if (modelInput.ToLower().Trim() == "q")
    {
        // , "Sydpolen" är ett skämt eftersom den returnerade variablen inte används vid fel eller avslut
        return ("ended", "none", DateTime.Now, 0, "Sydpolen");
    }

    // Return data for new entries in lists
    if (chosen == 1)
    {
        return ("comp", modelInput, truedate, intPrice, myOffice);
    }
    else if (chosen == 2)
    {
        return ("phone", modelInput, truedate, intPrice, myOffice);
    }
    else
    {
        return ("ended", "none", DateTime.Now, 0, "");
    }
    Console.ResetColor();


}






// CLASSES

interface IProducts
{
    public DateTime PurchaseDate { get; }

    public string Office { get; }

    void ShowProduct();

}


class Computer : IProducts
{

    public Computer(string modelName, DateTime purchaseDate, int price, string office)
    {
        ModelName = modelName;
        PurchaseDate = purchaseDate;
        Price = price;
        Office = office;
    }

    public string ModelName { get; set; }
    public DateTime PurchaseDate { get; set; }
    public int Price { get; set; }
    public string Office { set; get; }


    public void ShowProduct()
    {
        string currency = "";
        double localprice = 0.0;
        string MyOffice = "";

        if (Office == "sweden")
        {
            localprice = Price * 11.31;
            currency = "SEK";
            MyOffice = "Sweden";
        }
        else if (Office == "spain")
        {
            localprice = Price * 1.03;
            currency = "EUR";
            MyOffice = "Spain";
        }
        else if (Office == "usa")
        {
            localprice = Price;
            currency = "USD";
            MyOffice = "USA";
        }

        Console.WriteLine("Computer".PadRight(15) + ModelName.PadRight(15) + MyOffice.PadRight(15) + PurchaseDate.ToString("MM/dd/yyyy").PadRight(15) + Price.ToString().PadRight(15) + currency.PadRight(15) + localprice);

    }

}


class Phone : IProducts
{
    public Phone(string modelName, DateTime purchaseDate, int price, string office)
    {
        ModelName = modelName;
        PurchaseDate = purchaseDate;
        Price = price;
        Office = office;
    }

    public string ModelName { get; set; }
    public DateTime PurchaseDate { get; set; }
    public int Price { get; set; }
    public string Office { set; get; }

    public void ShowProduct()
    {
        string currency = "";
        double localprice = 0.0;
        string MyOffice = "";

        if (Office == "sweden")
        {
            localprice = Price * 11.31;
            currency = "SEK";
            MyOffice = "Sweden";
        }
        else if (Office == "spain")
        {
            localprice = Price * 1.03;
            currency = "EUR";
            MyOffice = "Spain";
        }
        else if (Office == "usa")
        {
            localprice = Price;
            currency = "USD";
            MyOffice = "USA";
        }

        Console.WriteLine("Phone".PadRight(15) + ModelName.PadRight(15) + MyOffice.PadRight(15) + PurchaseDate.ToString("MM/dd/yyyy").PadRight(15) + Price.ToString().PadRight(15) + currency.PadRight(15) + localprice);

    }

    public void ShowOffice()
    {
        Console.Write("Office: " + Office);
    }

}
