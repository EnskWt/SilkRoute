using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SilkRoute.Tools.RequestTools;
using SilkRoute.Tools.RequestTools.RequestParametersBinders.BinderContract;

namespace SilkRoute.Tools.RequestTools.RequestParametersBinders
{
    internal abstract class AttributeParametersBinder<T> : IRequestParametersBinder where T : Attribute
    {
        public virtual int Priority { get; } = 0;

        public bool CanBind(ParameterInfo parameterInfo, object? value)
        {
            if (value == null)
                return false;

            var allowedTypes = new[]
            {
                typeof(FromFormAttribute),
                typeof(FromBodyAttribute),
                typeof(FromRouteAttribute),
                typeof(FromQueryAttribute),
                typeof(FromHeaderAttribute)
            };

            var firstRelevant = parameterInfo
                .GetCustomAttributes()
                .Cast<Attribute>()
                .FirstOrDefault(attr => allowedTypes.Contains(attr.GetType()));

            return firstRelevant is T;
        }

        public abstract void Bind(RequestBuilder requestBuilder, ParameterInfo parameterInfo, object value);

    }
}
