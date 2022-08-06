namespace TestChecker.Core
{
    public interface IObjectSerialiser
    {
        void SetObject(object obj);
        string SerialiseObject(int depth);
        bool IsObject();
    }
}