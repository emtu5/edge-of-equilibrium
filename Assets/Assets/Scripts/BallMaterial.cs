using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewBallMaterial", menuName = "Ball/Ball Material")]
public class BallMaterial : ScriptableObject
{
    public Material material;
    public PhysicMaterial physicMaterial;
    public float mass;
    public float speed;
	public float drag;
}
