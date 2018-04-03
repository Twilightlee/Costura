﻿using System.Globalization;
using System.Threading;
using ApprovalTests;
using ApprovalTests.Namers;
using NUnit.Framework;

[TestFixture]
public class CultureResourceTests : BaseCosturaTest
{
    protected override string Suffix => "Culture";

    [OneTimeSetUp]
    public void CreateAssembly()
    {
        CreateIsolatedAssemblyCopy("ExeToProcess",
            "<Costura />",
            new[]
            {
                "AssemblyToReference.dll",
                "de\\AssemblyToReference.resources.dll",
                "fr\\AssemblyToReference.resources.dll",
                "AssemblyToReferencePreEmbedded.dll",
                "ExeToReference.exe"
            });
    }

    [Test]
    public void UsingResource()
    {
        var culture = Thread.CurrentThread.CurrentUICulture;
        try
        {
            Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture("fr-FR");
            LoadAssemblyIntoAppDomain();
            var instance1 = assembly.GetInstance("ClassToTest");
            Assert.AreEqual("Salut", instance1.InternationalFoo());
        }
        finally
        {
            Thread.CurrentThread.CurrentUICulture = culture;
        }
    }

    [Test]
    public void TemplateHasCorrectSymbols()
    {
        using (ApprovalResults.ForScenario(Suffix))
        {
            var text = Ildasm.Decompile(afterAssemblyPath, "Costura.AssemblyLoader");
            Approvals.Verify(text);
        }
    }
}