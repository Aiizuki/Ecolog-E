namespace Assets.Components.SaveService
{
	[System.Serializable]
	public class SaveData
	{
		public int RunCount;
		public int Score;

		public SaveData()
		{
			RunCount = 0;
			Score = 0;
		}
	}
}
