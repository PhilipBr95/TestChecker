using WebApplicationChild.Models;

namespace WebApplicationChild
{
    public class MyTestData
    {
        public string Surname { get; set; }
        public Town Town { get; set; } = new Town { Name = "Test", Time = DateTime.Now };
        public string City { get; set; } = "Test";
    }
}