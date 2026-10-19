using System.Runtime.CompilerServices;
using NetArchTest.Rules;
using ArchTypes = NetArchTest.Rules.Types;
using Xunit;

namespace Portfolio.Api.Tests.Architecture;

public class ArchitectureTests
{
    private const string MainAssembly = "Portfolio.Api";
    private const string ControllersNamespace = "Portfolio.Api.Controllers";
    private const string ServicesNamespace = "Portfolio.Api.Services";
    private const string RepositoriesNamespace = "Portfolio.Api.Repositories";
    private const string RootNamespace = "Portfolio.Api";
    private const string MigrationsNamespace = "Portfolio.Api.Migrations";

    [Fact]
    public void Controllers_ShouldNotDependOn_Repositories()
    {
        var result = ArchTypes.InAssembly(typeof(Portfolio.Api.Controllers.ProjectController).Assembly)
            .That().ResideInNamespace(ControllersNamespace)
            .ShouldNot().HaveDependencyOn(RepositoriesNamespace)
            .GetResult();

        Assert.True(result.IsSuccessful, $"Controllers must not reference repositories directly. Violations: {string.Join(", ", result.FailingTypeNames ?? [])}");
    }

    [Fact]
    public void Services_ShouldNotDependOn_Controllers()
    {
        var result = ArchTypes.InAssembly(typeof(Portfolio.Api.Services.ProjectService).Assembly)
            .That().ResideInNamespace(ServicesNamespace)
            .ShouldNot().HaveDependencyOn(ControllersNamespace)
            .GetResult();

        Assert.True(result.IsSuccessful, $"Services must not reference controllers. Violations: {string.Join(", ", result.FailingTypeNames ?? [])}");
    }

    [Fact]
    public void Repositories_ShouldNotDependOn_Controllers()
    {
        var result = ArchTypes.InAssembly(typeof(Portfolio.Api.Repositories.ProjectRepository).Assembly)
            .That().ResideInNamespace(RepositoriesNamespace)
            .ShouldNot().HaveDependencyOn(ControllersNamespace)
            .GetResult();

        Assert.True(result.IsSuccessful, $"Repositories must not reference controllers. Violations: {string.Join(", ", result.FailingTypeNames ?? [])}");
    }

    [Fact]
    public void Repositories_ShouldNotDependOn_Services()
    {
        var result = ArchTypes.InAssembly(typeof(Portfolio.Api.Repositories.ProjectRepository).Assembly)
            .That().ResideInNamespace(RepositoriesNamespace)
            .And().AreNotAbstract()
            .ShouldNot().HaveDependencyOn("Portfolio.Api.Services.Project")
            .GetResult();

        Assert.True(result.IsSuccessful, $"Repositories must not reference service implementations. Violations: {string.Join(", ", result.FailingTypeNames ?? [])}");
    }

    [Fact]
    public void ConcreteClasses_ShouldBeSealed()
    {
        var result = ArchTypes.InAssembly(typeof(Portfolio.Api.Controllers.ProjectController).Assembly)
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
