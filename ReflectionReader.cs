using System;
using System.Reflection;

namespace GanExtendDisplay
{
    internal static class ReflectionReader
    {
        private const BindingFlags InstanceFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        public static object Member(object instance, string name)
        {
            if (instance == null || string.IsNullOrWhiteSpace(name))
                return null;

            Type type = instance.GetType();
            FieldInfo field = type.GetField(name, InstanceFlags);
            if (field != null)
                return field.GetValue(instance);

            PropertyInfo property = type.GetProperty(name, InstanceFlags);
            if (property != null && property.GetIndexParameters().Length == 0)
                return property.GetValue(instance, null);

            return null;
        }

        public static object Path(object instance, string path)
        {
            object current = instance;
            foreach (string part in path.Split('.'))
            {
                current = Member(current, part);
                if (current == null)
                    return null;
            }
            return current;
        }

        public static int IntMember(object instance, params string[] names)
        {
            foreach (string name in names)
            {
                object value = name.Contains(".") ? Path(instance, name) : Member(instance, name);
                if (TryConvertToInt(value, out int result))
                    return result;
            }
            return -1;
        }

        public static string StringMember(object instance, params string[] names)
        {
            foreach (string name in names)
            {
                object value = name.Contains(".") ? Path(instance, name) : Member(instance, name);
                if (value != null)
                    return value.ToString();
            }
            return null;
        }

        public static int IntFromValue(object value)
        {
            return TryConvertToInt(value, out int result) ? result : -1;
        }

        public static string StringFromValue(object value)
        {
            if (value == null)
                return string.Empty;

            try
            {
                return value.ToString();
            }
            catch
            {
                return string.Empty;
            }
        }

        public static object InvokeNoArgs(object instance, params string[] methodNames)
        {
            if (instance == null)
                return null;

            Type type = instance.GetType();
            foreach (string methodName in methodNames)
            {
                MethodInfo method = type.GetMethod(methodName, InstanceFlags, null, Type.EmptyTypes, null);
                if (method == null)
                    continue;

                try
                {
                    return method.Invoke(instance, null);
                }
                catch
                {
                    // Ignore incompatible members and try the next fallback.
                }
            }
            return null;
        }

        public static object InvokeOneInt(object instance, int argument, params string[] methodNames)
        {
            if (instance == null)
                return null;

            Type type = instance.GetType();
            foreach (string methodName in methodNames)
            {
                MethodInfo method = type.GetMethod(methodName, InstanceFlags, null, new[] { typeof(int) }, null);
                if (method == null)
                    continue;

                try
                {
                    return method.Invoke(instance, new object[] { argument });
                }
                catch
                {
                    // Some Elin versions expose similarly named methods with different behavior.
                }
            }
            return null;
        }

        private static bool TryConvertToInt(object value, out int result)
        {
            result = -1;
            if (value == null)
                return false;

            try
            {
                result = Convert.ToInt32(value);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
