using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Utilities {
    public static class UnityEngineExtensions
    {
        public static T GetOrAddComponent<T>(this GameObject mb) where T : Component
        {
            if (mb == null)
            {
                return null;
            }

            var comp = mb.GetComponent<T>();
            if (!comp)
            {
                comp = mb.AddComponent<T>();
            }
            return comp;
        }
    }
}



