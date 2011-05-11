#region Copyright & License Information
/*
 * Copyright 2011 The libravatar-sharp Developers (see AUTHORS)
 * This file is part of libravatar-sharp, which is free software. It is made 
 * available to you under the terms of the GNU General Public License v3
 * as published by the Free Software Foundation. For more information,
 * see COPYING.
 */
#endregion


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
			using( var h = MD5.Create() )
				return Hash(s,h);
		}
		
		static string SHA256Hash( string s )
		{
			using( var h = SHA256.Create() )
				return Hash(s,h);
		}
		
		static string Hash( string s, HashAlgorithm h )
		{
			var bytes = Encoding.UTF8.GetBytes(s);
			
			return new string(h.ComputeHash(bytes)
			    .SelectMany(a => a.ToString("x2")).ToArray());
		}
	}
}

