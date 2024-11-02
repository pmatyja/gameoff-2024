using System;
using UnityEngine;

namespace OCSFX.Utility.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class MinMaxRangeAttribute : PropertyAttribute
    {
        public float MinLimit;
        public float MaxLimit;

        public float CenterClampValue { get; private set; }

        public bool ClampCenter { get; private set; }

        public MinMaxRangeAttribute(float minLimit, float maxLimit, float centerClampValue, bool clampCenter = true)
        {
            this.MinLimit = minLimit;
            this.MaxLimit = maxLimit;
            this.ClampCenter = clampCenter;
            this.CenterClampValue = centerClampValue;
        }
    
        public MinMaxRangeAttribute(float minLimit, float maxLimit)
        {
            this.MinLimit = minLimit;
            this.MaxLimit = maxLimit;
        }
    }
}
