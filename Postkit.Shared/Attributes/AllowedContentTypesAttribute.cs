using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Postkit.Shared.Attributes
{
    public class AllowedContentTypesAttribute : ValidationAttribute
    {
        private readonly string[] contentTypes;

        public AllowedContentTypesAttribute(string[] contentTypes)
        {
            this.contentTypes = contentTypes;
        }
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var file = value as IFormFile;
            if (file != null)
            {
                if (!contentTypes.Contains(file.ContentType.ToLower()))
                {
                    return new ValidationResult($"Invalid file type. Allowed types: {string.Join(", ", contentTypes)}");
                }
            }

            return ValidationResult.Success;
        }
    }
}
