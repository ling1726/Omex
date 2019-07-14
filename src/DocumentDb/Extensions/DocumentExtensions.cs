﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Azure.Documents;
using Microsoft.Omex.System.Validation;
using Newtonsoft.Json;

namespace Microsoft.Omex.DocumentDb.Extensions
{
	/// <summary>
	/// Document extensions.
	/// </summary>
	public static class DocumentExtensions
	{
		/// <summary>
		/// Converts document to specified type.
		/// </summary>
		/// <param name="document">Document db document.</param>
		/// <param name="jsonSerializerSettings">Json serialization settings.</param>
		public static T ConvertTo<T>(
			this Document document, JsonSerializerSettings jsonSerializerSettings = null)
		{
			Code.ExpectsArgument(document, nameof(document), 0);

			return JsonConvert.DeserializeObject<T>(document.ToString(), jsonSerializerSettings);
		}
	}
}