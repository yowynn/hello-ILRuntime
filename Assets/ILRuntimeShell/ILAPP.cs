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
        private static ILAPP currentInstance;

        private static AppDomain currentDomain;

        public static ILAPP CurrentInstance
        {
            get
            {
                if (currentInstance != null)
                    return currentInstance;
                else
                {
                    throw new System.Exception("[ILAPP]Current APP not found");
                }
            }
        }

        public static AppDomain CurrentDomain
        {
            get
            {
                if (currentDomain != null)
                {
                    return currentDomain;
                }
                else
                {
#if UNITY_EDITOR
                    currentDomain = CreateDefaultDomain();
                    return currentDomain;
#endif
                    throw new System.Exception("[ILAPP]Current Domain not found");
                }
            }
        }

        public static IType GetILRuntimeType(string typeName)
        {
            return CurrentDomain.GetType(typeName);
        }

        public static System.Type GetOriginType(string typeName)
        {
            var originDomain = System.AppDomain.CurrentDomain;
            var assemblies = originDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                var type = assembly.GetType(typeName);
                if (type != null)
                    return type;
            }
            return null;
        }

        private AppDomain domain;

        public AppDomain AppDomain
        {
            get
            {
                return domain;
            }
        }

        private ILAPP()
        {
            domain = new AppDomain();
        }

        public void SetCurrent()
        {
            var domain = this?.domain;
            if (domain == null)
                throw new System.Exception("[ILAPP]SetCurrent unavailable ILAPP");
            currentInstance = this;
            currentDomain = domain;
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
            if (reset)
                domain = new AppDomain();
            RegisterBindings(domain);
            RegisterCLRMethodRedirections(domain);
            // TODO load Assemblies
        }

        public static void LoadAssemblyLocal(AppDomain domain, string dllpath, string pdbpath = null)
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
            Debug.Log($"LoadAssemblyLocalFinish: {dllpath}: {dlldata?.Length} & {pdbpath}: {pdbdata?.Length}");
        }

        public static IEnumerator LoadAssemblyRemote(AppDomain domain, string dllurl, string pdburl = null)
        {
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
            Debug.Log($"LoadAssemblyRemoteFinish: {dllurl} : {pdburl}");
        }

        public static void RegisterBindings(AppDomain domain)
        {
            domain.RegisterValueTypeBinder(typeof(Vector3), new Vector3Binder());
            domain.RegisterValueTypeBinder(typeof(Vector2), new Vector2Binder());
            domain.RegisterValueTypeBinder(typeof(Quaternion), new QuaternionBinder());

            domain.RegisterCrossBindingAdaptor(new Adapters.MonoBehaviour.ILAdapter());
        }

        public static void RegisterCLRMethodRedirections(AppDomain domain)
        {
            Adapters.MonoBehaviour.ILAgentUtil.RegisterCLRMethodRedirections(domain);
        }

        private static AppDomain CreateDefaultDomain()
        {
#if UNITY_EDITOR
            var appDomain = new AppDomain();
            RegisterBindings(appDomain);
            RegisterCLRMethodRedirections(appDomain);
            var findPath = Path.Combine(Path.GetDirectoryName(Application.dataPath), "Library/ScriptAssemblies/");
            string[] files = Directory.GetFiles(findPath, "Product.*.dll", SearchOption.TopDirectoryOnly);
            foreach (string path in files)
            {
                var dllpath = path;
                var pdbpath = Path.ChangeExtension(path, ".pdb");
                LoadAssemblyLocal(appDomain, dllpath, pdbpath);
            }
            Debug.LogWarning($"[ILAPP]CreateDefaultDomain in DEBUG!!!");
            return appDomain;
#endif
            throw new System.Exception("Debug Method");
        }
    }
}