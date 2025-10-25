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

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Design", "CA1034:Nested types should not be visible", Justification = "Using nested classes is a common organizational approach in xUnit tests.")]
[assembly: SuppressMessage("Design", "CA1062:Validate arguments of public methods")]
[assembly: SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes")]
[assembly: SuppressMessage("Globalization", "CA1305:Specify IFormatProvider")]
[assembly: SuppressMessage("Maintainability", "CA1515:Consider making public types internal", Justification = "Unit requires test classes to be public")]
