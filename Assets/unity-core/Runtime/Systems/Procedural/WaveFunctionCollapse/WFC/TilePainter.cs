using System.IO;
using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(BoxCollider))]
public class TilePainter : MonoBehaviour
{
	public int gridsize = 1;
	public int width = 20;
	public int height = 20;
	public GameObject tiles;
	private bool _changed = true;
	public Vector3 cursor;
	public bool focused;
	public GameObject[,] tileobs;
	

	int colidx;
	public List<Object> palette = new();
	public Object color;
	Quaternion color_rotation;

	
#if UNITY_EDITOR

	private static bool IsAssetAFolder(Object obj){
		var path = "";
		if (obj == null){return false;}
		path = AssetDatabase.GetAssetPath(obj.GetInstanceID());
		if (path.Length > 0){
		if (Directory.Exists(path)){
			return true;
		}else{
			return false;}
		}
	 	return false;
	}
 

	public void Encode(){

	} 

	static GameObject CreatePrefab(Object fab, Vector3 pos, Quaternion rot) {	
		var o = PrefabUtility.InstantiatePrefab(fab as GameObject) as GameObject; 
		if (o == null){
			Debug.Log(IsAssetAFolder(fab));
			return o;}
		o.transform.position = pos;
		o.transform.rotation = rot;
		return o;
	}

	public void Restore(){

		var palt = this.transform.Find("palette");
		if (palt != null){GameObject.DestroyImmediate(palt.gameObject);}
		var pal = new GameObject("palette");
		pal.hideFlags = HideFlags.HideInHierarchy;
		var bc = pal.AddComponent<BoxCollider>();
		bc.size = new Vector3(this.palette.Count* this.gridsize, this.gridsize, 0f);
		bc.center = new Vector3((this.palette.Count-1f)* this.gridsize*0.5f, 0f, 0f);

		pal.transform.parent = this.gameObject.transform;
		pal.transform.localPosition = new Vector3(0f, -this.gridsize*2, 0f);
		pal.transform.rotation = this.transform.rotation;


		
		var palette_folder = -1;

		for (var i = 0; i < this.palette.Count; i++){
			var o = this.palette[i];
			if (IsAssetAFolder(o)){
				palette_folder = i;
			}
			else {
				if (o != null){
					var g = CreatePrefab(o, new Vector3() , this.transform.rotation);
					g.transform.parent = pal.transform;
					g.transform.localPosition = new Vector3(i* this.gridsize, 0f, 0f);
				}
			}
		}

		if (palette_folder != -1){
			var path = AssetDatabase.GetAssetPath(this.palette[palette_folder].GetInstanceID());
			path = path.Trim().Replace("Assets/Resources/", "");
			this.palette.RemoveAt(palette_folder);
			var contents = (Object[]) Resources.LoadAll(path);
			foreach (var o in contents){
				if (!this.palette.Contains(o)){
					this.palette.Add(o);}
			}

			this.Restore();
		}

		this.tileobs = new GameObject[this.width, this.height];
		if (this.tiles == null){
			this.tiles = new GameObject("tiles");
			this.tiles.transform.parent = this.gameObject.transform;
			this.tiles.transform.localPosition = new Vector3();
		}
		var cnt = this.tiles.transform.childCount;
		var trash = new List<GameObject>();
		for (var i = 0; i < cnt; i++){
			var tile = this.tiles.transform.GetChild(i).gameObject;
			var tilepos = tile.transform.localPosition;
			var X = (int)(tilepos.x / this.gridsize);
			var Y = (int)(tilepos.y / this.gridsize);
			if (this.ValidCoords(X, Y)){
				this.tileobs[X, Y] = tile; 
			} else {
				trash.Add(tile);
			}
		}
		for (var i = 0; i < trash.Count; i++){
			if (Application.isPlaying){Destroy(trash[i]);} else {DestroyImmediate(trash[i]);}}	

		if (this.color == null){
			if (this.palette.Count > 0){
				this.color = this.palette[0];
			}
		}
	}

	public void Resize(){
		this.transform.localScale = new Vector3(1,1,1);
		if (this._changed){
			this._changed = false;
			this.Restore(); 
		}
	}

	public void Awake(){
		this.Restore();
	}

	public void OnEnable(){
		this.Restore();
	}

	void OnValidate(){
		this._changed = true;
		var bounds = this.GetComponent<BoxCollider>();
		bounds.center = new Vector3((this.width* this.gridsize)*0.5f- this.gridsize*0.5f, (this.height* this.gridsize)*0.5f- this.gridsize*0.5f, 0f);
		bounds.size = new Vector3(this.width* this.gridsize, (this.height* this.gridsize), 0f);
	}

	public Vector3 GridV3(Vector3 pos){
		var p = this.transform.InverseTransformPoint(pos) + new Vector3(this.gridsize*0.5f, this.gridsize*0.5f, 0f);
		return new Vector3((int)(p.x/ this.gridsize), (int)(p.y/ this.gridsize), 0);
	}

	public bool ValidCoords(int x, int y){
		if (this.tileobs == null) {return false;}
		
		return (x >= 0 && y >= 0 && x < this.tileobs.GetLength(0) && y < this.tileobs.GetLength(1));
	}


	public void CycleColor(){
		this.colidx += 1;
		if (this.colidx >= this.palette.Count){
			this.colidx = 0;
		}

		this.color = (Object)this.palette[this.colidx];
	}

	public void Turn(){
		if (this.ValidCoords((int)this.cursor.x, (int)this.cursor.y)){
			var o = this.tileobs[(int)this.cursor.x, (int)this.cursor.y];
			if (o != null){
				o.transform.Rotate(0f, 0f, 90f);
			}
		}
	}

	public Vector3 Local(Vector3 p){
		return this.transform.TransformPoint(p);
	}

	public Object PrefabSource(GameObject o){
        if (o == null)
        {
            return null;
        }
        Object fab = PrefabUtility.GetCorrespondingObjectFromSource(o);
        if (fab == null)
        {
            fab = Resources.Load(o.name);
        }
        if (fab == null)
        {
            fab = this.palette[0];
        }
        return fab;
    }

	public void Drag(Vector3 mouse, TileLayerEditor.TileOperation op){
		this.Resize();
		if (this.tileobs == null){
			this.Restore();}
		if (this.ValidCoords((int)this.cursor.x, (int)this.cursor.y)){
			if (op == TileLayerEditor.TileOperation.Sampling){
				var s = this.PrefabSource(this.tileobs[(int)this.cursor.x, (int)this.cursor.y]);
                Debug.Log(s);
				if (s != null){
					this.color = s;
					this.color_rotation = this.tileobs[(int)this.cursor.x, (int)this.cursor.y].transform.localRotation;
				}
			} else {
				DestroyImmediate(this.tileobs[(int)this.cursor.x, (int)this.cursor.y]); 
				if (op == TileLayerEditor.TileOperation.Drawing){
					if (this.color == null){return;}
					var o = CreatePrefab(this.color, new Vector3() , this.color_rotation);
					o.transform.parent = this.tiles.transform;
					o.transform.localPosition = (this.cursor* this.gridsize);
					o.transform.localRotation = this.color_rotation;
					this.tileobs[(int)this.cursor.x, (int)this.cursor.y] = o;
				}
			}
		} else {
			if (op == TileLayerEditor.TileOperation.Sampling){
				if (this.cursor.y == -1 && this.cursor.x >= 0 && this.cursor.x < this.palette.Count){
					this.color = this.palette[(int)this.cursor.x];
					this.color_rotation = Quaternion.identity;
				}
			}
		}
	}

	public void Clear(){
		this.tileobs = new GameObject[this.width, this.height];
		DestroyImmediate(this.tiles);
		this.tiles = new GameObject("tiles");
		this.tiles.transform.parent = this.gameObject.transform;
		this.tiles.transform.localPosition = new Vector3();
	}

	public void OnDrawGizmos(){
		Gizmos.color = Color.white;
		Gizmos.matrix = this.transform.localToWorldMatrix;
		if (this.focused){
			Gizmos.color = new Color(1f,0f,0f,0.6f);
			Gizmos.DrawRay((this.cursor* this.gridsize)+Vector3.forward*-49999f, Vector3.forward*99999f);
			Gizmos.DrawRay((this.cursor* this.gridsize)+Vector3.right*-49999f, Vector3.right*99999f);
			Gizmos.DrawRay((this.cursor* this.gridsize)+Vector3.up*-49999f, Vector3.up*99999f);
			Gizmos.color = Color.yellow;
		}

		Gizmos.DrawWireCube( new Vector3((this.width* this.gridsize)*0.5f- this.gridsize*0.5f, (this.height* this.gridsize)*0.5f- this.gridsize*0.5f, 0f),
			new Vector3(this.width* this.gridsize, (this.height* this.gridsize), 0f));
	}
	#endif
}
 

#if UNITY_EDITOR
 [CustomEditor(typeof(TilePainter))]
 public class TileLayerEditor : Editor{
	public enum TileOperation {None, Drawing, Erasing, Sampling};
	private TileOperation operation;

	public override void OnInspectorGUI () {
		var me = (TilePainter)this.target;
		GUILayout.Label("Assign a prefab to the color property"); 
		GUILayout.Label("or the pallete array.");
		GUILayout.Label("drag        : paint tiles");
		GUILayout.Label("[s]+click  : sample tile color");
		GUILayout.Label("[x]+drag  : erase tiles");
		GUILayout.Label("[space]    : rotate tile");
		GUILayout.Label("[b]          : cycle color");
		if(GUILayout.Button("CLEAR")){
			me.Clear();}

		this.DrawDefaultInspector();}

	private bool AmHovering(Event e){
		var me = (TilePainter)this.target;
		RaycastHit hit;
		if (Physics.Raycast(HandleUtility.GUIPointToWorldRay(Event.current.mousePosition), out hit, Mathf.Infinity) && 
				hit.collider.GetComponentInParent<TilePainter>() == me)
		{
			me.cursor = me.GridV3(hit.point);
			me.focused = true;

			var rend = me.gameObject.GetComponentInChildren<Renderer>( );
			if( rend ) EditorUtility.SetSelectedRenderState( rend, EditorSelectedRenderState.Wireframe );
			return true;
		}
		me.focused = false;
		return false;
	}

	public void ProcessEvents(){
		var me = (TilePainter)this.target;
		var controlID = GUIUtility.GetControlID(1778, FocusType.Passive);
		var currentWindow = EditorWindow.mouseOverWindow;
		if(currentWindow && this.AmHovering(Event.current)){
			var current = Event.current;
			var leftbutton = (current.button == 0);
			switch(current.type){
				case EventType.KeyDown:

					if (current.keyCode == KeyCode.S) this.operation = TileOperation.Sampling;
					if (current.keyCode == KeyCode.X) this.operation = TileOperation.Erasing;
					current.Use();
					return;
				case EventType.KeyUp:
					this.operation = TileOperation.None;
					if (current.keyCode == KeyCode.Space) me.Turn();
					if (current.keyCode == KeyCode.B) me.CycleColor();
					current.Use();
					return;
				case EventType.MouseDown:
					if (leftbutton)
					{
						if (this.operation == TileOperation.None){
							this.operation = TileOperation.Drawing;
						}
						me.Drag(current.mousePosition, this.operation);

						current.Use();
						return;
					}
					break;
				case EventType.MouseDrag:
					if (leftbutton)
					{
						if (this.operation != TileOperation.None){
							me.Drag(current.mousePosition, this.operation);
							current.Use();
						}
						
						return;
					}
					break;
				case EventType.MouseUp:
					if (leftbutton)
					{
						this.operation = TileOperation.None;
						current.Use();
						return;
					}
				break;
				case EventType.MouseMove:
					me.Resize();
					current.Use();
				break;
				case EventType.Repaint:
				break;
				case EventType.Layout:
				HandleUtility.AddDefaultControl(controlID);
				break;
			}
		}
	}
	
	void OnSceneGUI (){
		this.ProcessEvents();
	}

	 void DrawEvents(){
		 Handles.BeginGUI();
		 Handles.EndGUI();
	 }}
#endif


