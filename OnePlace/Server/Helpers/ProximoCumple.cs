using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnePlace.Server.Helpers
{
    public class ProximoCumple
    {
        public static DateTime ProximoCumpleaños(int diaCumple, int mesCumple, int anioCumple)
        {
            //int diaCumple = 4;//Dia del Cumpleanios
            //int mesCumple = 4;//Mes de Cumple 4=Abril
            //int anioCumple = 1984; //Anio de Cumple
            //DateTime fechaNacimiento = new DateTime(anioCumple, mesCumple, diaCumple);

            ////Se calcula la Edad Actual A partir de la fecha actual Sustrayendo la fecha de nacimiento
            ////esto devuelve un TimeSpan por tanto tomaremos los Dias y lo dividimos en 365 días
            //int edad = (DateTime.Now.Subtract(fechaNacimiento).Days / 365);

            DateTime proximoCumple;
            //Define el proximo Cumple,
            //En caso de que el mes de cumple, sea menor al Mes Actual, se busca el Próxima fecha que seria del año que viene
            //es por ello el AddYear(1)
            //En caso de ser mayor se toma el año actual
            if (mesCumple < DateTime.Now.Month)
            {
                proximoCumple = new DateTime(DateTime.Now.AddYears(1).Year, mesCumple, diaCumple);
            }
            else
            {
                proximoCumple = new DateTime(DateTime.Now.Year, mesCumple, diaCumple);
            }

            //Definiremos los dias faltantes para el proximo cumple
            TimeSpan faltan = proximoCumple.Subtract(DateTime.Today);

            //los dias salian en negativo por eso los iba a transformar en positivo
            //var negativoApositivo = Math.Abs(faltan.Days);

            var fechaARetornar = DateTime.Today.AddDays(faltan.Days);

            //Ahora Elaboramos el Mensaje
            //StringBuilder sb = new StringBuilder();
            //sb.AppendFormat("Usted Tiene {0} Años ", edad);
            //sb.AppendFormat("y tu Próximo Cumpleaños es: {0} Días", faltan.Days);
            //sb.AppendFormat(", {0} Horas ", faltan.Hours);
            //sb.AppendFormat("y {0} Minutos ", faltan.Minutes);           

            //retornamos la fecha en formato string sin horas,minutos para no tener problemas con ello, mientras sea el dia correcto
            return fechaARetornar;
        }
        public static DateTime ProximoCumpleTodoMes(int diaCumple, int mesCumple, int anioCumple)
        {
            DateTime proximoCumple;
            //Define el proximo Cumple,
            //En caso de que el mes de cumple, sea menor al Mes Actual, se busca la Próxima fecha que seria del año que viene
            //es por ello el AddYear(1)
            //En caso de ser mayor se toma el año actual
            if (mesCumple < DateTime.Now.Month)
            {
                proximoCumple = new DateTime(DateTime.Now.AddYears(1).Year, mesCumple, diaCumple);
            }
            else
            {
                proximoCumple = new DateTime(DateTime.Now.Year, mesCumple, diaCumple);
            }

            //Definiremos los dias faltantes para el proximo cumple
            TimeSpan faltan = proximoCumple.Subtract(DateTime.Today);

            var fechaARetornar = DateTime.Today.AddDays(faltan.Days);

            return fechaARetornar;
        }
    }
}
