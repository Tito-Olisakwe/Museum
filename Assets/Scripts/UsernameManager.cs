using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using Normal.Realtime;

public class UsernameManager : MonoBehaviour {
    private TMP_InputField usernameInputField;
    private Button saveButton;
    private UsernameSync _usernameSync;
    private RealtimeView _realtimeView;

    private void Awake() {
        _realtimeView = GetComponent<RealtimeView>();
        _usernameSync = GetComponent<UsernameSync>();
    }

    private void OnEnable() {
        StartCoroutine(InitializeAfterSpawn());
    }

    private IEnumerator InitializeAfterSpawn() {
        yield return new WaitForSeconds(5f);

        GameObject introPanel = GameObject.Find("Intro Panel");
        if (introPanel != null) {
            usernameInputField = introPanel.transform.Find("Scroll View/Viewport/Content/Username/InputField (TMP)")
                               ?.GetComponent<TMP_InputField>();
            saveButton = introPanel.transform.Find("Scroll View/Viewport/Content/Username/Button")
                         ?.GetComponent<Button>();

            if (saveButton != null) {
                saveButton.onClick.AddListener(SaveUsername);
            }
        }

        if (usernameInputField == null || saveButton == null) {
            Debug.LogError("Username Input Field or Save Button not found in Intro Panel!");
            yield break;
        }

        if (_realtimeView.isOwnedLocallySelf) {
            string savedName = PlayerPrefs.GetString("Username", "Player");
            usernameInputField.text = savedName;
            _usernameSync.SetUsername(savedName);
        }
    }

    public void SaveUsername() {
        if (!_realtimeView.isOwnedLocallySelf) return;

        string username = usernameInputField.text;
        if (!string.IsNullOrEmpty(username)) {
            PlayerPrefs.SetString("Username", username);
            PlayerPrefs.Save();
            _usernameSync.SetUsername(username);
        }
    }
}
