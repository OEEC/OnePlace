using Microsoft.AspNetCore.Components;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnePlace.Client.Interfaces
{
    public interface IComponentInfoLoad : IComponent
    {
        public Task LoadInfo(Dictionary<string, string> dictionary);
        public Task<byte[]> ExportData(Dictionary<string, string> dictionary);
    }
}
