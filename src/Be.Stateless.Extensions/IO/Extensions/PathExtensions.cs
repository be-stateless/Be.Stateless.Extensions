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
using System.Linq;

namespace Be.Stateless.IO.Extensions;

[SuppressMessage("ReSharper", "ConvertToExtensionBlock")]
public static class PathExtensions
{
	/// <summary>Returns a string representing that part of the path tree that is common to all the paths.</summary>
	/// <param name="paths">The set of paths describing the path tree to extract the longest common path from.</param>
	/// <returns>The longest common path of the set of <paramref name="paths"/>.</returns>
	/// <remarks>
	/// The resultant path is a valid path and not just the longest common string; that is to say that no path segment will be
	/// truncated.
	/// </remarks>
	/// <seealso href="http://blogs.microsoft.co.il/yuvmaz/2013/05/10/longest-common-prefix-with-c-and-linq/">
	/// Longest Common Prefix
	/// with C# and LINQ
	/// </seealso>
	/// <seealso href="https://miafish.wordpress.com/2015/02/17/leetcode-oj-c-longest-common-prefix/">Longest Common Prefix</seealso>
	/// <seealso href="https://www.rosettacode.org/wiki/Find_common_directory_path">Find common directory path</seealso>
	/// <seealso href="https://stackoverflow.com/questions/2070356/find-common-prefix-of-strings">Find common prefix of strings</seealso>
	/// <seealso href="https://stackoverflow.com/questions/33709165/get-common-prefix-of-two-string">Get common prefix of two string</seealso>
	public static string GetCommonPath(this string[]? paths)
	{
		return GetCommonPath(paths, []);
	}

	/// <summary>Returns a string representing that part of the path tree that is common to all the paths.</summary>
	/// <param name="paths">The set of paths describing the path tree to extract the longest common path from.</param>
	/// <param name="separators">
	/// Path separators; it defaults to both <see cref="Path.DirectorySeparatorChar"/> and <c>, </c>
	/// <see cref="Path.AltDirectorySeparatorChar"/><c>]</c>.
	/// </param>
	/// <returns>The longest common path of the set of <paramref name="paths"/>.</returns>
	/// <remarks>
	/// The resultant path is a valid path and not just the longest common string; that is to say that no path segment will be
	/// truncated.
	/// </remarks>
	/// <seealso href="http://blogs.microsoft.co.il/yuvmaz/2013/05/10/longest-common-prefix-with-c-and-linq/">
	/// Longest Common Prefix
	/// with C# and LINQ
	/// </seealso>
	/// <seealso href="https://miafish.wordpress.com/2015/02/17/leetcode-oj-c-longest-common-prefix/">Longest Common Prefix</seealso>
	/// <seealso href="https://www.rosettacode.org/wiki/Find_common_directory_path">Find common directory path</seealso>
	/// <seealso href="https://stackoverflow.com/questions/2070356/find-common-prefix-of-strings">Find common prefix of strings</seealso>
	/// <seealso href="https://stackoverflow.com/questions/33709165/get-common-prefix-of-two-string">Get common prefix of two string</seealso>
	[SuppressMessage("Style", "IDE0305:Simplify collection initialization")]
	public static string GetCommonPath(this string[]? paths, params char[]? separators)
	{
		// @formatter:keep_existing_linebreaks true
		// @formatter:wrap_chained_method_calls wrap_if_long
		if (paths == null || paths.Length == 0) return string.Empty;
		if (separators == null || separators.Length == 0) separators = [Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar];
		var commonSegments = paths.Select(p => p.Split(separators, StringSplitOptions.RemoveEmptyEntries))
			.Aggregate(static (accumulatedCommonSegments, pathSegments) => accumulatedCommonSegments
				.TakeWhile((segment, i) => i < pathSegments.Length && pathSegments[i].Equals(segment, StringComparison.OrdinalIgnoreCase))
				.ToArray());
		// https://stackoverflow.com/questions/14897121/using-enumerable-aggregate-method-over-an-empty-sequence
		return string.Join(separators[0].ToString(), commonSegments);
	}
}
