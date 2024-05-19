Secure Token Generator
==================
![Main Build](https://github.com/raimana/SecureTokenGeneratR/actions/workflows/workflow.yml/badge.svg)

Generate random passwords or token; use the default generation rules or set your own.  
Using [System.Security.Cryptography.RandomNumberGenerator](https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.randomnumbergenerator) to generate cryptographically strong random values.

* Dependencies: none
* Compatibility: NET Framework 4.6.1+, Net Standard 1.0+, .NET 5.0+

### Default Characters Sets
* All numbers
* All lower case latin
* All upper case latin
* Special characters: !"#$%&'()*+,-./:;<=>?@[\]^_`{|}~

### Default Rules
* Length: 16 characters
* Max repeating characters: 0 (unlimited)

## Generator Algorithm
1. Populate random positions of the token with at least one random character from each character set.
2. Populate all unfilled positions in a random order, each using a random character from a randomly selected characters set.
3. Honour the max repeating characters constraint, replace any repeated character exceeding the constraint threshold, each using a random character from a randomly selected characters set.

## Installing SecureTokenGeneratR
Install [SecureTokenGeneratR with NuGet](https://www.nuget.org/packages/SecureTokenGeneratR):
```c#
Install-Package SecureTokenGeneratR
```
Or the .Net Core CLI
```c#
dotnet add package SecureTokenGeneratR
```

## Usage
### With Default Options
```c#
var secureTokenGenerator = new SecureTokenGenerator();
string secureToken = secureTokenGenerator.Generate();
```
The token generated using the default options will contain 16 characters, with **at least** one character from each character set (i.e. lower latin, upper latin, number & special character) without limiting the number of occurrence of any character.

**Notes:**
1. while improbable, a random sequence such as `1111111111111111` is not impossible.
2. the default options will generate tokens with 104.87 bits of entropy.

### With User Defined Options
```c#
// Define custom options
var customCharset1 = new CharsetAsSequence("%XW-[]{}"); // Custom characters sets can be provided as string
var customCharset2 = new CharsetAsUnicodeRange(65, 70); // Or as a unicode range, here ABCDEF
var allowedCharsets = new List<ICharacterSet> { customCharset1, customCharset2 };

const int tokenLength = 8;
const int maxRepeatingCharCount = 2; // at most two instances of any characters from all character sets 

var secureTokenGenerator = new SecureTokenGenerator(tokenLength, maxRepeatingCharCount, allowedCharsets);
string secureToken = secureTokenGenerator.Generate();
```
The token generated in the example above will contain 8 characters, **at least** one character from each characters set and **at most** two occurrences of any character from all characters sets.

### With User Defined Options - All Chars Except Controls
Perhaps a bit extreme...
```c#
var allExceptControlChars = Enumerable.Range(0, char.MaxValue+1)
    .Select(i => (char) i)
    .Where(chr => !char.IsControl(chr));

var charset = new CharsetAsSequence(allExceptControlChars);
var allowedCharSet = new List<ICharacterSet> { charset };

var secureTokenGenerator = new SecureTokenGenerator(128, 0, allowedCharSet);
string password = secureTokenGenerator.Generate(); 
// ꔫ檳蛪⇾᛼⩫뷾⛨㔟瘲⇐綇ꠎ굩胬㽇줉㫊ⅈ螵蕾뚏涙㼗쎕肁랎汅灸澋큺葾碑丆ኤ䘼㣬﹭క䬈就뒊ꄳ辬☴≣욺션煢我㛣⦥Ფ鈺녝㥃㕅鄵䳄0᪰䴖ꁹ巕唍◠梩ꦇ植?茹阁봻࡚㕦ꤑ麛濫?뗿煦㾏崏஌ꛄ祖쑖㏆⫈?ç櫔￭근틷褾剳聶墔諄ﲺ蕩늘単䷐慓愌ዦ烡쬩ᬐ봸ཌᤶ
```

### Get Entropy Metrics
[See Entropy](https://en.wikipedia.org/wiki/Entropy_(information_theory))
```c#
var secureTokenGenerator = new SecureTokenGenerator();

// Based on characters space and length
double tokenEntropy = secureTokenGenerator.GetTokenEntropy;
Console.WriteLine(tokenEntropy); // 104.8734216268422

//--

string secureToken = secureTokenGenerator.Generate();

// Number of bits needed to encode each characters, based on repeated characters.
double characterEntropy = secureTokenGenerator.GetCharacterEntropy(token);
Console.WriteLine(characterEntropy); // 4 
```
**Note:** Using entropy alone as a strength factor is not a good approach.