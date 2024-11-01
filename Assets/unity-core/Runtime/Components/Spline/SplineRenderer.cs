using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class SplineRenderer : MonoBehaviour
{
    public Material Material;

    private List<Vector3> points = new();
    public List<Vector3> Points
    {
        get => this.points;
        set => this.SetPoints(value);
    }

    [SerializeField]
    private bool closedLoop = true;
    public bool ClosedLoop
    {
        get => this.closedLoop;
        set
        {
            this.closedLoop = value;
            this.SetPoints(this.points);
        }
    }

    public void Clear()
    {
        this.points.Clear();
            
        var lineRenderer = this.GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
    }

    public void Add(Vector3 point)
    {
        this.points.Add(point);
        this.SetPoints(this.points);
    }

    public void SetPoints(List<Vector3> points)
    {
        this.points = points ?? new List<Vector3>();

        if (this.points == null)
        {
            return;
        }

        List<Vector3> smoothed = new();

        if (this.ClosedLoop)
        {
            Spline.Open(this.points, smoothed);
        }
        else
        {
            Spline.Closed(this.points, smoothed);
        }

        var lineRenderer = this.GetComponent<LineRenderer>();

        lineRenderer.material = this.Material;
        lineRenderer.loop = this.closedLoop;
        lineRenderer.positionCount = smoothed.Count;

        for (var i = 0; i < smoothed.Count; ++i)
        {
            lineRenderer.SetPosition(i, smoothed[i]);
        }
    }

    private void OnValidate()
    {
        this.SetPoints(this.points);
    }
}