using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Text;
using Nfield.Extensions;
using Nfield.Models;
using Nfield.Services;

namespace dodaj
{
    public class NfieldFieldworkOfficesManagement
    {
        private readonly INfieldFieldworkOfficesService _foService;

        /// <summary>
        /// Constructor.
        /// </summary>
        public NfieldFieldworkOfficesManagement(INfieldFieldworkOfficesService foService)
        {
            _foService = foService;
        }

        /// <summary>
        /// Performs query operation for available <see cref="FieldworkOffices"/>s asynchronously. 
        /// Note that this sample does not return the result, although your real class will do so.
        /// </summary>
        public IEnumerable<FieldworkOffice> QueryForOfficesAsync()
        {
            // execute async call
            Task<IQueryable<FieldworkOffice>> task = _foService.QueryAsync();

            // get async call result
            IEnumerable<FieldworkOffice> allOffices = task.Result.ToList();
            return allOffices;
        }
    }
}
