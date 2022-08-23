﻿using OnePlace.Shared.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnePlace.Shared.DTOs
{
    public class TemaFaseDTO
    {
        //Contiene todas las fases existentes
        public List<FaseCurso> ListadodeFases { get; set; }

        //se necesita ya que de aqui se optiene el valor bool iscomplete
        public List<TemaFase> ListaTemaFase { get; set; }        
    }
}