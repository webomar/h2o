using System;
using System.Collections.Generic;
using System.Text;

namespace WorkerService.Configuration
{
    public class SupplierCsvOptions
    {
        public string FilePath { get; set; } = "Imports/youforce.csv";
        public string Delimiter { get; set; } = ";";
        public string Encoding { get; set; } = "utf-8";
        public bool HasHeader { get; set; } = true;
    }

}
