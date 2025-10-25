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
using System.Runtime.CompilerServices;

namespace Be.Stateless.Extensions;

public static class ArgumentOutOfRangeExceptionFactory
{
	/// <summary>Throws an <see cref="System.ArgumentOutOfRangeException"/> if the value is not within the specified range.</summary>
	/// <typeparam name="T">The type of the value to be compared, which must implement <see cref="IComparable{T}"/>.</typeparam>
	/// <param name="value">The value to validate.</param>
	/// <param name="lowerBound">The lower bound of the acceptable range.</param>
	/// <param name="upperBound">The upper bound of the acceptable range.</param>
	/// <param name="paramName">The name of the parameter being validated. Automatically captured by the compiler.</param>
	/// <exception cref="System.ArgumentOutOfRangeException">
	/// Thrown when <paramref name="value"/> is less than
	/// <paramref name="lowerBound"/> or greater than <paramref name="upperBound"/>.
	/// </exception>
	/// <remarks>The method performs an inclusive range check, meaning the value can be equal to either the lower or upper bound.</remarks>
	public static void ThrowIfOutOfBounds<T>(T value, T lowerBound, T upperBound, [CallerArgumentExpression(nameof(value))] string? paramName = null)
		where T : IComparable<T>
	{
		if (value.CompareTo(lowerBound) < 0 || value.CompareTo(upperBound) > 0)
			throw new ArgumentOutOfRangeException(paramName, value, $"{paramName} ({value}) must be greater than or equal to '{lowerBound}' and be less than or equal to '{upperBound}'.");
	}
}
