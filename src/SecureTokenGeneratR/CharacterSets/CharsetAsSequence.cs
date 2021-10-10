using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using static SecureTokenGeneratR.CryptographicNumberGenerator;

namespace SecureTokenGeneratR.CharacterSets
{
    public class CharsetAsSequence : ICharacterSet
    {
        private readonly char[] _charactersAsCharArray;
        public int Count { get; }

        public CharsetAsSequence(IEnumerable<char> charactersCollection)
        {
            _charactersAsCharArray = charactersCollection.Distinct().ToArray();
            Count = _charactersAsCharArray.Length;
        }


        public char GetRandomChar()
        {
            var randomCharSelected = '\0';
            
            if (Count > 0)
            {
                randomCharSelected = _charactersAsCharArray[GetRandomUInt() % Count];
            }
            
            Debug.Assert(randomCharSelected != '\0', $"{nameof(GetRandomChar)} returned a null character"); 
                
            return randomCharSelected;
        }
    }
}