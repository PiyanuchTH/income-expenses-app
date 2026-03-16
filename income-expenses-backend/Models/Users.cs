namespace IncomeExpenses.Models
{
    public class Users
    {
            public int Id { get; set; }
            public string Username { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
            public string? FullName { get; set; }
            public DateTime CreateDate { get; set; }
    }


}
