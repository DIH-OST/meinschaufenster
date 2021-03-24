// DigitalesSchaufenster (C) 2020 DIH-OST

using System;

namespace WebApp.Models
{
    public class ShopViewModel : IBissSerialize
    {
        #region Properties

        public string type { get; set; }
        public List<Feature> features { get; set; }

        #endregion
    }

    public class Feature : IBissSerialize
    {
        #region Properties

        public Geometry geometry { get; set; }
        public string type { get; set; }
        public Properties properties { get; set; }

        #endregion
    }

    public class Geometry : IBissSerialize
    {
        #region Properties

        public string type { get; set; }
        public List<double> coordinates { get; set; }

        #endregion
    }

    public class Properties : IBissSerialize
    {
        #region Properties

        public string allHoursBlock { get; set; }
        public string category { get; set; }
        public string hours { get; set; }
        public string description { get; set; }
        public string name { get; set; }
        public string imgurl { get; set; }
        public string phone { get; set; }
        public string storeid { get; set; }
        public string paymentoptions { get; set; }
        public string symbol { get; set; }
        public string website { get; set; }
        public bool isopen { get; set; }

        #endregion
    }
}