using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.XR;

public class FormManager : MonoBehaviour
{
    [Header("Feedback Form")]
    public GameObject feedbackFormPanel;
    public TMP_InputField nameInput;
    public TMP_InputField countryInput;
    public TMP_InputField feedbackInput;
    public Button submitFeedbackButton;
    private string feedbackFormURL = "https://docs.google.com/forms/d/e/1FAIpQLSfHGy_xDGjQ7NCSoRH-yyJaiPLLXbwujV_VbxWLnVLeYicu5w/formResponse";

    [Header("Talent Request Form")]
    public GameObject talentFormPanel;
    public TMP_InputField companyNameInput;
    public TMP_InputField companyWebsiteInput;
    public TMP_InputField requesterEmailInput;
    public TMP_InputField requestInput;
    public Button submitTalentButton;
    private string talentFormURL = "https://docs.google.com/forms/u/0/d/e/1FAIpQLSf_prpOVdeHSOiom_-NEuYldwChiPn6ke3rdbKjzO11jENv7g/formResponse";

    private bool feedbackFormActive = false;
    private bool talentFormActive = false;
    private bool aButtonPressed = false;
    private bool bButtonPressed = false;
    private float buttonCooldown = 0.3f;
    private float lastButtonPressTime = 0;

    void Start()
    {
        submitFeedbackButton.onClick.AddListener(SendFeedback);
        submitTalentButton.onClick.AddListener(SendTalentRequest);
    }

    void Update()
    {
        float currentTime = Time.time;

        // Detect A button press (Feedback Form)
        if (XRInputCheck(XRNode.RightHand, CommonUsages.primaryButton, ref aButtonPressed) && currentTime - lastButtonPressTime > buttonCooldown)
        {
            ToggleForm(feedbackFormPanel, ref feedbackFormActive);
            lastButtonPressTime = currentTime;
        }

        // Detect B button press (Talent Form)
        if (XRInputCheck(XRNode.RightHand, CommonUsages.secondaryButton, ref bButtonPressed) && currentTime - lastButtonPressTime > buttonCooldown)
        {
            ToggleForm(talentFormPanel, ref talentFormActive);
            lastButtonPressTime = currentTime;
        }
    }

    void ToggleForm(GameObject form, ref bool formState)
    {
        formState = !formState;

        if (formState)
        {
            form.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 2.5f;
            form.transform.LookAt(Camera.main.transform);
            form.transform.Rotate(0, 180, 0);
        }

        form.SetActive(formState);
    }

    void SendFeedback()
    {
        StartCoroutine(PostFeedbackToGoogleForm(
            feedbackFormURL, 
            nameInput.text, 
            countryInput.text, 
            feedbackInput.text
        ));
    }

    void SendTalentRequest()
    {
        StartCoroutine(PostTalentToGoogleForm(
            talentFormURL, 
            companyNameInput.text, 
            companyWebsiteInput.text, 
            requesterEmailInput.text, 
            requestInput.text
        ));
    }

    IEnumerator PostFeedbackToGoogleForm(string url, string name, string country, string feedback)
    {
        WWWForm form = new WWWForm();
        form.AddField("entry.928339295", name);
        form.AddField("entry.169476443", country);
        form.AddField("entry.1566282203", feedback);

        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Feedback Submitted Successfully!");
                ClearAndHideFeedbackForm();
            }
            else
            {
                Debug.Log("Feedback Submission Failed: " + www.error);
            }
        }
    }

    IEnumerator PostTalentToGoogleForm(string url, string companyName, string companyWebsite, string requesterEmail, string requestDetails)
    {
        WWWForm form = new WWWForm();
        form.AddField("entry.928339295", companyName);
        form.AddField("entry.1330070163", companyWebsite);
        form.AddField("entry.1457564798", requesterEmail);
        form.AddField("entry.1575572694", requestDetails);

        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Talent Request Submitted Successfully!");
                ClearAndHideTalentForm();
            }
            else
            {
                Debug.Log("Talent Request Submission Failed: " + www.error);
            }
        }
    }

    void ClearAndHideFeedbackForm()
    {
        nameInput.text = "";
        countryInput.text = "";
        feedbackInput.text = "";
        feedbackFormPanel.SetActive(false);
        feedbackFormActive = false;
    }

    void ClearAndHideTalentForm()
    {
        companyNameInput.text = "";
        companyWebsiteInput.text = "";
        requesterEmailInput.text = "";
        requestInput.text = "";
        talentFormPanel.SetActive(false);
        talentFormActive = false;
    }

    bool XRInputCheck(XRNode node, InputFeatureUsage<bool> button, ref bool buttonState)
    {
        InputDevice device = InputDevices.GetDeviceAtXRNode(node);
        bool pressed = false;

        if (device.TryGetFeatureValue(button, out pressed))
        {
            if (pressed && !buttonState)
            {
                buttonState = true;
                return true;
            }
            else if (!pressed)
            {
                buttonState = false;
            }
        }
        return false;
    }
}
