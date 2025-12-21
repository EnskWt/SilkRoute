using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using SilkRoute.Tools.ActionResultTools.ActionResultWrapper.WrapperContract;

namespace SilkRoute.Tools.ActionResultTools.ActionResultWrapper
{
    internal sealed class GenericActionResultWrapper : IActionResultWrapper
    {
        public int Priority => 0;

        public bool CanWrap(Type responseType)
            => responseType.IsGenericType
               && responseType.GetGenericTypeDefinition() == typeof(ActionResult<>);

        public object Wrap(HttpResponseMessage response, Type responseType, object? payload)
        {
            var argType = responseType.GetGenericArguments()[0];

            object? effectivePayload = payload;
            if (effectivePayload == null && argType.IsValueType)
                effectivePayload = Activator.CreateInstance(argType);

            var ctor = responseType
                .GetConstructors(BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(ci =>
                {
                    var ps = ci.GetParameters();
                    return ps.Length == 1 && ps[0].ParameterType == argType;
                });

            if (ctor == null)
                throw new InvalidOperationException($"No suitable ctor for {responseType.Name}({argType.Name}).");

            return ctor.Invoke(new[] { effectivePayload! });
        }
    }
}
