using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnePlace.Client.ComponentesGenericos.Interface
{
    public interface IFormComponent
    {
        public bool Required { get; set; }
        public bool Error { get; set; }
        public bool HasErrors { get; }
        public bool Touched { get; }
        public List<string> ValidationErrors { get; set; }
        public Task Validate();
        public void Reset();
        public void ResetValidation();
    }
}
