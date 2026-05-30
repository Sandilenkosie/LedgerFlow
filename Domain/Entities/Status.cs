namespace Domain.Entities;

public class Status
{
    // EF Core materialization
    protected Status() { }

    public Guid Id { get; private set; }
    public string Name { get; private set; }

    // Well-known ids for convenience
    public static readonly Guid OpenId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    public static readonly Guid ClosedId = Guid.Parse("22222222-2222-2222-2222-222222222222");

    public Status(string name)
    {
        Name = name ?? string.Empty;
    }

    public static Status Open() => new Status("Open") { Id = OpenId };
    public static Status Closed() => new Status("Closed") { Id = ClosedId };
}
