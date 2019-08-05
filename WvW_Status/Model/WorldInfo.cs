namespace WvW_Status.Model
{
    internal class WorldInfo
    {
        public string Name { get; set; }
        private string _population;

        public string Population
        {
            get => _population;
            set
            {
                switch (value)
                {
                    case "Low":
                        _population = "Low".PadRight(15) + "\t Free";
                        break;
                    case "Medium":
                        _population = "Medium".PadRight(15) + "\t 500 Gems";
                        break;
                    case "High":
                        _population = "High".PadRight(15) + "\t 1,000 Gems";
                        break;
                    case "VeryHigh":
                        _population = "Very High".PadRight(15) + "\t 1,800 Gems";
                        break;
                    case "Full":
                        _population = "Full".PadRight(15) + "\t Closed";
                        break;
                    default:
                        _population = "No Data";
                        break;
                }
            }
        }
    }
}