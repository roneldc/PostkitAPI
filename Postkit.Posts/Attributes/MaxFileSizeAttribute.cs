using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Postkit.Shared.Attributes
{
    public class MaxFileSizeAttribute : ValidationAttribute
    {
        private readonly int maxFileSizeInBytes;

        public MaxFileSizeAttribute(int maxFileSizeInBytes)
        {
            this.maxFileSizeInBytes = maxFileSizeInBytes;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var file = value as IFormFile;
            if (file != null && file.Length > maxFileSizeInBytes)
            {
                return new ValidationResult($"Maximum allowed file size is {maxFileSizeInBytes / 1024} KB.");
            }

            return ValidationResult.Success;
        }
    }
}
