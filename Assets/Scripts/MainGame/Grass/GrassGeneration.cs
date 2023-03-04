using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace gamer
{
    public class GrassGeneration : MonoBehaviour
    {
        public struct GrassData
        {
            public Matrix4x4 transformMatrix;
            public Vector2 worldUV;
        }

        [SerializeField] ComputeShader _computeShader;
        [SerializeField] Mesh _grassMesh;
        [SerializeField] Material _grassMaterial;
        [SerializeField] TerrainData _terrainData;
        [SerializeField] Terrain _terrain;

        [Header("Grass Options")]
        [SerializeField] int _grassBladesResolution;
        [SerializeField] float _emptyCenterDistance;
        [SerializeField] float _distanceCutoff;

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
        
        GraphicsBuffer _grassVertexBuffer;
        GraphicsBuffer _grassUVBuffer;
        MaterialPropertyBlock _propertyBlock;

        Bounds _bounds;

        // Culling Data
        [Header("Culling Data")]
        [SerializeField] ComputeShader _cullGrassShader;

        GraphicsBuffer _voteBuffer;
        GraphicsBuffer _scanBuffer;
        GraphicsBuffer _groupSumArrayBuffer;
        GraphicsBuffer _scannedGroupSumBuffer;

        GraphicsBuffer _culledtransformMatrixBuffer;

        GraphicsBuffer _argsBuffer;
        uint[] _args;

        int _numThreadGroups;
        int _numVoteThreadGroups;
        int _numGroupScanThreadGroups;

        void Start()
        {
            kernel = _computeShader.FindKernel("CalculateBladePositions");

            CreateGrassBladeBuffers();

            CreateGrassBuffers();

            // Bounds
            _bounds = new Bounds();
            _bounds.center = transform.position;
            _bounds.size = new Vector3(_terrainData.size.x, _terrainData.size.y, _terrainData.size.z);
            _bounds.Expand(_maxBladeHeight);

            BindComputeShaderVariables();

            _computeShader.GetKernelThreadGroupSizes(kernel, out var threadGroupSize, out _, out _);
            int threadGroups = Mathf.CeilToInt(_transformMatrixBuffer.count / threadGroupSize);
            _computeShader.Dispatch(kernel, threadGroups, 1, 1);

            CreateCullGrassBuffers();

            // Bind buffers to a MaterialPropertyBlock
            _propertyBlock = new MaterialPropertyBlock();
            _propertyBlock.SetBuffer("_CulledGrassOutputBuffer", _culledtransformMatrixBuffer);
            _propertyBlock.SetBuffer("_positions", _grassVertexBuffer);
            _propertyBlock.SetBuffer("_UVs", _grassUVBuffer);
        }

        void Update()
        {
            Matrix4x4 P = Camera.main.projectionMatrix;
            Matrix4x4 V = Camera.main.transform.worldToLocalMatrix;
            var VP = P * V;

            CullGrass(VP);
            Graphics.DrawMeshInstancedIndirect(_grassMesh, 0, _grassMaterial, _bounds, _argsBuffer,
                properties: _propertyBlock,
                castShadows: _castShadows,
                receiveShadows: _receiveShadows);
        }

        void OnDestroy()
        {
            _transformMatrixBuffer.Dispose();
            _worldUVBuffer.Dispose();
            _terrainHeightBuffer.Dispose();

            _grassVertexBuffer.Dispose();
            _grassUVBuffer.Dispose();

            _voteBuffer.Dispose();
            _scanBuffer.Dispose();
            _groupSumArrayBuffer.Dispose();
            _scannedGroupSumBuffer.Dispose();
            _culledtransformMatrixBuffer.Dispose();

            _argsBuffer.Dispose();
        }

        void CreateGrassBuffers()
        {
            var emptyDistanceSquared = _emptyCenterDistance * _emptyCenterDistance;
            var resolutionSquared = _grassBladesResolution * _grassBladesResolution;
            var worldUVs = new List<Vector2>(resolutionSquared);
            var centerOfMapPosition = new Vector2(_terrainData.size.x / 2f, _terrainData.size.z / 2f);

            // Generate worldUVs
            for (int i = 0; i < resolutionSquared; i++)
            {
                var coords = new Vector2(i % _grassBladesResolution, i / _grassBladesResolution);
                var uv = coords / _grassBladesResolution;
                var position = new Vector2(uv.x * _terrainData.size.x, uv.y * _terrainData.size.z);
                var distanceToCenterSquared = (centerOfMapPosition - position).sqrMagnitude;

                if (distanceToCenterSquared > emptyDistanceSquared)
                {
                    worldUVs.Add(uv);
                }
            }

            // Get terrain heights
            var terrainHeights = new float[worldUVs.Count];
            for (int i = 0; i < worldUVs.Count; i++)
            {
                terrainHeights[i] = _terrainData.GetInterpolatedHeight(worldUVs[i].x, worldUVs[i].y);
            }

            // Create world UV buffer
            _worldUVBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, terrainHeights.Length, sizeof(float) * 2);
            _worldUVBuffer.SetData(worldUVs.ToArray());
            _computeShader.SetBuffer(kernel, "_worldUVBuffer", _worldUVBuffer);

            // Create heights buffer
            _terrainHeightBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, terrainHeights.Length, sizeof(float));
            _terrainHeightBuffer.SetData(terrainHeights);
            _computeShader.SetBuffer(kernel, "_terrainHeightBuffer", _terrainHeightBuffer);

            // Create transformations buffer
            _transformMatrixBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, terrainHeights.Length, sizeof(float) * 16);
            _computeShader.SetBuffer(kernel, "_transformMatrices", _transformMatrixBuffer);

            _culledtransformMatrixBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, terrainHeights.Length, sizeof(float) * (16 + 2));

            worldUVs.Clear();
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
            _computeShader.SetInt("_grassbladesCount", _transformMatrixBuffer.count);
        }

        void CreateGrassBladeBuffers()
        {
            Vector3[] grassVertices = _grassMesh.vertices;
            _grassVertexBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, grassVertices.Length, sizeof(float) * 3);
            _grassVertexBuffer.SetData(grassVertices);

            Vector2[] grassUVs = _grassMesh.uv;
            _grassUVBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, grassUVs.Length, sizeof(float) * 2);
            _grassUVBuffer.SetData(grassUVs);
        }

        void CreateCullGrassBuffers()
        {
            var grassBladesCount = _worldUVBuffer.count;
            _numThreadGroups = Mathf.CeilToInt(grassBladesCount / 128.0f);
            if (_numThreadGroups > 128)
            {
                int powerOfTwo = 128;
                while (powerOfTwo < _numThreadGroups)
                    powerOfTwo *= 2;

                _numThreadGroups = powerOfTwo;
            }
            else
            {
                while (128 % _numThreadGroups != 0)
                    _numThreadGroups++;
            }

            _numVoteThreadGroups = Mathf.CeilToInt(grassBladesCount / 128.0f);
            _numGroupScanThreadGroups = Mathf.CeilToInt(grassBladesCount / 1024.0f);

            _voteBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, _worldUVBuffer.count, sizeof(uint));
            _scanBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, _worldUVBuffer.count, sizeof(uint));
            _groupSumArrayBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, _numThreadGroups, sizeof(uint));
            _scannedGroupSumBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, _numThreadGroups, sizeof(uint));

            _args = new uint[5] { 0u, 0u, 0u, 0u, 0u };
            _args[0] = _grassMesh.GetIndexCount(0);
            _args[1] = 0u;
            _args[2] = _grassMesh.GetIndexStart(0);
            _args[3] = _grassMesh.GetBaseVertex(0);

            _argsBuffer = new GraphicsBuffer(GraphicsBuffer.Target.IndirectArguments, 1, sizeof(uint) * _args.Length);
            _argsBuffer.SetData(_args);

            CullGrassSetup();
        }

        void CullGrassSetup()
        {
            _cullGrassShader.SetBuffer(4, "_ArgsBuffer", _argsBuffer);

            // Vote
            _cullGrassShader.SetBuffer(0, "_transformMatrices", _transformMatrixBuffer);
            _cullGrassShader.SetBuffer(0, "_VoteBuffer", _voteBuffer);
            _cullGrassShader.SetFloat("_Distance", _distanceCutoff);

            // Scan Instances
            _cullGrassShader.SetBuffer(1, "_VoteBuffer", _voteBuffer);
            _cullGrassShader.SetBuffer(1, "_ScanBuffer", _scanBuffer);
            _cullGrassShader.SetBuffer(1, "_GroupSumArray", _groupSumArrayBuffer);

            // Scan Groups
            _cullGrassShader.SetInt("_NumOfGroups", _numThreadGroups);
            _cullGrassShader.SetBuffer(2, "_GroupSumArrayIn", _groupSumArrayBuffer);
            _cullGrassShader.SetBuffer(2, "_GroupSumArrayOut", _scannedGroupSumBuffer);

            // Compact
            _cullGrassShader.SetBuffer(3, "_transformMatrices", _transformMatrixBuffer);
            _cullGrassShader.SetBuffer(3, "_worldUV", _worldUVBuffer);
            _cullGrassShader.SetBuffer(3, "_VoteBuffer", _voteBuffer);
            _cullGrassShader.SetBuffer(3, "_ScanBuffer", _scanBuffer);
            _cullGrassShader.SetBuffer(3, "_ArgsBuffer", _argsBuffer);
            _cullGrassShader.SetBuffer(3, "_CulledGrassOutputBuffer", _culledtransformMatrixBuffer);
            _cullGrassShader.SetBuffer(3, "_GroupSumArray", _scannedGroupSumBuffer);
        }

        void CullGrass(Matrix4x4 VP)
        {
            // Reset instance count
            _cullGrassShader.Dispatch(4, 1, 1, 1);

            // Vote
            _cullGrassShader.SetMatrix("MATRIX_VP", VP);
            _cullGrassShader.SetVector("_CameraPosition", Camera.main.transform.position);
            _cullGrassShader.Dispatch(0, _numVoteThreadGroups, 1, 1);

            // Scan Instances
            _cullGrassShader.Dispatch(1, _numThreadGroups, 1, 1);

            // Scan Groups
            _cullGrassShader.Dispatch(2, _numGroupScanThreadGroups, 1, 1);

            // Compact
            _cullGrassShader.Dispatch(3, _numThreadGroups, 1, 1);
        }
    }
}
