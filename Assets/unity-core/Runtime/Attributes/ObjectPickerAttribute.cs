using System;

public class ObjectPickerAttribute : BaseAttribute
{
    public readonly Type ObjectType;
    public readonly string ObjectTypeSource;
    public readonly bool AllowSceneObjects;
    public readonly string SearchFilter;

    public ObjectPickerAttribute(Type objectType, bool allowSceneObjects = true, string searchFilter = "")
    {
        this.ObjectType = objectType;
        this.AllowSceneObjects = allowSceneObjects;
        this.SearchFilter = searchFilter;
    }

    public ObjectPickerAttribute(string objectTypeSource, bool allowSceneObjects = true, string searchFilter = "")
    {
        this.ObjectTypeSource = objectTypeSource;
        this.AllowSceneObjects = allowSceneObjects;
        this.SearchFilter = searchFilter;
    }
}