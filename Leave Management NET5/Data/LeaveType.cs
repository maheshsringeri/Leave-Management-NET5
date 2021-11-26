﻿using System;
using System.ComponentModel.DataAnnotations;

namespace Leave_Management_NET5.Data
{
    public class LeaveType
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public DateTime DateCreated { get; set; }

    }
}
