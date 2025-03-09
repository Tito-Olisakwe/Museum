using UnityEngine;
using TMPro;
using Normal.Realtime;

public class UsernameSync : RealtimeComponent<UsernameModel> {
    private TMP_Text nameTag;

    private void Start() {
        nameTag = GetComponentInChildren<TMP_Text>();
    }

    protected override void OnRealtimeModelReplaced(UsernameModel previousModel, UsernameModel currentModel) {
        if (previousModel != null) {
            previousModel.usernameDidChange -= UpdateNameTag;
        }

        if (currentModel != null) {
            if (currentModel.isFreshModel)
                currentModel.username = "Player";

            UpdateNameTag(currentModel, currentModel.username);
            currentModel.usernameDidChange += UpdateNameTag;
        }
    }

    private void UpdateNameTag(UsernameModel model, string username) {
        if (nameTag != null) {
            nameTag.text = username;
        }
    }

    public void SetUsername(string newUsername) {
        if (model != null) {
            model.username = newUsername;
        }
    }
}
