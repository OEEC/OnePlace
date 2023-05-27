using Newtonsoft.Json;
using OnePlace.Shared.Entidades.SimsaCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace OnePlace.Client.Service
{
    public class SimsaCoreService : ISimsacoreService
    {
        private readonly HttpClient httpClient;
        public SimsaCoreService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<ResultObjectStation> GetAllEstaciones()
        {
            return JsonConvert.DeserializeObject<ResultObjectStation>(
             await httpClient.GetStringAsync("/?query=estaciones"));
        }

        public async Task<ResultObjectEmployee> GetAllEmpleados()
        {
            return JsonConvert.DeserializeObject<ResultObjectEmployee>(
             await httpClient.GetStringAsync("/?query=empleados"));
        }

        public async Task<ResultObjectPerson> GetAllPersonas()
        {
            return JsonConvert.DeserializeObject<ResultObjectPerson>(
             await httpClient.GetStringAsync("/?query=personas"));
        }

        public async Task<ResultObjectPosition> GetAllPuestos()
        {
            return JsonConvert.DeserializeObject<ResultObjectPosition>(
             await httpClient.GetStringAsync("/?query=puestos"));
        }

        public async Task<ResultObjectAreaDos> GetAllAreas()
        {
            return JsonConvert.DeserializeObject<ResultObjectAreaDos>(
             await httpClient.GetStringAsync("/?query=areas"));
        }

        public async Task<ResultObjectDepartments> GetAllDepartamentos()
        {
            return JsonConvert.DeserializeObject<ResultObjectDepartments>(
             await httpClient.GetStringAsync("/?query=departamentos"));
        }
        public async Task<ResultObjectBrand> GetAllMarcas()
        {
            return JsonConvert.DeserializeObject<ResultObjectBrand>(
             await httpClient.GetStringAsync("/?query=marcas"));
        }
        public async Task<ResultObjectCompany> GetAllRazonesSociales()
        {            
            return JsonConvert.DeserializeObject<ResultObjectCompany>(
             await httpClient.GetStringAsync("/?query=razones_sociales"));
        }
        public async Task<ResultObjectZone> GetAllZonas()
        {
            //convertir (deserializar) los datos del json a el objeto zona
            return JsonConvert.DeserializeObject<ResultObjectZone>(
             await httpClient.GetStringAsync("/?query=zonas"));
        }       
    }
}
