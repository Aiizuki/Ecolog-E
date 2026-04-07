namespace Assets.Components.SaveService
{
	using Newtonsoft.Json;
	using System;
	using System.IO;
	using System.Security.Cryptography;
	using System.Text;
	using UnityEngine;

	namespace Components.SaveService
	{
		public static class SaveServiceController
		{
			private const string FILE_NAME = "EcologE.sav";

			private static readonly byte[] AESKey = CreateAESKey();
			private static readonly byte[] AESIV = Encoding.UTF8.GetBytes("1234567891234560");

			private static string FilePath
				=> Path.Combine(Application.persistentDataPath, FILE_NAME);

			public static void Save(SaveData data)
			{
				string json = JsonConvert.SerializeObject(data, Formatting.Indented);

#if UNITY_EDITOR
				File.WriteAllText(FilePath + ".json", json);
				Debug.Log("Data saved (unencrypted) at: " + FilePath + ".json");
#else
				byte[] encrypted = Encrypt(json);
				File.WriteAllBytes(FilePath, encrypted);
				Debug.Log("Data saved (encrypted) at: " + FilePath);
#endif
			}

			public static SaveData Load()
			{
				try
				{
#if UNITY_EDITOR
					string json = File.ReadAllText(FilePath + ".json");
#else
					byte[] encrypted = File.ReadAllBytes(FilePath);
					string json = Decrypt(encrypted);
#endif
					return JsonConvert.DeserializeObject<SaveData>(json);
				}
				catch (Exception exception)
				{
					Debug.LogWarning("No data found, creating a new one... Details: " + exception);
					return new SaveData();
				}
			}

			private static byte[] Encrypt(string plainText)
			{
				using Aes aes = Aes.Create();
				aes.Key = AESKey;
				aes.IV = AESIV;

				using MemoryStream ms = new();
				using CryptoStream cs = new(ms, aes.CreateEncryptor(), CryptoStreamMode.Write);
				using (StreamWriter sw = new(cs))
					sw.Write(plainText);

				return ms.ToArray();
			}

			private static string Decrypt(byte[] cipherBytes)
			{
				using Aes aes = Aes.Create();
				aes.Key = AESKey;
				aes.IV = AESIV;

				using MemoryStream ms = new(cipherBytes);
				using CryptoStream cs = new(ms, aes.CreateDecryptor(), CryptoStreamMode.Read);
				using StreamReader sr = new(cs);
				return sr.ReadToEnd();
			}

			private static byte[] CreateAESKey()
			{
				string seed = SystemInfo.deviceUniqueIdentifier + "EcologE_$v1_s3cur3";
				return SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(seed));
			}
		}
	}
}