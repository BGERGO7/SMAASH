using System.Collections;
using UnityEngine;
using System;
using Photon.Pun;

public class CameraController : MonoBehaviourPunCallbacks
{
    private Camera cam;
    public float yOffset = 1.5f;
    public float zOffset = -9f;
    private Vector3 temp;
    private Vector3 last;
    private float yLevel;
    private bool CamIsActive = false;
    private float Bounds;


    //Wait for spawned player to fall down to the ground before setting camera Y level
    private IEnumerator SetYLevel()
    {
        yield return new WaitForSeconds(1f);
        yLevel = transform.position.y;
        CamIsActive = true;
    }

    private void UpdatePosition()
    {
        if (Math.Abs(transform.position.x) > Bounds)
        {
            cam.transform.position = last;
            return;
        }

        temp.x = transform.position.x;
        temp.y = yLevel + yOffset;
        temp.z = transform.position.z + zOffset;

        cam.transform.position = temp;
        last = temp;
    }

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponentInChildren<Camera>();
        Bounds = 10f;

        if (photonView.IsMine)
        {
            cam.enabled = true;
        }
        else
        {
            cam.enabled = false;
        }

        StartCoroutine(SetYLevel());
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine && CamIsActive)
        {
            UpdatePosition();
        }
    }
}
