// (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 25.03.2020 09:56
// Entwickler      Michael Kollegger
// Projekt         DigitalesSchaufenster

using System;

namespace AndroidApp
{
    /// <summary>
    /// <para>DESCRIPTION</para>
    /// Klasse AzurePushConstants. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class AzurePushConstants
    {
        public const string ListenConnectionString = "Endpoint=sb://digitalwindowhubnamespace.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=BDcV2c7a9ix7VHGVCG6dLUAf3tSe9dmazboXWgLRz9o=";
        public const string NotificationHubName = "digitalwindowhub";
    }
}