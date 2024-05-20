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

		public static Result<T> OK(T data) => new() { Data = data, Failed = false };

		public static Result<T> Fail(string error) => new() { Error = error, Failed = true };

		public static Result<T> Fail(string error, int errorType) => new() { Error = error, Failed = true, ErrorType = errorType };
	}
}
