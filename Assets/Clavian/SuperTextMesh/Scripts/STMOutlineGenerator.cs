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
    [Tooltip("Make sure this is ABOVE your STM in the hierarchy for UI objects!!")]
    public Transform outlineParent;
    [Range(0, 16)]
    public int detailLevel = 8;
    public float size = 0.05f;
    public Color32 color;
    [Tooltip("Offset text with this")]
    public Vector3 offset = new Vector3(0f, 0f, 0.01f);
    public bool updateEveryFrame = true;
    //public float distance = 0.001f;

    private Mesh sharedOutlineMesh = null;
    private GameObject[] outlineObjects = new GameObject[0];

    [System.Serializable]
    public class OutlineRenderer {
        public Transform transform;
        public Vector3 offset;
        public GameObject gameObject;
        public MeshFilter meshFilter;
        public MeshRenderer meshRenderer;
        public CanvasRenderer canvasRenderer;
    }
    private OutlineRenderer[] allRenderers = new OutlineRenderer[0];

    private OutlineRenderer newRenderer;

    public void Reset() {
        superTextMesh = GetComponent<SuperTextMesh>();

    }
    public void OnEnable() {
        if (Application.isPlaying) {
            superTextMesh.OnRebuildEvent += GenerateOutlines;
        }
    }
    public void OnDisable() {
        if (Application.isPlaying) {
            superTextMesh.OnRebuildEvent -= GenerateOutlines;
        }
    }
    private bool validate;
    public void OnValidate() {
        validate = true;
    }
    public void Update() {
        if (!Application.isPlaying && validate) {
            validate = false;
            GenerateOutlines();
        }
    }
    public void LateUpdate() {
        if (Application.isPlaying && updateEveryFrame) {
            RefreshOutlines();
        }
    }
    public void GenerateOutlines(Vector3[] verts, Vector3[] middles, Vector3[] positions) {
        GenerateOutlines();
    }
    public void RefreshOutlines() {
        for (int i = 0; i < detailLevel; i++) {
            if(allRenderers[i] == null) {
                continue;
            }

            if (superTextMesh.uiMode && allRenderers[i].canvasRenderer !=  null) {
                allRenderers[i].canvasRenderer.SetMesh(superTextMesh.textMesh);
            } else if (allRenderers[i].meshFilter != null) {
                CloneTextMesh();
                allRenderers[i].meshFilter.sharedMesh = sharedOutlineMesh;
            }
        }
    }
    private void CloneTextMesh() {
        sharedOutlineMesh = new Mesh();
        if (superTextMesh.textMesh == null)
            return;

        sharedOutlineMesh.vertices = superTextMesh.textMesh.vertices;
        sharedOutlineMesh.triangles = superTextMesh.textMesh.triangles;
        sharedOutlineMesh.normals = superTextMesh.textMesh.normals;
        sharedOutlineMesh.uv = superTextMesh.textMesh.uv;
        sharedOutlineMesh.uv2 = superTextMesh.textMesh.uv2;
        //colors
        colors = superTextMesh.textMesh.colors32;
        for (int j = 0; j < colors.Length; j++) {
            colors[j] = color; //assign outline color
        }
        sharedOutlineMesh.colors32 = colors;
    }

    private Color32[] colors;
    public void GenerateOutlines() {
        if (superTextMesh != null && outlineParent != null) {
            sharedOutlineMesh = null; //clear last mesh

            for (int i = 0; i < outlineObjects.Length; i++) {
                if (Application.isPlaying) {
                    Destroy(outlineObjects[i]);
                } else {
                    DestroyImmediate(outlineObjects[i]);
                }
            }
            foreach (Transform child in outlineParent) {
                //destroy all
                if (Application.isPlaying) {
                    Destroy(child.gameObject);
                } else {
                    DestroyImmediate(child.gameObject);
                }
            }

            //recreate all
            allRenderers = new OutlineRenderer[detailLevel];
            outlineObjects = new GameObject[detailLevel];
            for (int i = 0; i < allRenderers.Length; i++) {
                //create outline gameobjects
                newRenderer = new OutlineRenderer();
                newRenderer.gameObject = new GameObject();
                outlineObjects[i] = newRenderer.gameObject;
                newRenderer.transform = newRenderer.gameObject.transform;
                newRenderer.transform.name = "Outline";
                newRenderer.transform.SetParent(outlineParent); //parent to STM
                newRenderer.transform.localScale = new Vector3(1f, 1f, 1f);
                // If we have a separate parent, update rotation
                if (newRenderer.transform.parent != superTextMesh.transform) {
                    newRenderer.transform.localRotation = superTextMesh.transform.rotation;
                } else {
                    // Keep rotation neutral
                    newRenderer.transform.localRotation = Quaternion.identity;
                }

                //UI text
                if (superTextMesh.uiMode && superTextMesh.c.GetMaterial() != null) {
                    newRenderer.canvasRenderer = newRenderer.gameObject.AddComponent<CanvasRenderer>();
                    newRenderer.canvasRenderer.SetMesh(superTextMesh.textMesh);
                    if (superTextMesh.c.GetMaterial().HasProperty("_MainTex")) {
                        newRenderer.canvasRenderer.SetTexture(superTextMesh.c.GetMaterial().GetTexture("_MainTex"));

                        newRenderer.canvasRenderer.materialCount = 1;
                        newRenderer.canvasRenderer.SetMaterial(superTextMesh.c.GetMaterial(), 0);
                    }
                    newRenderer.canvasRenderer.SetColor(color);
                }
                //regular text
                else if (superTextMesh.r.sharedMaterials != null) {

                    newRenderer.meshFilter = newRenderer.gameObject.AddComponent<MeshFilter>();
                    newRenderer.meshRenderer = newRenderer.gameObject.AddComponent<MeshRenderer>();
                    
                    // If this text has a sorting order component
                    if(GetComponent<STMChangeSortingOrder>() != null) {
                        newRenderer.meshRenderer.sortingOrder = GetComponent<STMChangeSortingOrder>().sortingOrder;
                    }

                    if (sharedOutlineMesh == null) {
                        CloneTextMesh();
                    }

                    newRenderer.meshFilter.sharedMesh = sharedOutlineMesh;
                    newRenderer.meshRenderer.sharedMaterials = superTextMesh.r.sharedMaterials;


                }
                //give it an offset... for now uhh
                newRenderer.offset.x = (superTextMesh.t.position.x + Mathf.Cos(Mathf.PI * 2f * ((float)i / detailLevel)) * size) + offset.x;
                newRenderer.offset.y = (superTextMesh.t.position.y + Mathf.Sin(Mathf.PI * 2f * ((float)i / detailLevel)) * size) + offset.y;
                newRenderer.offset.z = (superTextMesh.t.position.z) + offset.z;

                newRenderer.transform.position = newRenderer.offset;

                //assign to array
                allRenderers[i] = newRenderer;
            }
        }
    }
}

