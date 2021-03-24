// DigitalesSchaufenster (C) 2020 DIH-OST

using System;

namespace Exchange.Model
{
    /// <summary>
    ///     <para>CategoricalData - Demodaten für Chart</para>
    ///     Klasse ExCategoricalData. (C) 2017 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class ExCategoricalData
    {
        #region Properties

        public object Category { get; set; }

        public double Value { get; set; }

        #endregion
    }


    /// <summary>
    ///     <para>CategoricalDataArray  - Demodaten für Chart</para>
    ///     Klasse CategoricalDataArray. (C) 2017 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class ExCategoricalDataArray
    {
        #region Properties

        public List<ExCategoricalData> DataA { get; set; } = new List<ExCategoricalData>();

        public List<ExCategoricalData> DataB { get; set; } = new List<ExCategoricalData>();

        public List<ExCategoricalData> DataC { get; set; } = new List<ExCategoricalData>();

        #endregion
    }

    public class ExPieData
    {
        #region Properties

        public double Value { get; set; }

        #endregion
    }


    public class ExHistoricalData
    {
        #region Properties

        /// <summary>
        ///     Daten für Kreisdiagramm letzte Woche
        /// </summary>
        public ObservableCollection<ExPieData> LastWeek { get; set; } = new ObservableCollection<ExPieData>();

        /// <summary>
        ///     Daten für Kreisdiagramm letztes Monat
        /// </summary>
        public ObservableCollection<ExPieData> LastMonth { get; set; } = new ObservableCollection<ExPieData>();

        /// <summary>
        ///     Daten für Kreisdiagramm letztes Jahr
        /// </summary>
        public ObservableCollection<ExPieData> LastYear { get; set; } = new ObservableCollection<ExPieData>();

        #endregion
    }
}