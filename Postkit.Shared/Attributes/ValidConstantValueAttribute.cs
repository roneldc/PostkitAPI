using System.ComponentModel.DataAnnotations;
using System.Reflection;
namespace Postkit.Shared.Attributes
{
    public class ValidConstantValueAttribute : ValidationAttribute
    {
        private readonly Type constantClass;

        public ValidConstantValueAttribute(Type constantClass)
        {
            this.constantClass = constantClass;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is not string input || string.IsNullOrWhiteSpace(input))
            {
                return new ValidationResult($"{validationContext.MemberName} is required.");
            }

            var allowedValues = constantClass
                .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(f => f.IsLiteral && !f.IsInitOnly && f.FieldType == typeof(string))
                .Select(f => f.GetRawConstantValue() as string)
                .Where(v => v is not null)
                .ToList();

            if (!allowedValues.Contains(input.ToLower()))
            {
                return new ValidationResult(
                    $"Invalid {validationContext.MemberName}: '{input}'. Allowed values: {string.Join(", ", allowedValues)}.");
            }

            return ValidationResult.Success;
        }
    }
}
