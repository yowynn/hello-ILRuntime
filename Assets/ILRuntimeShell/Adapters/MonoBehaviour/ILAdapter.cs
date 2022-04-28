using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using System;
using System.Collections;
using UnityEngine;

namespace Assets.ILRuntimeShell.Adapters.MonoBehaviour
{
    public class ILAdapter : CrossBindingAdaptor
    {
        public override Type BaseCLRType => typeof(UnityEngine.MonoBehaviour);

        public override Type AdaptorType => typeof(ILAgent);

        public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
        {
            return new ILAgent()
            {
                ILInstance = instance,
            };
        }
    }
}