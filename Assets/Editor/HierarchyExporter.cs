using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MMO.Editor
{
    public static class HierarchyExporter
    {
        [MenuItem("Tools/Export Hierarchy")]
        public static void ExportHierarchy()
        {
            var scene = SceneManager.GetActiveScene();
            string projectRoot = Directory.GetParent(Application.dataPath).FullName;
            string path = Path.Combine(projectRoot, "Hierarchy.txt");
            using (var writer = new StreamWriter(path, false))
            {
                foreach (var root in scene.GetRootGameObjects())
                {
                    DumpGameObject(writer, root.transform, 0);
                }
            }
            Debug.Log($"Hierarchy exported to {path}");
        }

        private static void DumpGameObject(StreamWriter writer, Transform transform, int indent)
        {
            string prefix = new string(' ', indent * 2);
            writer.WriteLine($"{prefix}{transform.gameObject.name}");
            foreach (var comp in transform.gameObject.GetComponents<Component>())
            {
                if (comp == null)
                {
                    writer.WriteLine($"{prefix}  Missing Component");
                    continue;
                }
                writer.WriteLine($"{prefix}  {comp.GetType().Name}");
                foreach (var field in comp.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public))
                {
                    object value = field.GetValue(comp);
                    writer.WriteLine($"{prefix}    {field.Name}: {value}");
                }
            }

            foreach (Transform child in transform)
            {
                DumpGameObject(writer, child, indent + 1);
            }
        }
    }
}
