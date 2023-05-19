using Microsoft.Xrm.Sdk;
using System;

namespace Sample.Dataverse.Plugins.Services
{
    internal static class PluginExtensions
    {
        internal static Entity GetEntityFromContext(this IExecutionContext context)
        {
            if (context.IsRetrieveOperation() && context.OutputParameters.Contains("BusinessEntity") && context.InputParameters["BusinessEntity"] is Entity _entity)
                return _entity;

            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity _target)
                return _target;

            return null;
        }

        internal static EntityReference GetEntityReferenceFromContext(this IExecutionContext context)
        {
            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is EntityReference _entityRef)
            {
                return _entityRef;
            }
            return null;
        }

        internal static bool IsPreValidationStage(this IPluginExecutionContext context)
        {
            return context.Stage == (int)StageEnum.PreValidation;
        }

        internal static bool IsPreOperationStage(this IPluginExecutionContext context)
        {
            return context.Stage == (int)StageEnum.PreOperation;
        }

        internal static bool IsPostOperationStage(this IPluginExecutionContext context)
        {
            return context.Stage == (int)StageEnum.PostOperation;
        }

        /// <summary>
        ///     Return true when it matches the name of operation
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        internal static bool IsSetStateOperation(this IExecutionContext context, int? targetState = null)
        {
            if (string.Equals(context.MessageName, "SetState", StringComparison.InvariantCultureIgnoreCase) ||
                   string.Equals(context.MessageName, "SetStateDynamicEntity", StringComparison.InvariantCultureIgnoreCase))
            {
                if (targetState == null)
                    return true;

                return !context.IsAttributeImageEqual<OptionSetValue>("statecode", PluginImageExtensions.PreEntityImageName); //if it didn't change then return false as if it never changed
            }

            return false;
        }

        /// <summary>
        ///     Return true when it is create operation
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        internal static bool IsCreateOperation(this IExecutionContext context)
        {
            return string.Equals(context.MessageName, "Create", StringComparison.InvariantCultureIgnoreCase);
        }

        public static bool IsAssignOperation(this IExecutionContext context)
        {
            return string.Equals(context.MessageName, "Assign", StringComparison.InvariantCultureIgnoreCase);
        }

        public static bool IsAssociateOperation(this IExecutionContext context)
        {
            return string.Equals(context.MessageName, "Associate", StringComparison.InvariantCultureIgnoreCase);
        }

        public static bool IsDisassociateOperation(this IExecutionContext context)
        {
            return string.Equals(context.MessageName, "Disassociate", StringComparison.InvariantCultureIgnoreCase);
        }

        internal static bool IsRetrieveOperation(this IExecutionContext context)
        {
            return string.Equals(context.MessageName, "Retrieve", StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        ///     Return true when it is update operation
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        internal static bool IsUpdateOperation(this IExecutionContext context)
        {
            return string.Equals(context.MessageName, "Update", StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        ///     Return true when it is delete operation
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        internal static bool IsDeleteOperation(this IExecutionContext context)
        {
            return string.Equals(context.MessageName, "Delete", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}