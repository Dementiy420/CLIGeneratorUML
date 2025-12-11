using System;
using System.Collections.Generic;
using System.Text;

namespace GeneratorUML.Database.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DateTime { get; set; }
    }
}
