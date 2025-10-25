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
using System.Globalization;

namespace Be.Stateless.Extensions;

[SuppressMessage("ReSharper", "ConvertToConstant.Local")]
public class ArgumentOutOfRangeExceptionFactoryFixture
{
	[Fact]
	public void DoesNotThrowWhenValueIsBetweenLowerAndBounds()
	{
		var value = 5;
		Invoking(() => ArgumentOutOfRangeExceptionFactory.ThrowIfOutOfBounds(value, lowerBound: 1, upperBound: 9))
			.Should()
			.NotThrow();
	}

	[Fact]
	public void DoesNotThrowWhenValueIsEqualToLowerBound()
	{
		var value = 1;
		Invoking(() => ArgumentOutOfRangeExceptionFactory.ThrowIfOutOfBounds(value, lowerBound: 1, upperBound: 9))
			.Should()
			.NotThrow();
	}

	[Fact]
	public void DoesNotThrowWhenValueIsEqualToUpperBound()
	{
		var value = 9;
		Invoking(() => ArgumentOutOfRangeExceptionFactory.ThrowIfOutOfBounds(value, lowerBound: 1, upperBound: 9))
			.Should()
			.NotThrow();
	}

	[Fact]
	public void ThrowsForDoubles()
	{
		var value = 5.5d;
		Invoking(() => ArgumentOutOfRangeExceptionFactory.ThrowIfOutOfBounds(value, lowerBound: 0.3, upperBound: 4.9))
			.Should()
			.ThrowExactly<ArgumentOutOfRangeException>();
	}

	[Fact]
	public void ThrowsWhenValueIsGreaterThanUpperBound()
	{
		var value = 10;
		Invoking(() => ArgumentOutOfRangeExceptionFactory.ThrowIfOutOfBounds(value, lowerBound: 1, upperBound: 9))
			.Should()
			.ThrowExactly<ArgumentOutOfRangeException>()
			.WithMessage("value (10) must be greater than or equal to '1' and be less than or equal to '9'. (Parameter 'value')\r\nActual value was 10.");
	}

	[Fact]
	public void ThrowsWhenValueIsLessThanLowerBound()
	{
		var value = 0;
		Invoking(() => ArgumentOutOfRangeExceptionFactory.ThrowIfOutOfBounds(value, lowerBound: 1, upperBound: 9))
			.Should()
			.ThrowExactly<ArgumentOutOfRangeException>()
			.WithMessage("value (0) must be greater than or equal to '1' and be less than or equal to '9'. (Parameter 'value')\r\nActual value was 0.");

		Invoking(static () => ArgumentOutOfRangeExceptionFactory.ThrowIfOutOfBounds(value: 0, lowerBound: 1, upperBound: 9))
			.Should()
			.ThrowExactly<ArgumentOutOfRangeException>()
			.WithMessage("0 (0) must be greater than or equal to '1' and be less than or equal to '9'. (Parameter '0')\r\nActual value was 0.");
	}

	[Fact]
	public void ThrowsWithCustomComparableType()
	{
		var value = new ComparableDummy(value: 1);
		var lowerBound = new ComparableDummy(value: 3);
		var upperBound = new ComparableDummy(value: 5);

		Invoking(() => ArgumentOutOfRangeExceptionFactory.ThrowIfOutOfBounds(value, lowerBound, upperBound))
			.Should()
			.ThrowExactly<ArgumentOutOfRangeException>()
			.WithMessage(
				"value (ComparableDummy (1)) must be greater than or equal to 'ComparableDummy (3)' and be less than or equal to 'ComparableDummy (5)'. (Parameter 'value')\r\nActual value was ComparableDummy (1).");
	}

	private sealed class ComparableDummy(decimal value) : IComparable<ComparableDummy>
	{
		#region IComparable<ComparableDummy> Members

		public int CompareTo(ComparableDummy? other)
		{
			return other == null
				? 1
				: Value.CompareTo(other.Value);
		}

		#endregion

		#region Base Class Member Overrides

		public override string ToString()
		{
			return $"{nameof(ComparableDummy)} ({Value.ToString(CultureInfo.InvariantCulture)})";
		}

		#endregion

		private decimal Value { get; } = value;
	}
}
