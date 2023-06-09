﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OnePlace.Client.Service;
using OnePlace.Server.Data;
using OnePlace.Shared.Entidades.SimsaCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnePlace.Server.Services
{
    public interface IApiaBdService
    {
        Task DatosdeApiABaseDatos();
    }
    public class ApiaBdService : IApiaBdService
    {
        private readonly oneplaceContext context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ILogger<ApiaBdService> logger;
        private readonly ISimsacoreService simsacoreService;
        public ApiaBdService(oneplaceContext context, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor, ILogger<ApiaBdService> logger, ISimsacoreService simsacoreService)
        {
            this.context = context;
            this.logger = logger;
            _userManager = userManager;
            this.httpContextAccessor = httpContextAccessor;
            this.simsacoreService = simsacoreService;
        }
        public async Task DatosdeApiABaseDatos()
        {
            logger.LogInformation("Se ejecuto un job para obtener datos de un api y guardarlos en la bd");           

            var resultadozonas = await simsacoreService.GetAllZonas();

            List<Zona> listadezonas = new List<Zona>();

            foreach (var item in resultadozonas.Zonas)
            {
                Zona zona = new Zona();
                zona.ZonaId = item.ZonaId;
                zona.Zona1 = item.Zona1;
                zona.Idestatus = item.Idestatus;
                zona.Fchcreacion = item.Fchcreacion;
                zona.Fchmod = item.Fchmod;
                zona.Fchbaja = item.Fchbaja;
                zona.Idusuario = item.Idusuario;

                var existe = await context.Zonas.AnyAsync(x => x.ZonaId == item.ZonaId);
                if (!existe)
                {
                    listadezonas.Add(zona);                    
                }
            }

            context.Zonas.AddRange(listadezonas);
            await context.SaveChangesAsync();

            var resultadoempresas = await simsacoreService.GetAllRazonesSociales();

            List<Empresa> listadeempresas = new List<Empresa>();

            foreach (var item in resultadoempresas.ListadeRazonesSociales)
            {
                Empresa empresa = new Empresa();
                empresa.Idempresa = item.Idempresa;
                empresa.Rfc = item.Rfc;
                empresa.Patronal = item.Patronal;
                empresa.Razonsocial = item.Domicilio;
                empresa.Idestatus = item.Idestatus;
                empresa.Fchcreacion = item.Fchcreacion;
                empresa.Fchmod = item.Fchmod;
                empresa.Fchbaja = item.Fchbaja;
                empresa.Idusuario = item.Idusuario;

                var existe = await context.Empresas.AnyAsync(x => x.Idempresa == item.Idempresa);
                if (!existe)
                {
                    listadeempresas.Add(empresa);
                }
            }

            context.Empresas.AddRange(listadeempresas);
            await context.SaveChangesAsync();

            var resultadomarcas = await simsacoreService.GetAllMarcas();

            List<Marca> listademarcas = new List<Marca>();

            foreach (var item in resultadomarcas.ListadeMrcas)
            {
                Marca marca = new Marca();
                marca.Idmarca = item.Idmarca;
                marca.Idempresa = item.Idempresa;
                marca.Img = item.Img;
                marca.Marca1 = item.Marca1;          
                marca.Idtipo = item.Idtipo;
                marca.Idestatus = item.Idestatus;
                marca.Fchcreacion = item.Fchcreacion;
                marca.Fchmod = item.Fchmod;
                marca.Fchbaja = item.Fchbaja;
                marca.Idusuario = item.Idusuario;

                var existe = await context.Marcas.AnyAsync(x => x.Idmarca == item.Idmarca);
                if (!existe)
                {
                    listademarcas.Add(marca);
                }
            }

            context.Marcas.AddRange(listademarcas);
            await context.SaveChangesAsync();

            var resultadodepartamentos = await simsacoreService.GetAllDepartamentos();

            List<Departamento> listadedepartamentos = new List<Departamento>();

            foreach (var item in resultadodepartamentos.ListadeDepartamentos)
            {
                Departamento depto = new Departamento();
                depto.Iddepartamento = item.Iddepartamento;
                depto.Idempresa = item.Idempresa;
                depto.Departamento1 = item.Departamento1;
                depto.Fchcreacion = item.Fchcreacion;
                depto.Fchmod = item.Fchmod;
                depto.Fchbaja = item.Fchbaja;
                depto.Idusuario = item.Idusuario;
                //depto.Idestatus = item.Idestatus;

                var existe = await context.Departamentos.AnyAsync(x => x.Iddepartamento == item.Iddepartamento);
                if (!existe)
                {
                    listadedepartamentos.Add(depto);
                }
            }

            context.Departamentos.AddRange(listadedepartamentos);
            await context.SaveChangesAsync();

            var resultadoareas = await simsacoreService.GetAllAreas();

            List<Area> listadeareas = new List<Area>();

            foreach (var item in resultadoareas.ListadeAreas)
            {
                Area area = new Area();
                area.Idarea = item.Idarea;
                area.Iddepartamento = item.Iddepartamento;
                area.Area1 = item.Area1;

                #region ConvertirStringenDatetime
                ////verificar si la fecha no viene null
                //if (!String.IsNullOrEmpty(item.Fchcreacion))
                //{          
                //    //reemplazar el guin por un slash
                //    var fechacreacion = item.Fchcreacion.Replace(@"-", "/");
                //    //convertir string en fecha
                //    area.Fchcreacion = DateTime.ParseExact(fechacreacion, "yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture);                   
                //}
                //if(!String.IsNullOrEmpty(item.Fchmod))
                //{
                //    var fechamod = item.Fchmod.Replace(@"-", "/");
                //    area.Fchmod = DateTime.ParseExact(fechamod, "yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture);                    
                //}
                //if(!String.IsNullOrEmpty(item.Fchbaja))
                //{
                //    var fechabaja = item.Fchbaja.Replace(@"-", "/");
                //   area.Fchbaja = DateTime.ParseExact(fechabaja, "yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture);                    
                //}
                #endregion

                area.Fchcreacion = item.Fchcreacion;
                area.Fchmod = item.Fchmod;
                area.Fchbaja = item.Fchbaja;

                area.Idusuario = item.Idusuario;

                var existe = await context.Areas.AnyAsync(x => x.Idarea == item.Idarea);
                if (!existe)
                {
                    listadeareas.Add(area);
                }
            }

            context.Areas.AddRange(listadeareas);
            await context.SaveChangesAsync();

            var resultadopuestos = await simsacoreService.GetAllPuestos();

            List<Puesto> listadepuestos = new List<Puesto>();

            foreach (var item in resultadopuestos.ListadePuestos)
            {
                Puesto puesto = new Puesto();
                puesto.Idpuesto = item.Idpuesto;
                puesto.Puesto1 = item.Puesto1;
                puesto.Fchcreacion = item.Fchcreacion;
                puesto.Fchmod = item.Fchmod;
                puesto.Fchbaja = item.Fchbaja;
                puesto.Idusuario = item.Idusuario;

                var existe = await context.Puestos.AnyAsync(x => x.Idpuesto == item.Idpuesto);
                if (!existe)
                {
                    listadepuestos.Add(puesto);
                }
            }

            context.Puestos.AddRange(listadepuestos);
            await context.SaveChangesAsync();       

            var resultadoestaciones = await simsacoreService.GetAllEstaciones();

            List<Estacion> listadeestaciones = new List<Estacion>();

            foreach (var item in resultadoestaciones.ListadeEstaciones)
            {
                Estacion estacion = new Estacion();
                estacion.Idestacion = item.Idestacion;
                estacion.Codgas = item.Codgas;
                estacion.Nomcg = item.Nomcg;
                estacion.Img = item.Img;
                estacion.Permisocre = item.Permisocre;
                estacion.Noest = item.Noest;

                estacion.Nombre = item.Nombre;
                estacion.Marca = item.Marca;
                estacion.Correo = item.Correo;
                estacion.Tel = item.Tel;
                estacion.Url = item.Url;
                estacion.Idestacion = item.Idestacion;

                estacion.Razonsocial = item.Razonsocial;
                estacion.Calle = item.Calle;
                estacion.Noint = item.Noint;
                estacion.Noext = item.Noext;
                estacion.Colonia = item.Colonia;
                estacion.Municipio = item.Municipio;

                estacion.Estado = item.Estado;
                estacion.Cp = item.Cp;
                estacion.Zona = item.Zona;
                estacion.Latitud = item.Latitud;

                estacion.Longitud = item.Longitud;
                estacion.Qr = item.Qr;
                estacion.Fchcreacion = item.Fchcreacion;
                estacion.Fchmodificacion = item.Fchmodificacion;

                estacion.Idusuario = item.Idusuario;
                estacion.Estatus = item.Estatus;
                estacion.Idrazonsocial = item.Idrazonsocial;

                var existe = await context.Estaciones.AnyAsync(x => x.Idestacion == item.Idestacion);
                if (!existe)
                {
                    listadeestaciones.Add(estacion);
                }
            }

            context.Estaciones.AddRange(listadeestaciones);
            await context.SaveChangesAsync();
        }
    }
}
