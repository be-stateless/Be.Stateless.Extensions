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
using Nuke.Common.CI.GitHubActions.Configuration;
using Nuke.Common.Utilities;

namespace Build.Common.CI.GitHubActions.Configuration;

internal abstract class GitHubActionsStepGeneric : GitHubActionsStep
{
	[SetsRequiredMembers]
	protected GitHubActionsStepGeneric(string name, string uses)
	{
		Name = name;
		Uses = uses;
	}

	#region Base Class Member Overrides

	public override void Write([JetBrains.Annotations.NotNull] CustomFileWriter writer)
	{
		writer.WriteLine($"- name: {Name}");
		using (writer.Indent())
		{
			writer.WriteLine($"uses: {Uses}");
			if (With.Count == 0) return;
			writer.WriteLine("with:");
			using (writer.Indent())
			{
				foreach (var (key, value) in With) writer.WriteLine($"{key}: {value}");
			}
		}
	}

	#endregion

	public required string Name { get; init; }

	public required string Uses { get; init; }

	public Dictionary<string, string> With { get; init; } = new();
}
