namespace FinanceApp.Models;

public enum TransactionType { Income, Expense }

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public TransactionType Type { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = null!;
}
