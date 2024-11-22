﻿// ReSharper disable UnusedMember.Global
namespace NoeticTools.Common.Interops.DotNet;

/// <summary>
///     .NET System.IO.File static members interop to enable unit testing.
/// </summary>
public interface IFiles
{
    bool Exists(string filePath);
}