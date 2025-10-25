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

namespace Be.Stateless.Linq;

public class LambdaComparerFixture
{
#pragma warning disable CS0618 // Type or member is obsolete
	[Fact]
	[SuppressMessage("ReSharper", "NullableWarningSuppressionIsUsed")]
	public void EqualityWithCustomComparison()
	{
		var sut1 = new LambdaComparer<Tuple<int, int>>(static (t1, t2) => t1!.Item1 == t2!.Item1, static t => t.GetHashCode());
		sut1.Equals(new Tuple<int, int>(item1: 1, item2: 2), new Tuple<int, int>(item1: 1, item2: 3))
			.Should()
			.BeTrue();

		var sut2 = EqualityComparer<Tuple<int, int>>.Create(static (t1, t2) => t1!.Item1 == t2!.Item1, static t => t.GetHashCode());
		sut2.Equals(new Tuple<int, int>(item1: 1, item2: 2), new Tuple<int, int>(item1: 1, item2: 3))
			.Should()
			.BeTrue();
	}

	[Fact]
	public void EqualityWithDefaultComparison()
	{
		var sut = EqualityComparer<Tuple<int, int>>.Default;
		sut.Equals(new Tuple<int, int>(item1: 1, item2: 2), new Tuple<int, int>(item1: 1, item2: 3))
			.Should()
			.BeFalse();
	}

	[Fact]
	[SuppressMessage("ReSharper", "NullableWarningSuppressionIsUsed")]
	public void HashCodeWithCustomComputation()
	{
		var sut1 = new LambdaComparer<Tuple<int, int>>(static (t1, t2) => t1!.Item1 == t2!.Item1, static t => t.GetHashCode());
		sut1.GetHashCode(new Tuple<int, int>(item1: 1, item2: 2))
			.Should()
			.Be(EqualityComparer<Tuple<int, int>>.Default.GetHashCode(new Tuple<int, int>(item1: 1, item2: 2)));

		var sut2 = EqualityComparer<Tuple<int, int>>.Create(static (t1, t2) => t1!.Item1 == t2!.Item1, static t => t.GetHashCode());
		sut2.GetHashCode(new Tuple<int, int>(item1: 1, item2: 2))
			.Should()
			.Be(EqualityComparer<Tuple<int, int>>.Default.GetHashCode(new Tuple<int, int>(item1: 1, item2: 2)));
	}

	[Fact]
	[SuppressMessage("ReSharper", "NullableWarningSuppressionIsUsed")]
	public void HashCodeWithDefaultComputation()
	{
		var sut1 = new LambdaComparer<Tuple<int, int>>(static (t1, t2) => t1!.Item1 == t2!.Item1);
		sut1.GetHashCode(new Tuple<int, int>(item1: 1, item2: 2))
			.Should()
			.Be(expected: 0);

		var sut2 = EqualityComparer<Tuple<int, int>>.Create(static (t1, t2) => t1!.Item1 == t2!.Item1, static _ => 0);
		sut2.GetHashCode(new Tuple<int, int>(item1: 1, item2: 2))
			.Should()
			.Be(expected: 0);
	}
#pragma warning disable CS0618 // Type or member is obsolete
}
