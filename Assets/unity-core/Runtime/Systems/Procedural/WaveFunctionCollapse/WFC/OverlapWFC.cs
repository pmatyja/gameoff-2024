using System;
using UnityEngine;

[ExecuteInEditMode]
class OverlapWFC : MonoBehaviour
{
	public Transform Root;
	public PrefabGroupSO Group;

    [Header("Settings")]
	public float gridsize = 2.5f;
	public int sizeX = 20;
	public int sizeZ = 20;
	public int seed;
	public int N = 2;
	public bool periodicInput;
	public bool periodicOutput;
	public int symmetry = 1;
	public int foundation;
	public int iterations;
	public bool incremental;
	public OverlappingModel model;

    private bool undrawn = true;
	private GameObject[,] objects;
	private byte[,] sample;

	public void Clear()
	{
		if (this.Root != null)
		{
			if (Application.isPlaying)
			{
				foreach (Transform child in this.Root)
				{
					Destroy(child.gameObject);
				}
			}
			else
			{
				foreach (Transform child in this.Root)
				{
					DestroyImmediate(child.gameObject);
				}
			}
		}
	}

	public void Generate()
	{
		this.sample = new byte[this.sizeX, this.sizeZ];
		this.objects = new GameObject[this.sizeX, this.sizeZ];
		this.model = new OverlappingModel(this.sample, this.N, this.sizeX, this.sizeZ, this.periodicInput, this.periodicOutput, this.symmetry, this.foundation);
		this.undrawn = true;
    }

	public void Run()
	{
		if (this.model == null)
		{
			return;
		}

        if (this.undrawn == false)
		{ 
			return;
		}

        if (this.model.Run(this.seed, this.iterations))
		{
			this.Draw();
		}
	}

	public void Draw()
	{
		if (this.Root == null)
		{
			return;
		}

		if (this.Group == null)
		{
			return;
		}

		this.undrawn = false;

		try
		{
			for (var y = 0; y < this.sizeZ; y++)
			{
				for (var x = 0; x < this.sizeX; x++)
				{
					if (this.objects[x,y] == null)
					{
						this.model.Sample(x, y);

						var prefab = default(PrefabSO);
						if (prefab != null)
						{
							var rotation = 0; // (int)training.RS[v];

							this.objects[x,y] = this.Root.CreateChild
							(
								prefab.Model,
								new Vector3(x * this.gridsize, y * this.gridsize, 0f),
								Quaternion.AngleAxis(rotation * prefab.RotationStep, prefab.RotationAxis),
								prefab.Scale,
								prefab.Pivot,
								prefab.Layer
							);
						}
					}
				}
	  		}
	  	} 
		catch (IndexOutOfRangeException)
		{
			this.model = null;
	  		return;
	  	}
	}

	private void Start()
	{
		this.Generate();
	}

	private void Update()
	{
		if (this.incremental)
		{
			this.Run();
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.cyan;
		Gizmos.matrix = this.transform.localToWorldMatrix;
		Gizmos.DrawWireCube(new Vector3(this.sizeX * this.gridsize / 2f - this.gridsize * 0.5f, 0f, this.sizeZ * this.gridsize / 2f - this.gridsize * 0.5f),
							new Vector3(this.sizeX * this.gridsize, this.gridsize, this.sizeZ * this.gridsize));
	}
}