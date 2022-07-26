using System.Collections.Generic;
using UnityEngine;
using Dongpn.ProtoBuf;

public class ModuleHandler : MonoBehaviour
{
    private static Dictionary<string, BaseController> _controllers = new Dictionary<string, BaseController>();

    public static T ProtoBuf<T>() where T : BaseController, new()
    {
        string key = typeof(T).ToString();
        T rt;
        if (_controllers.ContainsKey(key))
        {
            rt = (T)_controllers[key];

        }
        else
        {
            rt = new T();
            _controllers.Add(key, new T());
        }

        rt.Innit();
        return rt;
    }
}
