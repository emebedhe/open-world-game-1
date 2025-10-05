using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
public class enemyscript : MonoBehaviour
{
    public int health = 10;
    public GameObject player;
    public GameObject healthbar;
    public GameObject enemy;
    public GameObject healthbar_rotation;
    public GameObject border;
    NavMeshAgent agent;
    RectTransform rt;
    Rigidbody rb;
    void Start()
    {
        rt = healthbar.GetComponent<RectTransform>();
        healthbar.GetComponent<Image>().color = Color.green;
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
    }
    void Update()
    {
        #region Movement
        agent.SetDestination(player.transform.position);
        #endregion
        #region Enemy Cloning
        if (Input.GetKeyDown(KeyCode.C))
        {
            var clone = Instantiate(enemy, enemy.transform.position, enemy.transform.rotation);
        }
        #endregion
        transform.LookAt(Camera.main.transform);
        //transform.position = transform.forward * 2;
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
                        healthbar.GetComponent<Image>().color = Color.Lerp(Color.red, Color.green, health / 10.0f);
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