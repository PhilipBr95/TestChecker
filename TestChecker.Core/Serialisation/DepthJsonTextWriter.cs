using Newtonsoft.Json;
using System.IO;

namespace TestChecker.Core.Serialisation
{
    public class DepthJsonTextWriter : JsonTextWriter
    {
        public DepthJsonTextWriter(TextWriter textWriter) : base(textWriter) { }
        public int CurrentDepth { get; private set; }

        public override void WriteStartObject()
        {
            CurrentDepth++;
            base.WriteStartObject();
        }

        public override void WriteEndObject()
        {
            CurrentDepth--;
            base.WriteEndObject();
        }
    }
}
