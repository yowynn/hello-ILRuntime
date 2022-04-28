using System;
using System.Collections.Generic;
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

        public static bool ReplaceMonoBehaviour(UnityEngine.MonoBehaviour behaviour)
        {
            ILAgent iLAgent = ILAgentUtil.CreateILAgent(behaviour);
            if (iLAgent != null)
            {
                EditorApplication.delayCall += () => UnityEngine.MonoBehaviour.DestroyImmediate(behaviour); // TODO
                return true;
            }
            else
                return false;
        }

        public static bool RestoreMonoBehaviour(ILAgent iLAgent)
        {
            UnityEngine.MonoBehaviour behaviour = ILAgentUtil.CreateMonoBehaviour(iLAgent);
            if (behaviour != null)
            {
                EditorApplication.delayCall += () => UnityEngine.MonoBehaviour.DestroyImmediate(iLAgent); // TODO
                return true;
            }
            else
                return false;
        }
    }
}