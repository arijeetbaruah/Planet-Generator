using Sirenix.OdinInspector;
using UnityEngine;

public class Planet : MonoBehaviour
{
    [Range(2, 256)]
    public int resolution = 10;

    [SerializeField, AssetsOnly] private Material material;

    [SerializeField, HideInInspector] private MeshFilter[] meshFilters;
    private TerrainFace[] terrainFaces;

    [SerializeField] private ConfigRegistry configRegistry;

    private ColourSettings colourSettings;
    private ShapeSettings shapeSettings;

    public bool isAutoUpdate;

    [Button("Generate Mesh")]
    public void GenerateMeshButton()
    {
        Initialize();
        GenerateMesh();
        GenerateColours();
    }

    public void Initialize()
    {
        if (!configRegistry.TryGetConfig(out colourSettings) || !configRegistry.TryGetConfig(out shapeSettings))
        {
            return;
        }

        if (meshFilters == null || meshFilters.Length == 0)
        {
            meshFilters = new MeshFilter[6];
        }
        terrainFaces = new TerrainFace[6];

        Vector3[] directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };

        for (int i = 0; i < 6; i++)
        {
            if (meshFilters[i] == null)
            {
                GameObject meshObj = new GameObject("mesh");
                meshObj.transform.parent = transform;
                meshObj.AddComponent<MeshRenderer>().sharedMaterial = material;
                meshFilters[i] = meshObj.AddComponent<MeshFilter>();
                meshFilters[i].sharedMesh = new Mesh();
            }

            terrainFaces[i] = new TerrainFace(meshFilters[i].sharedMesh, resolution, directions[i]);
        }
    }

    public void GenerateMesh()
    {
        foreach (TerrainFace face in terrainFaces)
        {
            face.ConstructMesh();
        }
    }

    public void GenerateColours()
    {
        foreach (MeshFilter meshFilter in meshFilters)
        {
            meshFilter.GetComponent<MeshRenderer>().sharedMaterial.color = colourSettings.Data.planetColour;
        }
    }

    private void OnValidate()
    {
        if (isAutoUpdate)
        {
            GenerateMeshButton();
        }
    }
}
