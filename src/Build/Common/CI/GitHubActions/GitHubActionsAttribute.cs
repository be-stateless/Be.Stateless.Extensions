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
using Build.Common.CI.GitHubActions.Configuration;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.CI.GitHubActions.Configuration;
using Nuke.Common.Execution;

namespace Build.Common.CI.GitHubActions;

internal class GitHubActionsAttribute : Nuke.Common.CI.GitHubActions.GitHubActionsAttribute
{
	public GitHubActionsAttribute(string name, GitHubActionsImage image, params GitHubActionsImage[] images) : base(name, image, images) { }

	#region Base Class Member Overrides

	[return: NotNull]
	protected override GitHubActionsJob GetJobs(GitHubActionsImage image, IReadOnlyCollection<ExecutableTarget> relevantTargets)
	{
		var job = base.GetJobs(image, relevantTargets);
		var newSteps = new List<GitHubActionsStep>(job.Steps);
		newSteps.Insert(index: 1, new GitHubActionsStepSetupDotNet("10.0.100"));
		job.Steps = newSteps.ToArray();
		return job;
	}

	#endregion
}
