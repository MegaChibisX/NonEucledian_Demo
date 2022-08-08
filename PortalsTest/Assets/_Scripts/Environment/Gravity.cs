using System.Collections;
using System.Collections.Generic;
using System.Net.Cache;
using UnityEngine;

[System.Serializable]
public class Gravity
{
    public Transform source;
    private Player entity;

    public enum GravityType { Linear, Spherical, Cylindrical }
    public GravityType gravityType;

    public Vector3 normal = Vector3.down;
    public float strength = 1f;

    public bool invert = false;
    public float maxDistance;



    public Gravity()
    {
        source = null;
        entity = null;

        gravityType = GravityType.Linear;
        normal = Vector3.down;
        strength = 1f;
        invert = false;
        maxDistance = 0f;
    }
    public Gravity(Transform gravitySource)
    {
        this.source = gravitySource;
    }
    public Gravity(Player entity)
    {
        this.entity = entity;
    }
    public Gravity(Gravity toCopy, Player entity)
    {
        this.entity = entity;

        this.source = toCopy.source;

        this.normal = toCopy.normal;
        this.strength = toCopy.strength;

        this.gravityType = toCopy.gravityType;
        this.invert = toCopy.invert;
        this.maxDistance = toCopy.maxDistance;
    }

    public void ApplyGravity()
    {
        switch (gravityType)
        {
            default:
            case GravityType.Linear:
                if (!entity.isGrounded())
                {
                    entity.body.AddForce(GlobalNormal() * strength * 100f);
                }
                break;
            case GravityType.Spherical:
                if (!source)
                {
                    Debug.LogWarning("No gravity source!");
                    gravityType = GravityType.Linear;
                    return;
                }
                entity.body.AddForce(GlobalNormal() * strength * 100f);
                break;
        }
    }

    public Vector3 GlobalNormal()
    {
        switch (gravityType)
        {
            default:
            case GravityType.Linear:
                if (source)
                    return source.transform.TransformDirection(normal.normalized) * entity.transform.lossyScale.y;
                else
                    return entity.transform.TransformVector(normal.normalized);

            case GravityType.Spherical:
                return (source.position - entity.transform.position).normalized * entity.transform.lossyScale.y;
        }
    }


}
