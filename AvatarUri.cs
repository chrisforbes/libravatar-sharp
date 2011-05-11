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
using System.Collections.Generic;
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
			var identity = CanonicalizeEmail(email);
			var hashFunction = options.UseSHA256 ? (Func<string,string>)SHA256Hash : MD5Hash;
			return FromHashedIdentity( hashFunction(identity), options );
		}
		
		static Uri FromHashedIdentity( string hash, AvatarOptions options )
		{
			var args = new Dictionary<string,string>();
			if (options.DefaultImage != null)
				args["d"] = options.DefaultImage;
			if (options.Size != null)
				args["s"] = options.Size.ToString();
			
			var baseUri = options.PreferHttps ? options.SecureBaseUri : options.BaseUri;
			var uri = baseUri + hash + UriQueryFromArgs(args);
			return new Uri(uri);
		}
		
		static string UriQueryFromArgs( Dictionary<string,string> args )
		{
			if (args.Count == 0)
				return "";
			
			return "?" + string.Join( "&", args
				.Select( kv => string.Format( "{0}={1}", kv.Key, 
					Uri.EscapeDataString(kv.Value) ) )
				.ToArray() );
		}
		
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
		
		static string CanonicalizeEmail( string email )
		{
			return email.ToLowerInvariant();
		}
		
		static string CanonicalizeOpenID( string openid )
		{
			var ub = new UriBuilder(openid);
			ub.Scheme = ub.Scheme.ToLowerInvariant();
			ub.Host = ub.Host.ToLowerInvariant();
			return ub.ToString();
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

		/// <summary>
		/// URI for a default image, if no image is found for the user. This also accepts
		/// any of the "special" values in AvatarDefaultImages
		/// </summary>
		public string DefaultImage = AvatarDefaultImages.Default;
		
		/// <summary>
		/// Size of the image requested. Valid values are between 1 and 512 pixels. The default
		/// size is 80 pixels.
		/// </summary>
		public int? Size;
		
		/// <summary>
		/// Specifies a custom base URI for HTTP use. The default is to use the official 
		/// libravatar HTTP server. If you *really* wanted to use a non-free server, you
		/// could set this to "http://gravatar.com/avatar/", but why would you do such a thing?
		/// </summary>
		public string BaseUri = "http://cdn.libravatar.org/avatar/";
		
		/// <summary>
		/// Specifies a custom base URI for HTTPS use. The default is to use the official
		/// libravatar HTTPS server.
		/// </summary>
		public string SecureBaseUri = "https://seccdn.libravatar.org/avatar/";
	}
	
	public static class AvatarDefaultImages
	{
		/// <summary>
		/// The default image provided by the libravatar server
		/// </summary>
		public static readonly string Default = null;
		/// <summary>
		/// No image at all. The server will return an HTTP 404 Not Found response instead
		/// </summary>
		public static readonly string Error = "404";
		/// <summary>
		/// A generic "person" image
		/// </summary>
		public static readonly string Person = "mm";
		/// <summary>
		/// A colored geometric pattern generated from the hash
		/// </summary>
		public static readonly string Identicon = "identicon";
		/// <summary>
		/// A monster image generated from the hash
		/// </summary>
		public static readonly string MonsterID = "monsterid";
		/// <summary>
		/// A face image generated from the hash
		/// </summary>
		public static readonly string Wavatar = "wavatar";
		/// <summary>
		/// A retro-styled image generated from the hash
		/// </summary>
		public static readonly string Retro = "retro";
	}
}

