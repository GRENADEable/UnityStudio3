﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SharkAITutorial : MonoBehaviour
{

    #region Public Variables
    public float maxSpeed;
    public float maxForce;
    [SerializeField]
    //private SharkSpawning AI;
    //public float spaceBetween;
    #endregion

    #region Private Variables
    private Rigidbody sharkRB;
    [SerializeField]
    private GameObject Player;
    [SerializeField]
    private Animator anim;
    #endregion

    void Start()
    {
        sharkRB = GetComponent<Rigidbody>();
        Player = GameObject.FindGameObjectWithTag("Player");
        anim = GetComponent<Animator>();
    }


    void FixedUpdate()
    {
        Vector3 headDir = (new Vector3(Player.gameObject.transform.position.x, 0, Player.gameObject.transform.position.z) - new Vector3(this.gameObject.transform.position.x, 0, this.gameObject.transform.position.z)).normalized;

        Vector3 desiredVelocity = (Player.transform.position - transform.position).normalized * maxSpeed;
        Vector3 steering = desiredVelocity - sharkRB.velocity;
        Vector3 clampSteering = Vector3.ClampMagnitude(steering, maxForce);

        sharkRB.AddForce(clampSteering, ForceMode.Impulse);
        transform.LookAt(transform.position + sharkRB.velocity);


    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            SceneManagement.instance.TutorialToMainMenu();
        }
    }
}
