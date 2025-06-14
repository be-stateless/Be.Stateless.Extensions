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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Be.Stateless.Linq.Extensions;

[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Public API.")]
[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "Public API.")]
[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Public API.")]
public static class EnumerableExtensions
{
	/// <summary>Returns distinct elements from a sequence by using a specified <paramref name="comparer"/> to compare values.</summary>
	/// <typeparam name="TSource">The type of the elements of source.</typeparam>
	/// <param name="source">The sequence to remove duplicate elements from.</param>
	/// <param name="comparer">A lambda to compare values for equality.</param>
	/// <returns>An <see cref="IEnumerable{TSource}"/> that contains distinct elements from the source sequence.</returns>
	public static IEnumerable<TSource> Distinct<TSource>(this IEnumerable<TSource> source, Func<TSource?, TSource?, bool> comparer)
	{
		return source.Distinct(EqualityComparer<TSource>.Create(comparer, static _ => 0));
	}

	/// <summary>Produces the set difference of two sequences by using the specified <paramref name="comparer"/> to compare values.</summary>
	/// <typeparam name="TSource">The type of the elements of the input sequences.</typeparam>
	/// <param name="first">An <see cref="IEnumerable{TSource}"/> whose elements that are not also in second will be returned.</param>
	/// <param name="second">
	/// An <see cref="IEnumerable{TSource}"/> whose elements that also occur in the first sequence will cause
	/// those elements to be removed from the returned sequence.
	/// </param>
	/// <param name="comparer">A lambda to compare values for equality.</param>
	/// <returns>A sequence that contains the set difference of the elements of two sequences.</returns>
	public static IEnumerable<TSource> Except<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second, Func<TSource?, TSource?, bool> comparer)
	{
		return first.Except(second, EqualityComparer<TSource>.Create(comparer, static _ => 0));
	}

	/// <summary>Calls an <see cref="Action{TSource}"/> delegate for each element of a sequence.</summary>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
	/// <param name="source">A sequence of values to invoke an action delegate on.</param>
	/// <param name="action">An <see cref="Action{TSource}"/> delegate to apply to each element.</param>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> or <paramref name="action"/> is null.</exception>
	public static void ForEach<TSource>(this IEnumerable<TSource> source, Action<TSource> action)
	{
		ArgumentNullException.ThrowIfNull(source);
		ArgumentNullException.ThrowIfNull(action);
		foreach (var element in source) action.Invoke(element);
	}

	/// <summary>Asynchronously calls an <see cref="Func{TSource, Task}"/> delegate for each element of a sequence.</summary>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
	/// <param name="source">A sequence of values to invoke an action delegate on.</param>
	/// <param name="action">An <see cref="Func{TSource, Task}"/> delegate to apply to each element.</param>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> or <paramref name="action"/> is null.</exception>
	public static async Task ForEachAsync<TSource>(this IEnumerable<TSource> source, Func<TSource, Task> action)
	{
		ArgumentNullException.ThrowIfNull(source);
		ArgumentNullException.ThrowIfNull(action);
		foreach (var item in source) await action(item);
	}

	/// <summary>
	/// Calls an <see cref="Action{Int32,TSource}"/> delegate for each element of a sequence by incorporating the element's
	/// index.
	/// </summary>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
	/// <param name="source">A sequence of values to invoke an action delegate on.</param>
	/// <param name="action">
	/// An <see cref="Action{Int32, TSource}"/> delegate to apply to each element; the first parameter of the
	/// action represents the index of the source element.
	/// </param>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> or <paramref name="action"/> is null.</exception>
	public static void ForEach<TSource>(this IEnumerable<TSource> source, Action<int, TSource> action)
	{
		ArgumentNullException.ThrowIfNull(source);
		ArgumentNullException.ThrowIfNull(action);
		var index = 0;
		foreach (var element in source) action.Invoke(index++, element);
	}

	/// <summary>
	/// Asynchronously calls an <see cref="Action{Int32,TSource}"/> delegate for each element of a sequence by incorporating
	/// the element's index.
	/// </summary>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
	/// <param name="source">A sequence of values to invoke an action delegate on.</param>
	/// <param name="action">
	/// An <see cref="Action{Int32, TSource}"/> delegate to apply to each element; the first parameter of the
	/// action represents the index of the source element.
	/// </param>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> or <paramref name="action"/> is null.</exception>
	public static async Task ForEachAsync<TSource>(this IEnumerable<TSource> source, Func<int, TSource, Task> action)
	{
		ArgumentNullException.ThrowIfNull(source);
		ArgumentNullException.ThrowIfNull(action);
		var index = 0;
		foreach (var item in source) await action(index++, item);
	}
}
