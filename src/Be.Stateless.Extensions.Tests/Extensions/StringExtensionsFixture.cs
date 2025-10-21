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
using System.Xml.Serialization;
using Moq;
using Range = Moq.Range;

namespace Be.Stateless.Extensions;

[SuppressMessage("Naming", "CA1720:Identifier contains type name")]
[SuppressMessage("Style", "IDE1006:Naming Styles")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public class StringExtensionsFixture
{
	[Theory]
	[InlineData(null, 3, "")]
	[InlineData("", 3, "")]
	[InlineData("123456", 3, "123")]
	[InlineData("123456", 9, "123456")]
	[SuppressMessage("Usage", "xUnit1012:Null should only be used for nullable parameters")]
	public void ExtractSubstringTakesPrefix(string @string, int length, string expected)
	{
		@string.ExtractSubstring(length)
			.Should()
			.Be(expected);
	}

	[Theory]
	[InlineData(null, -3, "")]
	[InlineData("", -3, "")]
	[InlineData("123456", -3, "456")]
	[InlineData("123456", -9, "123456")]
	[SuppressMessage("Usage", "xUnit1012:Null should only be used for nullable parameters")]
	public void ExtractSubstringTakesSuffix(string @string, int length, string expected)
	{
		@string.ExtractSubstring(length)
			.Should()
			.Be(expected);
	}

	[Fact]
	public void IfNotNullOrEmptyDoesNotInvokeActionDelegate()
	{
		var @string = string.Empty;
		var mock = new Mock<Action<string>>();

		@string.IfNotNullOrEmpty(mock.Object);

		mock.Verify(a => a.Invoke(@string), Times.Never);
	}

	[Fact]
	public void IfNotNullOrEmptyDoesNotInvokeFunctionDelegate()
	{
		var @string = string.Empty;
		var mock = new Mock<Func<string, bool>>();
		mock.Setup(f => f.Invoke(@string))
			.Returns(value: true);

		@string.IfNotNullOrEmpty(mock.Object)
			.Should()
			.BeFalse();

		mock.Verify(a => a.Invoke(@string), Times.Never);
	}

	[Fact]
	public void IfNotNullOrEmptyInvokesActionDelegate()
	{
		const string @string = "anything";
		var mock = new Mock<Action<string>>();

		@string.IfNotNullOrEmpty(mock.Object);

		mock.Verify(static a => a.Invoke(@string), Times.Once);
	}

	[Fact]
	public void IfNotNullOrEmptyInvokesFunctionDelegate()
	{
		const string @string = "anything";
		var mock = new Mock<Func<string, bool>>();
		mock.Setup(static f => f.Invoke(@string))
			.Returns(value: true);

		@string.IfNotNullOrEmpty(mock.Object)
			.Should()
			.BeTrue();

		mock.Verify(static a => a.Invoke(@string), Times.Once);
	}

	[Fact]
	public void IfNotNullOrWhiteSpaceDoesNotInvokeActionDelegate()
	{
		const string @string = "   ";
		var mock = new Mock<Action<string>>();

		@string.IfNotNullOrWhiteSpace(mock.Object);

		mock.Verify(static a => a.Invoke(@string), Times.Never);
	}

	[Fact]
	public void IfNotNullOrWhiteSpaceDoesNotInvokeFunctionDelegate()
	{
		const string @string = "   ";
		var mock = new Mock<Func<string, bool>>();
		mock.Setup(static f => f.Invoke(@string))
			.Returns(value: true);

		@string.IfNotNullOrWhiteSpace(mock.Object)
			.Should()
			.BeFalse();

		mock.Verify(static a => a.Invoke(@string), Times.Never);
	}

	[Fact]
	public void IfNotNullOrWhiteSpaceInvokesActionDelegate()
	{
		const string @string = "anything";
		var mock = new Mock<Action<string>>();

		@string.IfNotNullOrWhiteSpace(mock.Object);

		mock.Verify(static a => a.Invoke(@string), Times.Once);
	}

	[Fact]
	public void IfNotNullOrWhiteSpaceInvokesFunctionDelegate()
	{
		const string @string = "anything";
		var mock = new Mock<Func<string, bool>>();
		mock.Setup(static f => f.Invoke(@string))
			.Returns(value: true);

		@string.IfNotNullOrWhiteSpace(mock.Object)
			.Should()
			.BeTrue();

		mock.Verify(static a => a.Invoke(@string), Times.Once);
	}

	[Theory]
	[InlineData(data: null)]
	[InlineData("")]
	[InlineData("/name.txt")]
	[SuppressMessage("Usage", "xUnit1012:Null should only be used for nullable parameters")]
	public void IsValidFileNameReturnsFalse(string @string)
	{
		@string.IsValidFileName()
			.Should()
			.BeFalse();
	}

	[Theory]
	[InlineData(@"\name.txt")]
	[SuppressMessage("Usage", "xUnit1012:Null should only be used for nullable parameters")]
	public void IsValidFileNameReturnsFalseOnWindows(string @string)
	{
		if (OperatingSystem.IsWindows())
			@string.IsValidFileName()
				.Should()
				.BeFalse();
	}

	[Theory]
	[InlineData("name")]
	[InlineData("name.txt")]
	[InlineData("name-0.txt")]
	[InlineData("name_0.txt")]
	public void IsValidFileNameReturnsTrue(string @string)
	{
		@string.IsValidFileName()
			.Should()
			.BeTrue();
	}

	[Theory]
	[InlineData("Exclusive", Range.Exclusive)]
	[InlineData("EXCLUSIVE", Range.Exclusive)]
	public void ParseEnumLabel(string value, Range expected)
	{
		value.Parse<Range>()
			.Should()
			.Be(expected);
	}

	[Fact]
	public void ParseEnumThrowsWhenLabelIsUnknown()
	{
		Invoking(static () => "Unknown".Parse<Range>())
			.Should()
			.Throw<ArgumentException>()
			.WithMessage("Requested value 'Unknown' was not found.");
	}

	[Fact]
	public void UnlessIsNullOrEmptyDoesNotThrow()
	{
		const string value = "anything";
		value.UnlessIsNullOrEmpty()
			.Should()
			.Be(value);
	}

	[Fact]
	public void UnlessIsNullOrEmptyThrows()
	{
		var name = string.Empty;
		Invoking(() => name.UnlessIsNullOrEmpty())
			.Should()
			.Throw<InvalidOperationException>()
			.WithMessage($"{nameof(name)} cannot be null or an empty string.");
	}

	[Fact]
	public void UnlessIsNullOrEmptyThrowsWithContext()
	{
		var xmlRootAttribute = new XmlRootAttribute();
		Invoking(() => xmlRootAttribute.Namespace.UnlessIsNullOrEmpty("An XML contract must have a namespace."))
			.Should()
			.Throw<InvalidOperationException>()
			.WithMessage("xmlRootAttribute.Namespace cannot be null or an empty string." + Environment.NewLine + "An XML contract must have a namespace.");
	}
}
