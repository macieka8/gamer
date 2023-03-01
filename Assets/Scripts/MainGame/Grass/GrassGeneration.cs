using UnityEngine;
using UnityEngine.Rendering;

namespace gamer
{
    public class GrassGeneration : MonoBehaviour
    {
        [SerializeField] ComputeShader _computeShader;
        [SerializeField] Mesh _grassMesh;
        [SerializeField] Material _grassMaterial;
        [SerializeField] TerrainData _terrainData;
        [SerializeField] Terrain _terrain;

        [Header("Grass Options")]
        [SerializeField] int _grassBladesResolution;

        [Header("Light & Shadows")]
        [SerializeField] ShadowCastingMode _castShadows;
        [SerializeField] bool _receiveShadows;

        [Header("Grass Blade Options")]
        
        [Range(0.0f, 5.0f)]
        [SerializeField] float _minBladeHeight;
        [Range(0.0f, 5.0f)]
        [SerializeField] float _maxBladeHeight;
        
        [Range(-1.0f, 1.0f)]
        [SerializeField] float _minOffset;
        [Range(-1.0f, 1.0f)]
        [SerializeField] float _maxOffset;

        [SerializeField] float _bladeThicknessScale;

        int kernel;

        GraphicsBuffer _transformMatrixBuffer;
        GraphicsBuffer _worldUVBuffer;

        GraphicsBuffer _terrainHeightBuffer;
        
        GraphicsBuffer _grassTriangleBuffer;
        GraphicsBuffer _grassVertexBuffer;
        GraphicsBuffer _grassUVBuffer;
        MaterialPropertyBlock _propertyBlock;

        Bounds _bounds;

        void Start()
        {
            kernel = _computeShader.FindKernel("CalculateBladePositions");
            var grassBladesCount = _grassBladesResolution * _grassBladesResolution;

            CreateGrassBladeBuffers();

            // Get Terrain Height
            float[,] terrainHeights = _terrainData.GetInterpolatedHeights(
                0, 0,
                _grassBladesResolution, _grassBladesResolution,
                1f / _grassBladesResolution, 1f / _grassBladesResolution);

            _terrainHeightBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, terrainHeights.Length, sizeof(float));
            _terrainHeightBuffer.SetData(terrainHeights);
            
            _computeShader.SetBuffer(kernel, "_terrainHeightBuffer", _terrainHeightBuffer);

            // Create transformations structured buffer
            _transformMatrixBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, grassBladesCount, sizeof(float) * 16);
            _computeShader.SetBuffer(kernel, "_transformMatrices", _transformMatrixBuffer);

            _worldUVBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, grassBladesCount, sizeof(float) * 2);
            _computeShader.SetBuffer(kernel, "_worldUVBuffer", _worldUVBuffer);

            // Bind buffers to a MaterialPropertyBlock
            _propertyBlock = new MaterialPropertyBlock();
            _propertyBlock.SetBuffer("_transformMatrices", _transformMatrixBuffer);
            _propertyBlock.SetBuffer("_worldUVBuffer", _worldUVBuffer);
            _propertyBlock.SetBuffer("_positions", _grassVertexBuffer);
            _propertyBlock.SetBuffer("_UVs", _grassUVBuffer);

            // Bounds
            _bounds = new Bounds();
            _bounds.center = transform.position;
            _bounds.size = new Vector3(_terrainData.size.x, _terrainData.size.y, _terrainData.size.z);
            _bounds.Expand(_maxBladeHeight);

            BindComputeShaderVariables();

            _computeShader.GetKernelThreadGroupSizes(kernel, out var threadGroupSize, out _, out _);
            int threadGroups = Mathf.CeilToInt(grassBladesCount / threadGroupSize);
            _computeShader.Dispatch(kernel, threadGroups, 1, 1);
        }

        void Update()
        {
            Graphics.DrawProcedural(_grassMaterial, _bounds, MeshTopology.Triangles,
                _grassTriangleBuffer, _grassTriangleBuffer.count,
                instanceCount: _grassBladesResolution * _grassBladesResolution,
                properties: _propertyBlock,
                castShadows: _castShadows,
                receiveShadows: _receiveShadows);
        }

        void OnDestroy()
        {
            _transformMatrixBuffer.Dispose();
            _worldUVBuffer.Dispose();
            _terrainHeightBuffer.Dispose();

            _grassTriangleBuffer.Dispose();
            _grassVertexBuffer.Dispose();
            _grassUVBuffer.Dispose();
        }

        void BindComputeShaderVariables()
        {
            _computeShader.SetFloat("_grassTerrainLengthX", _terrainData.size.x);
            _computeShader.SetFloat("_grassTerrainLengthZ", _terrainData.size.z);
            _computeShader.SetInt("_grassBladesResolution", _grassBladesResolution);

            _computeShader.SetMatrix("_terrainObjectToWorld", transform.localToWorldMatrix);
            _computeShader.SetFloat("_minBladeHeight", _minBladeHeight);
            _computeShader.SetFloat("_maxBladeHeight", _maxBladeHeight);
            _computeShader.SetFloat("_minOffset", _minOffset);
            _computeShader.SetFloat("_maxOffset", _maxOffset);
            _computeShader.SetFloat("_bladeThicknessScale", _bladeThicknessScale);
        }

        void CreateGrassBladeBuffers()
        {
            Vector3[] grassVertices = _grassMesh.vertices;
            _grassVertexBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, grassVertices.Length, sizeof(float) * 3);
            _grassVertexBuffer.SetData(grassVertices);

            int[] grassTriangles = _grassMesh.triangles;
            _grassTriangleBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, grassTriangles.Length, sizeof(int));
            _grassTriangleBuffer.SetData(grassTriangles);

            Vector2[] grassUVs = _grassMesh.uv;
            _grassUVBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, grassUVs.Length, sizeof(float) * 2);
            _grassUVBuffer.SetData(grassUVs);
        }
    }
}
