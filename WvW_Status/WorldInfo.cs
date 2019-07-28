namespace WvW_Status
{
    class WorldInfo
    {
        public string Name { get; set; }
        private string _Population;
        public string Population
        {
            get
            {
                return _Population;
            }
            set
            {
                if (value == "Low") { _Population = "Low".PadRight(15) + "\t Free"; }
                else
                if (value == "Medium") { _Population = "Medium".PadRight(15) + "\t 500 Gems"; }
                else
                if (value == "High") { _Population = "High".PadRight(15) + "\t 1,000 Gems"; }
                else
                if (value == "VeryHigh") { _Population = "Very High".PadRight(15) + "\t 1,800 Gems"; }
                else
                if (value == "Full") { _Population = "Full".PadRight(15) + "\t Closed"; }
                else
                {
                    _Population = "No Data";
                }
            }
        }
    }
}