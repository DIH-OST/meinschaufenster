// DigitalesSchaufenster (C) 2020 DIH-OST

using System;

namespace WebRestApi
{
    /// <summary>
    ///     Programm
    /// </summary>
    public class Program
    {
        /// <summary>
        ///     Main
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .UseApplicationInsights()
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConsole();
                    logging.AddDebug();
                })
                .Build();

            host.Run();
        }
    }
}