using UnityEngine;

[CreateAssetMenu(fileName = "TextForHint", menuName = "UI/Hint Text")]
public class HintTextPrompt : ScriptableObject
{
    [TextArea(3, 10)]
    [SerializeField] private string _infoTextToShow = "��� ����� ������ ����������.";

    public string InfoText => _infoTextToShow;
}
