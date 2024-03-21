using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CameraController : MonoBehaviourPunCallbacks
{
    private Camera cam;
    public float yOffset = 1.5f;
    public float zOffset = -9f;
    Vector3 temp;

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponentInChildren<Camera>();

        if (photonView.IsMine)
        {
            cam.enabled = true;
        }
        else
        {
            cam.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            temp.x = transform.position.x;
            temp.y = transform.position.y + yOffset;
            temp.z = transform.position.z + zOffset;

            cam.transform.position = temp;
        }
    }
}
