﻿namespace NoeticTools.Git2SemVer.Core.Exceptions;

public class Git2SemVerGitOperationException : Git2SemverExceptionBase
{
    public Git2SemVerGitOperationException(string message) : base(message)
    {
    }

    // ReSharper disable once UnusedMember.Global
    public Git2SemVerGitOperationException(string message, Exception innerException) : base(message, innerException)
    {
    }
}