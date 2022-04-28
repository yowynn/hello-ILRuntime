using System;
using UnityEditor;
using UnityEngine;

namespace Assets.ILRuntimeShell.Adapters.MonoBehaviour.Editor
{
    [CustomEditor(typeof(ILAgent))]
    [CanEditMultipleObjects]
    public class ILAgentInspector : UnityEditor.Editor
    {
        private UnityEditor.Editor origin;
        private ILAgent agent;

        private void OnEnable()
        {
            //ILAPP.GetInstance().gameObject.
            agent = (ILAgent)target;
            var type = Type.GetType(agent.ILType);
            var so = ScriptableObject.CreateInstance(type);
            ILAgentUtil.WhiteToScriptableObject(agent, so);
            origin = CreateEditor(so);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GUI.enabled = false;
            EditorGUILayout.LabelField("Name", agent.ILType, new GUILayoutOption[] { });
            //EditorGUILayout.ObjectField("Script", ilbehaviour.GetMonoScript(), typeof(MonoBehaviour), false, null);
            GUI.enabled = true;

            EditorGUI.BeginChangeCheck();
            //origin.OnInspectorGUI();
            if (EditorGUI.EndChangeCheck())
            {
                //ILData.SaveSerializedObject(editor.serializedObject, ilbehaviour.IL_ARGS);
                //ILSerializer.Serialize(editor.serializedObject, ilbehaviour.IL_DATA);
                ILAgentUtil.ILSerialize(origin.serializedObject, agent.ILData);
                ILAgentUtil.ILDeserialize(agent.ILInstance, agent.ILData);
                serializedObject.Update();
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(agent);
            }
        }
    }
}