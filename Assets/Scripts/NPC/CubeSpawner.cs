using System.Collections;
using System.Collections.Generic;
using Rewired;
using UnityEngine;

public class CubeSpawner : MonoBehaviour
{
    public int maxCubes = 48;
    public int cubesDestroyed = 0;
    public int spawned = 0;
    public bool bonusEarned = false;

    [SerializeField] private int count = 25;
    [SerializeField] private int startX = -40;
    [SerializeField] private int startZ = -20;
    [SerializeField]
    private GameObject cubePrefab;

    [SerializeField] private float nextWaveTime = 5.0f;
    public Player player;
    public int playerId = 0; // The Rewired player id of this character
    public bool rightTriggerDown = false;
    public Vector2 mousePosition = Vector2.zero;

    private float timer = 0;
    // Start is called before the first frame update
    void Start()
    {

        player = ReInput.players.GetPlayer(playerId);

        Spawner();


    }


    void Spawner()
    {
        for (int i = 0; i < count; i++)
        {
            float xLocation = (int)Random.Range(-startX, startX);
            float zLocation = (int)Random.Range(-startZ, startZ);
            GameObject spawnCube = Instantiate(cubePrefab, new Vector3(xLocation, 0, zLocation), cubePrefab.transform.rotation);
            spawned += 1;
        }

    }

    // Update is called once per frame
    void Update()
    {

        if (!ReInput.isReady) return;
        mousePosition = player.controllers.Mouse.screenPosition;
        rightTriggerDown = player.GetButtonDown("RightTrigger");

        if(spawned >= maxCubes) return;
        



        timer = timer + Time.deltaTime;
        if (timer > nextWaveTime)
        {
            timer = 0;
            Spawner();
        }


}
}
