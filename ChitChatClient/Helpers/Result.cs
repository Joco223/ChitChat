using System.Diagnostics;
using Serilog;

namespace ChitChatClient.Helpers {
	public class Result<T> {
		public T Data { get; set; }

		public string Error { get; set; }

		public bool Failed { get; set; }

		public int ErrorType { get; set; }

		public Result() {
			Data = default!;
			Error = string.Empty;
			Failed = false;
			ErrorType = 0;
		}

		public static Result<T> OK(T data, bool customData = false) {
			Result<T> result = new() {
				Failed = false
			};

			// If custom data is not set, log the message to the logger
			if (!customData) {
				var methodInfo = new StackTrace().GetFrame(2).GetMethod();
				var className = methodInfo.ReflectedType.Name;
				Log.Information($"{data} - {className}");
			}

			return result;
		}

		public static Result<T> Fail(string error, bool customData = false) {
			Result<T> result = new() {
				Error = error,
				Failed = true
			};

			// If custom data is not set, log the message to the logger
			if (!customData) {
				var methodInfo = new StackTrace().GetFrame(2).GetMethod();
				var className = methodInfo.ReflectedType.Name;
				Log.Error($"{error} - {className}");
			}

			return result;
		}

		public static Result<T> Fail(string error, int errorType, bool customData = false)  {
			Result<T> result = new() {
				Error = error,
				ErrorType = errorType,
				Failed = true
			};

			// If custom data is not set, log the message to the logger
			if (!customData) {
				var methodInfo = new StackTrace().GetFrame(2).GetMethod();
				var className = methodInfo.ReflectedType.Name;
				Log.Error($"{error}, {errorType} - {className}");
			}

			return result;
		}
	}
}
