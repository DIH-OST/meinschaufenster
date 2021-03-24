// DigitalesSchaufenster (C) 2020 DIH-OST

using System;

namespace WebApp.Services
{
    /// <summary>
    ///     <para>DESCRIPTION</para>
    ///     Klasse GeocodingService. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public static class GeocodeService
    {
        /// <summary>
        ///     This method converts an address to its GPS coordinates.
        /// </summary>
        /// <param name="postcode">The post code of the location.</param>
        /// <param name="location">The name of the town.</param>
        /// <param name="address">The address, including street name and house number.</param>
        /// <param name="country">The country.</param>
        /// <returns>A result object containing the GPS coordinates.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if either of the strings are null.
        /// </exception>
        /// <exception cref="FormatException">
        ///     Is thrown if the service could not get a single result.
        /// </exception>
        public static Result ConvertToGPSCoordinates(string postcode, string location, string address, string country = "Austria")
        {
            GoogleSigned.AssignAllServices(new GoogleSigned("[KEY]"));

            if (address == null || location == null || country == null)
            {
                throw new ArgumentNullException("Address parameters, including postcode, location and street name, must not be null!");
            }

            var request = new GeocodingRequest();
            request.Address = string.Join(", ", address, location, postcode, country);

            var response = new GeocodingService().GetResponse(request);

            if (response.Status == ServiceResponseStatus.Ok)
            {
                return response.Results.First();
            }

            throw new FormatException("Specified format was incorrect. Could not get result.");
        }
    }
}