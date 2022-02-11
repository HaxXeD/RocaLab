using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour
{
    [SerializeField] GameObject impact;
    GameObject effect;
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        Destroy(gameObject, 2f);
        // Destroy();
    }

    private void Destroy()
    {
        Destroy(gameObject, 2f);
        effect = Instantiate(impact, transform.position, Quaternion.identity);
        Destroy(effect, 5f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        effect =  Instantiate(impact, transform.position, Quaternion.identity);
        Destroy(effect, 1f);
        Destroy(gameObject);
    }
}
