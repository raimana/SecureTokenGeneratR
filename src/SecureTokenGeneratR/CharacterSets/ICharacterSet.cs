namespace SecureTokenGeneratR.CharacterSets;

/// <summary>
/// Represents a set of characters from which a character can be selected.
/// </summary>
public interface ICharacterSet
{
    /// <summary>
    /// Gets the number of distinct characters in the set.
    /// </summary>
    public int Count { get; }

    /// <summary>
    /// Selects a random character from the set.
    /// </summary>
    /// <returns>A randomly selected character.</returns>
    char GetRandomChar();
}