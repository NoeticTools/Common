﻿using NoeticTools.Git2SemVer.Core.Tools.Git.FluentApi;


namespace NoeticTools.Git2SemVer.Core.Tools.Git;

public interface IGitTool
{
    /// <summary>
    ///     The current head's branch name.
    /// </summary>

    string BranchName { get; }

    /// <summary>
    ///     True if there are uncommited local changes.
    /// </summary>
    bool HasLocalChanges { get; }

    ///// <summary>
    /////     Get commits working from head downwards (to older commits).
    ///// </summary>
    //IReadOnlyList<Commit> GetCommits(int skipCount, int takeCount);

    /// <summary>
    ///     Invoke Git with given arguments.
    /// </summary>
    string Run(string arguments);

    Commit Head { get; }

    Commit Get(CommitId commitId);

    Commit Get(string commitSha);

    /// <summary>
    ///     Get all commits contributing to code at a commit after a prior commit.
    /// </summary>
    IReadOnlyList<Commit> GetContributingCommits(CommitId head, CommitId prior);

    /// <summary>
    ///     Get commits starting with given commit SHA and then parent (older) commits.
    /// </summary>
    IReadOnlyList<Commit> GetCommits(string commitSha, int? takeCount=null);

    /// <summary>
    ///     Get commits working from head downwards (to older commits).
    /// </summary>
    IReadOnlyList<Commit> GetCommits(int skipCount, int takeCount);

    /// <summary>
    ///     Get commits in range given by range builder.
    /// </summary>
    IReadOnlyList<Commit> GetCommits(Action<ICommitsRangeBuilder> rangeBuilder);

    /// <summary>
    ///     Get commits in range given by given commit ranges.
    /// </summary>
    IReadOnlyList<Commit> GetCommitsInRange(params string[] commitRanges);
}