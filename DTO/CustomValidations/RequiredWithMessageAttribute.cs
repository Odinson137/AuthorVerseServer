using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using Xunit.Sdk;

namespace AuthorVerseServer.DTO.CustomValidations
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class RequiredMessageAttribute : ValidationAttribute
    {
        public RequiredMessageAttribute(string errorMessage) : base(errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        public override bool IsValid(object value)
        {
            return value != null && !string.IsNullOrWhiteSpace(value.ToString());
        }
    }
}
