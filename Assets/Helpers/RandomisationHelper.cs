using System.Collections.Generic;

namespace Assets.Scripts.Helpers
{
    /// <summary>
    /// Helps with randomisation tasks such as shuffling lists, getting random items from lists, and making random boolean choices.
    /// </summary>
    public static class RandomisationHelper
    {
        private static readonly System.Random rng = new System.Random();

        public static List<T> ShuffleList<T>(List<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int randomIndex = rng.Next(0, i + 1);
                T temp = list[i];
                list[i] = list[randomIndex];
                list[randomIndex] = temp;
            }

            return list;
        }

        public static T GetRandomItemFromList<T>(List<T> list)
            => list[rng.Next(list.Count)];

        internal static bool RandomChooseBoolean()
            => rng.NextDouble() < 0.5;
    }
}