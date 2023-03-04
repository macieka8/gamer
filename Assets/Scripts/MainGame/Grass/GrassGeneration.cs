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

        public struct ChunkData
        {
            public GraphicsBuffer argsBuffer;
            public GraphicsBuffer transformMatrixBuffer;
            public GraphicsBuffer worldUVsBuffer; 
            public GraphicsBuffer terrainHeightBuffer;
            public GraphicsBuffer culledtransformMatrixBuffer;
            public Bounds bounds;
        }

        [SerializeField] ComputeShader _computeShader;
        [SerializeField] Mesh _grassMesh;
        [SerializeField] Material _grassMaterial;
        [SerializeField] TerrainData _terrainData;
        [SerializeField] Terrain _terrain;

        [Header("Grass Options")]
        [SerializeField] float _distanceCutoff;

        [Header("Chunk Options")]
        [SerializeField] int _chunkGrassResolution = 128;
        [SerializeField] int _chunksPerDimension = 2;

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

        [Header("Culling Data")]
        [SerializeField] ComputeShader _cullGrassShader;

        GraphicsBuffer _grassVertexBuffer;
        GraphicsBuffer _grassUVBuffer;
        MaterialPropertyBlock _propertyBlock;

        // Culling Data
        GraphicsBuffer _voteBuffer;
        GraphicsBuffer _scanBuffer;
        GraphicsBuffer _groupSumArrayBuffer;
        GraphicsBuffer _scannedGroupSumBuffer;

        int _numThreadGroups;
        int _numVoteThreadGroups;
        int _numGroupScanThreadGroups;

        ChunkData[] _chunks;

        void Start()
        {
            CreateSharedBuffers();

            CreateChunks();

            CreateCullGrassBuffers();

            // Bind buffers to MaterialPropertyBlock
            _propertyBlock = new MaterialPropertyBlock();
            _propertyBlock.SetBuffer("_positions", _grassVertexBuffer);
            _propertyBlock.SetBuffer("_UVs", _grassUVBuffer);
        }

        void Update()
        {
            Matrix4x4 P = Camera.main.projectionMatrix;
            Matrix4x4 V = Camera.main.transform.worldToLocalMatrix;
            var VP = P * V;

            for (int i = 0; i < _chunks.Length; i++)
            {
                _propertyBlock.SetBuffer("_CulledGrassOutputBuffer", _chunks[i].culledtransformMatrixBuffer);

                CullGrass(VP, _chunks[i]);
                Graphics.DrawMeshInstancedIndirect(_grassMesh, 0, _grassMaterial, _chunks[i].bounds, _chunks[i].argsBuffer,
                    properties: _propertyBlock,
                    castShadows: _castShadows,
                    receiveShadows: _receiveShadows);
            }
        }

        void OnDestroy()
        {
            _grassVertexBuffer.Dispose();
            _grassUVBuffer.Dispose();

            _voteBuffer.Dispose();
            _scanBuffer.Dispose();
            _groupSumArrayBuffer.Dispose();
            _scannedGroupSumBuffer.Dispose();

            for (int i = 0; i < _chunks.Length; i++)
            {
                _chunks[i].argsBuffer.Dispose();
                _chunks[i].transformMatrixBuffer.Dispose();
                _chunks[i].worldUVsBuffer.Dispose();
                _chunks[i].terrainHeightBuffer.Dispose();
                _chunks[i].culledtransformMatrixBuffer.Dispose();
            }
        }

        void CreateChunks()
        {
            _chunks = new ChunkData[_chunksPerDimension * _chunksPerDimension];
            for (int yOffset = 0; yOffset < _chunksPerDimension; yOffset++)
            {
                for (int xOffset = 0; xOffset < _chunksPerDimension; xOffset++)
                {
                    _chunks[yOffset * _chunksPerDimension + xOffset] = CreateChunk(xOffset, yOffset);
                    ComputeChunkData(_chunks[yOffset * _chunksPerDimension + xOffset]);
                }
            }
        }

        ChunkData CreateChunk(int xChunkOffset, int yChunkOffset)
        {
            var resolutionSquared = _chunkGrassResolution * _chunkGrassResolution;
            var worldOffset = new Vector2(xChunkOffset * _chunkGrassResolution, yChunkOffset * _chunkGrassResolution);
            var worldResolution = (_chunkGrassResolution * _chunksPerDimension);
            var worldUVs = new List<Vector2>(resolutionSquared);
            
            // Generate worldUVs
            for (int i = 0; i < resolutionSquared; i++)
            {
                var localChunkCoords = new Vector2(i % _chunkGrassResolution, i / _chunkGrassResolution);
                var uv = (worldOffset + localChunkCoords) / worldResolution;

                worldUVs.Add(uv);
            }

            // Get terrain heights
            var terrainHeights = new float[worldUVs.Count];
            for (int i = 0; i < worldUVs.Count; i++)
            {
                terrainHeights[i] = _terrainData.GetInterpolatedHeight(worldUVs[i].x, worldUVs[i].y);
            }

            // Create world UV buffer
            var worldUVBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, terrainHeights.Length, sizeof(float) * 2);
            worldUVBuffer.SetData(worldUVs.ToArray());
            worldUVs.Clear();

            // Create heights buffer
            var terrainHeightBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, terrainHeights.Length, sizeof(float));
            terrainHeightBuffer.SetData(terrainHeights);

            // Create transformations buffer
            var transformMatrixBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, terrainHeights.Length, sizeof(float) * 16);

            var culledtransformMatrixBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, terrainHeights.Length, sizeof(float) * (16 + 2));

            // Bounds
            var bounds = new Bounds();

            var chunkTerrainSize = new Vector3(
                _terrainData.size.x / _chunksPerDimension,
                _terrainData.size.y,
                _terrainData.size.z / _chunksPerDimension);
            var offsetToCenter = new Vector3(chunkTerrainSize.x / 2f, 0f, chunkTerrainSize.z / 2f);
            var chunkTerrainOffset = new Vector3(xChunkOffset * chunkTerrainSize.x, 0f, yChunkOffset * chunkTerrainSize.z);

            bounds.center = _terrain.GetPosition() + offsetToCenter + chunkTerrainOffset;
            bounds.size = chunkTerrainSize;
            bounds.Expand(_maxBladeHeight);

            // Args
            var args = new uint[5] { 0u, 0u, 0u, 0u, 0u };
            args[0] = _grassMesh.GetIndexCount(0);
            args[1] = 0u;
            args[2] = _grassMesh.GetIndexStart(0);
            args[3] = _grassMesh.GetBaseVertex(0);

            var argsBuffer = new GraphicsBuffer(GraphicsBuffer.Target.IndirectArguments, 1, sizeof(uint) * args.Length);
            argsBuffer.SetData(args);

            var chunkData = new ChunkData
            {
                argsBuffer = argsBuffer,
                transformMatrixBuffer = transformMatrixBuffer,
                worldUVsBuffer = worldUVBuffer,
                terrainHeightBuffer = terrainHeightBuffer,
                culledtransformMatrixBuffer = culledtransformMatrixBuffer,
                bounds = bounds
            };

            return chunkData;
        }

        void ComputeChunkData(ChunkData chunk)
        {
            _computeShader.SetFloat("_grassTerrainLengthX", _terrainData.size.x);
            _computeShader.SetFloat("_grassTerrainLengthZ", _terrainData.size.z);

            _computeShader.SetMatrix("_terrainObjectToWorld", transform.localToWorldMatrix);
            _computeShader.SetFloat("_minBladeHeight", _minBladeHeight);
            _computeShader.SetFloat("_maxBladeHeight", _maxBladeHeight);
            _computeShader.SetFloat("_minOffset", _minOffset);
            _computeShader.SetFloat("_maxOffset", _maxOffset);
            _computeShader.SetFloat("_bladeThicknessScale", _bladeThicknessScale);
            _computeShader.SetInt("_grassbladesCount", chunk.transformMatrixBuffer.count);

            _computeShader.SetBuffer(0, "_transformMatrices", chunk.transformMatrixBuffer);
            _computeShader.SetBuffer(0, "_worldUVBuffer", chunk.worldUVsBuffer);
            _computeShader.SetBuffer(0, "_terrainHeightBuffer", chunk.terrainHeightBuffer);

            _computeShader.GetKernelThreadGroupSizes(0, out var threadGroupSize, out _, out _);
            int threadGroups = Mathf.CeilToInt(chunk.transformMatrixBuffer.count / threadGroupSize);
            _computeShader.Dispatch(0, threadGroups, 1, 1);
        }

        void CreateSharedBuffers()
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
            var grassBladesCount = _chunkGrassResolution * _chunkGrassResolution;
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
            _voteBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, grassBladesCount, sizeof(uint));
            _scanBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, grassBladesCount, sizeof(uint));
            _groupSumArrayBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, _numThreadGroups, sizeof(uint));
            _scannedGroupSumBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, _numThreadGroups, sizeof(uint));

            #region SetupComputeShader
            // Vote
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
            _cullGrassShader.SetBuffer(3, "_VoteBuffer", _voteBuffer);
            _cullGrassShader.SetBuffer(3, "_ScanBuffer", _scanBuffer);
            _cullGrassShader.SetBuffer(3, "_GroupSumArray", _scannedGroupSumBuffer);
            #endregion
        }

        void CullGrass(Matrix4x4 VP, ChunkData chunk)
        {
            // Reset instance count
            _cullGrassShader.SetBuffer(4, "_ArgsBuffer", chunk.argsBuffer);
            _cullGrassShader.Dispatch(4, 1, 1, 1);

            // Vote
            _cullGrassShader.SetMatrix("MATRIX_VP", VP);
            _cullGrassShader.SetVector("_CameraPosition", Camera.main.transform.position);
            _cullGrassShader.SetBuffer(0, "_transformMatrices", chunk.transformMatrixBuffer);
            _cullGrassShader.Dispatch(0, _numVoteThreadGroups, 1, 1);

            // Scan Instances
            _cullGrassShader.Dispatch(1, _numThreadGroups, 1, 1);

            // Scan Groups
            _cullGrassShader.Dispatch(2, _numGroupScanThreadGroups, 1, 1);

            // Compact
            _cullGrassShader.SetBuffer(3, "_transformMatrices", chunk.transformMatrixBuffer);
            _cullGrassShader.SetBuffer(3, "_worldUV", chunk.worldUVsBuffer);
            _cullGrassShader.SetBuffer(3, "_CulledGrassOutputBuffer", chunk.culledtransformMatrixBuffer);
            _cullGrassShader.SetBuffer(3, "_ArgsBuffer", chunk.argsBuffer);
            _cullGrassShader.Dispatch(3, _numThreadGroups, 1, 1);
        }
    }
}
