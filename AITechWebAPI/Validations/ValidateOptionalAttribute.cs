using System.ComponentModel.DataAnnotations;
namespace AITechWebAPI.Validations
{
    public class ValidateOptionalAttribute : ValidationAttribute
    {
        private readonly string _pattern;

        public ValidateOptionalAttribute(string pattern)
        {
            _pattern = pattern;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (!string.IsNullOrEmpty(value.ToString()))
            {
                if (value is string stringValue && !System.Text.RegularExpressions.Regex.IsMatch(stringValue, _pattern))
                {
                    return new ValidationResult(ErrorMessage);
                }
            }

            return ValidationResult.Success;
        }
    }
}
