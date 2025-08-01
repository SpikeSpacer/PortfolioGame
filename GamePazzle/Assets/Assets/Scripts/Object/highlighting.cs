using UnityEngine;

public class highlighting : MonoBehaviour
{
    [SerializeField] private GameObject _highlightingObject;

    public void EnableHighlighting()
    {
        if (_highlightingObject != null)
        {
            _highlightingObject.SetActive(true);
        }
        else
        {
            Debug.LogError("Объект подсветки не назначен в инспекторе!");
        }
    }

    public void DisableHighlighting()
    {
        if (_highlightingObject != null)
        {
            _highlightingObject.SetActive(false);
        }
    }
}
