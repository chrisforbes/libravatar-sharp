using System;
using libravatarsharp;

namespace examples
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			var uri = AvatarUri.FromEmail( "chrisf@ijw.co.nz" );
			Console.WriteLine (uri);
		}
	}
}

