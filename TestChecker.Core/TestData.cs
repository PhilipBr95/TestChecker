using System;
using System.Collections.Generic;
using System.Linq;

namespace TestChecker.Core
{
    public class TestData 
    {        
        public int MemberId { get; set; }
        public string Refno => MemberId.ToString();
        public int EmployerId { get; set; }
    }
}