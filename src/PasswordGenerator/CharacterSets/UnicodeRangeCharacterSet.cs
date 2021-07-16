using System;
using System.Diagnostics;
using static PasswordGenerator.CryptographicNumberGenerator;

namespace PasswordGenerator.CharacterSets
{
    public class UnicodeRangeCharacterSet : ICharacterSet
    {
        private readonly int _startRange;
        public int Count { get; }

        public UnicodeRangeCharacterSet(int startRange, int endRange)
        {
            if (startRange >= endRange) 
                throw new ArgumentOutOfRangeException(nameof(startRange), startRange, "Start is equal or greater than end of range");

            Count = endRange - startRange + 1;
            _startRange = startRange;
        }
        

        public char GetRandomChar()
        {
            var randomCharSelected = '\0';
            
            if (Count > 0)
            {
                randomCharSelected = (char) (_startRange + GetRandomUInt() % Count);
            }
            
            Debug.Assert(randomCharSelected != '\0', $"{nameof(GetRandomChar)} returned a null character"); 
                
            return randomCharSelected;
        }
    }
}