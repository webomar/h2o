using System;
using System.Collections.Generic;
using System.Text;

namespace WorkerService.Domain
{
    public class Employee
    {
        public string ExternalId { get; set; } = "";
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string Source { get; set; } = "";
    }
}
