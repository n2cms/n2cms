namespace N2.Persistence.Serialization
{
    public class WrongVersionException : DeserializationException
    {
        public WrongVersionException(string message)
            : base(message)
        {
        }
    }
}
