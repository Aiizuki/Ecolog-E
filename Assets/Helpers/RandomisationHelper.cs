using System.Collections.Generic;
using System.Linq;

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

		public static T GetRandomItemFromList<T>(List<T> list, T exclude)
		{
			int idx = rng.Next(list.Count - 1);
			T result = list[idx];
			return result.Equals(exclude) ? list[list.Count - 1] : result;
		}

		public static T GetRandomItemFromStack<T>(Stack<T> stack)
			=> GetRandomItemFromList(new List<T>(stack));

		public static Stack<T> ShuffleStack<T>(Stack<T> stack)
		{
			List<T> list = new List<T>(stack);
			list = list.OrderBy(_ => rng.Next()).ToList();
			return new Stack<T>(list);
		}

		internal static bool RandomChooseBoolean()
			=> rng.NextDouble() < 0.5;
	}
}