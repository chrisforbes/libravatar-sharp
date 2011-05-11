using System;
using System.Security.Cryptography;
using System.Linq;
using System.Text;

namespace libravatarsharp
{
	public static class AvatarUri
	{	
		/// <summary>
		/// Converts an email address to a libravatar URI
		/// </summary>
		/// <param name="email">
		/// The email address associated with the avatar you want to retrieve
		/// </param>
		/// <returns>
		/// A URI which points to the user's avatar.
		/// </returns>
		public static Uri FromEmail( string email )
		{
			return new Uri(BaseUrl + MD5Hash(email.ToLowerInvariant()));
		}
	
		static readonly string BaseUrl = "http://cdn.libravatar.org/avatar/";
		
		static string MD5Hash( string s )
		{
			var bytes = Encoding.UTF8.GetBytes(s);
			
			using( var csp = MD5.Create())
				return new string(csp.ComputeHash(bytes)
				    .SelectMany(a => a.ToString("x2")).ToArray());
		}
	}
}

