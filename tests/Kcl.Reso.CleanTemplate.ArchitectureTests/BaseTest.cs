using System.Reflection;
using Kcl.Reso.CleanTemplate.Application.Abstractions.Messaging;
using Kcl.Reso.CleanTemplate.Domain.Users;
using Kcl.Reso.CleanTemplate.Infrastructure.Database;
using Kcl.Reso.CleanTemplate.Web.Api;

namespace Kcl.Reso.CleanTemplate.ArchitectureTests;

public abstract class BaseTest
{
    protected static readonly Assembly DomainAssembly = typeof(User).Assembly;
    protected static readonly Assembly ApplicationAssembly = typeof(ICommand).Assembly;
    protected static readonly Assembly InfrastructureAssembly = typeof(ApplicationDbContext).Assembly;
    protected static readonly Assembly PresentationAssembly = typeof(Program).Assembly;
}
