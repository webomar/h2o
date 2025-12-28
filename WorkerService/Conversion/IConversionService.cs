using System;
using System.Collections.Generic;
using System.Text;
using WorkerService.Domain;

namespace WorkerService.Conversion
{
    public interface IConversionService
    {
        /// <summary>
        /// Converteert alle leverancier-data naar het uniforme domeinmodel
        /// </summary>
        IEnumerable<Employee> Convert();
    }
}
