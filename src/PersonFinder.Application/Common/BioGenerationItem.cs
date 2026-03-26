namespace PersonFinder.Application.Common;

public sealed record BioGenerationItem(
    long PersonId,
    string JobTitle,
    IReadOnlyCollection<string> Hobbies);
