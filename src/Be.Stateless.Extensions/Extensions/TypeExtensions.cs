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
using System.Linq;

namespace Be.Stateless.Extensions;

[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "Public API.")]
[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Public API.")]
public static class TypeExtensions
{
	/// <summary>Determines whether a type is a subclass of a generic type, considering both class and interface inheritance.</summary>
	/// <param name="type">The type to check for subclass relationship.</param>
	/// <param name="baseType">The generic base type to compare against.</param>
	/// <returns>
	/// <see langword="true"/> if <paramref name="type"/> is a subclass of <paramref name="baseType"/>; otherwise,
	/// <see langword="false"/>.
	/// </returns>
	/// <exception cref="ArgumentNullException">Thrown when either <paramref name="type"/> or <paramref name="baseType"/> is null.</exception>
	/// <remarks>
	/// This method handles various scenarios including: <list type="bullet">
	/// <item><description>Direct subclass relationships</description></item>
	/// <item><description>Generic interface inheritance</description></item>
	/// <item>
	/// <description>Closed and open generic types Comparison is performed against the generic type definition</description>
	/// </item>
	/// </list>
	/// </remarks>
	/// <example>
	/// <code><![CDATA[
	/// bool isSubclass = typeof(MyClass).IsSubclassOfGenericType(typeof(GenericBaseClass<>));
	/// ]]></code>
	/// </example>
	public static bool IsSubclassOfGenericType(this Type type, Type baseType)
	{
		ArgumentNullException.ThrowIfNull(type);
		ArgumentNullException.ThrowIfNull(baseType);
		// http://stackoverflow.com/questions/457676/check-if-a-class-is-derived-from-a-generic-class
		if (ReferenceEquals(type, baseType) || !baseType.IsGenericType || baseType.IsConstructedGenericType) return type.IsSubclassOf(baseType);
		return !type.IsInterface && baseType.IsInterface
			? type.IsSubclassOfGenericInterface(baseType)
			: type.IsSubclassOfGenericClass(baseType);
	}

	/// <summary>Checks if the type inherits from a generic interface.</summary>
	/// <param name="type">The type to check for interface inheritance.</param>
	/// <param name="baseType">The generic interface type to compare against.</param>
	/// <returns>
	/// <see langword="true"/> if the type implements an interface derived from the generic base type; otherwise,
	/// <see langword="false"/>.
	/// </returns>
	/// <remarks>Examines all interfaces implemented by the type to find a match with the generic base type.</remarks>
	private static bool IsSubclassOfGenericInterface(this Type type, Type baseType)
	{
		var interfaces = type.GetInterfaces();
		return interfaces.Any(i => i.IsSubclassOfGenericClass(baseType));
	}

	/// <summary>Recursively checks if a type is a subclass of a generic class.</summary>
	/// <param name="type">The type to check for class inheritance.</param>
	/// <param name="baseType">The generic base class type to compare against.</param>
	/// <returns><see langword="true"/> if the type is derived from the generic base class; otherwise, <see langword="false"/>.</returns>
	/// <remarks>
	/// Traverses the type hierarchy, comparing against the generic type definition. Stops at <see cref="object"/> or when no
	/// base type exists.
	/// </remarks>
	private static bool IsSubclassOfGenericClass(this Type? type, Type baseType)
	{
		while (type is not null && type != typeof(object))
		{
			if (type.IsGenericType) type = type.GetGenericTypeDefinition();
			if (type == baseType) return true;
			type = type.BaseType;
		}
		return false;
	}
}
