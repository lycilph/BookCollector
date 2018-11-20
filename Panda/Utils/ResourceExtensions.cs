using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Panda.Utils
{
    public static class ResourceExtensions
    {
        public static string GetResource(Assembly assembly, string name)
        {
            var full_resource_name = assembly.GetManifestResourceNames().First(n => n.Contains(name));
            using (var s = assembly.GetManifestResourceStream(full_resource_name))
            using (var sr = new StreamReader(s))
            {
                return sr.ReadToEnd();
            }
        }

        public static string GetResource(string name)
        {
            return GetResource(Assembly.GetEntryAssembly(), name);
        }

        public static string GetResource(this Type type, string name)
        {
            return GetResource(Assembly.GetAssembly(type), name);
        }
    }
}
