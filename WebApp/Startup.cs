// DigitalesSchaufenster (C) 2020 DIH-OST

using System;
using Database;
using Exchange;
using Exchange.Resources;
using WebApp.Services;

namespace WebApp
{
    public class DummyNavigtor : VmNavigator
    {
        /// <summary>VmNavigator</summary>
        /// <param name="settings">Einstellungen</param>
        public DummyNavigtor(IAppSettingsNavigation settings) : base(settings)
        {
        }

        public override int GetItemsOnPageStack()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Menü (zB. Sprache) hat sich geändert =&gt; neu erzeugen
        /// </summary>
        public override void UpdateMenu()
        {
            throw new NotImplementedException();
        }

        /// <summary>Typ der View für neue Instanz des Objekts</summary>
        /// <param name="viewClassName"></param>
        /// <returns></returns>
        public override Type GetViewType(string viewClassName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Ein Eigabefenster starten und die eingegeben Daten zurück liefern
        ///     WPF: Ein neues Fenster wird erzeugt (kann gebunden asModal angezeigt werden)
        ///     APPS: Eine neue View wird auf den aktuellen NaviagtionStack gepusht
        /// </summary>
        /// <param name="view">Instanzierte View</param>
        /// <param name="asModal"></param>
        /// <returns></returns>
        public override Task<object> NavToWindow(object view, bool asModal)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Eine View laden
        ///     WPF: View wird im CONTENT FRAME des MainWindow geladen
        ///     APPS: View wird mit Menü angezeigt und ist dann die einzige View am Stack
        /// </summary>
        /// <param name="viewInstance"></param>
        /// <param name="showMenu">Menü - (App Standart von jeder Plattform) Seite anzueigen. Ab 3.2 eher immer null!</param>
        public override void NavToPage(object viewInstance, bool? showMenu = null)
        {
            throw new NotImplementedException();
        }
    }

    public class DummyFiles : VmFiles
    {
        /// <summary>Plattformabhängige Dateizugriff Implementierung</summary>
        /// <param name="root">APP Root Folder</param>
        /// <param name="rootTmp">APP TMP Folder</param>
        /// <param name="publicFolderPath">Public Folder</param>
        /// <param name="appDataSecret"></param>
        public DummyFiles(string root, string rootTmp, string publicFolderPath, string appDataSecret) : base(root, rootTmp, publicFolderPath, appDataSecret)
        {
        }

        public DummyFiles() : base(string.Empty, string.Empty, string.Empty, string.Empty)
        {
        }

        public override string CreateXmlFile(object data, Type dataType, string fileName = "", bool autoOpen = true)
        {
            // TODO
            return string.Empty;
        }

        public override async Task<string> GetLocalFileNameForOpen(string initialDirectory = "", string filter = "")
        {
            // TODO
            return string.Empty;
        }

        public override async Task<string> GetLocalFileNameForSave(string fileName, bool useFileSaveDialog, string filter = "")
        {
            // TODO
            return string.Empty;
        }

        public override void OpenFile(string localFile)
        {
            // TODO
        }
    }

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        #region Properties

        public IConfiguration Configuration { get; }

        #endregion

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(
                CookieAuthenticationDefaults.AuthenticationScheme
            ).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme,
                options =>
                {
                    options.LoginPath = "/Account/LoginUser";
                    options.LogoutPath = "/Account/Logout";
                });

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddSession();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1).AddJsonOptions(options =>
                options.SerializerSettings.ContractResolver = new DefaultContractResolver());

            // Add Kendo UI services to the services container
            services.AddKendo();

            //DB Context
            services.AddDbContext<Db>
                ();

            // authentication 
            services.AddAuthentication(options => { options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme; });

            services.AddTransient(
                m => new UserManager(
                    Configuration
                        .GetValue<string>(
                            "" //this is a string constant
                        )
                )
            );
            services.AddDistributedMemoryCache();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            //app.UseCookiePolicy();
            app.UseAuthentication();

            var setting = AppSettings.Current();

            setting.BaseNavigator = new DummyNavigtor(AppSettings.Current());
            setting.BaseFiles = new DummyFiles();
            setting.AppCenterServices = new[] {typeof(Analytics), typeof(Crashes)};
            setting.LanguageContent = Language.GetText();

            app.UseOoui();

            Forms.Init();

            VmBase.InitComponentManager(setting);
            VmBase.CManager.InitAllNotInitialized();

            app.UseSession();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    "default",
                    "{controller=Home}/{action=Index}/{id?}");
            });
            app.UseCookiePolicy();
        }
    }
}