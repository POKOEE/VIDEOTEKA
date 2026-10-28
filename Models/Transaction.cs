namespace VideoLibraryApi.Models;

// Тип операции
public enum TransactionType
{
    TopUp,      // Пополнение
    Payment,    // Оплата аренды
    Refund      // Возврат
}

// Способ оплаты
public enum PaymentMethod
{
    Balance,    // Баланс
    Card,       // Банковская карта
    Cash        // Наличные
}

public class Transaction
{
    public int TransactionId { get; set; }

    public int CustomerId { get; set; }
    public Customer? Customer { get; set; }

    public decimal Amount { get; set; }
    public TransactionType Type { get; set; }
    public PaymentMethod Method { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? Description { get; set; }
}