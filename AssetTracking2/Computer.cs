namespace AppEFDBClasses
{
    internal class ComputerDB
    {
        public int Id { get; set; }
        public string? ModelName { get; set; }
        public DateTime PurchaseDate { get; set; }
        public int Price { get; set; }
        public string? Office { set; get; }

    }
}
