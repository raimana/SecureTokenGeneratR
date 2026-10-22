using System.Collections.Generic;
using SecureTokenGeneratR.CharacterSets;

namespace SecureTokenGeneratR;

public static class SecureToken
{
    private static readonly SecureTokenGenerator DefaultGenerator = SecureTokenGenerator.Create();

    public static SecureTokenGenerator Default => DefaultGenerator;

    public static SecureTokenGenerator WithLength(uint tokenLength) =>
        SecureTokenGenerator.Create(tokenLength: tokenLength);

    public static SecureTokenGenerator WithMaxRepeatingCharCount(uint maxRepeatingCharCount) =>
        SecureTokenGenerator.Create(maxRepeatingCharCount: maxRepeatingCharCount);

    public static SecureTokenGenerator WithCharset(IEnumerable<char> characters) =>
        SecureTokenGenerator.Create(allowedCharSet: [new CharsetAsSequence(characters)]);

    public static SecureTokenGenerator WithCharset(int startRange, int endRange) =>
        SecureTokenGenerator.Create(allowedCharSet: [new CharsetAsUnicodeRange(startRange, endRange)]);

    public static SecureTokenGenerator WithCharsets(IEnumerable<char> characters) =>
        SecureTokenGenerator.Create(allowedCharSet: [new CharsetAsSequence(characters)]);

    public static SecureTokenGenerator WithCharsets(int startRange, int endRange) =>
        SecureTokenGenerator.Create(allowedCharSet: [new CharsetAsUnicodeRange(startRange, endRange)]);

    public static SecureTokenGenerator WithCharsets(IReadOnlyList<ICharacterSet> allowedCharsets) =>
        SecureTokenGenerator.Create(allowedCharSet: allowedCharsets);
}
