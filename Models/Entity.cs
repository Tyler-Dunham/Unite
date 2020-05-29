using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Discord_Bot_Tutorial.Models
{
    public abstract class Entity
    {
        [Key] public int Id { get; set; }
    }
}
