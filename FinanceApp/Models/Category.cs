namespace FinanceApp.Models
{
    public class Category
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        // Връзка към транзакции
        public ICollection<Transaction>? Transactions { get; set; }
    }
}
