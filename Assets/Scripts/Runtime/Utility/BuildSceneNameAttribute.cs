using System;
using UnityEngine;

namespace Runtime.Utility
{
    [AttributeUsage(AttributeTargets.Field)]
    public class BuildSceneNameAttribute : PropertyAttribute
    {
        public BuildSceneNameAttribute() { }
    }
}