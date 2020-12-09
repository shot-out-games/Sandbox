using Rewired;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{

    public Player player;
    public int playerId = 0; // The Rewired player id of this character
    float deadZone = Mathf.Epsilon;
    Quaternion rotation;
    Camera cam;

    // Start is called before the first frame update
    [SerializeField] private Transform target;
    [SerializeField] float rotationValue;
    void Start()
    {
        if (!ReInput.isReady) return;
        player = ReInput.players.GetPlayer(playerId);
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        float leftStickX = player.GetAxis("Move Horizontal");
        float leftStickY = player.GetAxis("Move Vertical");

        rotation =  Quaternion.AngleAxis(rotationValue * leftStickX * Time.deltaTime, Vector3.up);

        Vector3 forward = transform.TransformDirection(Vector3.forward);
        forward.y = 0;
    }

    void LateUpdate()
    {


        transform.position = target.position;
        transform.rotation *= rotation;
    }
}
