using Hangfire;
using Hangfire.MySql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using OnePlace.Client.Service;
using OnePlace.Server.Data;
using OnePlace.Server.Extenciones;
using OnePlace.Server.Helpers;
using OnePlace.Server.Services;
using System;
using System.Linq;
using System.Text;
using System.Transactions;

namespace OnePlace.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }       

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            //A�adimos nuestro DBContext
            string mySqlConnectionStr = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<oneplaceContext>
                (options => options.UseMySql(mySqlConnectionStr, ServerVersion.AutoDetect(mySqlConnectionStr)));

            //configuramos identity para control de usuarios
            services.AddIdentity<ApplicationUser, IdentityRole>(
                 options =>
                 {
                     /*De forma predeterminada, requiere que las contrase�as contengan un car�cter en may�sculas,
                     un car�cter en min�sculas, un d�gito y un car�cter no Identity alfanum�rico.
                     Las contrase�as deben tener al menos seis caracteres.*/

                     //alterar la configuracion predeterminada
                     options.Password.RequireDigit = true;
                     //options.Password.RequireLowercase = true;
                     options.Password.RequireNonAlphanumeric = true;
                     options.Password.RequireUppercase = true;
                     options.Password.RequiredLength = 5;
                     options.Password.RequiredUniqueChars = 1;
                     //options.User.RequireUniqueEmail = true;
                     //options.SignIn.RequireConfirmedEmail = true;
                 }
                )
               .AddEntityFrameworkStores<oneplaceContext>()
               .AddDefaultTokenProviders();

            //configuramos los jsonwebtokens con esto vamos a poder enviar hacia el proyecto de blazor las credenciales del usuario
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                  .AddJwtBearer(options =>
               options.TokenValidationParameters = new TokenValidationParameters
               {
                   ValidateIssuer = false,
                   ValidateAudience = false,
                   ValidateLifetime = true,
                   ValidateIssuerSigningKey = true,
                   IssuerSigningKey = new SymmetricSecurityKey(
                   Encoding.UTF8.GetBytes(Configuration["jwt:key"])),
                   ClockSkew = System.TimeSpan.Zero
               });

            //uso de hangfire, en este caso no lo pusimos en startup, creamos una extension de la clase
            services.ConfigureHangFire(Configuration);

            //uso de Automapper          
            services.AddAutoMapper(typeof(Startup));

            //servicio para guardar imagen de manera local 
            services.AddScoped<IAlmacenadorArchivos, AlmacenadorArchivosLocal>();

            //servicio custom para usar un job de hangfire 
            services.AddScoped<ITerminarCursoFechaServicio, TerminarCursoFechaServicio>();
            services.AddScoped<IApiaBdService, ApiaBdService>();
            services.AddScoped<IApiEmpleadosService, ApiEmpleadosService>();

            //servicio para consumir api rest externa del lado del server
            services.AddHttpClient<ISimsacoreService, SimsaCoreService>(client =>
            {
                client.BaseAddress = new Uri("http://api_simsacore.dgl.com.mx/");
                client.Timeout = TimeSpan.FromMinutes(5);
            });

            //agremaos el servicios de addhttpcontextaccesor
            services.AddHttpContextAccessor();         

            //si se encuentra con un bucle de referencia al deserealizar debe ignorar esa situacion
            services.AddMvc().AddNewtonsoftJson(options =>
            options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            services.AddControllersWithViews();
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();            

            app.UseRouting();           

            //creamos un middlewaer de autenticacion y autorizacion
            app.UseAuthentication();
            app.UseAuthorization();

            //ver la interfaz de hangfire, y cambiar la ruta por defecto
            app.UseHangfireDashboard("/jobs");

            //TODO:convertir datetime a cronexpression
            //var input = DateTime.Parse("2014-12-31 00:00:00");
            //var str = string.Format("0 0 * * task", input(n));           

            //se ejecutara cada mes , A las 00:00, el d�a 1 del mes
            RecurringJob.AddOrUpdate<ITerminarCursoFechaServicio>("JobTerminarCursoFecha", servicio => servicio.TerminarCursoporFecha(), "0 0 1 */1 *" );
            RecurringJob.AddOrUpdate<IApiaBdService>("JobApiaBd", servicio => servicio.DatosdeApiABaseDatos(), "0 0 1 */1 *");
            //A las 12:00 p.m, s�lo los domingos
            RecurringJob.AddOrUpdate<IApiEmpleadosService>("JobApiEmpleados", servicio => servicio.DatosdeApiABaseDatosEmpleados(), "0 0 12 * * SUN");
            
            //A las 12:00:00 p. m., todos los domingos, todos los meses
            //0 0 12 ? * SUN *

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("index.html");                
            });
        }
    }
}
