using System;
using System.Collections.Generic;
using System.Text;

namespace GeneratorUML.Database.Models
{
    public class Accounts
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public int PostForeignKey { get; set; }
        public List<Post> Posts { get; set; }
    }
}