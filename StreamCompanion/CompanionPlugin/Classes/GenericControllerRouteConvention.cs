using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace CompanionPlugin.Classes;

/// <summary>
/// Соглашение о путях поиска контроллеров с шаблонными типами
/// </summary>
public class GenericControllerRouteConvention : IControllerModelConvention
{
    public void Apply(ControllerModel controller)
    {
        if (controller.ControllerType.IsGenericType)
        {
            Type genericType = controller.ControllerType.GenericTypeArguments[0];
            controller.ControllerName = genericType.Name;
        }
    }
}