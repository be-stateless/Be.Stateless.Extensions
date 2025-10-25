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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Be.Stateless.Linq.Extensions;

[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "Public API.")]
[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Public API.")]
public static class EnumeratorExtensions
{
	/// <summary>Casts the elements of an <see cref="IEnumerator"/> to the specified type.</summary>
	/// <typeparam name="TResult">The type to cast the elements of source to.</typeparam>
	/// <param name="source">
	/// The <see cref="IEnumerator"/> that contains the elements to be cast to type <typeparamref name="TResult"/>
	/// .
	/// </param>
	/// <returns>
	/// An <see cref="IEnumerable{T}"/> that contains each element of the <paramref name="source"/> sequence cast to the
	/// specified type.
	/// </returns>
	public static IEnumerable<TResult?> Cast<TResult>(this IEnumerator source)
	{
		ArgumentNullException.ThrowIfNull(source);
		while (source.MoveNext()) yield return (TResult) source.Current;
	}
}
