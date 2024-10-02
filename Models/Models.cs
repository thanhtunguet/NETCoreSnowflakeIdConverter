namespace WebTestJsonConverter;

public class ParentModel
{
    public long? ParentId { get; set; } // Nullable long
    public string Name { get; set; }
    public ChildModel Child { get; set; }
}

public class ChildModel
{
    public long ChildId { get; set; } // Regular long
    public string Description { get; set; }
}