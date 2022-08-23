using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnePlace.Server.Data;
using OnePlace.Shared.DTOs;
using OnePlace.Shared.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnePlace.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrador,Usuario")]
    public class FaseCursoController : ControllerBase
    {
        private readonly oneplaceContext context;
        private readonly UserManager<ApplicationUser> _userManager;
        public FaseCursoController(oneplaceContext context, UserManager<ApplicationUser> userManager)
        {
            this.context = context;
            _userManager = userManager;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TemaFaseDTO>> Get(int id)
        {
            //buscamos un listado de temasfases que coincida con el id del tema enviado
            var listadodetemasyfases = await context.TemaFases.Where(x => x.TemaId == id).ToListAsync();

            List<FaseCurso> listadefases = new List<FaseCurso>();          
       
            //recorremos el listado de temasfases relacionados
            foreach(var item in listadodetemasyfases)
            {
                //creamos un listado de fases solo con las fases que pertenecen a un tema en especifico
                var fasecurso = await context.FaseCursos.Where(x => x.FaseCursoId == item.FaseCursoId).FirstOrDefaultAsync();
                listadefases.Add(fasecurso);      
            }

            var model = new TemaFaseDTO();
            model.ListadodeFases = listadefases;
            model.ListaTemaFase = listadodetemasyfases;            

            return model;
        }       

        //actualizar de false a true para decir que ya se completo una fase
        [Route("FaseCompleta")]
        [HttpPut]
        public async Task<ActionResult> PutFaseCompleta(TemaFase fase)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            //obtener el registro original usando el método FindAsync (para un key o llave compuesta se necesita pasar los dos id en el metodo find, con uno marca error)
            var oldfase = await context.TemaFases.FindAsync(fase.TemaId,fase.FaseCursoId);

            //las propiedades sin cambios se ignoran y solo los valores de cambios se incluyen en la consulta de actualización
            context.Entry(oldfase).CurrentValues.SetValues(fase);

            await context.SaveChangesAsync(user.Id);
            return NoContent();
        }
    }
}
