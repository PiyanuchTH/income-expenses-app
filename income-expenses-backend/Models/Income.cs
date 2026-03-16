namespace IncomeExpenses.Models
{
    public class Income
    {
        public int Id { get; set; }

        public string Type { get; set; }
        public string Catagory { get; set; }

        public decimal Amount { get; set; }

        public DateTime Date { get; set; }
        public string Description { get; set; }
        public DateTime? UpdateDate { get; set; }
        public DateTime CreateDate { get; set; }

    }
}
