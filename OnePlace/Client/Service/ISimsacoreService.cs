using OnePlace.Shared.Entidades.SimsaCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnePlace.Client.Service
{
    public interface ISimsacoreService
    {
        Task<ResultObjectZone> GetAllZonas();
        Task<ResultObjectCompany> GetAllRazonesSociales();
        Task<ResultObjectBrand> GetAllMarcas();
        Task<ResultObjectDepartments> GetAllDepartamentos();
        Task<ResultObjectAreaDos> GetAllAreas();
        Task<ResultObjectPosition> GetAllPuestos();
        Task<ResultObjectPerson> GetAllPersonas();
        Task<ResultObjectEmployee> GetAllEmpleados();
        Task<ResultObjectStation> GetAllEstaciones();
    }
}
