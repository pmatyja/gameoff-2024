using System.Collections.Generic;
using UnityEngine;

public class OutlineRenderer : MonoBehaviour
{
    [SerializeField]
    private GameObject outlinePrefab;

    [SerializeField]
    private Material outlineMaterial;

    [SerializeField]
    private Vector3 offset = new(-0.5f, 0.03f, 0.5f);
    
    private readonly List<GameObject> outlines = new();

    public void Clear()
    {
        foreach (var outline in this.outlines)
        {
            GameObject.Destroy(outline);
        }

        this.outlines.Clear();
    }

    public void Add(List<Vector3> points, Material material = null, float layer = 0.0f)
    {
        if (points == null)
        {
            return;
        }

        this.transform.CreateChild($"Area[{this.outlines.Count}]");

        var obj = GameObject.Instantiate(this.outlinePrefab, this.offset + new Vector3(0.0f, layer, 0.0f), Quaternion.identity, this.transform);

        obj.transform.SetParent(this.transform);

        var spline = obj.GetComponent<SplineRenderer>();

        spline.Material = material ?? this.outlineMaterial;
        spline.ClosedLoop = true;
        spline.Points = points;

        this.outlines.Add(obj);
    }
}