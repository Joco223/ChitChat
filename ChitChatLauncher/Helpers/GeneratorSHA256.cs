using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ChitChatLauncher.Helpers
{
	public class GeneratorSHA256
	{
		public static string GenerateSHA256FromFile(string filePath)
		{
			using FileStream fileStream = new(filePath, FileMode.Open, FileAccess.Read);

			using SHA256 sha256 = SHA256.Create();
			
			byte[] hashBytes = sha256.ComputeHash(fileStream);
			StringBuilder hashBuilder = new();

			foreach (byte b in hashBytes)
			{
				hashBuilder.Append(b.ToString("x2"));
			}

			return hashBuilder.ToString();
		}

		public static string ReadSHA256FromFile(string filePath)
		{
			using StreamReader reader = new(filePath);
			string content = reader.ReadToEnd();
			content = content[..content.IndexOf(' ')];
			return content;
		}
	}
}
