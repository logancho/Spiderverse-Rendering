using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DrawBrushstrokes : MonoBehaviour
{
    private int population;
    //public float range;

    public Material material;
    public RenderTexture paintBuffer;
    public RenderTexture normalsBuffer;
    public RenderTexture depthBuffer;
    public RenderTexture colorBuffer;
    public RenderTexture outputBufferTest;
    public RenderTexture seenBuffer;
    public RenderTexture seedBuffer;
    public ComputeShader compute;
    //public Camera mainCamera;
    //public Transform pusher;

    private ComputeBuffer meshPropertiesBuffer;
    private ComputeBuffer argsBuffer;

    private Mesh mesh;
    private Bounds bounds;

    private struct MeshProperties
    {
        public Matrix4x4 mat;
        public Vector2 UV;
        public Vector4 color;
        public float scaleFactor; //[0, ?]
        public float seed; //[0, 1]
        public float opacity;

        public static int Size()
        {
            return
                sizeof(float) * 4 * 4 + // matrix;
                sizeof(float) * 4 + // color vec4
                sizeof(float) * 2 + // vector2 for UV
                sizeof(float) + //float for scaleFactor
                sizeof(float) + //float for seed
                sizeof(float); //float for opacity
        }
    }

    private void Setup()
    {
        Mesh mesh = CreateQuad();
        this.mesh = mesh;

        
        population = paintBuffer.width * paintBuffer.height / 25;

        outputBufferTest.enableRandomWrite = true;
        outputBufferTest.Create();
        //outputBufferTest.
        // Boundary surrounding the meshes we will be drawing.  Used for occlusion.
        bounds = new Bounds(Vector3.zero, Vector3.one * (1000));
        InitializeBuffers();
        Debug.Log("hi");
    }

    private void InitializeBuffers()
    {
        int kernel = compute.FindKernel("PaintMain");

        // Argument buffer used by DrawMeshInstancedIndirect.
        uint[] args = new uint[5] { 0, 0, 0, 0, 0 };
        // Arguments for drawing mesh.
        // 0 == number of triangle indices of instanced mesh, 1 == population, others are only relevant if drawing submeshes.
        args[0] = (uint)mesh.GetIndexCount(0);
        args[1] = (uint)population;
        args[2] = (uint)mesh.GetIndexStart(0);
        args[3] = (uint)mesh.GetBaseVertex(0);
        argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
        argsBuffer.SetData(args);

        // Initialize buffer with the given population.
        MeshProperties[] properties = new MeshProperties[population];
        Debug.Log(population);

        
        for (int r = 0; r < paintBuffer.height / 5.0f; r++)
        {
            for (int c = 0; c < paintBuffer.width / 5.0f; c++)
            {
                int idx = (int)(r * (paintBuffer.width / 5.0f) + c);
                //Debug.Log(idx);
                MeshProperties props = new MeshProperties();
                //Vector3 position = new Vector3(Random.Range(-range, range), Random.Range(-range, range), Random.Range(-range, range));
                Vector3 position = new Vector3(c, r, -10);
                Quaternion rotation = Quaternion.Euler(Random.Range(-180, 180), Random.Range(-180, 180), Random.Range(-180, 180));
                rotation = Quaternion.identity;
                Vector3 scale = Vector3.one;

                props.mat = Matrix4x4.TRS(position, rotation, scale);
                props.UV = new Vector2((float)c / (paintBuffer.width / 5.0f), (float)r / (paintBuffer.height / 5.0f));

                props.color = Vector4.one;
                props.opacity = 1.0f;
                //Debug.Log(idx);
                //Debug.Log(props.UV);
                //props.color = Color.Lerp(Color.red, Color.blue, Random.value);
                props.scaleFactor = 1.0f;
                if (idx < population)
                {
                    properties[idx] = props;
                } else
                {
                    Debug.Log(idx);
                    Debug.Log("BRUHHHHHH");
                }
            }
        }

        meshPropertiesBuffer = new ComputeBuffer(population, MeshProperties.Size());
        meshPropertiesBuffer.SetData(properties);

        compute.SetBuffer(kernel, "_Properties", meshPropertiesBuffer);
        material.SetBuffer("_Properties", meshPropertiesBuffer);
    }

    // Start is called before the first frame update
    void Start()
    {
        Setup();
    }

    // Update is called once per frame
    void Update()
    {
        int kernel = compute.FindKernel("PaintMain");

        //compute.SetVector("_PusherPosition", pusher.position);
        compute.SetTexture(kernel, "_PaintBuffer", paintBuffer);
        compute.SetTexture(kernel, "_NormalsBuffer", normalsBuffer);
        compute.SetTexture(kernel, "_DepthBuffer", depthBuffer);
        compute.SetTexture(kernel, "_ColorBuffer", colorBuffer);
        compute.SetTexture(kernel, "_Result", outputBufferTest);
        compute.SetTexture(kernel, "_SeenBuffer", seenBuffer);
        compute.SetTexture(kernel, "_SeedBuffer", seedBuffer);

        //depthToWorldShader.SetMatrix("_InvViewMatrix", _camera.cameraToWorldMatrix);
        //depthToWorldShader.SetMatrix("_InvProjectionMatrix", _camera.projectionMatrix.inverse);
        //compute.SetTextureFromGlobal(kernel, "_CameraDepthAttachment", "_CameraDepthAttachment");
        //compute.SetTexture(kernel, "_DepthTexture", Shader.GetGlobalTexture("_CameraDepthAttachment"));

        //if (Shader.GetGlobalTexture("_CameraDepthAttachment"))
        //{
        //    Debug.Log("hello\n");
        //    compute.SetTextureFromGlobal(kernel, "_DepthBuffer", "_CameraDepthAttachment");
        //}

        //mainCamera.projectionMatrix.inverse
        compute.SetMatrix("_invViewMat", Camera.main.cameraToWorldMatrix); //game camera
        compute.SetMatrix("_invProjectionMatrix", Camera.main.projectionMatrix.inverse);

        compute.SetFloat("_CameraNearPlane", Camera.main.nearClipPlane);
        compute.SetFloat("_CameraFarPlane", Camera.main.farClipPlane);
        //Debug.Log(mainCamera.nearClipPlane);
        //Debug.Log(mainCamera.farClipPlane);

        //Debug.Log(mainCamera.cameraToWorldMatrix);
        //_CameraDepthAttachment fs     1920x1080 Tex2D         None D32_SFloat_S8_UInt   _CameraDepthAttachment_1920x1080_Depth_MSAA8x

        //Camera main;
        //main.cameraToWorldMatrix

        //compute.SetTextureFromGlobal(kernel, "_Result", outputBufferTest);

        // We used to just be able to use `population` here, but it looks like a Unity update imposed a thread limit (65535) on my device.
        // This is probably for the best, but we have to do some more calculation.  Divide population by numthreads.x in the compute shader.
        //paintBuffer.col
        //outputBufferTest.Release();
        compute.Dispatch(kernel, Mathf.CeilToInt(population / 64f), 1, 1);
        Graphics.DrawMeshInstancedIndirect(mesh, 0, material, bounds, argsBuffer);
    }

    private Mesh CreateQuad(float width = 1f, float height = 1f)
    {
        // Create a quad mesh.
        var mesh = new Mesh();

        float w = width * .5f;
        float h = height * .5f;
        var vertices = new Vector3[4] {
            new Vector3(-w, -h, 0),
            new Vector3(w, -h, 0),
            new Vector3(-w, h, 0),
            new Vector3(w, h, 0)
        };

        var tris = new int[6] {
            // lower left tri.
            0, 2, 1,
            // lower right tri
            2, 3, 1
        };

        var normals = new Vector3[4] {
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
        };

        var uv = new Vector2[4] {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1),
        };

        mesh.vertices = vertices;
        mesh.triangles = tris;
        mesh.normals = normals;
        mesh.uv = uv;

        return mesh;
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
