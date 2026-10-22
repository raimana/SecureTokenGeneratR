# SecureTokenGeneratR

![Build status](https://github.com/raimana/SecureTokenGeneratR/actions/workflows/workflow.yml/badge.svg)

SecureTokenGeneratR generates random tokens using .NET's [cryptographically secure random number generator](https://learn.microsoft.com/dotnet/api/system.security.cryptography.randomnumbergenerator). It has no external dependencies.

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

Start with `SecureToken` to configure a generator and reuse it to create multiple tokens:

```csharp
using SecureTokenGeneratR;

var generator = SecureToken.WithLength(24)
    .WithMaxRepeatingCharCount(2);

string firstToken = generator.Generate();
string secondToken = generator.Generate();
```

Each `With...` method returns a new configured generator. `WithCharset` adds a sequence or inclusive Unicode range to the current sets. `WithCharsets` replaces all current sets; choose an overload based on the configuration you need:

- `WithCharsets(sequence)` replaces the sets with one character sequence.
- `WithCharsets(start, end)` replaces them with one inclusive Unicode range.
- `WithCharsets(charsets)` replaces them with the supplied `ICharacterSet` collection, allowing multiple built-in or custom sets.

### Custom character sets

Define character sets directly with the overloaded `WithCharset` method:

```csharp
using SecureTokenGeneratR;

var generator = SecureToken.WithCharset("%XW-[]{}")
    .WithCharset('A', 'F')
    .WithLength(8)
    .WithMaxRepeatingCharCount(2);

string token = generator.Generate();
var digitsOnlyGenerator = generator.WithCharsets('0', '9');
```

The generated token includes at least one character from each configured set and no more than two occurrences of any character. `digitsOnlyGenerator` replaces both configured sets with the digits 0 through 9.

To replace the current sets with several sets, pass a collection:

```csharp
using SecureTokenGeneratR;
using SecureTokenGeneratR.CharacterSets;

ICharacterSet[] charsets =
[
    new CharsetAsSequence("%XW-[]{}"),
    new CharsetAsUnicodeRange('A', 'F')
];

var mixedGenerator = SecureToken.Default.WithCharsets(charsets);
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

Targets .NET Framework 4.6.1, .NET Standard 2.0 and 2.1, and .NET 6, 8, and 10.