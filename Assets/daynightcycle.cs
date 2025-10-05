using UnityEngine;

public class daynightcycle : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float timerate = 1f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(1,0,0) * Time.deltaTime * timerate);
    }
}
