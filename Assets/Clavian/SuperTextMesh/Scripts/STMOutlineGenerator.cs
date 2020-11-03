using UnityEngine;
using System.Collections;

/*

Attach to UI STM object
Make SURE that outlineParent is a gameObject that's is above STM in the hierarchy for UI elements.
Script isn't perfect yet, sometimes gameobjects don't get deleted when they should, but it seems to work fine in Play mode.

*/

[ExecuteInEditMode]
public class STMOutlineGenerator : MonoBehaviour {

	public SuperTextMesh superTextMesh;
	public Transform outlineParent; //make sure this is ABOVE your STM in the hierarchy!!
	[Range(0,16)]
	public int detailLevel;
	public float size = 1f;
	public Color32 color;
	public bool updateEveryFrame = true;
	//public float distance = 0.001f;

	[System.Serializable]
	public class OutlineRenderer
	{
		public Transform transform;
		public Vector3 offset;
		public GameObject gameObject;
		public MeshFilter meshFilter;
		public MeshRenderer meshRenderer;
		public CanvasRenderer canvasRenderer;
	}
	private OutlineRenderer[] allRenderers = new OutlineRenderer[0];

	private OutlineRenderer newRenderer;


	public void OnEnable()
	{
		if(Application.isPlaying)
		{
			superTextMesh.OnRebuildEvent += GenerateOutlines;
		}
	}
	public void OnDisable()
	{
		if(Application.isPlaying)
		{
			superTextMesh.OnRebuildEvent -= GenerateOutlines;
		}
	}
	private bool validate;
	public void OnValidate()
	{
		validate = true;
	}
	public void Update()
	{	
		if(!Application.isPlaying && validate)
		{
			validate = false;
			GenerateOutlines();
		}
	}
	public void LateUpdate()
	{
		if(Application.isPlaying && updateEveryFrame)
		{
			RefreshOutlines();
		}
	}
	public void GenerateOutlines(Vector3[] verts, Vector3[] middles, Vector3[] positions)
	{
		GenerateOutlines();
	}
	public void RefreshOutlines()
	{	
		for(int i=0; i<detailLevel; i++)
		{
            if (allRenderers[i].canvasRenderer != null) {
                allRenderers[i].canvasRenderer.SetMesh(superTextMesh.textMesh);
            }
		}
	}
	public void GenerateOutlines()
	{
		if(superTextMesh != null && outlineParent != null)
		{
			foreach(Transform child in outlineParent)
			{
				//destroy all
				if(Application.isPlaying)
				{
					Destroy(child.gameObject);
				}
				else
				{
					DestroyImmediate(child.gameObject);
				}
			}

			//recreate all
			allRenderers = new OutlineRenderer[detailLevel];
			for(int i=0; i<allRenderers.Length; i++)
			{
				//create UI gameobjects
				newRenderer = new OutlineRenderer();
				newRenderer.gameObject = new GameObject();
				newRenderer.transform = newRenderer.gameObject.transform;
				newRenderer.transform.SetParent(outlineParent); //parent to STM
                newRenderer.transform.localScale = new Vector3(1f, 1f, 1f);
                newRenderer.transform.localRotation = superTextMesh.transform.rotation;

				if(superTextMesh.uiMode && superTextMesh.c.GetMaterial() != null)
				{
					newRenderer.canvasRenderer = newRenderer.gameObject.AddComponent<CanvasRenderer>();
					newRenderer.canvasRenderer.SetMesh(superTextMesh.textMesh);
					if(superTextMesh.c.GetMaterial().HasProperty("_MainTex"))
					{
						newRenderer.canvasRenderer.SetTexture(superTextMesh.c.GetMaterial().GetTexture("_MainTex"));
						
						newRenderer.canvasRenderer.materialCount = 1;
						newRenderer.canvasRenderer.SetMaterial(superTextMesh.c.GetMaterial(), 0);
					}
					newRenderer.canvasRenderer.SetColor(color);
				}
				else
				{
					//not yet
				}

				//give it an offset... for now uhh
				newRenderer.offset.x = superTextMesh.t.position.x + Mathf.Cos(Mathf.PI * 2f * ((float)i/detailLevel)) * size;
				newRenderer.offset.y = superTextMesh.t.position.y + Mathf.Sin(Mathf.PI * 2f * ((float)i/detailLevel)) * size;
				newRenderer.offset.z = superTextMesh.t.position.z;

				newRenderer.transform.position = newRenderer.offset;

				//assign to array
				allRenderers[i] = newRenderer;
			}
		}
	}
}
