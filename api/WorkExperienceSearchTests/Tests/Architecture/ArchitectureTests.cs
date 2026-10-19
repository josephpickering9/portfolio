using System.Runtime.CompilerServices;
using NetArchTest.Rules;
using Xunit;

namespace WorkExperienceSearchTests.Tests.Architecture;

public class ArchitectureTests
{
    private const string MainAssembly = "WorkExperienceSearch";
    private const string ControllersNamespace = "Work_Experience_Search.Controllers";
    private const string ServicesNamespace = "Work_Experience_Search.Services";
    private const string RepositoriesNamespace = "Work_Experience_Search.Repositories";
    private const string RootNamespace = "Work_Experience_Search";
    private const string MigrationsNamespace = "Work_Experience_Search.Migrations";

    [Fact]
    public void Controllers_ShouldNotDependOn_Repositories()
    {
        var result = Types.InAssembly(typeof(Work_Experience_Search.Controllers.ProjectController).Assembly)
            .That().ResideInNamespace(ControllersNamespace)
            .ShouldNot().HaveDependencyOn(RepositoriesNamespace)
            .GetResult();

        Assert.True(result.IsSuccessful, $"Controllers must not reference repositories directly. Violations: {string.Join(", ", result.FailingTypeNames ?? [])}");
    }

    [Fact]
    public void Services_ShouldNotDependOn_Controllers()
    {
        var result = Types.InAssembly(typeof(Work_Experience_Search.Services.ProjectService).Assembly)
            .That().ResideInNamespace(ServicesNamespace)
            .ShouldNot().HaveDependencyOn(ControllersNamespace)
            .GetResult();

        Assert.True(result.IsSuccessful, $"Services must not reference controllers. Violations: {string.Join(", ", result.FailingTypeNames ?? [])}");
    }

    [Fact]
    public void Repositories_ShouldNotDependOn_Controllers()
    {
        var result = Types.InAssembly(typeof(Work_Experience_Search.Repositories.ProjectRepository).Assembly)
            .That().ResideInNamespace(RepositoriesNamespace)
            .ShouldNot().HaveDependencyOn(ControllersNamespace)
            .GetResult();

        Assert.True(result.IsSuccessful, $"Repositories must not reference controllers. Violations: {string.Join(", ", result.FailingTypeNames ?? [])}");
    }

    [Fact]
    public void Repositories_ShouldNotDependOn_Services()
    {
        var result = Types.InAssembly(typeof(Work_Experience_Search.Repositories.ProjectRepository).Assembly)
            .That().ResideInNamespace(RepositoriesNamespace)
            .And().AreNotAbstract()
            .ShouldNot().HaveDependencyOn("Work_Experience_Search.Services.Project")
            .GetResult();

        Assert.True(result.IsSuccessful, $"Repositories must not reference service implementations. Violations: {string.Join(", ", result.FailingTypeNames ?? [])}");
    }

    [Fact]
    public void ConcreteClasses_ShouldBeSealed()
    {
        var result = Types.InAssembly(typeof(Work_Experience_Search.Controllers.ProjectController).Assembly)
            .That().AreClasses()
            .And().AreNotAbstract()
            .And().ResideInNamespaceStartingWith(RootNamespace)
            .And().DoNotResideInNamespace(MigrationsNamespace)
            .And().DoNotHaveCustomAttribute(typeof(CompilerGeneratedAttribute))
            .Should().BeSealed()
            .GetResult();

        Assert.True(result.IsSuccessful, $"Classes not designed for inheritance must be sealed. Violations: {string.Join(", ", result.FailingTypeNames ?? [])}");
    }
}
