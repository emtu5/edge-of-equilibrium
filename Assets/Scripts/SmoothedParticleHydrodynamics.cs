using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

[System.Serializable]
[StructLayout(LayoutKind.Sequential, Size = SmoothedParticleHydrodynamics.structSize)]
public struct Particle
{
    // total: 44 bytes
    public float pressure; // 4 bytes
    public float density; // 4 bytes
    public Vector3 currentForce; // 12 bytes
    public Vector3 currentVelocity; // 12 bytes
    public Vector3 position; // 12 bytes
}

public class SmoothedParticleHydrodynamics : MonoBehaviour
{
    public const int structSize = 44;
    //[Header("General")]
    public bool showSpheres = true;
    public Vector3Int numToSpawn = new Vector3Int(10, 10, 10);
    private int totalParticles
    {
        get { return numToSpawn.x * numToSpawn.y * numToSpawn.z; }
    }
    public Vector3 boxSize = new Vector3(4, 10, 3);
    public Vector3 spawnCenter;
    public float particleRadius = 0.1f;

    //[Header("Particle Rendering")]
    public Mesh particleMesh;
    public float particleRenderSize = 8f;
    public Material particleMaterial;

    //[Header("Compute")]
    public ComputeShader computeShader;
    public Particle[] particles;

    private ComputeBuffer argsBuffer;
    private ComputeBuffer particlesBuffer;

    private void Awake()
    {
        SpawnParticlesInBox();

        uint[] args =
        {
            particleMesh.GetIndexCount(0),
            (uint)totalParticles,
            particleMesh.GetIndexStart(0),
            particleMesh.GetBaseVertex(0),
            0
        };

        argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
        argsBuffer.SetData(args);

        particlesBuffer = new ComputeBuffer(totalParticles, structSize);
        particlesBuffer.SetData(particles);
    }

    private void SpawnParticlesInBox()
    {
        Vector3 spawnpoint = spawnCenter;
        List<Particle> _particles = new List<Particle>();

        for (int x = 0; x < numToSpawn.x; x++)
        {
            for (int y = 0; y < numToSpawn.y; y++)
            {
                for (int z = 0; z < numToSpawn.z; z++)
                {
                    Vector3 spawnPos = spawnpoint + new Vector3(x * particleRadius * 2, y * particleRadius * 2, z * particleRadius * 2);
                    Particle particle = new Particle
                    {
                        position = spawnPos
                    };
                    _particles.Add(particle);
                }
            }
        }

        particles = _particles.ToArray();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(Vector3.zero, boxSize);

        if (!Application.isPlaying)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(spawnCenter, 0.1f);
        }
    }

    private static readonly int SizeProperty = Shader.PropertyToID("size");
    private static readonly int ParticlesBufferProperty = Shader.PropertyToID("particlesBuffer");

    private void Update()
    {
        particleMaterial.SetFloat(SizeProperty, particleRenderSize);
        particleMaterial.SetBuffer(ParticlesBufferProperty, particlesBuffer);

        if (showSpheres)
        {
            Graphics.DrawMeshInstancedIndirect( // mysterious looking function that is obsolete!? should replace at some point with RenderMeshIndirect
                particleMesh,
                0,
                particleMaterial,
                new Bounds(Vector3.zero, boxSize),
                argsBuffer,
                castShadows: UnityEngine.Rendering.ShadowCastingMode.Off
            );
        }
    }
}
