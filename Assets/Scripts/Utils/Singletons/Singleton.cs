// Singleton - thread safe

namespace Jatewit {
	public sealed class Singleton<T> where T : new() {
		public static T Instance {
			get {
				return Nested.Instance;
			}
		}
		
		// Thread safe - lazy instantiation
		private class Nested
		{
			internal static readonly T Instance = new T();
			static Nested () {}
		}
	}
}
