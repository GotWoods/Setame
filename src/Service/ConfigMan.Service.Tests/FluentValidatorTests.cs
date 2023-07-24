using System.Reflection;
using ConfigMan.Data.Handlers.Applications;
using FluentValidation;
using Xunit.Abstractions;

namespace ConfigMan.Service.Tests;

public class ValidatorTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public ValidatorTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void Validate_AllValidatorsInAssembly_HaveUniqueErrorCodes()
    {
        var validators = GetValidatorsFromAssembly(typeof(CreateApplicationValidator).Assembly);

        var foundCodes = new List<string>();
        foreach (var validator in validators)
        foreach (var rule in (IEnumerable<IValidationRule>)validator)
        foreach (var component in rule.Components)
        {
            if (component.ErrorCode == null)
                Assert.Fail("no error code " + component.Validator.Name);
            foundCodes.Add(component.ErrorCode);
        }

        var duplicates = foundCodes.GroupBy(s => s)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key);

        foreach (var dup in duplicates)
        {
            Assert.Fail("Duplicate code " + dup + " was found");
        }
    }

    private IEnumerable<IValidator> GetValidatorsFromAssembly(Assembly assembly)
    {
        return assembly.GetTypes()
            .Where(t => t.BaseType?.IsGenericType == true &&
                        t.BaseType.GetGenericTypeDefinition() == typeof(AbstractValidator<>))
            .Select(Activator.CreateInstance)
            .Cast<IValidator>();
    }
}