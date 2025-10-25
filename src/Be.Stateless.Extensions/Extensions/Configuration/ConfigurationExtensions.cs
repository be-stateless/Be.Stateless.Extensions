#region Copyright & License

// Copyright © 2012-2025 François Chabot
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#endregion

using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;

namespace Be.Stateless.Extensions.Configuration;

[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "Public API.")]
[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Public API.")]
public static class ConfigurationExtensions
{
	/// <summary>Retrieves a required connection string from the configuration, throwing an exception if not defined.</summary>
	/// <param name="configuration">The configuration source to search for connection strings.</param>
	/// <param name="name">The connection string key.</param>
	/// <returns>The connection string.</returns>
	/// <exception cref="InvalidOperationException">Thrown when the connection string is not defined or is empty.</exception>
	public static string GetRequiredConnectionString(this IConfiguration configuration, string name)
	{
		return configuration.GetConnectionString(name)
			.UnlessIsNullOrEmpty($"Configuration does not define the connection string '{name}'.");
	}
}
