using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using static PasswordGenerator.CryptographicNumberGenerator;

namespace PasswordGenerator.CharacterSets
{
    public class CharsCollectionCharacterSet : ICharacterSet
    {
        private readonly char[] _charactersAsCharArray;
        public int Count { get; }

        public CharsCollectionCharacterSet(IEnumerable<char> charactersCollection)
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