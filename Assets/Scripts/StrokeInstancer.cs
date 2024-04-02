using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class StrokeInstancer : MonoBehaviour
{
    private GameObject sourceObject;
    private Mesh sourceMesh;
    public Mesh instancedMesh;
    public Material instancedMaterial;
    public float instanceScale;

    [Range(0, 1.0f)]
    public float coverage = 1.0f;

    private ComputeBuffer meshPropertiesBuffer;
    private ComputeBuffer argsBuffer;

    private Bounds bounds;

    // Mesh Properties struct to be read from the GPU.
    // Size() is a convenience funciton which returns the stride of the struct.
    private struct MeshProperties
    {
        //eventually, you should switch the mat4 with 3 float4s so that the matrix calculation happens in the gpu
        public Matrix4x4 mat;
        public Matrix4x4 inverseMat;
        public Vector3 foliageNormal;
        public float seed1;
        public float seed2;

        public static int Size()
        {
            return
                sizeof(float) * 4 * 4 * 2 + // matrix + matrix;
                sizeof(float) * 3 +
                sizeof(float) * 2;
        }
    }

    private void Setup()
    {
        instancedMaterial = new Material(instancedMaterial);
        Vector3 object_pos = sourceObject.transform.position;
        Vector3 object_scale = sourceObject.transform.localScale;
        //Cm_Collider = GetComponent<Collider>();
        bounds = new Bounds(object_pos, object_scale * 2.0f);
        bounds = GetComponent<Collider>().bounds;
        //sourceObject

        //Initialize buffers to be passed to gpu
        //InitializeBuffers();
    }

    private void InitializeBuffers()
    {
        int numSourceTriangles = sourceMesh.triangles.Length / 3;
        Debug.Log(numSourceTriangles);
        // Argument buffer used by DrawMeshInstancedIndirect.
        //uint[] args = new uint[5] { 0, 0, 0, 0, 0 };

        // Arguments for drawing mesh.
        // 0 == number of triangle indices, 1 == population, others are only relevant if drawing submeshes.
        //args[0] = (uint)instancedMesh.GetIndexCount(0);
        //args[1] = (uint)numSourceTriangles;
        //args[2] = (uint)instancedMesh.GetIndexStart(0);
        //args[3] = (uint)instancedMesh.GetBaseVertex(0);
        //argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
        //argsBuffer.SetData(args);

        // Initialize buffer with the given population.
        //MeshProperties[] properties = new MeshProperties[numSourceTriangles];

        int counter = 0;
        //int inv_counter = 0;
        int skip = numSourceTriangles;
        int inv_skip = 0;
        if (coverage < 1.0f && coverage > 0)
        {
            skip = (int)((coverage / (1.0f - coverage)));
            inv_skip = (int)(((1.0f - coverage) / coverage));
        }
        if (coverage == 0)
        {
            skip = 0;
            inv_skip = numSourceTriangles;
        }
        //Debug.Log(inv_skip);
        //we want 0.7 of the originals. This means we skip for every 7 we skip 3 so for every 
        //skip 1 every 3
        if (skip > 0)
        {
            for (int i = 0; i < numSourceTriangles; i++)
            {
                counter++;
                if (counter > skip)
                {
                    counter = 0;
                    continue;
                }
                MeshProperties props = new MeshProperties();
                //Read triangle number
                Vector3 triangleVert1 = sourceMesh.vertices[sourceMesh.triangles[i * 3]];
                Vector3 triangleVert2 = sourceMesh.vertices[sourceMesh.triangles[i * 3 + 1]];
                Vector3 triangleVert3 = sourceMesh.vertices[sourceMesh.triangles[i * 3 + 2]];
                //sourceMesh.get
                Vector3 localPosition = triangleVert1 + triangleVert2 + triangleVert3;
                localPosition /= 3.0f;

                float rand = 0.1f * Random.Range(-1.0f, 1.0f);

                //localPosition += new Vector3(rand, rand, rand);

                //Scale
                //position.y += 0.5f * instanceScale;

                //IMPORTANT//
                //Make sure y-scale is 1 if your source terrain is a flat object!!
                Vector3 position = sourceObject.transform.localToWorldMatrix * localPosition;
                position += sourceObject.transform.position;

                props.foliageNormal = Vector3.Normalize(position - sourceObject.transform.position);
                //localPosition;

                Quaternion rotation = Quaternion.identity;
                rotation = Quaternion.FromToRotation(new Vector3(0, 0, 1), props.foliageNormal);
                //Scale
                Vector3 scale = new Vector3(instanceScale, instanceScale, instanceScale);

                props.mat = Matrix4x4.TRS(position, rotation, scale);
                //props.inverseMat = props.mat.inverse;
                ////props.inverseMat = props.mat;

                //props.seed1 = Random.value;
                //props.seed2 = Random.value;

                //properties[i] = props;
            }
        }
        else
        {
            for (int i = 0; i < numSourceTriangles; i += inv_skip)
            {
                MeshProperties props = new MeshProperties();
                //Read triangle number
                Vector3 triangleVert1 = sourceMesh.vertices[sourceMesh.triangles[i * 3]];
                Vector3 triangleVert2 = sourceMesh.vertices[sourceMesh.triangles[i * 3 + 1]];
                Vector3 triangleVert3 = sourceMesh.vertices[sourceMesh.triangles[i * 3 + 2]];
                //sourceMesh.get
                Vector3 localPosition = triangleVert1 + triangleVert2 + triangleVert3;
                localPosition /= 3.0f;

                float rand = 0.1f * Random.Range(-1.0f, 1.0f);

                //localPosition += new Vector3(rand, rand, rand);

                //Scale
                //position.y += 0.5f * instanceScale;

                //IMPORTANT//
                //Make sure y-scale is 1 if your source terrain is a flat object!!
                Vector3 position = sourceObject.transform.localToWorldMatrix * localPosition;
                position += sourceObject.transform.position;

                props.foliageNormal = Vector3.Normalize(position - sourceObject.transform.position);
                //localPosition;

                Quaternion rotation = Quaternion.identity;
                rotation = Quaternion.FromToRotation(new Vector3(0, 0, 1), props.foliageNormal);
                //Scale
                Vector3 scale = new Vector3(instanceScale, instanceScale, instanceScale);

                props.mat = Matrix4x4.TRS(position, rotation, scale);
                //props.inverseMat = props.mat.inverse;
                ////props.inverseMat = props.mat;

                //props.seed1 = Random.value;
                //props.seed2 = Random.value;

                //properties[i] = props;
            }
        }

        //meshPropertiesBuffer = new ComputeBuffer(numSourceTriangles, MeshProperties.Size());
        //meshPropertiesBuffer.SetData(properties);
        //instancedMaterial.SetBuffer("_Properties", meshPropertiesBuffer);
    }

    // Start is called before the first frame update
    void OnEnable()
    {
        sourceObject = this.gameObject;
        sourceMesh = sourceObject.GetComponent<MeshFilter>().sharedMesh;
        //Debug.Log("sourceMesh assigned");
        Setup();
    }

    // Update is called once per frame
    void Update()
    {
        //InitializeBuffers();
        //Graphics.DrawMeshInstancedIndirect(instancedMesh, 0, instancedMaterial, bounds, argsBuffer);

        int numSourceTriangles = sourceMesh.triangles.Length / 3;

        //for (int i = 0; i < 1; i++)
        //{
        //    //Debug.Log("BRUHH\n");
        //}
    }

    private void OnDisable()
    {
        if (meshPropertiesBuffer != null)
        {
            meshPropertiesBuffer.Release();
        }
        meshPropertiesBuffer = null;

        if (argsBuffer != null)
        {
            argsBuffer.Release();
        }
        argsBuffer = null;
    }
}

