using System;
using System.Collections.Generic;
using System.Text;

namespace Discord_Bot_Tutorial.Models
{
    public class Ban : Entity
    {
        public ulong userID { get; set; }
        public string userName { get; set; }
        public DateTime banTime { get; set; }
        public DateTime unbanTime { get; set; }
        public string banReason{get; set;}
    }
}
