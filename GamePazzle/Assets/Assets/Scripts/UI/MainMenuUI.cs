using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("Main"); // замените на имя вашей игровой сцены
    }

    public void OpenSettings()
    {
        Debug.Log("Открываем настройки");
        // Тут можно будет открыть отдельное окно или сцену
    }

    public void ExitGame()
    {
        Debug.Log("Выход из игры");
        Application.Quit();
    }
}
