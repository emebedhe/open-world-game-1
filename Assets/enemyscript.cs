using UnityEngine;
public class enemyscript : MonoBehaviour
{
    public int health = 10;
    public GameObject healthbar;
    public GameObject healthbar_rotation;
    public GameObject border;
    RectTransform rt;
    void Start()
    {
        rt = healthbar.GetComponent<RectTransform>();
    }
    void Update()
    {
        transform.LookAt(Camera.main.transform);
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;


            if (Physics.Raycast(ray, out hit))
            {
                if (Vector3.Distance(Camera.main.transform.position, hit.point) < 12.0f)
                {
                    if (hit.collider.gameObject == this.gameObject)
                    {
                        Debug.Log("GameObject " + hit.collider.gameObject.name + " clicked!");
                        health--;
                        Debug.Log(health);
                        rt.offsetMin = new Vector2(10 - health, rt.offsetMin.y);
                        if (health <= 0)
                        {
                            Destroy(this.gameObject);
                            Destroy(healthbar);
                        }
                    }
                }

            }
        }
    }
}