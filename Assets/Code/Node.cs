using UnityEngine;
using UnityEngine.EventSystems;

public class Node : MonoBehaviour
{
    public string system_name;

    private void Update()
    {
        // Check if the left mouse button is clicked
        if (Input.GetMouseButtonDown(0))
        {
            // Create a ray from the mouse position
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // Create a RaycastHit object to store the hit information
            RaycastHit hit;

            // Check if the ray hits any UI element
            if (!Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("UI")))
            {
                if(hit.transform == transform)
                {
                    GameManager.Session.setSelectedSystem(system_name);
                }
            }
        }
    }
}
