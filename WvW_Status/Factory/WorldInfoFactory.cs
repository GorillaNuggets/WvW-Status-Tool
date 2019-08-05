using System.Collections.Generic;
using WvW_Status.Model;

namespace WvW_Status.Factory
{
    internal class WorldInfoFactory
    {
        public static Dictionary<int, WorldInfo> Create(List<WorldsList> worlds)
        {
            var worldDictionary = new Dictionary<int, WorldInfo>();

            foreach (var world in worlds)
            {
                worldDictionary.Add(world.Id, new WorldInfo()
                {
                    Name = world.Name,
                    Population = world.Population
                });
            }

            return worldDictionary;
        }
    }
}