#region Copyright & License

// Copyright © 2012 - 2025 François Chabot
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
using System.Threading.Tasks;
using Moq;

namespace Be.Stateless.Linq.Extensions;

public class EnumerableExtensionsFixture
{
	[Fact]
	[SuppressMessage("ReSharper", "CoVariantArrayConversion")]
	public void Distinct()
	{
		(int, int)[] source = [(1, 2), (2, 2), (3, 2), (4, 2)];

		// equality on Tuple.Item1
		source.Distinct(static (t1, t2) => t1.Item1 == t2.Item1)
			.Should()
			.BeEquivalentTo(source);
		// equality on Tuple.Item2
		source.Distinct(static (t1, t2) => t1.Item2 == t2.Item2)
			.Should()
			.BeEquivalentTo([(1, 2)]);
	}

	[Fact]
	[SuppressMessage("ReSharper", "CoVariantArrayConversion")]
	public void Except()
	{
		(int, int)[] one = [(1, 2), (2, 3), (3, 4), (4, 5)];
		(int, int)[] two = [(1, 1), (2, 1), (3, 1), (4, 1)];

		// equality on Tuple.Item1
		one.Except(two, static (t1, t2) => t1.Item1 == t2.Item1)
			.Should()
			.BeEmpty();
		// equality on Tuple.Item2
		one.Except(two, static (t1, t2) => t1.Item2 == t2.Item2)
			.Should()
			.BeEquivalentTo(one);
	}

	[Fact]
	public void ForEach()
	{
		(int, int)[] source = [(1, 4), (2, 5), (3, 6)];
		var actionMock = new Mock<Action<(int, int)>>();

		source.ForEach(actionMock.Object);

		actionMock.Verify(a => a.Invoke(source[0]));
		actionMock.Verify(a => a.Invoke(source[1]));
		actionMock.Verify(a => a.Invoke(source[2]));
	}

	[Fact]
	public async Task ForEachAsync()
	{
		(int, int)[] source = [(1, 4), (2, 5), (3, 6)];
		var actionMock = new Mock<Func<(int, int), Task>>();

		await source.ForEachAsync(actionMock.Object);

		actionMock.Verify(a => a.Invoke(source[0]));
		actionMock.Verify(a => a.Invoke(source[1]));
		actionMock.Verify(a => a.Invoke(source[2]));
	}

	[Fact]
	public async Task ForEachAsyncIncorporatesElementIndex()
	{
		(int, int)[] source = [(1, 4), (2, 5), (3, 6)];
		var actionMock = new Mock<Func<int, (int, int), Task>>();

		await source.ForEachAsync(actionMock.Object);

		actionMock.Verify(a => a.Invoke(0, source[0]));
		actionMock.Verify(a => a.Invoke(1, source[1]));
		actionMock.Verify(a => a.Invoke(2, source[2]));
	}

	[Fact]
	public void ForEachIncorporatesElementIndex()
	{
		(int, int)[] source = [(1, 4), (2, 5), (3, 6)];
		var actionMock = new Mock<Action<int, (int, int)>>();

		source.ForEach(actionMock.Object);

		actionMock.Verify(a => a.Invoke(0, source[0]));
		actionMock.Verify(a => a.Invoke(1, source[1]));
		actionMock.Verify(a => a.Invoke(2, source[2]));
	}
}
