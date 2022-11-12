namespace TestChecker.Core.Models
{
    public class MethodName
    {
        public string AssemblyName { get; set; }
        public string Method { get; set; }
        public string Description { get; set; }
        public string FullName => $"{AssemblyName}.{Method}";

        public override string ToString() => string.IsNullOrWhiteSpace(Description) ? FullName : $"{FullName} - {Description}";
    }
}
