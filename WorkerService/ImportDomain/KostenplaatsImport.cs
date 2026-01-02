using System;
using System.Collections.Generic;
using System.Text;

namespace WorkerService.ImportDomain
{
    public class KostenplaatsImport
    {
        public string Code { get; set; } = null!;
        public string? Omschrijving { get; set; }
        public string OrganisatorischeEenheidCode { get; set; } = null!;
    }
}
