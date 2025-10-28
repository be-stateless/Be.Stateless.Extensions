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

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Build.Common.CI.GitHubActions.Configuration;

internal class GitHubActionsStepSetupDotNet : GitHubActionsStepGeneric
{
	[SetsRequiredMembers]
	public GitHubActionsStepSetupDotNet(string version) : base("Setup .NET", "actions/setup-dotnet@v4")
	{
		With = new Dictionary<string, string> {
			["dotnet-version"] = version
		};
	}
}
