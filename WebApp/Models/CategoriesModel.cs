// DigitalesSchaufenster (C) 2020 DIH-OST

using System;
using Exchange.Model;

namespace WebApp.Models
{
    public class CategoriesModel
    {
        #region Properties

        public List<ExCategoryItem> SelectedCategories { get; set; }

        public int MainCategoryId { get; set; }

        #endregion
    }
}