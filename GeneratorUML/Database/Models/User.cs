using System;
using System.Collections.Generic;
using System.Text;

namespace GeneratorUML.Database.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int AccountForeignKey { get; set; }
        public Accounts Accounts { get; set; }
    }
}