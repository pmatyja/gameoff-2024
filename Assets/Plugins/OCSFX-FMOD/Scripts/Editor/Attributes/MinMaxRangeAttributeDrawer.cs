using System;
using UnityEngine;
using UnityEditor;
using OCSFX.Utility.Attributes;

namespace OCSFXEditor.Attributes
{
    [CustomPropertyDrawer(typeof(MinMaxRangeAttribute))]
    public class MinMaxRangeAttributeDrawer : PropertyDrawer
    {
        private float _minLimit;
        private float _maxLimit;
        
        private float _minValue;
        private float _maxValue;

        private bool _clampCenter;
        private float _centerValue;

        private Rect _sliderDrawArea;

        private float _clampRatio;

        private const float _SLIDER_PADDING = 6f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.Vector2)
            {
                EditorGUILayout.BeginHorizontal();
                
                EditorGUI.LabelField(position, label);
                EditorGUILayout.HelpBox("MinMaxRange attribute can only be used with Vector2", MessageType.Error);
                
                EditorGUILayout.EndHorizontal();
                return;
            }
            
            var minMaxRangeAttribute = (MinMaxRangeAttribute)attribute;

            _minLimit = minMaxRangeAttribute.MinLimit;
            _maxLimit = minMaxRangeAttribute.MaxLimit;
            
            _minValue = property.vector2Value.x;
            _maxValue = property.vector2Value.y;

            _clampCenter = minMaxRangeAttribute.ClampCenter;
            _centerValue = minMaxRangeAttribute.CenterClampValue;

            _clampRatio = Mathf.Abs(_centerValue - _minLimit) / ( Mathf.Abs(_maxLimit - _minLimit) );
            //20 / 21
            
            EditorGUI.BeginProperty(position, label, property);

            DrawMinMaxSlider(property, position,
                ref _minValue,
                ref _maxValue,
                _minLimit,
                _maxLimit);
            
            EditorGUI.EndProperty();
        }
        
        private void DrawMinMaxSlider(SerializedProperty vector2Property, Rect position,
        ref float min, ref float max, float minLimit, float maxLimit)
        {
            // EditorGUILayout.BeginHorizontal();
            
            // first determine the label drawing
            var xPos = position.xMin;
            var yPos = position.yMin;
            var width = position.size.x;
            var height = EditorGUIUtility.singleLineHeight;

            var labelDrawArea = new Rect(xPos, yPos, width, height);

            var propertyLabel = $"{vector2Property.displayName}";
            if (_clampCenter) propertyLabel += $" [Clamp: {_centerValue}]";
            
            EditorGUI.LabelField(labelDrawArea, propertyLabel);

            // reuse the same vars for drawing the actual slider
            xPos = position.xMin
                   + EditorGUIUtility.labelWidth
                   + (EditorGUI.indentLevel < 1
                       ? EditorGUIUtility.fieldWidth
                       : EditorGUIUtility.fieldWidth - (EditorGUI.indentLevel * EditorGUIUtility.fieldWidth * 0.3f))
                   + (_SLIDER_PADDING);
            yPos = position.yMin;
            width = position.width
                    - 2 * EditorGUIUtility.fieldWidth
                    - (EditorGUIUtility.labelWidth - (EditorGUI.indentLevel * 15f))
                    - 2 * (_SLIDER_PADDING);
            height = EditorGUIUtility.singleLineHeight;
            
            var sliderDrawArea = new Rect(xPos, yPos, width, height);
            _sliderDrawArea = sliderDrawArea;
            
            DrawFloatMinField(position, ref min);
            DrawFloatMaxField(position, ref max);
            
            EditorGUI.MinMaxSlider(sliderDrawArea,
                ref min,
                ref max, 
                minLimit,
                maxLimit);
            
            DrawCenterClampLine(position);

            Validate(ref min, ref max, minLimit, maxLimit);
            vector2Property.vector2Value = new Vector2(min, max);
            
            // EditorGUILayout.EndHorizontal();
        }

        private void DrawFloatMinField(Rect position, ref float value)
        {
            var xPos = GetMinFloatFieldMinX();
            var yPos = position.yMin;
            var width = EditorGUIUtility.fieldWidth + (EditorGUIUtility.fieldWidth * EditorGUI.indentLevel * 0.3f);
            var height = EditorGUIUtility.singleLineHeight;

            var drawArea = new Rect(xPos, yPos, width, height);
            var roundedValue = (float)Math.Round((decimal)value, 3); // Limits the number of decimals
            value = EditorGUI.FloatField(drawArea, roundedValue);
        }
        
        private void DrawFloatMaxField(Rect position, ref float value)
        {
            var xPos = position.xMax 
                       - EditorGUIUtility.fieldWidth
                       * (EditorGUI.indentLevel > 0 ? EditorGUI.indentLevel * 0.8f : 1);
            var yPos = position.yMin;
            var width = EditorGUIUtility.fieldWidth + (EditorGUIUtility.fieldWidth * 0.3f * EditorGUI.indentLevel);
            var height = EditorGUIUtility.singleLineHeight;
            
            var drawArea = new Rect(xPos, yPos, width, height);
            var roundedValue = (float)Math.Round((decimal)value, 3); // Limits the number of decimals
            value = EditorGUI.FloatField(drawArea, roundedValue);
        }
        
        private void DrawCenterClampLine(Rect position)
        {
            // This does not work correctly, so keep it commented-out / disabled for now.
            
            // if (!_clampCenter) return;
            //
            // var xPos = position.xMin + EditorGUIUtility.labelWidth + (_sliderDrawArea.width * _clampRatio) - 2f * (2f + (EditorGUI.indentLevel * 3.75f) - 0.5f);
            // var yPos = position.yMin;
            // var width = EditorGUIUtility.labelWidth;
            // var height = EditorGUIUtility.singleLineHeight;
            //
            // var drawArea = new Rect(xPos, yPos, width, height);
            // EditorGUI.LabelField(drawArea, "|");
        }

        private float GetMinFloatFieldMinX()
        {
            return EditorGUIUtility.labelWidth + EditorGUIUtility.fieldWidth * 0.38f
                - (EditorGUI.indentLevel * EditorGUIUtility.singleLineHeight)
                + (EditorGUI.indentLevel > 0 ? 0 : 2f);
        }

        private float GetMinFloatFieldWidth()
        {
            return EditorGUIUtility.fieldWidth 
                * (EditorGUI.indentLevel > 0 ? EditorGUI.indentLevel * 0.8f : 1);
        }
        
        private void Validate(ref float min, ref float max, float minLimit, float maxLimit)
        {
            // min = Mathf.Clamp(min, minLimit, maxLimit);
            // max = Mathf.Clamp(max, minLimit, maxLimit);

            if (min > max) min = Mathf.Clamp(min, minLimit, max);
            if (max < min) max = Mathf.Clamp(max, min, maxLimit);

            if (!_clampCenter) return;

            min = Mathf.Clamp(min, minLimit, _centerValue);
            max = Mathf.Clamp(max, _centerValue, maxLimit);
        }
    }
}
