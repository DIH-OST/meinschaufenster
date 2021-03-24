// DigitalesSchaufenster (C) 2020 DIH-OST

using System;

namespace WebApp.Models
{
    public class ErrorViewModel
    {
        #region Properties

        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        #endregion
    }
}