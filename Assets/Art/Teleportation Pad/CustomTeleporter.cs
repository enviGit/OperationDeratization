using UnityEngine;
using UnityEngine.SceneManagement;

public class CustomTeleporter : MonoBehaviour
{
    public bool instantTeleport;
    public bool randomTeleport;
    public bool buttonTeleport;
    public string buttonName;
    public bool delayedTeleport;
    public float teleportTime = 3;
    public string objectTag = "if empty, any object will tp";
    public Transform[] destinationPad;
    public float teleportationHeightOffset = 1;
    private float curTeleportTime;
    private bool inside;
    [HideInInspector]
    public bool arrived;
    private Transform subject;
    public AudioSource teleportSound;
    public AudioSource teleportPadSound;
    public bool teleportPadOn = true;
    public SceneLoader sceneLoader;

    private void Start()
    {
        curTeleportTime = teleportTime;
    }
    private void Update()
    {
        if (inside)
        {
            if (!arrived && teleportPadOn)
                Teleport();
        }
    }
    private void Teleport()
    {
        if (instantTeleport)
        {
            if (randomTeleport)
            {
                int chosenPad = Random.Range(0, destinationPad.Length);
                destinationPad[chosenPad].GetComponent<CustomTeleporter>().arrived = true;
                subject.transform.position = destinationPad[chosenPad].transform.position + new Vector3(0, teleportationHeightOffset, 0);
                teleportSound.Play();
            }
            else
            {
                destinationPad[0].GetComponent<CustomTeleporter>().arrived = true;
                subject.transform.position = destinationPad[0].transform.position + new Vector3(0, teleportationHeightOffset, 0);
                teleportSound.Play();
            }
        }
        else if (delayedTeleport)
        {
            curTeleportTime -= 1 * Time.deltaTime;

            if (curTeleportTime <= 0)
            {
                curTeleportTime = teleportTime;

                if (randomTeleport)
                {
                    int chosenPad = Random.Range(0, destinationPad.Length);
                    destinationPad[chosenPad].GetComponent<CustomTeleporter>().arrived = true;
                    subject.transform.position = destinationPad[chosenPad].transform.position + new Vector3(0, teleportationHeightOffset, 0);
                    teleportSound.Play();
                }
                else
                {
                    teleportSound.Play();

                    if (sceneLoader != null)
                        sceneLoader.LoadScene(2);
                }
            }
        }
        else if (buttonTeleport)
        {
            if (Input.GetButtonDown(buttonName))
            {
                if (delayedTeleport)
                {
                    curTeleportTime -= 1 * Time.deltaTime;

                    if (curTeleportTime <= 0)
                    {
                        curTeleportTime = teleportTime;
                        if (randomTeleport)
                        {
                            int chosenPad = Random.Range(0, destinationPad.Length);
                            destinationPad[chosenPad].GetComponent<CustomTeleporter>().arrived = true;
                            subject.transform.position = destinationPad[chosenPad].transform.position + new Vector3(0, teleportationHeightOffset, 0);
                            teleportSound.Play();
                        }
                        else
                        {
                            destinationPad[0].GetComponent<CustomTeleporter>().arrived = true;
                            subject.transform.position = destinationPad[0].transform.position + new Vector3(0, teleportationHeightOffset, 0);
                            teleportSound.Play();
                        }
                    }
                }
                else if (randomTeleport)
                {
                    int chosenPad = Random.Range(0, destinationPad.Length);
                    destinationPad[chosenPad].GetComponent<CustomTeleporter>().arrived = true;
                    subject.transform.position = destinationPad[chosenPad].transform.position + new Vector3(0, teleportationHeightOffset, 0);
                    teleportSound.Play();
                }
                else
                {
                    teleportSound.Play();
                    sceneLoader.LoadScene(2);
                }
            }
        }
    }
    private void OnTriggerEnter(Collider trig)
    {
        if (objectTag != "")
        {
            if (trig.gameObject.tag == objectTag)
            {
                subject = trig.transform;
                inside = true;

                if (buttonTeleport)
                    arrived = false;
            }
        }
        else
        {
            subject = trig.transform;
            inside = true;

            if (buttonTeleport)
                arrived = false;
        }
    }
    private void OnTriggerExit(Collider trig)
    {
        if (objectTag != "")
        {

            if (trig.gameObject.tag == objectTag)
            {
                inside = false;
                curTeleportTime = teleportTime;

                if (trig.transform == subject)
                    arrived = false;

                subject = null;
            }
        }
        else
        {
            inside = false;
            curTeleportTime = teleportTime;

            if (trig.transform == subject)
                arrived = false;

            subject = null;
        }
    }
}