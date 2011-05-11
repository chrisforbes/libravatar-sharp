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
		/// Converts an email address to a libravatar URI, using default options
		/// </summary>
		/// <param name="email">
		/// The email address associated with the avatar you want to retrieve
		/// </param>
		/// <returns>
		/// A URI which points to the user's avatar.
		/// </returns>
		public static Uri FromEmail( string email )
		{
			return FromEmail( email, new AvatarOptions {} );
		}

		/// <summary>
		/// Converts an email address to a libravatar URI
		/// </summary>
		/// <param name="email">
		/// The email address associated with the avatar you want to retrieve
		/// </param>
		/// <param name="options">
		/// An AvatarOptions object, which provides various ways to customize the behavior
		/// </param>
		/// <returns>
		/// A URI which points to the user's avatar.
		/// </returns>
		public static Uri FromEmail( string email, AvatarOptions options )
		{
			var baseUrl = options.PreferHttps ? SecureBaseUrl : BaseUrl;
			var hash = options.UseSHA256 ? (Func<string,string>)SHA256Hash : MD5Hash;
			
			return new Uri(baseUrl + hash(email.ToLowerInvariant()));
		}
	
		static readonly string BaseUrl = "http://cdn.libravatar.org/avatar/";
		static readonly string SecureBaseUrl = "https://cdn.libravatar.org/avatar/";
		
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
	
	public class AvatarOptions
	{
		/// <summary>
		/// Produce https:// URIs where possible. This avoids mixed-content warnings in browsers
		/// when using libravatar-sharp from within a page served via HTTPS.
		/// </summary>
		public bool PreferHttps;
		
		/// <summary>
		/// Use the SHA256 hash algorithm, rather than MD5. SHA256 is significantly stronger,
		/// but is not supported by Gravatar, so libravatar's fallback to Gravatar for missing
		/// images will not work.
		/// </summary>
		public bool UseSHA256;
	}
}

