using Unity.VisualScripting;
using UnityEngine;

public class TPToNextStage : MonoBehaviour
{
    [SerializeField] private Transform _appearPosition;


    private void OnCollisionEnter(Collision collision)
    {
       if(collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.transform.position = _appearPosition.position;
        }
    }
}
