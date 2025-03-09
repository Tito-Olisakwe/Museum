using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using Normal.Realtime;
using UnityEngine.XR;

public class LiveChatManager : RealtimeComponent<ChatModel> {
    public GameObject chatPanel;
    public TMP_InputField chatInput;
    public Transform chatMessagesContainer;
    public GameObject chatMessagePrefab;
    public Button sendButton;

    private InputDevice leftController;
    private bool xButtonPressed = false;
    private float buttonCooldown = 0.3f;
    private float lastButtonPressTime = 0f;

    private void Start() {
        sendButton.onClick.AddListener(SendChatMessage);
    }

    void Update() {
        if (!leftController.isValid) {
            leftController = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        } else {
            if (leftController.TryGetFeatureValue(CommonUsages.secondaryButton, out bool isYPressed)) {
                if (isYPressed && !xButtonPressed && Time.time - lastButtonPressTime > buttonCooldown) {
                    ToggleChatPanel();
                    lastButtonPressTime = Time.time;
                }
                xButtonPressed = isYPressed;
            }
        }
    }

    void ToggleChatPanel() {
        bool isVisible = !chatPanel.activeSelf;
        chatPanel.SetActive(isVisible);

        if (isVisible) {
            PositionChatPanel();
        }
    }

    void PositionChatPanel() {
        Transform cameraTransform = Camera.main.transform;
        chatPanel.transform.position = cameraTransform.position + cameraTransform.forward * 3f;
        chatPanel.transform.LookAt(cameraTransform);
        chatPanel.transform.Rotate(0, 180, 0);
    }

    public void SendChatMessage() {
        if (string.IsNullOrEmpty(chatInput.text)) return;

        string message = chatInput.text;
        chatInput.text = "";

        string username = PlayerPrefs.GetString("Username", "Player");
        if (string.IsNullOrEmpty(username)) {
            username = "Unknown";
        }

        ChatEntry newEntry = new ChatEntry { username = username, message = message };

        if (model != null) {
            List<ChatEntry> chatEntries = new List<ChatEntry>();

            if (!string.IsNullOrEmpty(model.messagesJson)) {
                chatEntries = JsonUtility.FromJson<ChatEntryList>(model.messagesJson).messages;
            }

            // Add new message and update model
            chatEntries.Add(newEntry);
            ChatEntryList chatEntryList = new ChatEntryList { messages = chatEntries };
            model.messagesJson = JsonUtility.ToJson(chatEntryList);
        }
    }

    protected override void OnRealtimeModelReplaced(ChatModel previousModel, ChatModel currentModel) {
        if (previousModel != null) {
            previousModel.messagesJsonDidChange -= UpdateChat;
        }

        if (currentModel != null) {
            if (currentModel.isFreshModel) {
                currentModel.messagesJson = JsonUtility.ToJson(new ChatEntryList());
            }

            currentModel.messagesJsonDidChange += UpdateChat;
        }
    }

    private void UpdateChat(ChatModel model, string newMessagesJson) {
        // Clear previous messages to prevent duplication
        foreach (Transform child in chatMessagesContainer) {
            Destroy(child.gameObject);
        }

        ChatEntryList chatEntryList = JsonUtility.FromJson<ChatEntryList>(newMessagesJson);
        foreach (ChatEntry entry in chatEntryList.messages) {
            GameObject newChatMessage = Instantiate(chatMessagePrefab, chatMessagesContainer);
            TMP_Text messageText = newChatMessage.GetComponentInChildren<TMP_Text>();

            string username = string.IsNullOrEmpty(entry.username) ? "Unknown" : entry.username;
            messageText.text = $"<b>{username}:</b> {entry.message}";
        }
    }
}

[System.Serializable]
public class ChatEntry {
    public string username;
    public string message;
}

[System.Serializable]
public class ChatEntryList {
    public List<ChatEntry> messages = new List<ChatEntry>();
}
