﻿namespace PXWeb.Database
{
    public class MenuSortItem
    {
        public MenuSortItem(string sortString, string language)
        {
            SortString = sortString;
            Language = language;
        }

        public string SortString { get; set; }
        public string Language { get; set; }
    }
}
