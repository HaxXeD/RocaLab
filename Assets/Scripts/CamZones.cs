using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(Collider2D))]
public class CamZones : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera virtualCamera = null;
    // Start is called before the first frame update
    void Start()
    {
        virtualCamera.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            virtualCamera.enabled = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            virtualCamera.enabled = false;
    }
    private void OnValidate()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
