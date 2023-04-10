using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SettingsPanel : MonoBehaviour {
    [SerializeField]
    private GameObject panel;
    [SerializeField]
    private TMP_InputField inputField;
    private APIManager apiManager;


    private void Awake() {
        apiManager = FindObjectOfType<APIManager>();
        panel.SetActive(false);
    }

    public void TogglePanel() {
        panel.SetActive(!panel.activeSelf);
        if (panel.activeSelf) {
            inputField.text = apiManager.GetBaseUrl();
        }
    }

    public void OnRestartAPI() {
        apiManager.SaveBaseUrl(inputField.text);
        TogglePanel();
    }
}
