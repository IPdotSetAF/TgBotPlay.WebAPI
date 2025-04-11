using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace TgBotPlay.WebAPI;

internal class ControllerRoutingConvention(Type controllerType, string? routeTemplate, string? nameTemplate)
    : IControllerModelConvention
{
    /// <inheritdoc />
    public void Apply(ControllerModel controller)
    {
        ArgumentNullException.ThrowIfNull(controller);

        if (HasRouteAttributes(controller))
        {
            return;
        }

        if (!IsControllerInstance(controllerType, controller))
        {
            return;
        }

        ApplyNewRouteDynamically(controller);
        ApplyNewNameDynamically(controller);
    }

    private void ApplyNewNameDynamically(ControllerModel controllerModel)
    {
        if (string.IsNullOrEmpty(nameTemplate))
        {
            return;
        }

        controllerModel.ControllerName = nameTemplate;
    }

    private void ApplyNewRouteDynamically(ControllerModel controllerModel)
    {
        if (string.IsNullOrWhiteSpace(routeTemplate))
        {
            return;
        }

        routeTemplate = routeTemplate.Replace("[action]", "");

        foreach (var selector in controllerModel.Selectors)
        {
            selector.AttributeRouteModel = new AttributeRouteModel
            {
                Template = routeTemplate
            };
        }
    }

    private static bool HasRouteAttributes(ControllerModel controllerModel)
        => controllerModel.Selectors.Any(selector => selector.AttributeRouteModel is not null);

    private static bool IsControllerInstance(Type controller, ControllerModel controllerModel)
    {
        var controllerName =
            controller.Name.Replace(oldValue: "Controller", newValue: "", StringComparison.OrdinalIgnoreCase);

        return string.Equals(controllerModel.ControllerType.Namespace, controller.Namespace,
            StringComparison.OrdinalIgnoreCase) && string.Equals(controllerModel.ControllerName, controllerName,
            StringComparison.OrdinalIgnoreCase);
    }
}
