﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharkFlocking : MonoBehaviour
{
    #region Public Variables
    [Header("Flocking Weights")]
    public float alignWeight;
    public float seekWeight;
    public float cohWeight;
    public float seperateWeight;
    [Header("Flocking Speed")]
    public float maxSpeed;
    public float maxForce;
    #endregion
    #region Private Variables
    private Rigidbody sharkRB;
    [SerializeField]
    private GameObject player;
    [SerializeField]
    private SharkSpawning spawn;
    [SerializeField]
    private Animator anim;
    #endregion
    void Start()
    {
        sharkRB = GetComponent<Rigidbody>();
        player = GameObject.FindGameObjectWithTag("Player");
        spawn = GameObject.FindGameObjectWithTag("Spawner").GetComponent<SharkSpawning>();
        anim = GetComponent<Animator>();
    }

    void OnEnable()
    {
        player = GameObject.FindGameObjectWithTag("Player");

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            SceneManagement.instance.GameOver();
        }
    }

    void FixedUpdate()
    {
        Flocking();
    }

    #region Steering Behaviour
    void Flocking()
    {
        Vector3 align = Alignment();
        Vector3 seek = Seeking(player.transform.position);
        Vector3 coh = Cohesion();
        Vector3 seperate = Seperation();

        sharkRB.AddForce(align * alignWeight);
        sharkRB.AddForce(seek * seekWeight);
        sharkRB.AddForce(coh * cohWeight);
        sharkRB.AddForce(seperate * seperateWeight);

        Vector3 target = player.transform.position;
        target.y = transform.position.y;
        transform.LookAt(target + sharkRB.velocity);
    }
    Vector3 Alignment()
    {
        float neighborDistance = 30;
        Vector3 newPoint = Vector3.zero;
        int neighborCount = 0;

        foreach (var other in spawn.boids)
        {
            float distance = Vector3.Distance(transform.position, other.transform.position);
            if (distance > 0 && distance < neighborDistance)
            {
                newPoint += other.velocity;
                neighborCount++;
            }
        }

        if (neighborCount > 0)
        {
            Vector3 totalVel = (newPoint / spawn.boids.Count).normalized * maxSpeed;
            Vector3 steering = totalVel - sharkRB.velocity;
            Vector3 clampSteering = Vector3.ClampMagnitude(steering, maxForce);
            return clampSteering;
        }
        else
        {
            return Vector3.zero;
        }
    }

    Vector3 Seeking(Vector3 target)
    {
        Vector3 desiredVelocity = (target - transform.position).normalized * maxSpeed;
        Vector3 steering = desiredVelocity - sharkRB.velocity;
        Vector3 clampSteering = Vector3.ClampMagnitude(steering, maxForce);
        anim.Play("SharkMovement");
        return clampSteering;
    }
    Vector3 Cohesion()
    {
        float radius = 5;
        Vector3 newPoint = Vector3.zero;
        Vector3 desiredCoh = Vector3.zero;
        int neighborCount = 0;

        foreach (var other in spawn.boids)
        {
            float distance = Vector3.Distance(transform.position, other.transform.position);
            if (distance > 0 && distance < radius)
            {
                newPoint += other.transform.position;
                neighborCount++;
            }
        }

        if (neighborCount > 0)
        {
            Vector3 avgPos = newPoint / spawn.boids.Count;
            return Seeking(avgPos);
        }
        else
        {
            return Vector3.zero;
        }
    }

    Vector3 Seperation()
    {
        float seperationDistance = 5;
        Vector3 newPoint = Vector3.zero;
        Vector3 desiredSep = Vector3.zero;
        Vector3 sepClamp = Vector3.zero;
        int neighborCount = 0;

        foreach (var other in spawn.boids)
        {
            float distance = Vector3.Distance(transform.position, other.transform.position);
            if (distance > 0 && distance < seperationDistance)
            {
                desiredSep = (transform.position - other.transform.position).normalized;
                Vector3 divide = desiredSep / distance;
                newPoint = newPoint + divide;
                neighborCount++;
            }
        }

        if (neighborCount > 0)
        {
            Vector3 totalVel = (newPoint / spawn.boids.Count).normalized * maxSpeed;
            Vector3 steering = totalVel - sharkRB.velocity;
            sepClamp = Vector3.ClampMagnitude(steering, maxForce);
        }
        return sepClamp;
    }
    #endregion
}
