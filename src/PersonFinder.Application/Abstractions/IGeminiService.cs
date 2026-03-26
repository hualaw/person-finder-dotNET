namespace PersonFinder.Application.Abstractions;

public interface IGeminiService
{
    Task<string> GenerateBioAsync(
        string jobTitle,
        IEnumerable<string> hobbies,
        CancellationToken cancellationToken = default);
}
