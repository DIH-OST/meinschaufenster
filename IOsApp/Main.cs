﻿// DigitalesSchaufenster (C) 2020 DIH-OST

using System;
using System.Diagnostics;
using UIKit;


namespace IOsApp
{
    public class Application
    {
        // This is the main entry point of the application.
        static void Main(string[] args)
        {
            // if you want to use a different Application Delegate class from "AppDelegate"
            // you can specify it here.
            try
            {
                UIApplication.Main(args, null, "AppDelegate");
            }
            catch (Exception e)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine(e.ToString());
                    Debugger.Break();
                }

                throw;
            }
        }
    }
}