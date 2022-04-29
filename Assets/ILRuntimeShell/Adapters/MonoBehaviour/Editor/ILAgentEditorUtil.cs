using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Assets.ILRuntimeShell.Adapters.MonoBehaviour.Editor
{
    [InitializeOnLoad]
    public class ILAgentEditorUtil
    {
        static ILAgentEditorUtil()
        {
            ObjectFactory.componentWasAdded -= componentWasAdded;
            ObjectFactory.componentWasAdded += componentWasAdded;
        }

        private static void componentWasAdded(Component component)
        {
            if (component is UnityEngine.MonoBehaviour behaviour)
            {
                ReplaceMonoBehaviour(behaviour);
            }
        }

        public static ILAgent ReplaceMonoBehaviour(UnityEngine.MonoBehaviour behaviour)
        {
            ILAgent iLAgent = ILAgentUtil.CreateILAgent(behaviour);
            if (iLAgent != null)
            {
                EditorApplication.delayCall += () => UnityEngine.MonoBehaviour.DestroyImmediate(behaviour); // TODO
                return iLAgent;
            }
            else
                return null;
        }

        public static UnityEngine.MonoBehaviour RestoreMonoBehaviour(ILAgent iLAgent)
        {
            UnityEngine.MonoBehaviour behaviour = ILAgentUtil.CreateMonoBehaviour(iLAgent);
            if (behaviour != null)
            {
                EditorApplication.delayCall += () => UnityEngine.MonoBehaviour.DestroyImmediate(iLAgent); // TODO
                return behaviour;
            }
            else
                return null;
        }

        [MenuItem("ILRuntime/ReplaceMonoBehaviours")]
        private static void ReplaceMonoBehaviours()
        {
            //Debug.Log(Selection.GetFiltered<UnityEngine.MonoBehaviour>(SelectionMode.Unfiltered).Length);
            //var path = "";
            //var obj = Selection.activeObject;
            //if (obj == null) path = "Assets";
            //else path = AssetDatabase.GetAssetPath(obj.GetInstanceID());
            //Debug.Log($"Not in assets folddddddd:::::{path}");
            //if (path.Length > 0)
            //{
            //    if (Directory.Exists(path))
            //    {
            //        Debug.Log("Folder");
            //    }
            //    else
            //    {
            //        Debug.Log("File");
            //    }
            //}
            //else
            //{
            //    Debug.Log("Not in assets folder");
            //}
            //return;
            HashSet<string> prefabs = new HashSet<string>();
            foreach (var comp in Resources.FindObjectsOfTypeAll<UnityEngine.MonoBehaviour>())
            {
                //Debug.Log($"DDDDDDD{comp.gameObject}");
                if (comp.hideFlags == HideFlags.NotEditable || comp.hideFlags == HideFlags.HideAndDontSave)
                    continue;
                GameObject go = comp.gameObject;
                if (go == null || EditorUtility.IsPersistent(go))
                    continue;

                if (PrefabUtility.IsPartOfPrefabInstance(go))
                {
                    prefabs.Add(PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(go));
                    continue;
                }

                var iLAgent = ReplaceMonoBehaviour(comp);
                if (iLAgent)
                    EditorUtility.SetDirty(iLAgent);
            }

            foreach (var prefab in prefabs)
            {
                //FixPrefabAsset.ToIL(prefab);
                // TODO
                Debug.Log("================= " + prefab);
            }

            Debug.Log("[ILAgentEditorUtil]ReplaceMonoBehaviours Finished");
        }
    }
}