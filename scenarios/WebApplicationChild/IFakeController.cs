using WebApplicationChild.Models;

public interface IFakeController
{
    bool GetData(Town town);
    DateTime GetDateTime(string city);
}