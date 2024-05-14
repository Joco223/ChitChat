using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ChitChatLauncher.Helpers {
	public class GeneratorSHA256 {
		/// <summary>
		/// Generates a SHA256 hash from a file
		/// </summary>
		/// <param name="filePath">File to generate hash from</param>
		/// <returns></returns>
		public static string GenerateSHA256FromFile(string filePath) {
			using FileStream fileStream = new(filePath, FileMode.Open, FileAccess.Read);

			using SHA256 sha256 = SHA256.Create();

			byte[] hashBytes = sha256.ComputeHash(fileStream);
			StringBuilder hashBuilder = new();

			foreach (byte b in hashBytes) {
				hashBuilder.Append(b.ToString("x2"));
			}

			return hashBuilder.ToString();
		}

		/// <summary>
		/// Reads a SHA256 hash from a file
		/// </summary>
		/// <param name="filePath"></param>
		/// <returns></returns>
		public static string ReadSHA256FromFile(string filePath) {
			using StreamReader reader = new(filePath);
			string content = reader.ReadToEnd();
			content = content[..content.IndexOf(' ')];
			return content;
		}
	}
}
