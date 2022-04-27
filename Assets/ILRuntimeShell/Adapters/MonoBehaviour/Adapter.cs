using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using System.Collections;
using UnityEngine;

namespace Assets.ILRuntimeShell.Adapters.MonoBehaviour
{
    public class ILAgent : UnityEngine.MonoBehaviour, CrossBindingAdaptorType
    {
        public ILTypeInstance ILInstance { get; set; }

        public ILData ILData;

        // Use this for initialization
        private void Start()
        {
        }

        // Update is called once per frame
        private void Update()
        {
        }

        private void Never()
        {
            CrossBindingAdaptorType s;
        }
    }
}