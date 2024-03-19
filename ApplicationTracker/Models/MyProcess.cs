﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationTracker.Models
{
    internal class MyProcess
    {
        public int Id { get; set; }
        public string ProcessName { get; set; }
        //public int ProcessId { get; set; }
        public TimeSpan ProcessTime { get; set; }
        public DateTime DateStarted { get; set; }
    }
}
