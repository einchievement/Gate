using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Photon.Pun;
using UnityEngine;

public class CreationPUN : MonoBehaviourPun
{
    public GameObject cubePrefab;
    public GameObject spherePrefab;
    
    void Update()
    {
        if (!photonView.IsMine && PhotonNetwork.IsConnected)
        {
            return;
        }
        
        GameObject gameObject = null;
        
        if (Input.GetKeyDown(KeyCode.Alpha1))
        { 
            gameObject = PhotonNetwork.Instantiate(cubePrefab.name, transform.position, transform.rotation);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            gameObject = PhotonNetwork.Instantiate(spherePrefab.name, transform.position, transform.rotation);
        }

        if (gameObject != null)
        {
            Rigidbody rigidbody = gameObject.GetComponent<Rigidbody>();
            rigidbody.AddForce(transform.forward * 1000); 
        }
    }
    
    
    
}