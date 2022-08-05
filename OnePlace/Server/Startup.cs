using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using OnePlace.Server.Data;
using OnePlace.Server.Helpers;
using System.Linq;
using System.Text;

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
            //Añadimos nuestro DBContext
            string mySqlConnectionStr = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<oneplaceContext>
                (options => options.UseMySql(mySqlConnectionStr, ServerVersion.AutoDetect(mySqlConnectionStr)));

            //configuramos identity para control de usuarios
            services.AddIdentity<ApplicationUser, IdentityRole>(
                 options =>
                 {
                     /*De forma predeterminada, requiere que las contraseñas contengan un carácter en mayúsculas,
                     un carácter en minúsculas, un dígito y un carácter no Identity alfanumérico.
                     Las contraseñas deben tener al menos seis caracteres.*/

                     //alterar la configuracion predeterminada
                     options.Password.RequireDigit = true;
                     options.Password.RequireLowercase = true;
                     options.Password.RequireNonAlphanumeric = true;
                     options.Password.RequireUppercase = true;
                     options.Password.RequiredLength = 8;
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

            //uso de Automapper          
            services.AddAutoMapper(typeof(Startup));

            //servicio para guardar imagen de manera local 
            services.AddScoped<IAlmacenadorArchivos, AlmacenadorArchivosLocal>();

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

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("index.html");
            });
        }
    }
}
