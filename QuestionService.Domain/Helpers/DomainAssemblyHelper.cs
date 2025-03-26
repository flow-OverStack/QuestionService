using System.Reflection;

namespace QuestionService.Domain.Helpers;

public static class DomainAssemblyHelper
{
    private static readonly Assembly DomainAssembly = Assembly.GetExecutingAssembly();
    public static Assembly GetDomainAssembly() => DomainAssembly;
}