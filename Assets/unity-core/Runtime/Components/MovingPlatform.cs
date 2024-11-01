using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField]
    [Range(0f, 1f)]
    private float minDistance = 0.1f;

    [SerializeField]
    [Range(0f, 5f)]
    private float speedInUnits = 0.1f;

    [SerializeField]
    [Range(0f, 10f)]
    private float waitTimeAtPoints;

    [SerializeField]
    private List<Vector3> points = new();

    private int currentPoint;
    private int direction = 1;

    
    private void OnEnable()
    {
        this.StartCoroutine(this.Move());
    }

    private void OnDisable()
    {
        this.StopAllCoroutines();
    }
    
    private IEnumerator Move()
    {
        this.points.Insert(0, this.transform.localPosition);

        while (this.isActiveAndEnabled)
        {
            if (this.points.Count > 1)
            {
                var index = Mathf.Clamp(this.currentPoint, 0, this.points.Count - 1);

                this.transform.localPosition = Vector3.MoveTowards(this.transform.localPosition, this.points[index], this.speedInUnits * Time.deltaTime);

                if (Vector3.Distance(this.transform.localPosition, this.points[index]) <= this.minDistance)
                {
                    if (this.waitTimeAtPoints > 0)
                    {
                        yield return Wait.Seconds(this.waitTimeAtPoints);
                    }

                    if (this.direction == 1)
                    {
                        if (this.currentPoint >= this.points.Count - 1)
                        {
                            this.direction = -1;
                        }
                    }
                    else
                    {
                        if (this.currentPoint <= 0)
                        {
                            this.direction = 1;
                        }
                    }

                    this.currentPoint += this.direction;
                }   
            }

            yield return null;   
        }
    }
}
