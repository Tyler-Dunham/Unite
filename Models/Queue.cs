using System;
using System.Collections.Generic;
using System.Text;

namespace Discord_Bot_Tutorial.Models
{
    public class Queue : Entity
    {
        public ulong userID { get; set; }
        public string userName { get; set; }
        public string role { get; set; }
        public int queueSr { get; set; }
    }
}
