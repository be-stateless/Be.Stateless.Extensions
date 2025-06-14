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

namespace Be.Stateless.Extensions;

public class TypeExtensionsFixture
{
	[Theory]
	[InlineData(typeof(Dummy), typeof(CompleteDummy<,>), false)]
	[InlineData(typeof(Dummy), typeof(Dummy), false)]
	[InlineData(typeof(Dummy), typeof(HalfDummy<>), false)]
	[InlineData(typeof(Dummy), typeof(NoDummy), true)]
	[InlineData(typeof(Dummy), typeof(IHalfDummy<>), false)]
	[InlineData(typeof(HalfDummy<>), typeof(IHalfDummy<>), false)]
	[InlineData(typeof(IHalfDummy<>), typeof(IHalfDummy<>), false)]
	[InlineData(typeof(IHalfDummy<int>), typeof(IHalfDummy<>), false)]
	public void IsSubclassOf(Type actual, Type baseType, bool result)
	{
		actual.IsSubclassOf(baseType)
			.Should()
			.Be(result);
	}

	[Theory]
	[InlineData(typeof(Dummy), typeof(CompleteDummy<,>), true)]
	[InlineData(typeof(Dummy), typeof(Dummy), false)]
	[InlineData(typeof(Dummy), typeof(HalfDummy<>), true)]
	[InlineData(typeof(Dummy), typeof(NoDummy), true)]
	[InlineData(typeof(Dummy), typeof(IHalfDummy<>), true)]
	[InlineData(typeof(HalfDummy<>), typeof(IHalfDummy<>), true)]
	[InlineData(typeof(IHalfDummy<>), typeof(IHalfDummy<>), false)]
	[InlineData(typeof(IHalfDummy<int>), typeof(IHalfDummy<>), true)]
	public void IsSubclassOfGenericType(Type actual, Type baseType, bool result)
	{
		actual.IsSubclassOfGenericType(baseType)
			.Should()
			.Be(result);
	}

	[SuppressMessage("ReSharper", "UnusedTypeParameter")]
	private interface IHalfDummy<T>;

	[SuppressMessage("ReSharper", "UnusedTypeParameter")]
	private class CompleteDummy<T1, T2>;

	private class HalfDummy<T> : CompleteDummy<T, int>, IHalfDummy<T>;

	private class NoDummy : HalfDummy<string>;

	[SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Used by InlineDataAttribute.")]
	private sealed class Dummy : NoDummy;
}
