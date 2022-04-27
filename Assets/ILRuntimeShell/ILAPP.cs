using ILRuntime.Runtime.Enviorment;
using System.Collections;
using System.IO;
using UnityEngine;

namespace Assets.ILRuntimeShell
{
    public class ILAPP : MonoBehaviour
    {
        private static ILAPP instance;

        public static ILAPP GetInstance()
        {
            return instance ?? new ILAPP();
        }

        private AppDomain domain;

        private ILAPP()
        {
            GameObject go = new GameObject();
            instance = go.AddComponent<ILAPP>();
            go.hideFlags = HideFlags.DontSave;
            go.name = typeof(ILAPP).Name;
        }

        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
        }

        private void Start()
        {
            StartCoroutine(LoadAssembly());
        }

        public IEnumerator LoadAssembly()
        {
#if UNITY_EDITOR
            domain = new AppDomain();
            var findPath = Path.Combine(Path.GetDirectoryName(Application.dataPath), "Library/ScriptAssemblies/");
            string[] files = Directory.GetFiles(dir, "Game.*.dll", SearchOption.TopDirectoryOnly);
            foreach (string f in files)
            {
                string name = f.Substring(dir.Length, f.Length - dir.Length - 4);
                Load(name);
            }
#endif
            return null;
        }
    }
}