using System.Collections.Generic;

namespace DapperDemo
{
    public class Aardvark
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<string> Behaviours { get; set; }
    }
}