using WebApplicationChild.Models;
namespace WebApplicationChild;

public interface IFakeController
{
    bool GetData(Town town);
    DateTime GetDateTime(string city);
}