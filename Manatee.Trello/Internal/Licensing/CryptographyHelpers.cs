﻿#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
// Modified for use in Manatee.Trello
#endregion

using System;
using System.Security.Cryptography;
using Manatee.Json;
using Manatee.Json.Serialization;

namespace Manatee.Trello.Internal.Licensing
{
	internal static class CryptographyHelpers
	{
		private const string PublicKey = "{\"Exponent\":\"AQAB\",\"Modulus\":\"2m8UmVALYWjUB5+2muX2BUNk/VWMAbmuD6W1BHPqyRIqhZ+VT7kUHgQJgTTRUDokWfqR0u2hPdDlnBMPbbbdtWMjKxsdHy/5mdCxXzI1pOUfY/JyxP8B2d/RneLOxT7fDGvuhhejTJcAiRwQ1N81o1Z01IWVH4muZhZg/xLpSCk=\"}";

		static CryptographyHelpers()
		{
			SerializerFactory.AddSerializer(new ByteArraySerializer());
		}

		internal static bool ValidateData(byte[] data, byte[] signature)
		{
			bool valid;

			var publicKeyJson = JsonValue.Parse(PublicKey);
			var serializer = new JsonSerializer
				{
					Options =
						{
							AutoSerializeFields = true,
							CaseSensitiveDeserialization = false
						}
				};
			var parameters = serializer.Deserialize<RSAParameters>(publicKeyJson);

#if NET45
			using (var rsaCryptoServiceProvider = new RSACryptoServiceProvider())
			using (var sha1CryptoServiceProvider = new SHA1CryptoServiceProvider())
			{
				rsaCryptoServiceProvider.ImportParameters(parameters);

				valid = rsaCryptoServiceProvider.VerifyData(data, sha1CryptoServiceProvider, signature);
			}
#else
			using (var rsa = RSA.Create())
			{
				rsa.ImportParameters(parameters);

				valid = rsa.VerifyData(data, signature, HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1);
			}
#endif

			return valid;
		}
	}
}