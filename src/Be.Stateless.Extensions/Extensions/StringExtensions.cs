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
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace Be.Stateless.Extensions;

[SuppressMessage("Naming", "CA1720:Identifier contains type name")]
[SuppressMessage("ReSharper", "MemberCanBeInternal", Justification = "Public API.")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Public API.")]
[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Public API.")]
[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "Public API.")]
public static class StringExtensions
{
	/// <summary>
	/// Extract the first or last <c>length</c> characters of a string, whether <c>length</c> is respectively positive or
	/// negative.
	/// </summary>
	/// <param name="string">The string to extract characters of.</param>
	/// <param name="length">The number of characters to extract.</param>
	/// <returns>The substring of the input string.</returns>
	/// <remarks>
	/// Returns an empty string if the input string is null or empty. If the length of the input string is less than
	/// <c>length</c>, the whole string is returned.
	/// </remarks>
	public static string ExtractSubstring(this string? @string, int length)
	{
		if (@string.IsNullOrEmpty()) return string.Empty;
		return Math.Abs(length) > @string.Length ? @string : length < 0 ? @string[^-length..] : @string[..length];
	}

	/// <summary>Performs an <see cref="Action{T}"/> delegate on the <paramref name="string"/> if it is not null nor empty.</summary>
	/// <param name="string">The string to test and to pass as argument to the <paramref name="action"/> delegate.</param>
	/// <param name="action">The <see cref="Action{T}"/> delegate to perform.</param>
	public static void IfNotNullOrEmpty(this string? @string, Action<string> action)
	{
		ArgumentNullException.ThrowIfNull(action);
		if (!@string.IsNullOrEmpty()) action(@string);
	}

	/// <summary>
	/// Performs an <see cref="Func{TResult}"/> delegate on the <paramref name="string"/> if it is not null nor empty and
	/// returns the <paramref name="function"/> delegate value.
	/// </summary>
	/// <typeparam name="TR">The return type of the <see cref="Func{TResult}"/> delegate.</typeparam>
	/// <param name="string">The string to test and to pass as argument to the <paramref name="function"/> delegate.</param>
	/// <param name="function">The <see cref="Func{TResult}"/> delegate to perform.</param>
	/// <returns>
	/// The result of the <paramref name="function"/> delegate, or <c>default(TR)</c> if the <paramref name="string"/> is null
	/// or empty.
	/// </returns>
	[return: NotNullIfNotNull(nameof(@string))]
	public static TR? IfNotNullOrEmpty<TR>(this string? @string, Func<string, TR> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return @string.IsNullOrEmpty()
			? default
			: function(@string);
	}

	/// <summary>Performs an <see cref="Action{T}"/> delegate on the <paramref name="string"/> if it is not null or white space.</summary>
	/// <param name="string">The string to test and to pass as argument to the <paramref name="action"/> delegate.</param>
	/// <param name="action">The <see cref="Action{T}"/> delegate to perform.</param>
	public static void IfNotNullOrWhiteSpace(this string? @string, Action<string> action)
	{
		ArgumentNullException.ThrowIfNull(action);
		if (!@string.IsNullOrWhiteSpace()) action(@string);
	}

	/// <summary>
	/// Performs an <see cref="Func{TResult}"/> delegate on the <paramref name="string"/> if it is not null or white space and
	/// returns the <paramref name="function"/> delegate value.
	/// </summary>
	/// <typeparam name="TR">The return type of the <see cref="Func{TResult}"/> delegate.</typeparam>
	/// <param name="string">The string to test and to pass as argument to the <paramref name="function"/> delegate.</param>
	/// <param name="function">The <see cref="Func{TResult}"/> delegate to perform.</param>
	/// <returns>
	/// The result of the <paramref name="function"/> delegate, or <c>default(TR)</c> if the <paramref name="string"/> is null
	/// or white space.
	/// </returns>
	[return: NotNullIfNotNull(nameof(@string))]
	public static TR? IfNotNullOrWhiteSpace<TR>(this string? @string, Func<string, TR> function)
	{
		ArgumentNullException.ThrowIfNull(function);
		return @string.IsNullOrWhiteSpace()
			? default
			: function(@string);
	}

	/// <summary>Indicates whether the specified string is null or an <see cref="string.Empty"/> string.</summary>
	/// <param name="string">The string to test.</param>
	/// <returns>
	/// <c>true</c> if the <paramref name="string"/> argument is null or an <see cref="string.Empty"/>; otherwise,
	/// <c>false</c>.
	/// </returns>
	public static bool IsNullOrEmpty([NotNullWhen(returnValue: false)] this string? @string)
	{
		return string.IsNullOrEmpty(@string);
	}

	/// <summary>Indicates whether a specified string is null, empty, or consists only of white-space characters.</summary>
	/// <param name="string">The string to test.</param>
	/// <returns>
	/// <c>true</c> if the <paramref name="string"/> argument is null or an <see cref="string.Empty"/>, or if it consists
	/// exclusively of white-space characters; otherwise <c>false</c>.
	/// </returns>
	public static bool IsNullOrWhiteSpace([NotNullWhen(returnValue: false)] this string? @string)
	{
		return string.IsNullOrWhiteSpace(@string);
	}

	/// <summary>Verifies that the <paramref name="name"/> string is a valid file name.</summary>
	/// <param name="name">The file name to verify.</param>
	/// <returns><c>true</c> if it is a valid file name; <c>false</c> otherwise.</returns>
	public static bool IsValidFileName([NotNullWhen(returnValue: true)] this string? name)
	{
		return name.IfNotNullOrEmpty(static n => n.IndexOfAny(Path.GetInvalidFileNameChars()) < 0);
	}

	/// <summary>
	/// Converts the string representation of the name or numeric value of one or more enumerated constants to an equivalent
	/// enumerated object.
	/// </summary>
	/// <typeparam name="T">An enumeration type.</typeparam>
	/// <param name="value">A string containing the name or value to convert.</param>
	/// <param name="ignoreCase">Whether to ignore case or not; <c>true</c> by default.</param>
	/// <returns>One or more enumerated constants of type <typeparamref name="T"/> that represents <paramref name="value"/>.</returns>
	public static T Parse<T>(this string value, bool ignoreCase = true)
		where T : struct
	{
		return Enum.Parse<T>(value, ignoreCase);
	}

	/// <summary>Validates that a string is not null or empty, throwing an exception if the condition is not met.</summary>
	/// <param name="string">The string to validate.</param>
	/// <param name="context">Optional additional context or explanation for the validation failure.</param>
	/// <param name="expression">
	/// The expression representing the string parameter (automatically captured by the compiler). Used to
	/// provide a descriptive error message indicating the source of the null or empty string.
	/// </param>
	/// <returns>The original non-null and non-empty string.</returns>
	/// <exception cref="InvalidOperationException">Thrown when the input string is null or empty.</exception>
	/// <remarks>
	/// This method provides a concise way to validate string parameters, ensuring they have a meaningful value before further
	/// processing, with optional contextual information.
	/// </remarks>
	public static string UnlessIsNullOrEmpty([NotNull] this string? @string, string? context = null, [CallerArgumentExpression(nameof(@string))] string? expression = null)
	{
		if (!@string.IsNullOrEmpty()) return @string;

		// @formatter:wrap_chained_method_calls wrap_if_long
		var builder = new StringBuilder();
		builder.Append(expression).Append(" cannot be null or an empty string.");
		if (!context.IsNullOrEmpty()) builder.AppendLine().Append(context);
		throw new InvalidOperationException(builder.ToString());
	}
}
