# SecureTokenGeneratR

![Build status](https://github.com/raimana/SecureTokenGeneratR/actions/workflows/workflow.yml/badge.svg)

SecureTokenGeneratR generates random tokens using .NET's [cryptographically secure random number generator](https://learn.microsoft.com/dotnet/api/system.security.cryptography.randomnumbergenerator).
No external dependencies.

## Install

```shell
dotnet add package SecureTokenGeneratR
```

## Defaults

By default, generated tokens:

- Are 16 characters long.
- Include at least one character from each set: lowercase letters, uppercase letters, digits, and ASCII punctuation.
- Have no limit on repeated characters.

## Usage

### Default generator

Use `SecureToken.Default` to generate tokens with the default settings:

```csharp
using SecureTokenGeneratR;

string token = SecureToken.Default.Generate();
```

### Fluent configuration

Each call returns a new generator, so options can be chained:

| Call | Effect |
| --- | --- |
| `SecureToken.Default` | Starts with the default length and character sets. |
| `WithLength(length)` | Sets a fixed token length. |
| `WithLength(minimum, maximum)` | Randomly chooses an inclusive length between `minimum` and `maximum` for each generated token. |
| `WithMaxRepeatingCharCount(count)` | Limits how many times any one character may appear; `0` means no limit. |
| `AddCharset(sequence)` | Adds a character sequence to the current allowed sets. When called on `SecureToken`, starts with the default sets. |
| `AddCharset(start, end)` | Adds an inclusive Unicode range to the current allowed sets. When called on `SecureToken`, starts with the default sets. |
| `ReplaceCharsets(sequence)` | Replaces all current allowed sets with one character sequence. |
| `ReplaceCharsets(start, end)` | Replaces all current allowed sets with one inclusive Unicode range. |
| `ReplaceCharsets(charsets)` | Replaces all current allowed sets with the supplied `ICharacterSet` collection. |
| `Generate()` | Generates a token using the configured options. |

`AddCharset` adds; `ReplaceCharsets` replaces. Generated tokens include at least one character from each configured set.

For example, this generator adds two sets and picks a random token length from 8 to 24. The second generator replaces those sets with digits:

```csharp
using SecureTokenGeneratR;

var generator = SecureToken.AddCharset("%XW-[]{}") // character sequence
    .AddCharset('A', 'F') // inclusive Unicode range: U+0041 to U+0046
    .WithLength(8, 24)
    .WithMaxRepeatingCharCount(2);

string token = generator.Generate();
var digitsOnlyGenerator = generator.ReplaceCharsets('0', '9');
```

For multiple custom sets, construct each with `CharacterSet` and pass them to `ReplaceCharsets`:

```csharp
using SecureTokenGeneratR.CharacterSets;

ICharacterSet[] charsets =
[
    new CharacterSet("abc"), // character sequence
    new CharacterSet('0', '9') // inclusive Unicode range: U+0030 to U+0039
];
var customGenerator = SecureToken.Default.ReplaceCharsets(charsets);
```

### Entropy

The generator exposes estimates for token entropy and per-character entropy:

```csharp
var generator = SecureToken.Default;
string token = generator.Generate();

double tokenEntropy = generator.GetTokenEntropy;
double characterEntropy = generator.GetCharacterEntropy(token);
```

Entropy is one measure of strength and should not be used as the only security assessment.

## Compatibility

Targets .NET Framework 4.6.1 and 4.8.1, .NET Standard 2.0 and 2.1, and .NET 6, 8, and 10.