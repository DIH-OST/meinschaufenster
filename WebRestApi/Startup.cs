// DigitalesSchaufenster (C) 2020 DIH-OST

using System;
using Database;
using Exchange;
using Exchange.Resources;
using WebRestApi.Helper;
using WebRestApi.Hubs;
using WebRestApi.Services;

namespace WebRestApi
{
    /// <summary>
    ///     Rest Web API Startup
    /// </summary>
    public class Startup
    {
        /// <summary>
        ///     Welche Swagger Version soll verwendet werden (v1, v2, v3)
        /// </summary>
        private const string SwaggerVersion = "v2";

        /// <summary>
        ///     Rest Web API Startup
        /// </summary>
        /// <param name="env"></param>
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();

            //Datenbank
            Db.CreateAnFillUp();
        }

        #region Properties

        /// <summary>
        ///     Configuration
        /// </summary>
        public IConfigurationRoot Configuration { get; }

        #endregion

        /// <summary>
        ///     This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<Db>();

            // configure strongly typed settings objects
            var appSettingsSection = Configuration.GetSection("WebAppSettings");
            services.Configure<WebAppSettings>(appSettingsSection);

            var appSettings = appSettingsSection.Get<WebAppSettings>();

            // configure jwt authentication
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);

            services.AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                                                  {
                                                      ValidateIssuerSigningKey = true,
                                                      ValidateLifetime = true,
                                                      IssuerSigningKey = new SymmetricSecurityKey(key),
                                                      ValidateIssuer = false,
                                                      ValidateAudience = false,
                                                      ClockSkew = TimeSpan.Zero
                                                  };
                });

            // Add framework services.
            services.AddMvc();

            //Für EMail Versand
            services.AddScoped<ViewRender, ViewRender>();

            // configure DI for application services
            services.AddScoped<IAuthUserService, AuthUserService>();
            services.AddScoped<IAuthUserCredentials, ExAuthUserCredentials>();

            //Initialize Swagger
            if (Constants.AppConfiguration.CurrentBuildType != EnumCurrentBuildType.CustomerRelease)
            {
                string additionalInformation = "";

                if (Constants.AppConfiguration.CurrentBuildType == EnumCurrentBuildType.Developer)
                {
                    additionalInformation = "<br><br>";
                    additionalInformation += "Für geschützte API Zugriffe muss ein Token mitangegeben werden.<br>";
                    additionalInformation += "Schritt 1: ";
                    additionalInformation += @"Token anfordern: Abschnitt Users -> /api/authenticate <br>";
                    additionalInformation += "Schritt 2: ";
                    additionalInformation += "Rechts oben auf Authorize klicken, diesen Token angeben und auf Authorize klicken.<br>";
                    additionalInformation += "Schritt 3: Die gewünschte Funktion aufrufen.<br>";
                    additionalInformation += "Hinweis: Der Token hat standardmäßig eine Gültigkeit von 30min<br>";
                }

                services.AddSwaggerGen(c =>
                {
                    c.AddSecurityDefinition("Bearer", new ApiKeyScheme {In = "header", Description = "Token hier eingeben (Bearer + Token) bzw. reinkopieren", Name = "Authorization", Type = "apiKey"});
                    c.SwaggerDoc(SwaggerVersion, new Info
                                                 {
                                                     Title = ResWebCommon.SwaggerTitle,
                                                     Version = SwaggerVersion,
                                                     Description = ResWebCommon.SwaggerDescription + additionalInformation,
                                                     Contact = new Contact
                                                               {
                                                                   Name = ResWebCommon.SwaggerContactName,
                                                                   Email = ResWebCommon.SwaggerContactEMail,
                                                                   Url = ResWebCommon.SwaggerContactUrl
                                                               }
                                                 });

                    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, "Common", xmlFile);

                    if (File.Exists(xmlPath))
                        c.IncludeXmlComments(xmlPath);
                });
            }

            //Init für Biss.Apps.*.RestAccess
            var info = GetAssemblyInfos(Assembly.GetExecutingAssembly());
            services.AddRestAccess(new RestAccessServiceData
                                   {
                                       PingResult = info,
                                       GetUser = (a, b) =>
                                       {
                                           using (var db = new Db())
                                           {
                                               var u = db.TblUsers.FirstOrDefault(tu => tu.Id == a && tu.RestPasswort == b);
                                               if (u != null)
                                               {
                                                   return new ExAuthUser
                                                          {
                                                              Username = u.PhoneNumber,
                                                              Id = u.Id,
                                                              FirstName = u.Firstname,
                                                              Lastname = u.Lastname
                                                          };
                                               }

                                               return null;
                                           }
                                       },
                                       AppSecret = appSettings.Secret,
                                       GetUserById = a =>
                                       {
                                           using (var db = new Db())
                                           {
                                               var u = db.TblUsers.FirstOrDefault(tu => tu.Id == a);
                                               if (u != null)
                                               {
                                                   return new ExAuthUser
                                                          {
                                                              Username = u.PhoneNumber,
                                                              Id = u.Id,
                                                              FirstName = u.Firstname,
                                                              Lastname = u.Lastname
                                                          };
                                               }

                                               return null;
                                           }
                                       },
                                       LstRefreshToken = new ConcurrentDictionary<int, List<ExRefreshToken>>(), //TODO MFa: Von Storage laden
                                       RemoveRefreshToken = (a, b) => true, //TODO MFa: Von Storage ablegen
                                       AddRefreshToken = (a, b) => true //TODO MFa: In  Storage ablegen
                                   }
            );
            ExSaveDataResult.LanguageContent = Language.GetText();

            services.AddSignalR();
        }

        /// <summary>
        ///     This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="loggerFactory"></param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseMvc();

            app.UseWebSockets();
            app.UseSignalR(routes => { routes.MapHub<BissHub>("/bisshub"); });

            //Initialize Swagger

            if (Constants.AppConfiguration.CurrentBuildType != EnumCurrentBuildType.CustomerRelease)
            {
                app.UseSwagger();
                app.UseSwaggerUI(c => { c.SwaggerEndpoint($"/swagger/{SwaggerVersion}/swagger.json", "Rest API"); });
            }
        }

        /// <summary>
        ///     Infos über ein Assembly sammeln
        /// </summary>
        /// <param name="assembly">Assembly (zB Assembly.GetExecutingAssembly() für den Code der gerade läuft)</param>
        /// <returns></returns>
        private ExPingResult GetAssemblyInfos(Assembly assembly)
        {
            DateTime assemblyDate = DateTime.MinValue;
            string assemblyVersion = "?";
            if (assembly != null)
            {
                var attribute = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
                if (attribute != null)
                {
                    assemblyVersion = attribute.InformationalVersion;
                }

                try
                {
                    assemblyDate = File.GetLastWriteTime(assembly.Location);
                }
                catch
                {
                    Logging.Log.LogWarning("Cannot read assembly Date.");
                }
            }

            var info = new ExPingResult {VersionNr = assemblyVersion, VersionUpdatedAt = assemblyDate};
            return info;
        }
    }
}