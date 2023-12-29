using UnityEngine;
using UnityEngine.Rendering;

public class TerrainFace
{
    private Mesh mesh;
    private int resolution;
    private Vector3 localUp;

    private Vector3 axisA;
    private Vector3 axisB;

    public TerrainFace(Mesh mesh, int resolution, Vector3 localUp)
    {
        this.mesh = mesh;
        this.resolution = resolution;
        this.localUp = localUp;

        axisA = new Vector3(localUp.y, localUp.z, localUp.x);
        axisB = Vector3.Cross(localUp, axisA);
    }

    public void ConstructMesh()
    {
        Vector3[] vertices = new Vector3[resolution * resolution];
        int[] triangles = new int[(resolution - 1) * (resolution - 1) * 6];
        int triangleIndex = 0;

        for (int index = 0; index < resolution * resolution; index++)
        {
            int x = index % resolution;
            int y = index / resolution;

            Vector2 percent = new Vector2(x, y) / (resolution - 1);
            Vector3 pointOnUnitCube = localUp + (percent.x - .5f) * 2 * axisA + (percent.y - .5f) * 2 * axisB;
            Vector3 pointOnUnitSphere = pointOnUnitCube.normalized;
            vertices[index] = pointOnUnitSphere;

            if (x != resolution - 1 && y != resolution - 1)
            {
                triangles[triangleIndex++] = index;
                triangles[triangleIndex++] = index + resolution + 1;
                triangles[triangleIndex++] = index + resolution;

                triangles[triangleIndex++] = index;
                triangles[triangleIndex++] = index + 1;
                triangles[triangleIndex++] = index + resolution + 1;
            }
        }


        mesh.Clear();

        mesh.SetVertexBufferParams(vertices.Length, new VertexAttributeDescriptor(VertexAttribute.Position));
        mesh.SetVertexBufferData(vertices, 0, 0, vertices.Length);

        mesh.SetIndexBufferParams(triangles.Length, IndexFormat.UInt32);
        mesh.SetIndexBufferData(triangles, 0, 0, triangles.Length);

        mesh.subMeshCount = 1;
        var descriptor = new SubMeshDescriptor(0, triangles.Length, MeshTopology.Triangles);
        mesh.SetSubMesh(0, descriptor, MeshUpdateFlags.DontRecalculateBounds | MeshUpdateFlags.DontValidateIndices);

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
    }
}
