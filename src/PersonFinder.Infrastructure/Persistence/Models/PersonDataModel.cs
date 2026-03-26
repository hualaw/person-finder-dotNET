namespace PersonFinder.Infrastructure.Persistence.Models;

public sealed class PersonDataModel
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public string[] Hobbies { get; set; } = Array.Empty<string>();
}
