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

namespace Be.Stateless.IO.Extensions;

public class PathExtensionsFixture
{
	[Fact]
	[SuppressMessage("ReSharper", "RedundantStringFormatCall")]
	public void CommonFolderPath()
	{
		var paths = new[] {
			@"c/a/b/c/d/e/f",
			"c/a/b/c/d/k",
			"c/a/b/c"
		};
		paths.GetCommonPath()
			.Should()
			.Be(string.Format("c{0}a{0}b{0}c", Path.DirectorySeparatorChar));
	}

	[Fact]
	public void CommonFolderPathOnWindows()
	{
		var paths = new[] {
			@"c\a\b/c\d\e\f",
			"c/a/b/c/d/k",
			"c/a/b/c"
		};
		if (OperatingSystem.IsWindows())
			paths.GetCommonPath()
				.Should()
				.Be(@"c\a\b\c");
	}

	[Fact]
	public void CommonPath()
	{
		var paths = new[] {
			"a.b.c.d.e.f",
			"a.b.c.d.k",
			"a.b.c"
		};
		paths.GetCommonPath('.')
			.Should()
			.Be("a.b.c");
	}

	[Fact]
	public void CommonPathNonexistent()
	{
		var paths = new[] {
			"a.b.c.d.e.f",
			"a.b.c.d.k",
			"a.b.c",
			"x.y.z"
		};
		paths.GetCommonPath('.')
			.Should()
			.BeEmpty();
	}

	[Fact]
	public void CommonPathNonexistentToo()
	{
		var paths = new[] {
			"a.b.c.d.e.f",
			"x.y.z",
			"m.n.o.p"
		};
		paths.GetCommonPath('.')
			.Should()
			.BeEmpty();
	}

	[Fact]
	public void CommonPathOfEmptyArray()
	{
		var paths = Array.Empty<string>();
		paths.GetCommonPath('.')
			.Should()
			.BeEmpty();
	}

	[Fact]
	[SuppressMessage("ReSharper", "ExpressionIsAlwaysNull")]
	[SuppressMessage("ReSharper", "NullableWarningSuppressionIsUsed")]
	public void CommonPathOfNullArray()
	{
		string[] paths = null!;
		paths.GetCommonPath('.')
			.Should()
			.BeEmpty();
	}

	[Fact]
	public void CommonPathOfSingletonArray()
	{
		var paths = new[] {
			"a.b.c.d.e.f"
		};
		paths.GetCommonPath('.')
			.Should()
			.Be("a.b.c.d.e.f");
	}
}
