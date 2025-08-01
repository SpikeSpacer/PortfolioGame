using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("Main"); // �������� �� ��� ����� ������� �����
    }

    public void OpenSettings()
    {
        Debug.Log("��������� ���������");
        // ��� ����� ����� ������� ��������� ���� ��� �����
    }

    public void ExitGame()
    {
        Debug.Log("����� �� ����");
        Application.Quit();
    }
}
