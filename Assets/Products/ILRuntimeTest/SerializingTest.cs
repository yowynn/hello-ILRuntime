using Assets.ILRuntimeShell;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Products.ILRuntimeTest
{
    //[ExecuteInEditMode]
    public class SerializingTest : MonoBehaviour
    {
        public List<int> aaa;

        // Use this for initialization
        private void Start()
        {
            ILAPP app = ILAPP.GetInstance();
            print("helllodddddddddddddddddd");
        }

        // Update is called once per frame
        private void Update()
        {
        }

        [ExecuteInEditMode]
        private void OnEnable()
        {
            //Test();
        }

        private void Test()
        {
#if UNITY_EDITOR
            Debug.Log("UNITY_EDITOR");
#endif
#if UNITY_EDITOR_WIN
            Debug.Log("UNITY_EDITOR_WIN");
#endif
#if UNITY_EDITOR_OSX
            Debug.Log("UNITY_EDITOR_OSX");
#endif
#if UNITY_STANDALONE_OSX
            Debug.Log("UNITY_STANDALONE_OSX");
#endif
#if UNITY_STANDALONE_WIN
            Debug.Log("UNITY_STANDALONE_WIN");
#endif
#if UNITY_STANDALONE_LINUX
            Debug.Log("UNITY_STANDALONE_LINUX");
#endif
#if UNITY_STANDALONE
            Debug.Log("UNITY_STANDALONE");
#endif
#if UNITY_WII
            Debug.Log("UNITY_WII");
#endif
#if UNITY_IPHONE
            Debug.Log("UNITY_IPHONE");
#endif
#if UNITY_ANDROID
            Debug.Log("UNITY_ANDROID");
#endif
#if UNITY_PS4
            Debug.Log("UNITY_PS4");
#endif
#if UNITY_SAMSUNGTV
            Debug.Log("UNITY_SAMSUNGTV");
#endif
#if UNITY_XBOXONE
            Debug.Log("UNITY_XBOXONE");
#endif
#if UNITY_TIZEN
            Debug.Log("UNITY_TIZEN");
#endif
#if UNITY_TVOS
            Debug.Log("UNITY_TVOS");
#endif
#if UNITY_WSA
            Debug.Log("UNITY_WSA");
#endif
#if UNITY_WSA_10_0
            Debug.Log("UNITY_WSA_10_0");
#endif
#if UNITY_WINRT
            Debug.Log("UNITY_WINRT");
#endif
#if UNITY_WINRT_10_0
            Debug.Log("UNITY_WINRT_10_0");
#endif
#if UNITY_WEBGL
            Debug.Log("UNITY_WEBGL");
#endif
#if UNITY_FACEBOOK
            Debug.Log("UNITY_FACEBOOK");
#endif
#if UNITY_ADS
            Debug.Log("UNITY_ADS");
#endif
#if UNITY_ANALYTICS
            Debug.Log("UNITY_ANALYTICS");
#endif
#if UNITY_ASSERTIONS
            Debug.Log("UNITY_ASSERTIONS");
#endif
        }
    }
}