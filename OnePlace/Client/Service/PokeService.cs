using Newtonsoft.Json;
using OnePlace.Shared;
using OnePlace.Shared.Entidades.SimsaCore;
using OnePlace.Shared.PokeModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace OnePlace.Client.Service
{
    public class PokeService : IPokeService
    {
        private readonly HttpClient httpClient;
        public PokeService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }
        public async Task<ResultObject> GetAllPokemons(PaginationParameters parameters)
        {
            return JsonConvert.DeserializeObject<ResultObject>(
                  await httpClient.GetStringAsync($"pokemon?limit={parameters.PageSize}&offset={parameters.Offset}"));
        }
        public async Task<Pokemon> GetPokemon(string name)
        {
            //convertir (deserializar) los datos del json a el objeto pokemon
            var result = JsonConvert.DeserializeObject<Pokemon>(await httpClient.GetStringAsync($"pokemon/{name}"));
            return result;
        }
    }
}
