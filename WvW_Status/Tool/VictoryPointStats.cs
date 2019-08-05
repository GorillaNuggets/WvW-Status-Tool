namespace WvW_Status.Tool
{
    public class VictoryPointStats
    {
        public static (int lowest, int highest, string asString) Calculate(int victoryPoints, int skirmish)
        {
            var lowest = victoryPoints + ((85 - skirmish) * 3);
            var highest = victoryPoints + ((85 - skirmish) * 5);
            var tip = $"Lowest\t {lowest} \r\nHighest\t {highest}";

            return (lowest, highest, tip);
        }
    }
}