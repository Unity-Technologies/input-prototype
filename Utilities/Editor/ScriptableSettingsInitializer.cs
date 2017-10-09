using System.Linq;
using System.Reflection;
using UnityEditor;

namespace UnityEngine.Experimental.Input.Utilities.Internal
{
    /// <summary>
    /// Ensures that all scriptable settings have backing data that can be inspected and edited at compile-time.
    /// </summary>
    public static class ScriptableSettingsInitializer
    {
        [InitializeOnLoadMethod]
        static void EnsureSettingsInstances()
        {
            var settingsClasses = typeof(ScriptableSettings)
                .Assembly
                .GetTypes()
                .Where(t => t.IsSubclassOf(typeof(ScriptableSettings)) && !t.IsAbstract);
            foreach (var currentInstance in settingsClasses)
            {
                var instanceProperty = currentInstance.GetProperty(
                    "instance", BindingFlags.Static | BindingFlags.Public |
                    BindingFlags.GetProperty | BindingFlags.FlattenHierarchy);
                instanceProperty.GetValue(null, null);
            }
        }
    }
}