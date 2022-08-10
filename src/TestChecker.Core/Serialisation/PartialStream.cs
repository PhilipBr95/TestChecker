using System;
using System.IO;

namespace TestChecker.Core.Serialisation
{    
    internal class PartialStream
    {
        public int Length { get; }
        public string Content { get; }
        public string PartialContent { get; }
            
        public PartialStream(Stream obj)
        {
            if (obj == null)
                throw new Exception($"{nameof(PartialStream)} Failed");

            var data = new StreamReader(obj).ReadToEnd();

            Length = data.Length;

            if (data.Length < 254)
            {
                Content = data;
            }
            else
            {
                PartialContent = $"{data.Substring(0, 254).TrimEnd()}...";
            }
        }
    }
}