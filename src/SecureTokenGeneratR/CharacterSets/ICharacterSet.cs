namespace SecureTokenGeneratR.CharacterSets
{
    public interface ICharacterSet
    {
        public int Count { get;  }
        char GetRandomChar();
    }
}