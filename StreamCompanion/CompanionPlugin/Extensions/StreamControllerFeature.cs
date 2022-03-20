using System.Reflection;

using CompanionPlugin.Classes.Attributes;
using CompanionPlugin.Controllers;

using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace CompanionPlugin.Extensions;

public class StreamControllerFeature : IApplicationFeatureProvider<ControllerFeature>
{
    #region IApplicationFeatureProvider

    public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
    {
        foreach (IApplicationPartTypeProvider part in parts.OfType<IApplicationPartTypeProvider>())
        {
            foreach (TypeInfo type in part.Types)
            {
                StreamServiceAttribute? attribute = type.GetCustomAttribute<StreamServiceAttribute>();
                if (attribute == null)
                    continue;

                Type configType = ServiceCollectionExtensions.FindConfigType(type);
                if (configType == null)
                    continue;

                feature.Controllers.Add(typeof(BaseServiceController<>).MakeGenericType(configType).GetTypeInfo());
            }
        }
    }

    #endregion IApplicationFeatureProvider
}