using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Web;
using System.Web.Mvc;

namespace ASE.MVC
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class LocalizedCompareAttribute : ValidationAttribute, IClientValidatable
    {
        public static ResourceManager ResourceManager { get; set; }
        public static string ResourceName = "CompareAttribute";

        public LocalizedCompareAttribute(string otherProperty)
            : base()
        {
            OtherProperty = otherProperty;
        }

        public string OtherProperty { get; private set; }

        public string OtherPropertyDisplayName { get; internal set; }

        public override string FormatErrorMessage(string name)
        {
            var FormatString = ResourceManager.GetString(ResourceName);
            if (FormatString != null)
                return String.Format(CultureInfo.CurrentCulture, FormatString, name, OtherPropertyDisplayName ?? OtherProperty);

            return String.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, OtherPropertyDisplayName ?? OtherProperty);
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            PropertyInfo otherPropertyInfo = validationContext.ObjectType.GetProperty(OtherProperty);
            if (otherPropertyInfo == null)
            {
                return new ValidationResult(String.Format(CultureInfo.CurrentCulture, "1111", OtherProperty));
            }

            object otherPropertyValue = otherPropertyInfo.GetValue(validationContext.ObjectInstance, null);
            if (!Equals(value, otherPropertyValue))
            {
                if (OtherPropertyDisplayName == null)
                {
                    OtherPropertyDisplayName = ModelMetadataProviders.Current.GetMetadataForProperty(() => validationContext.ObjectInstance, validationContext.ObjectType, OtherProperty).GetDisplayName();
                }
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }
            return null;
        }

        public static string FormatPropertyForClientValidation(string property)
        {
            if (property == null)
            {
                throw new ArgumentException("2222", "property");
            }
            return "*." + property;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            if (metadata.ContainerType != null)
            {
                if (OtherPropertyDisplayName == null)
                {
                    OtherPropertyDisplayName = ModelMetadataProviders.Current.GetMetadataForProperty(() => metadata.Model, metadata.ContainerType, OtherProperty).GetDisplayName();
                }
            }
            yield return new ModelClientValidationEqualToRule(FormatErrorMessage(metadata.GetDisplayName()), FormatPropertyForClientValidation(OtherProperty));
        }
    }
}