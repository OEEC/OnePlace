using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnePlace.Client.ComponentesGenericos.Interface
{
    public interface IForm
    {
        public bool IsValid { get; }
        public string[] Errors { get; }
        internal void Add(IFormComponent formControl);
        internal void Remove(IFormComponent formControl);
        internal void Update(IFormComponent formControl);
    }
}
