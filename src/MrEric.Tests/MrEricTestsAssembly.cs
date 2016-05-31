using NUnit.Framework;
using JetBrains.TestFramework;
using MrEric.Tests;

#pragma warning disable 618
[assembly: TestDataPathBase(@".\Data")]
#pragma warning restore 618

// ReSharper disable once CheckNamespace
[SetUpFixture]
public class MrEricTestsAssembly : ExtensionTestEnvironmentAssembly<MrEricTestEnvironmentZone> { }