using System;
using UnityEngine;

namespace Runtime.Utility
{
    [AttributeUsage(AttributeTargets.Field)]
    public class TagAttribute : PropertyAttribute
    {
    }
    
    [AttributeUsage(AttributeTargets.Field)]
    public class TagMaskAttribute : PropertyAttribute { }
}