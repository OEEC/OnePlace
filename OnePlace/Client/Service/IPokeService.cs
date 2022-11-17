using OnePlace.Shared;
using OnePlace.Shared.Entidades.SimsaCore;
using OnePlace.Shared.PokeModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnePlace.Client.Service
{
    public interface IPokeService
    {
        Task<ResultObject> GetAllPokemons(PaginationParameters parameters);
        Task<Pokemon> GetPokemon(string name);
    }
}
