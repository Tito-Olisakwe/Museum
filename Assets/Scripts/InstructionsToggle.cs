using UnityEngine;
using UnityEngine.XR;

public class InstructionsToggle : MonoBehaviour {
    public GameObject instructionsPanel;

    private bool isVisible = false;
    private InputDevice leftController;
    private bool menuButtonPressed = false;
    private float buttonCooldown = 0.3f;
    private float lastButtonPressTime = 0f;

    void Update() {
        if (!leftController.isValid) {
            leftController = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        } else {
            if (leftController.TryGetFeatureValue(CommonUsages.menuButton, out bool isPressed)) {
                if (isPressed && !menuButtonPressed && Time.time - lastButtonPressTime > buttonCooldown) {
                    ToggleInstructions();
                    lastButtonPressTime = Time.time;
                }
                menuButtonPressed = isPressed;
            }
        }
    }

    void ToggleInstructions() {
        isVisible = !isVisible;
        instructionsPanel.SetActive(isVisible);

        if (isVisible) {
            PositionInstructionsPanel();
        }
    }

    void PositionInstructionsPanel() {
        Transform cameraTransform = Camera.main.transform;
        instructionsPanel.transform.position = cameraTransform.position + cameraTransform.forward * 2.5f;
        instructionsPanel.transform.LookAt(cameraTransform);
        instructionsPanel.transform.Rotate(0, 180, 0);
    }
}
