using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System.Runtime.InteropServices;
using UnityEditor;

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
    public float fadeDuration = 3f;
    private bool isFading = false;

    public const int structSize = 44;
    //[Header("General")]
	// public Rigidbody collisionSphere;
	public GameObject collisionSphere;
	
    public bool showSpheres = true;
    public Vector3Int numToSpawn = new Vector3Int(10, 10, 10);
    private int totalParticles
    {
        get { return numToSpawn.x * numToSpawn.y * numToSpawn.z; }
    }
    public Vector3 boxSize = new Vector3(4, 10, 3);
    public Vector3 spawnCenter;
    public float particleRadius = 0.1f;
    public float displacement = 0.2f;

    //[Header("Particle Rendering")]
    public Mesh particleMesh;
    public float particleRenderSize = 8f;
    public Material particleMaterialAsset;
    private Material particleMaterial; 

    //[Header("Compute")]
    public ComputeShader computeShaderAsset;
    private ComputeShader computeShader;
    public Particle[] particles;

    //[Header("Fluid Constants")]
    public float boundDamping = -0.3f;
    public float viscosity = -0.003f;
    public float restingDensity = 1f;
    public float gasConstant = 2f;
    public float particleMass = 1f;
    public float timestep = 0.01f;
    public float gravity = -9.81f;

    private ComputeBuffer argsBuffer;
    private ComputeBuffer particlesBuffer;
    private ComputeBuffer sphereForceBuffer;

    private ComputeBuffer particleIndices;
    private ComputeBuffer particleCellIndices;
    private ComputeBuffer cellOffsets;

    private int integrateKernel;
    private int computeForcesKernel;
    private int computeDensityPressureKernel;
    private int hashParticleKernel;
    private int bitonicSortKernel;
    private int calculateOffsetsKernel;

    private void SetupComputeBuffers()
    {
        integrateKernel = computeShader.FindKernel("Integrate");
        computeForcesKernel = computeShader.FindKernel("ComputeForces");
        computeDensityPressureKernel = computeShader.FindKernel("ComputeDensityPressure");
        hashParticleKernel = computeShader.FindKernel("HashParticles");
        bitonicSortKernel = computeShader.FindKernel("BitonicSort");
        calculateOffsetsKernel = computeShader.FindKernel("CalculateOffsets");

        computeShader.SetInt("particleLength", totalParticles);
        computeShader.SetFloat("boundDamping", boundDamping);
        computeShader.SetFloat("viscosity", viscosity);
        computeShader.SetFloat("restingDensity", restingDensity);
        computeShader.SetFloat("gasConstant", gasConstant);
        computeShader.SetFloat("particleMass", particleMass);
        computeShader.SetFloat("pi", Mathf.PI);
        computeShader.SetVector("boxSize", boxSize);
		computeShader.SetVector("boxCenter", this.gameObject.transform.position);
        computeShader.SetFloat("gravity", gravity);
        computeShader.SetFloat("radius1", particleRadius);
        computeShader.SetFloat("radius2", particleRadius * particleRadius);
        computeShader.SetFloat("radius3", particleRadius * particleRadius * particleRadius);
        computeShader.SetFloat("radius4", particleRadius * particleRadius * particleRadius * particleRadius);
        computeShader.SetFloat("radius5", particleRadius * particleRadius * particleRadius * particleRadius * particleRadius);

        computeShader.SetBuffer(integrateKernel, "_particles", particlesBuffer);
        computeShader.SetBuffer(computeForcesKernel, "_particles", particlesBuffer);
        computeShader.SetBuffer(computeDensityPressureKernel, "_particles", particlesBuffer);
        computeShader.SetBuffer(computeForcesKernel, "_sphereForce", sphereForceBuffer);

        computeShader.SetBuffer(computeDensityPressureKernel, "_particleIndices", particleIndices);
        computeShader.SetBuffer(computeDensityPressureKernel, "_particleCellIndices", particleCellIndices);
        computeShader.SetBuffer(computeDensityPressureKernel, "_cellOffsets", cellOffsets);

        computeShader.SetBuffer(computeForcesKernel, "_particleIndices", particleIndices);
        computeShader.SetBuffer(computeForcesKernel, "_particleCellIndices", particleCellIndices);
        computeShader.SetBuffer(computeForcesKernel, "_cellOffsets", cellOffsets);

        computeShader.SetBuffer(hashParticleKernel, "_particleIndices", particleIndices);
        computeShader.SetBuffer(hashParticleKernel, "_particles", particlesBuffer);
        computeShader.SetBuffer(hashParticleKernel, "_particleCellIndices", particleCellIndices);
        computeShader.SetBuffer(hashParticleKernel, "_cellOffsets", cellOffsets);

        computeShader.SetBuffer(bitonicSortKernel, "_particleIndices", particleIndices);
        computeShader.SetBuffer(bitonicSortKernel, "_particleCellIndices", particleCellIndices);

        computeShader.SetBuffer(calculateOffsetsKernel, "_particleIndices", particleIndices);
        computeShader.SetBuffer(calculateOffsetsKernel, "_particleCellIndices", particleCellIndices);
        computeShader.SetBuffer(calculateOffsetsKernel, "_cellOffsets", cellOffsets);
    }

    private void Awake()
    {
        // each sph simulator needs its own private copy of the compute shader and material
        // otherwise they're all going to use the same buffers (so all of them will be mirrored)
        computeShader = Instantiate(computeShaderAsset);
        particleMaterial = Instantiate(particleMaterialAsset);

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

        sphereForceBuffer = new ComputeBuffer(1, sizeof(float) * 3, ComputeBufferType.Raw);

        particleIndices = new ComputeBuffer(totalParticles, 4);
        particleCellIndices = new ComputeBuffer(totalParticles, 4);
        cellOffsets = new ComputeBuffer(totalParticles, 4);

        uint[] particleInd = new uint[totalParticles];

        for (int i = 0; i < particleInd.Length; i++)
        {
            particleInd[i] = (uint)i;
        }

        particleIndices.SetData(particleInd);

        Debug.Log("awake!");

        SetupComputeBuffers();
    }

    private void SortParticles()
    {
        for (int dimension = 2; dimension <= totalParticles; dimension <<= 1)
        {
            computeShader.SetInt("dimension", dimension);
            for (int block = dimension >> 1; block > 0; block >>= 1)
            {
                computeShader.SetInt("block", block);
                computeShader.Dispatch(bitonicSortKernel, totalParticles / 256, 1, 1);
            }
        }
    }

    private void FixedUpdate()
    {
        computeShader.SetVector("boxSize", boxSize);
        computeShader.SetFloat("timestep", timestep);
		computeShader.SetVector("spherePos", collisionSphere.transform.position - this.gameObject.transform.position);
		computeShader.SetFloat("sphereRadius", collisionSphere.transform.localScale.x / 2);
        sphereForceBuffer.SetData(new Vector3[] { Vector3.zero });

        computeShader.Dispatch(hashParticleKernel, totalParticles / 256, 1, 1);
        SortParticles();

        computeShader.Dispatch(calculateOffsetsKernel, totalParticles / 256, 1, 1);
        computeShader.Dispatch(computeDensityPressureKernel, totalParticles / 256, 1, 1);
		computeShader.Dispatch(computeForcesKernel, totalParticles / 256, 1, 1);
		computeShader.Dispatch(integrateKernel, totalParticles / 256, 1, 1);

        Vector3[] sphereForce = new Vector3[1];
        sphereForceBuffer.GetData(sphereForce);

        // Debug.Log(collisionSphere.GetComponent<MaterialController>().ballMaterial);
        var matCtrl = collisionSphere.GetComponent<MaterialController>();
        bool isPaper = matCtrl.ballMaterial.name == "Paper";
        bool isRock = matCtrl.ballMaterial.name == "Rock";
        bool hasForce = !sphereForce[0].Equals(Vector3.zero);
        Rigidbody rb = collisionSphere.GetComponent<Rigidbody>();

        if (isPaper && hasForce && !isFading)
        {
            // start fading once
            StartCoroutine(FadeAwayAndDestroy());
        }
        else if (!isFading)
        {
            rb.AddForce(sphereForce[0], ForceMode.Force);
        }

        if (isRock && hasForce && !isFading)
        {
            rb.AddForce(new Vector3(0, -100, 0), ForceMode.Force);
        }

        if (isFading)
        {
            rb.AddForce(new Vector3(0, 0.001f, 0), ForceMode.Force);
        }
        

        /*AsyncGPUReadback.Request(particlesBuffer, (req) => {
		  var data = req.GetData<Particle>();
		  Debug.Log($"P[0]: pos={data[0].position}, dens={data[0].density}, pres={data[0].pressure},currForce={data[0].currentForce},currVel={data[0].currentVelocity},");
		});*/

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
                    spawnPos += Random.onUnitSphere * particleRadius * displacement;
                    Particle particle = new Particle
                    
                    {
                        position = spawnPos,
						//density = restingDensity,
						//pressure = 0f
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
        //Gizmos.DrawWireCube(Vector3.zero, boxSize);
		Gizmos.DrawWireCube(this.gameObject.transform.position, boxSize);

        if (!Application.isPlaying)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(spawnCenter + this.gameObject.transform.position, 0.1f);
        }
    }

    private static readonly int SizeProperty = Shader.PropertyToID("_Size");
    private static readonly int ParticlesBufferProperty = Shader.PropertyToID("_particlesBuffer");

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
                new Bounds(this.gameObject.transform.position, boxSize),
                argsBuffer,
                castShadows: UnityEngine.Rendering.ShadowCastingMode.Off
            );
        }
    }

    private IEnumerator FadeAwayAndDestroy()
    {
        isFading = true;

        // assume the MaterialController holds a reference to the actual Material
        Material mat = collisionSphere.GetComponent<Renderer>().material;
        Color c = mat.color;
        float elapsed = 0f;

        // take away control from player
        // collisionSphere.GetComponent<Rigidbody>().isKinematic = true;
        collisionSphere.GetComponent<PlayerMovement>().setIsControllable(false);

        // gradually reduce alpha
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float a = Mathf.Lerp(c.a, 0f, elapsed / fadeDuration);
            mat.color = new Color(c.r, c.g, c.b, a);
            yield return null;
        }

        // ensure fully transparent
        mat.color = new Color(c.r, c.g, c.b, 0f);

        // optionally disable collision and physics
        collisionSphere.GetComponent<Collider>().enabled = false;
        

        collisionSphere.GetComponent<CheckpointSystem>().RespawnPlayer();
    }

    private void OnDestroy()
    {
        argsBuffer?.Release();
        particlesBuffer?.Release();
        sphereForceBuffer?.Release();
        particleIndices?.Release();
        particleCellIndices?.Release();
        cellOffsets?.Release();
    }

    private void OnDisable()
    {
        argsBuffer?.Release();
        particlesBuffer?.Release();
        sphereForceBuffer?.Release();
        particleIndices?.Release();
        particleCellIndices?.Release();
        cellOffsets?.Release();
    }
}


