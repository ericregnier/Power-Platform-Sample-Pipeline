using Microsoft.Xrm.Sdk;
using System;

namespace Sample.Dataverse.Plugins.Services
{
    internal static class PluginImageExtensions
    {
        public const string PreEntityImageName = "PreImage";
        public const string PostEntityImageName = "PostImage";

        /// <summary>
        /// This utility method can be used to find the final attribute of an entity in a given time.
        /// If the attribute is being updated it will retrieve the updating version of the attribute, otherwise it will use the value of the pre-image
        /// </summary>
        internal static object GetFinalAttributeValue(this IExecutionContext context, string attributeName, string preEntityImageName = PreEntityImageName)
        {
            return context.GetFinalAttributeValue(attributeName, out _, preEntityImageName);
        }

        internal static object GetFinalAttributeValue(this IExecutionContext context, string attributeName, out bool gettingSet, string preEntityImageName = PreEntityImageName)
        {
            object contextAtt = context.GetContextAttributeValue(attributeName, out gettingSet);
            if (gettingSet)
            {
                return contextAtt;
            }

            return context.GetPreImageAttributeValue(attributeName, preEntityImageName);
        }

        internal static object GetContextAttributeValue(this IExecutionContext context, string attributeName, out bool gettingSet)
        {
            // Use gettingSet boolean because if the value is getting set to null there is no other way to tell whether
            // the attribute is not getting set (not in the context) or whether it is getting set to null explicitly.
            gettingSet = false;

            if (context.InputParameters.ContainsKey("Target"))
            {
                var newVersion = context.InputParameters["Target"] as Entity;
                if (newVersion != null && newVersion.Contains(attributeName))
                {
                    // The attribute is being updated, use the new value
                    gettingSet = true;
                    return newVersion[attributeName];
                }
            }
            else if (attributeName.Equals("statecode", StringComparison.InvariantCultureIgnoreCase) && context.InputParameters.Contains("State"))
            {
                gettingSet = true;
                return context.InputParameters["State"];
            }
            else if (attributeName.Equals("statuscode", StringComparison.InvariantCultureIgnoreCase) && context.InputParameters.Contains("Status"))
            {
                gettingSet = true;
                return context.InputParameters["Status"];
            }
            return null;
        }

        internal static bool IsEntityReferenceImageEqual(EntityReference e1, EntityReference e2)
        {
            if (e1 == null && e2 == null)
            {
                return true;
            }
            if (e1 == null || e2 == null)
            {
                return false;
            }

            return e1.Id == e2.Id;
        }

        internal static bool IsOptionSetImageEqual(OptionSetValue o1, OptionSetValue o2)
        {
            if (o1 == null && o2 == null)
            {
                return true;
            }
            if (o1 == null || o2 == null)
            {
                return false;
            }
            return o1.Value == o2.Value;
        }

        internal static bool IsAttributeImageEqual<T>(this IExecutionContext context, string attributeName, string preEntityImageName = PreEntityImageName)
        {
            var preValue = context.GetPreImageAttributeValue(attributeName, preEntityImageName);
            var finalValue = context.GetFinalAttributeValue(attributeName, preEntityImageName);

            if (typeof(T) == typeof(OptionSetValue))
                return IsOptionSetImageEqual((OptionSetValue)preValue, (OptionSetValue)finalValue);
            if (typeof(T) == typeof(EntityReference))
                return IsEntityReferenceImageEqual((EntityReference)preValue, (EntityReference)finalValue);

            return preValue == finalValue;
        }

        internal static bool IsStringImageEqual(string s1, string s2)
        {
            // In Dataverse pipeline null is treated as string.Empty sometimes, so treat these equally.
            // For example, when setting string field to null, the plugin will actually catch "" value in the attributes
            if (s1 == null && s2 == null)
            {
                return true;
            }
            if (s1 == null && s2 == string.Empty)
            {
                return true;
            }
            if (s1 == string.Empty && s2 == null)
            {
                return true;
            }

            return s1 == s2;
        }

        internal static bool IsValueDifferent(object attributeValue, object originalValue)
        {
            if (attributeValue == null)
            {
                if (originalValue == null)
                {
                    return false;
                }
                return true;
            }

            if (attributeValue is string stringNewValue)
            {
                var stringOldValue = originalValue as string;
                return !IsStringImageEqual(stringOldValue, stringNewValue);
            }

            if (attributeValue is OptionSetValue osvNewValue)
            {
                var osvOldValue = originalValue as OptionSetValue;
                return !IsOptionSetImageEqual(osvOldValue, osvNewValue);
            }

            if (attributeValue is EntityReference erNewValue)
            {
                var erOldValue = originalValue as EntityReference;
                return !IsEntityReferenceImageEqual(erOldValue, erNewValue);
            }

            var iNewValue = attributeValue as int?;
            if (iNewValue != null)
            {
                var iOldValue = originalValue as int?;
                return iOldValue != iNewValue;
            }

            var bNewValue = attributeValue as bool?;
            if (bNewValue != null)
            {
                var bOldValue = originalValue as bool?;
                return bOldValue != bNewValue;
            }

            return true; // Other datatypes not covered, assume value is different
        }

        internal static object GetPreImageAttributeValue(this IExecutionContext context, string attributeName, string preEntityImageName = PreEntityImageName)
        {
            var entity = context.GetPreImage(preEntityImageName);
            if (entity?.Contains(attributeName) == true)
            {
                return entity[attributeName];
            }
            return null;
        }

        public static Entity GetPreImage(this IExecutionContext context, string preEntityImageName = PreEntityImageName)
        {
            if (context.PreEntityImages.Contains(preEntityImageName))
            {
                return context.PreEntityImages[preEntityImageName];
            }
            return null;
        }

        internal static object GetPostImageAttributeValue(this IExecutionContext context, string attributeName, string postEntityImageName = PostEntityImageName)
        {
            var entity = context.GetPostImage(postEntityImageName);
            if (entity?.Contains(attributeName) == true)
            {
                return entity[attributeName];
            }
            return null;
        }

        public static Entity GetPostImage(this IExecutionContext context, string postEntityImageName = PostEntityImageName)
        {
            if (context.PostEntityImages.Contains(postEntityImageName))
            {
                return context.PostEntityImages[postEntityImageName];
            }
            return null;
        }
    }
}
