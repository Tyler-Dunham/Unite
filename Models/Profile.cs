using System;
using System.Collections.Generic;
using System.Text;

namespace Discord_Bot_Tutorial.Models
{
    public class Profile : Entity
    {
        public ulong userID { get; set; }
        public string userName { get; set; }
        public int dps { get; set; }
        public int tank { get; set; }
        public int support { get; set; }
    }
}
