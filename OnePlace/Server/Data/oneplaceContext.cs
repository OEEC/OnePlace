using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using OnePlace.Shared.Entidades;
using OnePlace.Shared.Entidades.SimsaCore;

namespace OnePlace.Server.Data
{
    public partial class oneplaceContext : IdentityDbContext<ApplicationUser>//extendemos la clase identity al applicationuser
    {
        //esto lo genera databasefirst 
        //public oneplaceContext()
        //{
        //}
        public oneplaceContext(DbContextOptions<oneplaceContext> options)
            : base(options)
        {

        }

        //sobreescribimos el metodo SaveChangesAsync, para que acepte el id del usuario y mas modificaciones custom
        public virtual async Task<int> SaveChangesAsync(string userId = null)
        {
            OnBeforeSaveChanges(userId);//le pasamos el nuevo metodo creado
            var result = await base.SaveChangesAsync();
            return result;
        }
        private void OnBeforeSaveChanges(string userId)
        {
            ChangeTracker.DetectChanges();//analiza las entidades en busca de cambios. el changetracker es un seguimiento de cambios y el metodo los detecta

            var auditEntries = new List<AuditEntry>();

            //recorre la colección de todas las Entidades modificadas,en nuestro caso, el ciclo siempre tendrá UNA sola iteración
            foreach (var item in ChangeTracker.Entries())
            {
                if (item.Entity is Auditoria || item.State == EntityState.Detached || item.State == EntityState.Unchanged)
                    continue;

                var auditEntry = new AuditEntry(item);

                auditEntry.TableName = item.Entity.GetType().Name;//obtiene el nombre de la tabla
                auditEntry.UserId = userId;//obtiene el id del usuario
                auditEntries.Add(auditEntry);//guarda la propiedades de cada entidad en la lista de entidades

                foreach (var property in item.Properties)
                {
                    string propertyName = property.Metadata.Name;

                    //si la propiedad actual es una clave principal, agréguela al diccionario de claves principales y sáltela.
                    if (property.Metadata.IsPrimaryKey())
                    {
                        auditEntry.KeyValues[propertyName] = property.CurrentValue;
                        continue;
                    }

                    //en el switch detectamos el estado de la entidad (Agregado, Eliminado o Modificado)
                    //y por cada caso agregamos nuevos datos a cada campo de la tabla auditoria
                    switch (item.State)
                    {
                        case EntityState.Added:
                            auditEntry.AuditType = Shared.Enums.AuditType.Create;
                            auditEntry.NewValues[propertyName] = property.CurrentValue;
                            break;
                        case EntityState.Deleted:
                            auditEntry.AuditType = Shared.Enums.AuditType.Delete;
                            auditEntry.OldValues[propertyName] = property.OriginalValue;
                            break;
                        case EntityState.Modified:
                            if (property.IsModified)
                            {
                                auditEntry.ChangedColumns.Add(propertyName);
                                auditEntry.AuditType = Shared.Enums.AuditType.Update;
                                auditEntry.OldValues[propertyName] = property.OriginalValue;
                                auditEntry.NewValues[propertyName] = property.CurrentValue;
                            }
                            break;
                    }
                }
            }
            //convertimos todas las Entradas de Auditoría a Auditorías y guardamos los cambios en el metodo original: var result = await base.SaveChangesAsync();
            foreach (var auditEntry in auditEntries)
            {
                Auditorias.Add(auditEntry.ToAudit());
            }
        }

        //esto lo genera database first pero lo pasamos al startup
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    if (!optionsBuilder.IsConfigured)
        //    {
        //        optionsBuilder.UseMySql("server=10.0.0.34;port=3306;uid=Innovacion;password=Grup0S1msa2022*;database=oneplace", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.30-mysql"));
        //    }
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region DataBaseApiFluet

            modelBuilder.HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_0900_ai_ci");

            modelBuilder.Entity<Area>(entity =>
            {
                entity.HasKey(e => e.Idarea)
                    .HasName("PRIMARY");

                entity.ToTable("area");

                entity.Property(e => e.Idarea).HasColumnName("idarea");

                entity.Property(e => e.Area1)
                    .HasMaxLength(200)
                    .HasColumnName("area");

                entity.Property(e => e.Fchbaja)
                    .HasColumnType("datetime")
                    .HasColumnName("fchbaja");

                entity.Property(e => e.Fchcreacion)
                    .HasColumnType("datetime")
                    .HasColumnName("fchcreacion");

                entity.Property(e => e.Fchmod)
                    .HasColumnType("datetime")
                    .HasColumnName("fchmod");

                entity.Property(e => e.Iddepartamento).HasColumnName("iddepartamento");

                entity.Property(e => e.Idusuario).HasColumnName("idusuario");
            });

            modelBuilder.Entity<Departamento>(entity =>
            {
                entity.HasKey(e => e.Iddepartamento)
                    .HasName("PRIMARY");

                entity.ToTable("departamento");

                entity.Property(e => e.Iddepartamento).HasColumnName("iddepartamento");

                entity.Property(e => e.Departamento1)
                    .HasMaxLength(100)
                    .HasColumnName("departamento");

                entity.Property(e => e.Fchbaja)
                    .HasColumnType("datetime")
                    .HasColumnName("fchbaja");

                entity.Property(e => e.Fchcreacion)
                    .HasColumnType("datetime")
                    .HasColumnName("fchcreacion");

                entity.Property(e => e.Fchmod)
                    .HasColumnType("datetime")
                    .HasColumnName("fchmod");

                entity.Property(e => e.Idempresa).HasColumnName("idempresa");

                entity.Property(e => e.Idusuario).HasColumnName("idusuario");
            });

            modelBuilder.Entity<Empleado>(entity =>
            {
                entity.HasKey(e => e.Idempleado)
                    .HasName("PRIMARY");

                entity.ToTable("empleado");

                entity.Property(e => e.Idempleado).HasColumnName("idempleado");

                entity.Property(e => e.Correo)
                    .HasMaxLength(100)
                    .HasColumnName("correo");

                entity.Property(e => e.Fchalta)
                    .HasColumnType("datetime")
                    .HasColumnName("fchalta");

                entity.Property(e => e.Fchbaja)
                    .HasColumnType("datetime")
                    .HasColumnName("fchbaja");

                entity.Property(e => e.Idarea).HasColumnName("idarea");

                entity.Property(e => e.Iddepartamento).HasColumnName("iddepartamento");

                entity.Property(e => e.Idestatus)
                    .HasMaxLength(45)
                    .HasColumnName("idestatus");

                entity.Property(e => e.Idpersona).HasColumnName("idpersona");

                entity.Property(e => e.Idpuesto).HasColumnName("idpuesto");

                entity.Property(e => e.Idtipo).HasColumnName("idtipo");

                entity.Property(e => e.Img)
                    .HasMaxLength(1000)
                    .HasColumnName("img");

                entity.Property(e => e.Noemp)
                    .HasMaxLength(50)
                    .HasColumnName("noemp");

                entity.Property(e => e.Nomina).HasColumnName("nomina");

                entity.Property(e => e.Telefono)
                    .HasMaxLength(10)
                    .HasColumnName("telefono");

                entity.Property(e => e.Variable).HasColumnName("variable");
            });

            modelBuilder.Entity<Empresa>(entity =>
            {
                entity.HasKey(e => e.Idempresa)
                    .HasName("PRIMARY");

                entity.ToTable("empresa");

                entity.Property(e => e.Idempresa).HasColumnName("idempresa");

                entity.Property(e => e.Domicilio)
                    .HasMaxLength(500)
                    .HasColumnName("domicilio");

                entity.Property(e => e.Fchbaja)
                    .HasColumnType("datetime")
                    .HasColumnName("fchbaja");

                entity.Property(e => e.Fchcreacion)
                    .HasColumnType("datetime")
                    .HasColumnName("fchcreacion");

                entity.Property(e => e.Fchmod)
                    .HasColumnType("datetime")
                    .HasColumnName("fchmod");

                entity.Property(e => e.Idestatus).HasColumnName("idestatus");

                entity.Property(e => e.Idusuario).HasColumnName("idusuario");

                entity.Property(e => e.Patronal)
                    .HasMaxLength(15)
                    .HasColumnName("patronal");

                entity.Property(e => e.Razonsocial)
                    .HasMaxLength(200)
                    .HasColumnName("razonsocial");

                entity.Property(e => e.Rfc)
                    .HasMaxLength(15)
                    .HasColumnName("rfc");
            });

            modelBuilder.Entity<Estacion>(entity =>
            {
                entity.HasKey(e => e.Idestacion)
                    .HasName("PRIMARY");

                entity.ToTable("estacion");

                entity.Property(e => e.Idestacion).HasColumnName("idestacion");

                entity.Property(e => e.Calle)
                    .HasColumnType("text")
                    .HasColumnName("calle");

                entity.Property(e => e.Codgas).HasColumnName("codgas");

                entity.Property(e => e.Colonia)
                    .HasColumnType("text")
                    .HasColumnName("colonia");

                entity.Property(e => e.Correo)
                    .HasColumnType("text")
                    .HasColumnName("correo");

                entity.Property(e => e.Cp)
                    .HasColumnType("text")
                    .HasColumnName("cp");

                entity.Property(e => e.Estado)
                    .HasColumnType("text")
                    .HasColumnName("estado");

                entity.Property(e => e.Estatus).HasColumnName("estatus");

                entity.Property(e => e.Fchcreacion)
                    .HasColumnType("text")
                    .HasColumnName("fchcreacion");

                entity.Property(e => e.Fchmodificacion)
                    .HasColumnType("text")
                    .HasColumnName("fchmodificacion");

                entity.Property(e => e.Idrazonsocial).HasColumnName("idrazonsocial");

                entity.Property(e => e.Idusuario).HasColumnName("idusuario");

                entity.Property(e => e.Img)
                    .HasColumnType("text")
                    .HasColumnName("img");

                entity.Property(e => e.Latitud).HasColumnName("latitud");

                entity.Property(e => e.Longitud).HasColumnName("longitud");

                entity.Property(e => e.Marca).HasColumnName("marca");

                entity.Property(e => e.Municipio)
                    .HasColumnType("text")
                    .HasColumnName("municipio");

                entity.Property(e => e.Noest)
                    .HasColumnType("text")
                    .HasColumnName("noest");

                entity.Property(e => e.Noext)
                    .HasColumnType("text")
                    .HasColumnName("noext");

                entity.Property(e => e.Noint)
                    .HasColumnType("text")
                    .HasColumnName("noint");

                entity.Property(e => e.Nombre)
                    .HasColumnType("text")
                    .HasColumnName("nombre");

                entity.Property(e => e.Nomcg)
                    .HasColumnType("text")
                    .HasColumnName("nomcg");

                entity.Property(e => e.Permisocre)
                    .HasColumnType("text")
                    .HasColumnName("permisocre");

                entity.Property(e => e.Qr)
                    .HasColumnType("text")
                    .HasColumnName("qr");

                entity.Property(e => e.Razonsocial).HasColumnName("razonsocial");

                entity.Property(e => e.Tel).HasColumnName("tel");

                entity.Property(e => e.Url)
                    .HasColumnType("text")
                    .HasColumnName("url");

                entity.Property(e => e.Zona).HasColumnName("zona");
            });

            modelBuilder.Entity<Estatus>(entity =>
            {
                entity.HasKey(e => e.Idestatus)
                    .HasName("PRIMARY");

                entity.ToTable("estatus");

                entity.Property(e => e.Idestatus).HasColumnName("idestatus");

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(100)
                    .HasColumnName("descripcion");

                entity.Property(e => e.Estatus1)
                    .HasMaxLength(45)
                    .HasColumnName("estatus");
            });

            modelBuilder.Entity<Marca>(entity =>
            {
                entity.HasKey(e => e.Idmarca)
                    .HasName("PRIMARY");

                entity.ToTable("marca");

                entity.Property(e => e.Idmarca).HasColumnName("idmarca");

                entity.Property(e => e.Fchbaja)
                    .HasColumnType("datetime")
                    .HasColumnName("fchbaja");

                entity.Property(e => e.Fchcreacion)
                    .HasColumnType("datetime")
                    .HasColumnName("fchcreacion");

                entity.Property(e => e.Fchmod)
                    .HasColumnType("datetime")
                    .HasColumnName("fchmod");

                entity.Property(e => e.Idempresa).HasColumnName("idempresa");

                entity.Property(e => e.Idestatus).HasColumnName("idestatus");

                entity.Property(e => e.Idtipo).HasColumnName("idtipo");

                entity.Property(e => e.Idusuario).HasColumnName("idusuario");

                entity.Property(e => e.Img)
                    .HasMaxLength(500)
                    .HasColumnName("img");

                entity.Property(e => e.Marca1)
                    .HasMaxLength(100)
                    .HasColumnName("marca");
            });

            modelBuilder.Entity<Pagadora>(entity =>
            {
                entity.HasKey(e => e.Idpagadora)
                    .HasName("PRIMARY");

                entity.ToTable("pagadora");

                entity.Property(e => e.Idpagadora).HasColumnName("idpagadora");

                entity.Property(e => e.Fchbaja)
                    .HasColumnType("datetime")
                    .HasColumnName("fchbaja");

                entity.Property(e => e.Fchcreacion)
                    .HasColumnType("datetime")
                    .HasColumnName("fchcreacion");

                entity.Property(e => e.Fchmod)
                    .HasColumnType("datetime")
                    .HasColumnName("fchmod");

                entity.Property(e => e.Idestatus).HasColumnName("idestatus");

                entity.Property(e => e.Idusuario).HasColumnName("idusuario");

                entity.Property(e => e.Pagadora1)
                    .HasMaxLength(100)
                    .HasColumnName("pagadora");
            });

            modelBuilder.Entity<Persona>(entity =>
            {
                entity.HasKey(e => e.Idpersona)
                    .HasName("PRIMARY");

                entity.ToTable("persona");

                entity.Property(e => e.Idpersona).HasColumnName("idpersona");

                entity.Property(e => e.ApeMat)
                    .HasMaxLength(100)
                    .HasColumnName("ape_mat");

                entity.Property(e => e.ApePat)
                    .HasMaxLength(100)
                    .HasColumnName("ape_pat");

                entity.Property(e => e.Calle)
                    .HasMaxLength(100)
                    .HasColumnName("calle");

                entity.Property(e => e.Ciudad)
                    .HasMaxLength(100)
                    .HasColumnName("ciudad");

                entity.Property(e => e.Colonia)
                    .HasMaxLength(100)
                    .HasColumnName("colonia");

                entity.Property(e => e.Correo)
                    .HasMaxLength(100)
                    .HasColumnName("correo");

                entity.Property(e => e.Cp)
                    .HasMaxLength(5)
                    .HasColumnName("cp");

                entity.Property(e => e.Curp)
                    .HasMaxLength(45)
                    .HasColumnName("curp");

                entity.Property(e => e.Estado)
                    .HasMaxLength(100)
                    .HasColumnName("estado");

                entity.Property(e => e.Fchnac)
                    .HasColumnType("date")
                    .HasColumnName("fchnac");

                entity.Property(e => e.Noext)
                    .HasMaxLength(10)
                    .HasColumnName("noext");

                entity.Property(e => e.Noint)
                    .HasMaxLength(10)
                    .HasColumnName("noint");

                entity.Property(e => e.Nombre)
                    .HasMaxLength(100)
                    .HasColumnName("nombre");

                entity.Property(e => e.Nss)
                    .HasMaxLength(45)
                    .HasColumnName("nss");

                entity.Property(e => e.Rfc)
                    .HasMaxLength(45)
                    .HasColumnName("rfc");

                entity.Property(e => e.Sexo)
                    .HasMaxLength(1)
                    .HasColumnName("sexo")
                    .IsFixedLength(true);

                entity.Property(e => e.Telefono)
                    .HasMaxLength(10)
                    .HasColumnName("telefono");
            });

            modelBuilder.Entity<Protocolo>(entity =>
            {
                entity.HasKey(e => e.Idprotocolo)
                    .HasName("PRIMARY");

                entity.ToTable("protocolo");

                entity.Property(e => e.Idprotocolo).HasColumnName("idprotocolo");

                entity.Property(e => e.Decalogo).HasColumnName("decalogo");

                entity.Property(e => e.Empleado)
                    .HasMaxLength(100)
                    .HasColumnName("empleado");

                entity.Property(e => e.Fchcal)
                    .HasColumnType("datetime")
                    .HasColumnName("fchcal");

                entity.Property(e => e.Idempleado).HasColumnName("idempleado");

                entity.Property(e => e.Idestacion).HasColumnName("idestacion");

                entity.Property(e => e.Imagen).HasColumnName("imagen");

                entity.Property(e => e.Importe).HasColumnName("importe");

                entity.Property(e => e.Limpieza).HasColumnName("limpieza");

                entity.Property(e => e.Promociones).HasColumnName("promociones");

                entity.Property(e => e.Puntualidad).HasColumnName("puntualidad");

                entity.Property(e => e.Total).HasColumnName("total");
            });

            modelBuilder.Entity<Puesto>(entity =>
            {
                entity.HasKey(e => e.Idpuesto)
                    .HasName("PRIMARY");

                entity.ToTable("puesto");

                entity.Property(e => e.Idpuesto).HasColumnName("idpuesto");

                entity.Property(e => e.Fchbaja)
                    .HasColumnType("datetime")
                    .HasColumnName("fchbaja");

                entity.Property(e => e.Fchcreacion)
                    .HasColumnType("datetime")
                    .HasColumnName("fchcreacion");

                entity.Property(e => e.Fchmod)
                    .HasColumnType("datetime")
                    .HasColumnName("fchmod");

                //entity.Property(e => e.Iddepartamento).HasColumnName("iddepartamento"); //esta propiedad ya no la tiene

                entity.Property(e => e.Idusuario).HasColumnName("idusuario");

                entity.Property(e => e.Puesto1)
                    .HasMaxLength(45)
                    .HasColumnName("puesto");
            });

            modelBuilder.Entity<Tipo>(entity =>
            {
                entity.HasKey(e => e.Idtipo)
                    .HasName("PRIMARY");

                entity.ToTable("tipo");

                entity.Property(e => e.Idtipo).HasColumnName("idtipo");

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(50)
                    .HasColumnName("descripcion");

                entity.Property(e => e.Tipo1)
                    .HasMaxLength(45)
                    .HasColumnName("tipo");
            });

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(e => e.Idusuario)
                    .HasName("PRIMARY");

                entity.ToTable("usuario");

                entity.Property(e => e.Idusuario).HasColumnName("idusuario");

                entity.Property(e => e.Contrasena)
                    .HasMaxLength(45)
                    .HasColumnName("contrasena");

                entity.Property(e => e.FkId).HasColumnName("fk_id");

                entity.Property(e => e.Idempleado).HasColumnName("idempleado");

                entity.Property(e => e.Idtipo).HasColumnName("idtipo");

                entity.Property(e => e.Usuario1)
                    .HasMaxLength(45)
                    .HasColumnName("usuario");
            });

            //se quito con esto ef piensa que hay una tabla llamada zona y ahora se llama zonas
            //modelBuilder.Entity<Zona>(entity =>
            //{
            //    entity.HasKey(e => e.Idzona)
            //        .HasName("PRIMARY");

            //    entity.ToTable("zona");

            //    entity.Property(e => e.Idzona).HasColumnName("idzona");

            //    entity.Property(e => e.Fchbaja)
            //        .HasColumnType("datetime")
            //        .HasColumnName("fchbaja");

            //    entity.Property(e => e.Fchcreacion)
            //        .HasColumnType("datetime")
            //        .HasColumnName("fchcreacion");

            //    entity.Property(e => e.Fchmod)
            //        .HasColumnType("datetime")
            //        .HasColumnName("fchmod");

            //    entity.Property(e => e.Idestatus).HasColumnName("idestatus");

            //    entity.Property(e => e.Idusuario).HasColumnName("idusuario");

            //    entity.Property(e => e.Zona1)
            //        .HasMaxLength(100)
            //        .HasColumnName("zona");
            //});

            #endregion

            //esto lo genera databasefirst
            //OnModelCreatingPartial(modelBuilder);

            //creamos el modelo entity, creamos un objeto anonimo con new y le pasamos el haskey para crear una llave compuesta con temaid y fasecursoid        
            modelBuilder.Entity<TemaFase>().HasKey(x => new { x.TemaId, x.FaseCursoId });
            modelBuilder.Entity<PromocionZona>().HasKey(x => new { x.PromocionId, x.ZonaId });
            modelBuilder.Entity<CapacitacionContinuaZona>().HasKey(x => new { x.CapacitacionContinuaId, x.ZonaId });

            var roleAdmin = new IdentityRole()
            { Id = "f2e8377a-a95f-4e57-ac21-392bb3240cb4", Name = "Administrador", NormalizedName = "Administrador" };
            modelBuilder.Entity<IdentityRole>().HasData(roleAdmin);

            var Usuario = new IdentityRole()
            { Id = "65f535d8-b7c6-4811-9326-491a80031f60", Name = "Usuario", NormalizedName = "Usuario" };
            modelBuilder.Entity<IdentityRole>().HasData(Usuario);

            base.OnModelCreating(modelBuilder);
        }

        //esto lo genera databasefirst 
        //partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

        #region DbSetSimsaCore
        public virtual DbSet<Area> Areas { get; set; }
        public virtual DbSet<Departamento> Departamentos { get; set; }
        public virtual DbSet<Empleado> Empleados { get; set; }
        public virtual DbSet<Empresa> Empresas { get; set; }
        public virtual DbSet<Estacion> Estaciones { get; set; }
        public virtual DbSet<Estatus> Estatuses { get; set; }
        public virtual DbSet<Marca> Marcas { get; set; }
        public virtual DbSet<Pagadora> Pagadoras { get; set; }
        public virtual DbSet<Persona> Personas { get; set; }
        public virtual DbSet<Protocolo> Protocolos { get; set; }
        public virtual DbSet<Puesto> Puestos { get; set; }
        public virtual DbSet<Tipo> Tipos { get; set; }
        public virtual DbSet<Usuario> Usuarios { get; set; }
        public virtual DbSet<Zona> Zonas { get; set; }
        public virtual DbSet<Tienda> Tienda { get; set; }

        #endregion

        #region DbSetOnePlace
        public DbSet<Auditoria> Auditorias { get; set; }
        //public DbSet<Logs> Logs { get; set; }
        public DbSet<Evento> Eventos { get; set; }
        public DbSet<Promocion> Promociones { get; set; }
        public DbSet<ImagenesCarrusel> ImagenesCarruseles { get; set; }
        public DbSet<ImagenesCarruselEvento> ImagenesCarruselEventos { get; set; }
        public DbSet<Curso> Cursos { get; set; }
        public DbSet<Tema> Temas { get; set; }
        public DbSet<ArchivoAdjunto> ArchivoAdjuntos { get; set; }
        public DbSet<FaseCurso> FaseCursos { get; set; }
        public DbSet<TemaFase> TemaFases { get; set; }
        public DbSet<ActividadUsuario> ActividadUsuarios { get; set; }
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<Pregunta> Preguntas { get; set; }
        public DbSet<Respuesta> Respuestas { get; set; }
        public DbSet<PalabrasClave> PalabrasClave { get; set; }
        public DbSet<EstadosdelQuiz> EstadosdelQuiz { get; set; }        
        public DbSet<ActividadUsuarioQuiz> ActividadUsuarioQuiz { get; set; }
        public DbSet<CursoEstado> CursoEstado { get; set; }
        public DbSet<PromocionZona> PromocionZonas { get; set; }
        public DbSet<EstadodeCumpleaños> EstadodeCumpleaños { get; set; }
        public DbSet<CapacitacionContinua> CapacitacionContinua { get; set; }
        public DbSet<VideosCapacitacion> VideosCapacitacion { get; set; }
        public DbSet<Notificacion> Notificaciones { get; set; }
        public DbSet<CapacitacionContinuaZona> CapacitacionContinuaZona { get; set; }
        public DbSet<ImagenesCumpleEmpleado> ImagenesCumpleEmpleado { get; set; }

        #endregion
    }
}
