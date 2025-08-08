using System.ComponentModel.DataAnnotations;
using ValidationException = Postkit.Shared.Exceptions.ValidationException;

namespace Postkit.Shared.Attributes
{
    public class ValidEnumValueAttribute : ValidationAttribute
    {
        private readonly Type enumType;

        public ValidEnumValueAttribute(Type enumType)
        {
            if (!enumType.IsEnum)
                throw new ValidationException("ValidEnumValueAttribute must be used with an enum type.");

            this.enumType = enumType;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            if (!System.Enum.IsDefined(enumType, value))
            {
                return new ValidationResult($"{value} is not a valid value for {enumType.Name}.");
            }

            return ValidationResult.Success;
        }
    }
}
