namespace ServiceA.Models;

public class TransactionState
{
    public Guid Id { get; }

    public bool FromA { get; set; }
    public bool FromB { get; set; }


    public TransactionState(Guid id)
    {
        Id = id;
        CreatedAt = DateTime.UtcNow;
    }

    public DateTime CreatedAt { get; }
    
    public bool IsComplete => FromA && FromB;

}