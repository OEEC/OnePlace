using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnePlace.Client.ComponentesGenericos.SelectorMultiple
{
    public struct SelectorMultipleModel
    {
        //creamos un constructor el cual va a recibir como parametro la llave y el valor
        public SelectorMultipleModel(string llave, string valor)
        {
            Llave = llave;
            Valor = valor;
        }
        //creamos dos propiedades
        public string Llave { get; set; }
        public string Valor { get; set; }
    }
}
