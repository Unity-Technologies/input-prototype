using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace UnityEngine.Experimental.Input.Utilities
{
    /// <summary>
    /// Based off of Unity's Internal ScriptableSingleton with UnityEditorInternal bits removed
    /// </summary>
    /// <typeparam name="T">The class being created</typeparam>
    public abstract class ScriptableSettings<T> : Internal.ScriptableSettings where T : ScriptableObject
    {
        const string k_SavePath = "Assets/{0}Resources/ScriptableSettings/{1}.asset";
        const string k_LoadPath = "ScriptableSettings/{0}";
        static T s_Instance;

        /// <summary>
        /// Retrieves a reference to the given settings class. Will load and initialize once, and cache for all future access.
        /// </summary>
        public static T instance
        {
            get
            {
                if (s_Instance == null)
                {
                    CreateAndLoad();
                }

                return s_Instance;
            }
        }

        /// <summary>
        /// On domain reload ScriptableObject objects gets reconstructed from a backup. We therefore set the s_Instance here.
        /// </summary>
        protected ScriptableSettings()
        {
            if (s_Instance != null)
            {
                Debug.LogError("ScriptableSingleton already exists. Did you query the singleton in a constructor?");
            }
            else
            {
                object casted = this;
                s_Instance = casted as T;
                System.Diagnostics.Debug.Assert(s_Instance != null);
            }
        }

        static void CreateAndLoad()
        {
            System.Diagnostics.Debug.Assert(s_Instance == null);

            // Try to load the singleton
            s_Instance = Resources.Load(string.Format(k_LoadPath, GetFilePath())) as T;

            // Create it if it doesn't exist
            if (s_Instance == null)
            {
                s_Instance = CreateInstance<T>();

                // And save it back out if appropriate
                Save();
            }

            System.Diagnostics.Debug.Assert(s_Instance != null);
        }

        static void Save()
        {
            // We only save in the editor during edit mode
#if UNITY_EDITOR
            if (Application.isPlaying || !Application.isEditor)
            {
                // This is expected behavior so no log neccessary here
                return;
            }

            if (s_Instance == null)
            {
                Debug.Log("Cannot save ScriptableSettings: no instance!");
                return;
            }

            // We get the script path, and from there generate the save path.
            // This way settings will stick with packages/repositories they were created with
            var scriptData = MonoScript.FromScriptableObject(s_Instance);
            var assetPath = AssetDatabase.GetAssetPath(scriptData);

            // Get the first folder above 'assets'
            var specializationPath = "";
            const int folderStart = 7;  // assets + /
            var folderEnd = assetPath.Substring(folderStart).IndexOf('/');
            if (folderEnd > 0)
            {
                specializationPath = assetPath.Substring(folderStart, folderEnd) + '/';
            }


            var savePath = string.Format(k_SavePath, specializationPath, GetFilePath());

            var folderPath = Path.GetDirectoryName(savePath);
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            AssetDatabase.CreateAsset(s_Instance, savePath);
            AssetDatabase.SaveAssets();

            Debug.Log(string.Format("Created initial copy of settings: {0} at {1}", GetFilePath(), savePath));
#endif
        }

        static string GetFilePath()
        {
            var type = typeof(T);
            return type.Name;
        }
    }
}

namespace UnityEngine.Experimental.Input.Utilities.Internal
{
    /// <summary>
    /// Internal base for all scriptable settings that is easier to look up via-reflection.
    /// DO NOT USE THIS CLASS DIRECTLY - Use the generic version - ScriptableSettings<T> above
    /// </summary>
    public abstract class ScriptableSettings : ScriptableObject { }
}