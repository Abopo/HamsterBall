using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorManager : MonoBehaviour {

    public float conveyorSpeed;
    public GameObject[] hamsterContainers = new GameObject[6];
    RobotArm[] _robotArms = new RobotArm[4];

    private void Awake() {
        _robotArms = GetComponentsInChildren<RobotArm>();
    }
    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        if(AllTasksDone()) {
            // Move conveyor belt
            foreach(GameObject hC in hamsterContainers) {
                hC.transform.Translate(conveyorSpeed * Time.deltaTime, 0f, 0f);
            }

            // If the containers have moved far enough
            if(hamsterContainers[0].transform.position.x >= -8.54f) {
                // Make sure containers are in correct positions
                for(int i = 0; i < 6; ++i) {
                    hamsterContainers[i].transform.position = new Vector3(-8.54f + (4.27f * i), 
                                                                          hamsterContainers[i].transform.position.y, 
                                                                          hamsterContainers[i].transform.position.z);
                }

                // Stop conveyor and begin RobotArm tasks
                BeginTasks();


                // Also move the last container back to the front
                ResetContainer();
            }
        }
    }

    bool AllTasksDone() {
        foreach(RobotArm rA in _robotArms) {
            if(rA.working) {
                return false;
            }
        }

        return true;
    }

    void BeginTasks() {
        foreach(RobotArm rA in _robotArms) {
            rA.BeginTask();
        }
    }

    void ResetContainer() {
        // Move last container to front
        hamsterContainers[5].transform.position = new Vector3(-12.81f, hamsterContainers[5].transform.position.y, hamsterContainers[5].transform.position.z);
        // Turn off it's top
        hamsterContainers[5].transform.Find("Top").GetComponent<SpriteRenderer>().enabled = false;

        // Adjust array accordingly
        GameObject tempObj = hamsterContainers[5];
        for (int i = 5; i > 0; --i) {
            hamsterContainers[i] = hamsterContainers[i - 1];
        }
        hamsterContainers[0] = tempObj;
    }
}
