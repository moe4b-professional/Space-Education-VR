#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

using System.Reflection;

namespace Default
{
    /// <summary>
    /// A PreProcessor for scripts, used for replacing #VARIABLES like #NAMESPACE
    /// </summary>
    public class ScriptCreationPreprocessor : UnityEditor.AssetModificationProcessor
    {
        public static string GlobalNamespace
        {
            get
            {
                var value = EditorSettings.projectGenerationRootNamespace;

                if (string.IsNullOrEmpty(value)) value = "Default";

                return value;
            }
        }

        public static void OnWillCreateAsset(string path)
        {
            path = path.Replace(".meta", "");

            if (Path.GetExtension(path) != ".cs") return;

            var text = File.ReadAllText(path);
            text = Process(path, text);
            File.WriteAllText(path, text);

            AssetDatabase.Refresh();
        }

        static string Process(string path, string text)
        {
            text = SetNamespace(text);

            return text;
        }

        static string SetNamespace(string text)
        {
            return text.Replace("#NAMESPACE#", GlobalNamespace);
        }

        static ScriptCreationPreprocessor()
        {

        }

        //Utility

        public static string UnifyPathSeperator(string path) => path.Replace('\\', '/');
    }
}
#endif