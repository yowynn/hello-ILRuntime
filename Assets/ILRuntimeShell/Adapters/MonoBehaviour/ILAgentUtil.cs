using ILRuntime.Runtime.Intepreter;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ILRuntime.Reflection;

using System.Reflection;
using UnityEditor;
using ILRuntime.CLR.TypeSystem;
using ILRuntime.Runtime.Stack;
using ILRuntime.CLR.Method;

namespace Assets.ILRuntimeShell.Adapters.MonoBehaviour
{
    public static class ILAgentUtil
    {
        private static object Deserialize(Type type, ref ILData data, ref int index)
        {
            var node = data.GetNode(ref index);
            var tag = node.Tag;
            switch (tag)
            {
                case ILDataTag.Generic:
                    throw new NotImplementedException();

                case ILDataTag.Integer:
                    return Convert.ChangeType(node.Value.longValue, Type.GetType(type.FullName));

                case ILDataTag.Boolean:
                    return node.Value.boolValue;

                case ILDataTag.Float:
                    return Convert.ChangeType(node.Value.floatValue, Type.GetType(type.FullName));

                case ILDataTag.String:
                    return data.GetString(node);

                case ILDataTag.Color:
                    return node.Value.colorValue;

                case ILDataTag.ObjectReference:
                    return data.GetObject(node);

                case ILDataTag.LayerMask:
                    throw new NotImplementedException();
                case ILDataTag.Enum:
                    return Enum.ToObject(type, node.Value.intValue);

                case ILDataTag.Vector2:
                    return node.Value.vector2Value;

                case ILDataTag.Vector3:
                    return node.Value.vector3Value;

                case ILDataTag.Vector4:
                    return node.Value.vector4Value;

                case ILDataTag.Rect:
                    return node.Value.rectValue;

                case ILDataTag.ArraySize:
                    throw new NotImplementedException();
                case ILDataTag.Character:
                    throw new NotImplementedException();
                case ILDataTag.AnimationCurve:
                    return data.GetCurve(node);

                case ILDataTag.Bounds:
                    return new Bounds(node.Value.vector3Value, data.GetNode(ref index).Value.vector3Value);

                case ILDataTag.Gradient:
                    throw new NotImplementedException();
                case ILDataTag.Quaternion:
                    return node.Value.quaternionValue;

                case ILDataTag.ExposedReference:
                    throw new NotImplementedException();
                case ILDataTag.FixedBufferSize:
                    throw new NotImplementedException();
                case ILDataTag.Vector2Int:
                    return node.Value.vector2IntValue;

                case ILDataTag.Vector3Int:
                    return node.Value.vector3IntValue;

                case ILDataTag.RectInt:
                    return node.Value.rectIntValue;

                case ILDataTag.BoundsInt:
                    return new BoundsInt(node.Value.vector3IntValue, data.GetNode(ref index).Value.vector3IntValue);

                case ILDataTag.ManagedReference:
                    throw new NotImplementedException();
                case ILDataTag.Array:
                    var arraySize = node.Value.intValue;
                    if (type.IsArray)
                    {
                        var elementType = type.GetElementType();
                        var array = Array.CreateInstance(elementType, arraySize);
                        for (int i = 0; i < arraySize; ++i)
                            array.SetValue(Deserialize(elementType, ref data, ref index), i);
                        return array;
                    }
                    else
                    {
                        var elementType = type.GetGenericArguments()[0];
                        var list = Activator.CreateInstance(type) as IList;
                        for (int i = 0; i < arraySize; ++i)
                            list.Add(Deserialize(elementType, ref data, ref index));
                        return list;
                    }
                case ILDataTag.PlaceHolder:
                    throw new NotImplementedException();
                case ILDataTag.Other:
                    int fieldCount = node.Value.intValue;
                    if (type.UnderlyingSystemType == typeof(ILTypeInstance))
                    {
                        var ins = ((ILRuntimeType)type).ILType.Instantiate();
                        for (int i = 0; i < fieldCount; ++i)
                        {
                            var name = data.GetNode(index).Name;
                            var field = type.GetField(name);
                            var fieldType = field?.FieldType ?? typeof(object);
                            var value = Deserialize(fieldType, ref data, ref index);
                            field?.SetValue(ins, value);
                        }
                        return ins;
                    }
                    else
                    {
                        var obj = Activator.CreateInstance(type, true);
                        for (int i = 0; i < fieldCount; ++i)
                        {
                            var name = data.GetNode(index).Name;
                            var field = type.GetField(name);
                            var fieldType = field?.FieldType ?? typeof(object);
                            var value = Deserialize(fieldType, ref data, ref index);
                            field?.SetValue(obj, value);
                        }
                        return obj;
                    }
                default:
                    throw new NotImplementedException();
            }
        }

        private static void Serialize(SerializedProperty property, ref ILData data)
        {
            var type = property.propertyType;
            switch (type)
            {
                case SerializedPropertyType.Generic:
                    if (property.isArray)
                    {
                        var node = data.AddNode(property.name, ILDataTag.Array);
                        for (int i = 0; i < property.arraySize; ++i)
                            Serialize(property.GetArrayElementAtIndex(i), ref data);
                        node.Value = new ILDataVal { longValue = property.arraySize };
                    }
                    else
                    {
                        var node = data.AddNode(property.name, ILDataTag.Other);
                        var fieldCount = 0;
                        property = property.Copy();
                        var depth = property.depth;
                        while (property.Next(property.depth == depth))
                        {
                            if (property.depth <= depth)
                                break;
                            ++fieldCount;
                            Serialize(property, ref data);
                        }
                        node.Value = new ILDataVal { intValue = fieldCount };
                    }
                    break;

                case SerializedPropertyType.Integer:
                    data.AddNode(property.name, ILDataTag.Integer).Value = new ILDataVal { longValue = property.longValue };
                    break;

                case SerializedPropertyType.Boolean:
                    data.AddNode(property.name, ILDataTag.Boolean).Value = new ILDataVal { boolValue = property.boolValue };
                    break;

                case SerializedPropertyType.Float:
                    data.AddNode(property.name, ILDataTag.Float).Value = new ILDataVal { doubleValue = property.doubleValue };
                    break;

                case SerializedPropertyType.String:
                    data.SetString(data.AddNode(property.name, ILDataTag.String), property.stringValue);
                    break;

                case SerializedPropertyType.Color:
                    data.AddNode(property.name, ILDataTag.Color).Value = new ILDataVal { colorValue = property.colorValue };
                    break;

                case SerializedPropertyType.ObjectReference:
                    data.SetObject(data.AddNode(property.name, ILDataTag.ObjectReference), property.objectReferenceValue);
                    break;

                case SerializedPropertyType.LayerMask:
                    throw new NotImplementedException();
                case SerializedPropertyType.Enum:
                    data.AddNode(property.name, ILDataTag.Enum).Value = new ILDataVal { intValue = property.enumValueIndex };
                    break;

                case SerializedPropertyType.Vector2:
                    data.AddNode(property.name, ILDataTag.Vector2).Value = new ILDataVal { vector2Value = property.vector2Value };
                    break;

                case SerializedPropertyType.Vector3:
                    data.AddNode(property.name, ILDataTag.Vector3).Value = new ILDataVal { vector3Value = property.vector3Value };
                    break;

                case SerializedPropertyType.Vector4:
                    data.AddNode(property.name, ILDataTag.Vector4).Value = new ILDataVal { vector4Value = property.vector4Value };
                    break;

                case SerializedPropertyType.Rect:
                    data.AddNode(property.name, ILDataTag.Rect).Value = new ILDataVal { rectValue = property.rectValue };
                    break;

                case SerializedPropertyType.ArraySize:
                    throw new NotImplementedException();
                case SerializedPropertyType.Character:
                    throw new NotImplementedException();
                case SerializedPropertyType.AnimationCurve:
                    data.SetCurve(data.AddNode(property.name, ILDataTag.AnimationCurve), property.animationCurveValue);
                    break;

                case SerializedPropertyType.Bounds:
                    data.AddNode(property.name, ILDataTag.Bounds).Value = new ILDataVal { vector3Value = property.boundsValue.center };
                    data.AddNode(property.name, ILDataTag.PlaceHolder).Value = new ILDataVal { vector3Value = property.boundsValue.size };
                    break;

                case SerializedPropertyType.Gradient:
                    throw new NotImplementedException();
                case SerializedPropertyType.Quaternion:
                    data.AddNode(property.name, ILDataTag.Quaternion).Value = new ILDataVal { quaternionValue = property.quaternionValue };
                    break;

                case SerializedPropertyType.ExposedReference:
                    throw new NotImplementedException();
                case SerializedPropertyType.FixedBufferSize:
                    throw new NotImplementedException();
                case SerializedPropertyType.Vector2Int:
                    data.AddNode(property.name, ILDataTag.Vector2Int).Value = new ILDataVal { vector2IntValue = property.vector2IntValue };
                    break;

                case SerializedPropertyType.Vector3Int:
                    data.AddNode(property.name, ILDataTag.Vector3Int).Value = new ILDataVal { vector3IntValue = property.vector3IntValue };
                    break;

                case SerializedPropertyType.RectInt:
                    data.AddNode(property.name, ILDataTag.RectInt).Value = new ILDataVal { rectIntValue = property.rectIntValue };
                    break;

                case SerializedPropertyType.BoundsInt:
                    data.AddNode(property.name, ILDataTag.BoundsInt).Value = new ILDataVal { vector3IntValue = property.boundsIntValue.position };
                    data.AddNode(property.name, ILDataTag.PlaceHolder).Value = new ILDataVal { vector3IntValue = property.boundsIntValue.size };
                    break;

                case SerializedPropertyType.ManagedReference:
                    throw new NotImplementedException();
                default:
                    throw new NotImplementedException();
            }
        }

        public static void ILDeserialize(this ILTypeInstance instance, ILData data)
        {
            var type = instance.Type.ReflectionType;
            int index = 0;
            while (index < data.Nodes.Count)
            {
                var name = data.GetNode(index).Name;
                var field = type.GetField(name);
                var fieldType = field?.FieldType ?? typeof(object);
                var value = Deserialize(fieldType, ref data, ref index);
                field?.SetValue(instance, value);
            }
        }

        public static void ILSerialize(this SerializedObject obj, ILData data, bool ONLY_VISIBLE = true)
        {
            data.Initialize();
            SerializedProperty property = obj.GetIterator();
            while (ONLY_VISIBLE ? property.NextVisible(property.depth == -1) : property.Next(property.depth == -1))
            {
                Serialize(property, ref data);
            }
        }

        public static void WhiteToMonoBehaviour(ILAgent agent, UnityEngine.MonoBehaviour behaviour)
        {
            var type = behaviour.GetType();
            var data = agent.ILData;
            int index = 0;
            while (index < data.Nodes.Count)
            {
                var name = data.GetNode(index).Name;
                var field = type.GetField(name);
                var fieldType = field?.FieldType ?? typeof(object);
                var value = Deserialize(fieldType, ref data, ref index);
                field?.SetValue(behaviour, value);
            }
        }

        public static void WhiteToScriptableObject(ILAgent agent, ScriptableObject behaviour)
        {
            var type = behaviour.GetType();
            var data = agent.ILData;
            int index = 0;
            while (index < data.Nodes.Count)
            {
                var name = data.GetNode(index).Name;
                var field = type.GetField(name);
                var fieldType = field?.FieldType ?? typeof(object);
                var value = Deserialize(fieldType, ref data, ref index);
                field?.SetValue(behaviour, value);
            }
        }

        public static void ReadFromMonoBehaviour(ILAgent agent, UnityEngine.MonoBehaviour behaviour)
        {
            var serialized = new SerializedObject(behaviour);
            agent.ILData = agent.ILData ?? new ILData();
            ILSerialize(serialized, agent.ILData);
            ILDeserialize(agent.ILInstance, agent.ILData);
        }

        public static ILAgent CreateILAgent(UnityEngine.MonoBehaviour behaviour, GameObject go = null)
        {
            if (behaviour.GetType() == typeof(ILAPP))
                return null;
            var typeName = behaviour.GetType().FullName;
            var type = ILAPP.GetILRuntimeType(typeName);
            var iltype = type as ILType;
            if (iltype != null)
            {
                go = go ?? behaviour.gameObject;
                var agent = go.AddComponent<ILAgent>();
                var instance = iltype.Instantiate();
                instance.CLRInstance = agent;
                agent.ILInstance = instance;
                agent.ILType = typeName;
                ReadFromMonoBehaviour(agent, behaviour);
                return agent;
            }
            return null;
        }

        public static UnityEngine.MonoBehaviour CreateMonoBehaviour(ILAgent agent, GameObject go = null)
        {
            go = go ?? agent.gameObject;
            //var type = agent.ILInstance.Type.ReflectionType;
            var type = Type.GetType(agent.ILType);
            var behaviour = go.AddComponent(type) as UnityEngine.MonoBehaviour;
            if (behaviour != null)
            {
                WhiteToMonoBehaviour(agent, behaviour);
                return behaviour;
            }
            return null;
        }

        private static unsafe StackObject* AddComponent(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            //CLR重定向的说明请看相关文档和教程，这里不多做解释
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;

            var ptr = __esp - 1;
            //成员方法的第一个参数为this
            GameObject instance = StackObject.ToObject(ptr, __domain, __mStack) as GameObject;
            if (instance == null)
                throw new System.NullReferenceException();
            __intp.Free(ptr);

            var genericArgument = __method.GenericArguments;
            //AddComponent应该有且只有1个泛型参数
            if (genericArgument != null && genericArgument.Length == 1)
            {
                var type = genericArgument[0];
                object res;
                if (type is CLRType)
                {
                    //Unity主工程的类不需要任何特殊处理，直接调用Unity接口
                    res = instance.AddComponent(type.TypeForCLR);
                }
                else
                {
                    //热更DLL内的类型比较麻烦。首先我们得自己手动创建实例
                    var ilInstance = new ILTypeInstance(type as ILType, false);//手动创建实例是因为默认方式会new MonoBehaviour，这在Unity里不允许
                                                                               //接下来创建Adapter实例
                    var clrInstance = instance.AddComponent<ILAgent>();
                    //unity创建的实例并没有热更DLL里面的实例，所以需要手动赋值
                    clrInstance.ILInstance = ilInstance;
                    //这个实例默认创建的CLRInstance不是通过AddComponent出来的有效实例，所以得手动替换
                    ilInstance.CLRInstance = clrInstance;

                    res = clrInstance.ILInstance;//交给ILRuntime的实例应该为ILInstance

                    clrInstance.CallInitialUnityMessage();//因为Unity调用这个方法时还没准备好所以这里补调一次
                }

                return ILIntepreter.PushObject(ptr, __mStack, res);
            }

            return __esp;
        }

        private static unsafe StackObject* GetComponent(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            //CLR重定向的说明请看相关文档和教程，这里不多做解释
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;

            var ptr = __esp - 1;
            //成员方法的第一个参数为this
            GameObject instance = StackObject.ToObject(ptr, __domain, __mStack) as GameObject;
            if (instance == null)
                throw new System.NullReferenceException();
            __intp.Free(ptr);

            var genericArgument = __method.GenericArguments;
            //AddComponent应该有且只有1个泛型参数
            if (genericArgument != null && genericArgument.Length == 1)
            {
                var type = genericArgument[0];
                object res = null;
                if (type is CLRType)
                {
                    //Unity主工程的类不需要任何特殊处理，直接调用Unity接口
                    res = instance.GetComponent(type.TypeForCLR);
                }
                else
                {
                    //因为所有DLL里面的MonoBehaviour实际都是这个Component，所以我们只能全取出来遍历查找
                    var clrInstances = instance.GetComponents<ILAgent>();
                    for (int i = 0; i < clrInstances.Length; i++)
                    {
                        var clrInstance = clrInstances[i];
                        if (clrInstance.ILInstance != null)//ILInstance为null, 表示是无效的MonoBehaviour，要略过
                        {
                            if (clrInstance.ILInstance.Type == type)
                            {
                                res = clrInstance.ILInstance;//交给ILRuntime的实例应该为ILInstance
                                break;
                            }
                        }
                    }
                }

                return ILIntepreter.PushObject(ptr, __mStack, res);
            }
            return __esp;
        }

        public static unsafe void RegisterCLRMethodRedirections(ILRuntime.Runtime.Enviorment.AppDomain domain)
        {
            var arr = typeof(GameObject).GetMethods();
            foreach (var i in arr)
            {
                if (i.Name == "AddComponent" && i.GetGenericArguments().Length == 1)
                    domain.RegisterCLRMethodRedirection(i, AddComponent);

                if (i.Name == "GetComponent" && i.GetGenericArguments().Length == 1)
                    domain.RegisterCLRMethodRedirection(i, GetComponent);
            }
        }
    }
}