using ILRuntime.CLR.Method;
using ILRuntime.CLR.TypeSystem;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using System.Collections;
using UnityEngine;

namespace Assets.ILRuntimeShell.Adapters.MonoBehaviour
{
    public class ILAgent : UnityEngine.MonoBehaviour, CrossBindingAdaptorType, ISerializationCallbackReceiver
    {
        private ILTypeInstance iLTypeInstance;

        public string ILType;

        public ILData ILData;

        public ILTypeInstance ILInstance
        {
            get
            {
                if (iLTypeInstance == null)
                {
                    if (this.ILType == null)
                        return null;
                    var type = ILAPP.GetILRuntimeType(this.ILType);
                    var iltype = type as ILType;
                    if (iltype == null)
                        throw new System.Exception($"[ILAgent]can not find IL type: {this.ILType == null}");
                    var instance = iltype.Instantiate();
                    instance.CLRInstance = this;
                    ILInstance = instance;
                    ILAgentUtil.ILDeserialize(ILInstance, ILData);
                }
                return iLTypeInstance;
            }

            set
            {
                iLTypeInstance = value;
            }
        }

        public ILAgent(ILTypeInstance instance)
        {
            ILInstance = instance;
        }

        public ILAgent()
        {
            //Debug.Log($"?????????????????????????????????????ddd???????????????{ILType}");
        }

        public void OnBeforeSerialize()
        {
            //throw new System.NotImplementedException();
            //if (ILInstance == null)
            //{
            //    ILInstance
            //}
            //Debug.Log($"?????????????????????????????????????ddd???????????????11111{ILType}");
        }

        public void OnAfterDeserialize()
        {
            //throw new System.NotImplementedException();
            //ILAgentUtil.ILDeserialize(ILInstance, ILData);
            //Debug.Log("DDDDDDDDDDDDDDDDDD");
        }

        public void CallInitialUnityMessage()
        {
            Awake();
            OnEnable();
            //OnValidate();
        }

        private struct AdapterMethodCache
        {
            public string Name;
            public IMethod Method;
        }

        private System.Collections.Generic.Dictionary<string, AdapterMethodCache> mAdapterMethodCache;

        private IMethod GetAdapterMethod(string name, int paramCount = 0)
        {
            if (ILInstance == null)
            {
                return null;
            }
            if (mAdapterMethodCache == null)
                mAdapterMethodCache = new System.Collections.Generic.Dictionary<string, AdapterMethodCache>();

            AdapterMethodCache cache;
            if (!mAdapterMethodCache.TryGetValue(name, out cache))
            {
                cache = new AdapterMethodCache();
                cache.Name = name;
                cache.Method = ILInstance.Type.GetMethod(name, paramCount);
                mAdapterMethodCache[name] = cache;
            }
            return cache.Method;
        }

        private void CallAdapterMethod(string name, params object[] args)
        {
            IMethod method = GetAdapterMethod(name, args.Length);
            if (method != null)
            {
                ILAPP.CurrentDomain.Invoke(method, ILInstance, args);
            }
        }

        private void CallAdapterMethod(string name)
        {
            IMethod method = GetAdapterMethod(name);
            if (method != null)
            {
                ILAPP.CurrentDomain.Invoke(method, ILInstance, null);
            }
        }

        // TODO @wynn register all unity message, that called optimization problems
        private void OnEnable()
        { CallAdapterMethod("OnEnable"); }

        private void OnDisable()
        { CallAdapterMethod("OnDisable"); }

        private void OnDestroy()
        { CallAdapterMethod("OnDestroy"); }

        private void OnApplicationQuit()
        { CallAdapterMethod("OnApplicationQuit"); }

        private void OnApplicationFocus(bool focus)
        { CallAdapterMethod("OnApplicationFocus", focus); }

        private void OnApplicationPause(bool pause)
        { CallAdapterMethod("OnApplicationPause", pause); }

        private void OnBecameVisible()
        { CallAdapterMethod("OnBecameVisible"); }

        private void OnBecameInvisible()
        { CallAdapterMethod("OnBecameInvisible"); }

        private void OnCollisionEnter(Collision collision)
        { CallAdapterMethod("OnCollisionEnter", collision); }

        private void OnCollisionExit(Collision collision)
        { CallAdapterMethod("OnCollisionExit", collision); }

        private void OnCollisionStay(Collision collision)
        { CallAdapterMethod("OnCollisionStay", collision); }

        private void OnCollisionEnter2D(Collision2D collision)
        { CallAdapterMethod("OnCollisionEnter2D", collision); }

        private void OnCollisionExit2D(Collision2D collision)
        { CallAdapterMethod("OnCollisionExit2D", collision); }

        private void OnCollisionStay2D(Collision2D collision)
        { CallAdapterMethod("OnCollisionStay2D", collision); }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        { CallAdapterMethod("OnControllerColliderHit", hit); }

        private void OnDrawGizmos()
        { CallAdapterMethod("OnDrawGizmos"); }

        private void OnDrawGizmosSelected()
        { CallAdapterMethod("OnDrawGizmosSelected"); }

        private void OnJointBreak(float breakForce)
        { CallAdapterMethod("OnJointBreak", breakForce); }

        private void OnJointBreak2D(Joint2D brokenJoint)
        { CallAdapterMethod("OnJointBreak2D", brokenJoint); }

        private void OnMouseDown()
        { CallAdapterMethod("OnMouseDown"); }

        private void OnMouseDrag()
        { CallAdapterMethod("OnMouseDrag"); }

        private void OnMouseEnter()
        { CallAdapterMethod("OnMouseEnter"); }

        private void OnMouseExit()
        { CallAdapterMethod("OnMouseExit"); }

        private void OnMouseOver()
        { CallAdapterMethod("OnMouseOver"); }

        private void OnMouseUp()
        { CallAdapterMethod("OnMouseUp"); }

        private void OnMouseUpAsButton()
        { CallAdapterMethod("OnMouseUpAsButton"); }

        private void OnParticleCollision(GameObject other)
        { CallAdapterMethod("OnParticleCollision", other); }

        private void OnPostRender()
        { CallAdapterMethod("OnPostRender"); }

        private void OnPreCull()
        { CallAdapterMethod("OnPreCull"); }

        private void OnPreRender()
        { CallAdapterMethod("OnPreRender"); }

        private void OnRenderImage(RenderTexture src, RenderTexture dest)
        { CallAdapterMethod("OnRenderImage", src, dest); }

        private void OnRenderObject()
        { CallAdapterMethod("OnRenderObject"); }

        private void OnTransformChildrenChanged()
        { CallAdapterMethod("OnTransformChildrenChanged"); }

        private void OnTransformParentChanged()
        { CallAdapterMethod("OnTransformParentChanged"); }

        private void OnTriggerEnter(Collider other)
        { CallAdapterMethod("OnTriggerEnter", other); }

        private void OnTriggerEnter2D(Collider2D other)
        { CallAdapterMethod("OnTriggerEnter2D", other); }

        private void OnTriggerExit(Collider other)
        { CallAdapterMethod("OnTriggerExit", other); }

        private void OnTriggerExit2D(Collider2D other)
        { CallAdapterMethod("OnTriggerExit2D", other); }

        private void OnTriggerStay(Collider other)
        { CallAdapterMethod("OnTriggerStay", other); }

        private void OnTriggerStay2D(Collider2D other)
        { CallAdapterMethod("OnTriggerStay2D", other); }

        private void Reset()
        { CallAdapterMethod("Reset"); }

        private void Start()
        { CallAdapterMethod("Start"); }

        private void Update()
        { CallAdapterMethod("Update"); }

        private void Awake()
        {
            CallAdapterMethod("Awake");
            print("AWAKEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEE");
        }

        //void OnValidate() { CallAdapterMethod("OnValidate"); }

        public void ILFUNC_OnAnimationEvent(AnimationEvent ev)
        {
            string[] arr = ev.stringParameter.Split(new char[] { '$' }, 2);
            ev.functionName = arr[0];
            ev.stringParameter = arr[1];

            //Debug.Log("[ILBehaviour]OnAnimationEvent: " + ev.functionName + " @ " + gameObject + ":" + this);

            //TODO: Method Cache
            int methodType = 0;
            IMethod methodFunc = ILInstance.Type.GetMethod(ev.functionName, 1);
            if (methodFunc != null)
            {
                System.Type t = methodFunc.Parameters[0].ReflectionType;
                if (t.Equals(typeof(string))) methodType = 1;
                else if (t.Equals(typeof(int))) methodType = 2;
                else if (t.Equals(typeof(float))) methodType = 3;
                else if (t.IsSubclassOf(typeof(Object))) methodType = 4;
                else if (t.Equals(typeof(AnimationEvent))) methodType = 5;
            }

            if (methodType == 0)
            {
                methodFunc = ILInstance.Type.GetMethod(ev.functionName, 0);
                if (methodFunc != null)
                    methodType = 6;
                else
                    methodType = -1;
            }

            switch (methodType)
            {
                case 1:
                    ILAPP.CurrentDomain.Invoke(methodFunc, ILInstance, ev.stringParameter);
                    break;

                case 2:
                    ILAPP.CurrentDomain.Invoke(methodFunc, ILInstance, ev.intParameter);
                    break;

                case 3:
                    ILAPP.CurrentDomain.Invoke(methodFunc, ILInstance, ev.floatParameter);
                    break;

                case 4:
                    ILAPP.CurrentDomain.Invoke(methodFunc, ILInstance, ev.objectReferenceParameter);
                    break;

                case 5:
                    ILAPP.CurrentDomain.Invoke(methodFunc, ILInstance, ev);
                    break;

                case 6:
                    ILAPP.CurrentDomain.Invoke(methodFunc, ILInstance, null);
                    break;

                default:
                    Debug.Log("[ILBehaviour]OnAnimationEvent Missing: " + ev.functionName +
                        "\n" + methodFunc +
                        "\nstring " + ev.stringParameter +
                        "\nint     " + ev.intParameter +
                        "\nfloat   " + ev.floatParameter +
                        "\nobject  " + ev.objectReferenceParameter?.ToString());
                    break;
            }
        }

        // TODO
        private void AnimatorEvent(string EventName)
        { CallAdapterMethod("AnimatorEvent", EventName); }

        private void SoundEvent(string EventName)
        { CallAdapterMethod("SoundEvent", EventName); }

        private void GameEnd()
        { CallAdapterMethod("GameEnd"); }
    }
}