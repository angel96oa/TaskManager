using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskManager.API.Models
{
    public class TaskElement
    {
        public Guid id { get; set; } = Guid.NewGuid();
        public string TaskName { get; set; }
        public string TaskDescription { get; set; }
        public DateTime TaskCreateDate { get; set; } = DateTime.Now;
        public DateTime finishTaskDate { get; set; }
        public bool TaskCompleted { get; set; } = false;
    }
}