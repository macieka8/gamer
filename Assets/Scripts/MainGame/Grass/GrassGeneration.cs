using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace gamer
{
    public class GrassGeneration : MonoBehaviour
    {
        public struct GrassData
        {
            public Vector3 positionWS;
            public Vector2 worldUV;
        }

        public struct ChunkData
        {
            public GraphicsBuffer argsBuffer;
            public GraphicsBuffer positionBuffer;
            public GraphicsBuffer worldUVsBuffer; 
            public GraphicsBuffer terrainHeightBuffer;
            public GraphicsBuffer culledPositionBuffer;
            public Bounds bounds;
            public bool isEmpty;

            public static ChunkData EmptyChunk => new ChunkData { isEmpty = true };
        }

        [SerializeField] ComputeShader _grassGenerationShader;
        [SerializeField] Mesh _grassMesh;
        [SerializeField] Material _grassMaterial;
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
        [SerializeField] float _bladeHeight;

        [SerializeField] float _bladeThicknessScale;
        
        [Range(-1.0f, 1.0f)]
        [SerializeField] float _minOffset;
        [Range(-1.0f, 1.0f)]
        [SerializeField] float _maxOffset;


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
        float[,,] _terrainAlphaMap;

        void Start()
        {
            _terrainAlphaMap = _terrain.terrainData.GetAlphamaps(
                0, 0, _terrain.terrainData.alphamapWidth, _terrain.terrainData.alphamapHeight);
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
                if (_chunks[i].isEmpty) continue;
                _propertyBlock.SetBuffer("_culledGrassOutputBuffer", _chunks[i].culledPositionBuffer);

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
                if (_chunks[i].isEmpty) continue;
                _chunks[i].argsBuffer.Dispose();
                _chunks[i].positionBuffer.Dispose();
                _chunks[i].worldUVsBuffer.Dispose();
                _chunks[i].terrainHeightBuffer.Dispose();
                _chunks[i].culledPositionBuffer.Dispose();
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

                var alphaMapCoordinate = new Vector2Int(
                    (int)(uv.x * _terrain.terrainData.alphamapWidth),
                    (int)(uv.y * _terrain.terrainData.alphamapHeight));
                //if (alphaMapCoordinate.y >= _terrainAlphaMap.GetLength(0)) continue;
                //if (alphaMapCoordinate.x >= _terrainAlphaMap.GetLength(1)) continue;
                if (_terrainAlphaMap[alphaMapCoordinate.y, alphaMapCoordinate.x, 0] > 0f)
                    worldUVs.Add(uv);
            }

            if (worldUVs.Count == 0)
            {
                return ChunkData.EmptyChunk;
            }
            // Get terrain heights
            var terrainHeights = new float[worldUVs.Count];
            for (int i = 0; i < worldUVs.Count; i++)
            {
                terrainHeights[i] = _terrain.terrainData.GetInterpolatedHeight(worldUVs[i].x, worldUVs[i].y);
            }

            // Create world UV buffer
            var worldUVBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, terrainHeights.Length, sizeof(float) * 2);
            worldUVBuffer.SetData(worldUVs.ToArray());
            worldUVs.Clear();

            // Create heights buffer
            var terrainHeightBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, terrainHeights.Length, sizeof(float));
            terrainHeightBuffer.SetData(terrainHeights);

            // Create transformations buffer
            var positionBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, terrainHeights.Length, sizeof(float) * 3);
            var culledPositionBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, terrainHeights.Length, sizeof(float) * (3 + 2));

            // Bounds
            var bounds = new Bounds();

            var chunkTerrainSize = new Vector3(
                _terrain.terrainData.size.x / _chunksPerDimension,
                _terrain.terrainData.size.y,
                _terrain.terrainData.size.z / _chunksPerDimension);
            var offsetToCenter = new Vector3(chunkTerrainSize.x / 2f, 0f, chunkTerrainSize.z / 2f);
            var chunkTerrainOffset = new Vector3(xChunkOffset * chunkTerrainSize.x, 0f, yChunkOffset * chunkTerrainSize.z);

            bounds.center = _terrain.GetPosition() + offsetToCenter + chunkTerrainOffset;
            bounds.size = chunkTerrainSize;
            bounds.Expand(_bladeHeight);

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
                positionBuffer = positionBuffer,
                worldUVsBuffer = worldUVBuffer,
                terrainHeightBuffer = terrainHeightBuffer,
                culledPositionBuffer = culledPositionBuffer,
                bounds = bounds,
                isEmpty = false
            };

            return chunkData;
        }

        void ComputeChunkData(ChunkData chunk)
        {
            if (chunk.isEmpty) return;
            _grassGenerationShader.SetFloat("_grassTerrainLengthX", _terrain.terrainData.size.x);
            _grassGenerationShader.SetFloat("_grassTerrainLengthZ", _terrain.terrainData.size.z);

            _grassGenerationShader.SetMatrix("_terrainObjectToWorld", Matrix4x4.Translate(
                _terrain.GetPosition() + new Vector3(_terrain.terrainData.size.x / 2f, 0f, _terrain.terrainData.size.z / 2f)));
            _grassGenerationShader.SetFloat("_minOffset", _minOffset);
            _grassGenerationShader.SetFloat("_maxOffset", _maxOffset);
            _grassGenerationShader.SetInt("_grassbladesCount", chunk.positionBuffer.count);

            _grassGenerationShader.SetBuffer(0, "_positionBuffer", chunk.positionBuffer);
            _grassGenerationShader.SetBuffer(0, "_worldUVBuffer", chunk.worldUVsBuffer);
            _grassGenerationShader.SetBuffer(0, "_terrainHeightBuffer", chunk.terrainHeightBuffer);

            _grassGenerationShader.GetKernelThreadGroupSizes(0, out var threadGroupSize, out _, out _);
            int threadGroups = Mathf.CeilToInt(chunk.positionBuffer.count / threadGroupSize);
            _grassGenerationShader.Dispatch(0, threadGroups, 1, 1);
        }

        void CreateSharedBuffers()
        {
            Vector3[] grassVertices = _grassMesh.vertices;
            for (int i = 0; i < grassVertices.Length; i++)
            {
                grassVertices[i] = new Vector3(
                    grassVertices[i].x * _bladeThicknessScale,
                    grassVertices[i].y * _bladeHeight,
                    grassVertices[i].z * _bladeThicknessScale);
            }
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
            _cullGrassShader.SetBuffer(0, "_voteBuffer", _voteBuffer);
            _cullGrassShader.SetFloat("_distance", _distanceCutoff);

            // Scan Instances
            _cullGrassShader.SetBuffer(1, "_voteBuffer", _voteBuffer);
            _cullGrassShader.SetBuffer(1, "_scanBuffer", _scanBuffer);
            _cullGrassShader.SetBuffer(1, "_groupSumArray", _groupSumArrayBuffer);

            // Scan Groups
            _cullGrassShader.SetInt("_numOfGroups", _numThreadGroups);
            _cullGrassShader.SetBuffer(2, "_groupSumArrayIn", _groupSumArrayBuffer);
            _cullGrassShader.SetBuffer(2, "_groupSumArrayOut", _scannedGroupSumBuffer);

            // Compact
            _cullGrassShader.SetBuffer(3, "_voteBuffer", _voteBuffer);
            _cullGrassShader.SetBuffer(3, "_scanBuffer", _scanBuffer);
            _cullGrassShader.SetBuffer(3, "_groupSumArray", _scannedGroupSumBuffer);
            #endregion
        }

        void CullGrass(Matrix4x4 VP, ChunkData chunk)
        {
            // Reset instance count
            _cullGrassShader.SetBuffer(4, "_argsBuffer", chunk.argsBuffer);
            _cullGrassShader.Dispatch(4, 1, 1, 1);

            // Vote
            _cullGrassShader.SetMatrix("MATRIX_VP", VP);
            _cullGrassShader.SetVector("_cameraPosition", Camera.main.transform.position);
            _cullGrassShader.SetBuffer(0, "_positionBuffer", chunk.positionBuffer);
            _cullGrassShader.Dispatch(0, _numVoteThreadGroups, 1, 1);

            // Scan Instances
            _cullGrassShader.Dispatch(1, _numThreadGroups, 1, 1);

            // Scan Groups
            _cullGrassShader.Dispatch(2, _numGroupScanThreadGroups, 1, 1);

            // Compact
            _cullGrassShader.SetBuffer(3, "_positionBuffer", chunk.positionBuffer);
            _cullGrassShader.SetBuffer(3, "_worldUV", chunk.worldUVsBuffer);
            _cullGrassShader.SetBuffer(3, "_culledGrassOutputBuffer", chunk.culledPositionBuffer);
            _cullGrassShader.SetBuffer(3, "_argsBuffer", chunk.argsBuffer);
            _cullGrassShader.Dispatch(3, _numThreadGroups, 1, 1);
        }
    }
}
