using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SelectableContainerUI : MonoBehaviour
{
    [SerializeField] private GameObject selectedBackground;
    [SerializeField] private GameObject unselectedBackground;
    [SerializeField] private Button selectButton;

    public void Selected() {
        selectedBackground.SetActive(true);
        unselectedBackground.SetActive(false);
    }

    public void Unselected() {
        selectedBackground.SetActive(false);
        unselectedBackground.SetActive(true);
    }

    public void AddButtonAction(UnityAction action) {
        selectButton.onClick.AddListener(action);
    }
}
