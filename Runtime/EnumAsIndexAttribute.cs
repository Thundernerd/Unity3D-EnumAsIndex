using System;
using UnityEngine;

namespace TNRD.Utilities
{
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class EnumAsIndexAttribute : PropertyAttribute
    {
        public Type Type { get; private set; }

        public EnumAsIndexAttribute(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type), "To use the EnumAsIndex attribute please pass a valid type");

            if (!type.IsEnum)
                throw new ArgumentException($"Type '{type.Name}' is not an enum");

            Type = type;
        }
    }
}
