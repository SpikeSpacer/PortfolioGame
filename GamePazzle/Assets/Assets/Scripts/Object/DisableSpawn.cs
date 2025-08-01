using UnityEngine;

public class DisableSpawn : MonoBehaviour
{
    private bool _state = true;

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            PlayerRespawn respawnScript = other.GetComponent<PlayerRespawn>();
            if (respawnScript != null && _state)
            {
                respawnScript._spawnpoint_1 = false;
                respawnScript._spawnpoint_2 = true;
                _state = false;
            }
            else
            {
                respawnScript._spawnpoint_1 = true;
                respawnScript._spawnpoint_2 = false;
                _state = true;
            }

            //if (other.CompareTag("Player"))
            //{
            //    PlayerRespawn respawnScript = other.GetComponent<PlayerRespawn>();
            //    if (respawnScript != null && _state)
            //    {
            //        respawnScript.enabled = false;
            //        _state = false;
            //    }
            //    else
            //    {
            //        respawnScript.enabled = true;
            //        _state = true;
            //    }
            //}
        }
    }

}