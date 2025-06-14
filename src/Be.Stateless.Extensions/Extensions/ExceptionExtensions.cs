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
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace Be.Stateless.Extensions;

[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "Public API.")]
public static class ExceptionExtensions
{
	/// <summary>Determines whether an exception is fatal, that is to say <b>unrecoverable</b>, or not.</summary>
	/// <param name="exception">The exception to assess.</param>
	/// <returns><c>true</c> if <paramref name="exception"/> is fatal; <c>false</c> otherwise.</returns>
	/// <seealso href="https://vasters.com/archive/Are-You-Catching-Falling-Knives.html"/>
	[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Public API.")]
	public static bool IsFatal(this Exception exception)
	{
		// @formatter:keep_existing_arrangement true
		var currentException = exception;
		while (currentException is not null)
		{
			if (currentException is OutOfMemoryException and not InsufficientMemoryException
				or AccessViolationException
				or BadImageFormatException
				or SEHException
				or StackOverflowException
				or ThreadAbortException)
				return true;
			if (currentException is not TypeInitializationException and not TargetInvocationException) break;
			currentException = currentException.InnerException;
		}
		return false;
	}

	/// <summary>
	/// Rethrows an <paramref name="exception"/>, maintaining the original Watson information and augmenting rather than
	/// replacing the original stack trace.
	/// </summary>
	/// <param name="exception">The exception to be rethrown.</param>
	/// <seealso cref="ExceptionDispatchInfo.Capture">ExceptionDispatchInfo.Capture</seealso>
	/// <seealso cref="ExceptionDispatchInfo.Throw()">ExceptionDispatchInfo.Throw</seealso>
	/// <seealso href="https://stackoverflow.com/a/17091351/1789441">How to rethrow InnerException without losing stack trace in C#?</seealso>
	[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Public API.")]
	public static void Rethrow(this Exception exception)
	{
		// @formatter:wrap_chained_method_calls wrap_if_long
		ExceptionDispatchInfo.Capture(exception).Throw();
	}
}
