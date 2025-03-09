using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.XR;
using Normal.Realtime;

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

        if (model != null) {
            model.latestMessage = message;
        }
    }

    protected override void OnRealtimeModelReplaced(ChatModel previousModel, ChatModel currentModel) {
        if (previousModel != null) {
            previousModel.latestMessageDidChange -= UpdateChat;
        }

        if (currentModel != null) {
            if (currentModel.isFreshModel) {
                currentModel.latestMessage = "";
            }

            currentModel.latestMessageDidChange += UpdateChat;
        }
    }

    private void UpdateChat(ChatModel model, string newMessage) {
        GameObject newChatMessage = Instantiate(chatMessagePrefab, chatMessagesContainer);
        newChatMessage.GetComponentInChildren<TMP_Text>().text = newMessage;
    }
}
