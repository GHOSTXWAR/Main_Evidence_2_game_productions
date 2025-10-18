using UnityEngine;
using UnityEngine.EventSystems;

public class EventSystemRefresh : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField]
    private GameObject NewButton;
    [SerializeField]
    private EventSystem UIEventSystem;
    void OnEnable()
    {
        UIEventSystem.firstSelectedGameObject = NewButton;
        UIEventSystem.SetSelectedGameObject(NewButton);
        UIEventSystem.enabled = false;
        UIEventSystem.enabled = true;
    }

}
