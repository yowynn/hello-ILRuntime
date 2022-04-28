using Assets.ILRuntimeShell.Bingdings;
using ILRuntime.CLR.TypeSystem;
using ILRuntime.Runtime.Enviorment;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace Assets.ILRuntimeShell
{
    public class ILAPP : MonoBehaviour
    {
        private static ILAPP instance;

        public static ILAPP GetInstance()
        {
            if (instance != null)
                return instance;
            else
            {
                GameObject go = new GameObject();
                instance = go.AddComponent<ILAPP>();
                go.hideFlags = HideFlags.DontSave;
                go.name = typeof(ILAPP).Name;
                instance.Initialize();
                return instance;
            }
        }

        public static IType GetType(string typeName)
        {
            return GetInstance().domain.GetType(typeName);
        }

        public static AppDomain AppDomain => GetInstance().domain;

        private AppDomain domain;

        private ILAPP()
        {
        }

        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
            Initialize();
        }

        private void Start()
        {
            //Initialize();
        }

        public void Initialize(bool reset = false)
        {
            print($"[ILAPP.Initialize]{domain == null}");
            if (domain != null && !reset)
                return;
            domain = new AppDomain();
            RegisterBindings();
            StartCoroutine(LoadAssemblies());
        }

        public IEnumerator LoadAssemblies()
        {
#if UNITY_EDITOR
            var findPath = Path.Combine(Path.GetDirectoryName(Application.dataPath), "Library/ScriptAssemblies/");
            string[] files = Directory.GetFiles(findPath, "Product.*.dll", SearchOption.TopDirectoryOnly);
            foreach (string path in files)
            {
                var dllpath = path;
                var pdbpath = Path.ChangeExtension(path, ".pdb");
                LoadAssemblyLocal(dllpath, pdbpath);
                //yield return StartCoroutine(LoadAssembly("file://" + dllpath, File.Exists(pdbpath) ? ("file://" + pdbpath) : null));
            }
            yield return null;
#endif
        }

        public void LoadAssemblyLocal(string dllpath, string pdbpath = null)
        {
            byte[] dlldata = null, pdbdata = null;
            dlldata = File.ReadAllBytes(dllpath);
            if (pdbpath != null)
            {
                pdbdata = File.ReadAllBytes(pdbpath);
                domain.LoadAssembly(new MemoryStream(dlldata), new MemoryStream(pdbdata), new ILRuntime.Mono.Cecil.Pdb.PdbReaderProvider());
            }
            else
            {
                domain.LoadAssembly(new MemoryStream(dlldata));
            }
            print($"LoadAssemblyFinish: {dllpath}: {dlldata?.Length} & {pdbpath}: {pdbdata?.Length}");
        }

        public IEnumerator LoadAssembly(string dllurl, string pdburl = null)
        {
            //print($"LoadAssembly: {dllurl} : {pdburl}");
            byte[] dlldata = null, pdbdata = null;
            using (UnityWebRequest uwr = new UnityWebRequest(dllurl))
            {
                uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
                yield return uwr.SendWebRequest();
                if (uwr.isDone && !uwr.isHttpError && !uwr.isNetworkError)
                {
                    dlldata = uwr.downloadHandler.data;
                }
            }
            if (dlldata == null)
                throw new System.Exception($"Cannot load DLL: {dllurl}");
            if (pdburl != null)
            {
                using (UnityWebRequest uwr = new UnityWebRequest(pdburl))
                {
                    uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
                    yield return uwr.SendWebRequest();
                    if (uwr.isDone && !uwr.isHttpError && !uwr.isNetworkError)
                    {
                        pdbdata = uwr.downloadHandler.data;
                    }
                }
                domain.LoadAssembly(new MemoryStream(dlldata), new MemoryStream(pdbdata), new ILRuntime.Mono.Cecil.Pdb.PdbReaderProvider());
            }
            else
            {
                domain.LoadAssembly(new MemoryStream(dlldata));
            }
            print($"LoadAssemblyFinish: {dllurl} : {pdburl}");
        }

        public void RegisterBindings()
        {
            domain.RegisterValueTypeBinder(typeof(Vector3), new Vector3Binder());
            domain.RegisterValueTypeBinder(typeof(Vector2), new Vector2Binder());
            domain.RegisterValueTypeBinder(typeof(Quaternion), new QuaternionBinder());

            domain.RegisterCrossBindingAdaptor(new Adapters.MonoBehaviour.ILAdapter());
        }
    }
}