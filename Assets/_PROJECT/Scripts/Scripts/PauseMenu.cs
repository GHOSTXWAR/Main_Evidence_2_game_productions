using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class PauseMenu : MonoBehaviour
{
    public GameObject MainPauseMenu;
    public GameObject PauseOptionsMenu;

    private void Awake()
    {

    }


    private void OnEnable()
    {
        MainPauseMenu.SetActive(true);
        PauseOptionsMenu.SetActive(false);
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;
    }
    private void OnDisable()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
    }
}
