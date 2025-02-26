using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;
using System.Collections;
using UnityEngine.XR.Management;

public class SearchManager : MonoBehaviour
{
    [System.Serializable]
    public class Exhibit
    {
        public string name;
        public TeleportationAnchor teleportAnchor;
    }

    public List<Exhibit> exhibits;
    public GameObject searchForm;
    public TMP_InputField searchInput;
    public GameObject searchResultsContainer;
    public GameObject searchResultPrefab;

    private bool isSearchOpen = false;
    private InputDevice leftController;
    private bool xButtonPressed = false;
    private float buttonCooldown = 0.3f;
    private float lastButtonPressTime = 0f;
    private TeleportationProvider teleportationProvider;

    void Start()
    {
        searchInput.onValueChanged.AddListener(UpdateSearchResults);

        StartCoroutine(FindTeleportationProvider());
    }

    void Update()
    {
        if (!leftController.isValid)
        {
            leftController = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        }
        else
        {
            if (leftController.TryGetFeatureValue(CommonUsages.primaryButton, out bool isXPressed))
            {
                if (isXPressed && !xButtonPressed && Time.time - lastButtonPressTime > buttonCooldown)
                {
                    ToggleSearchForm();
                    lastButtonPressTime = Time.time;
                }
                xButtonPressed = isXPressed;
            }
        }
    }

    void ToggleSearchForm()
    {
        isSearchOpen = !isSearchOpen;
        searchForm.SetActive(isSearchOpen);

        if (isSearchOpen)
        {
            PositionSearchForm();
        }
    }

    void PositionSearchForm()
    {
        Transform cameraTransform = Camera.main.transform;
        searchForm.transform.position = cameraTransform.position + cameraTransform.forward * 2.5f;
        searchForm.transform.LookAt(cameraTransform);
        searchForm.transform.Rotate(0, 180, 0);
    }

    void UpdateSearchResults(string query)
    {
        foreach (Transform child in searchResultsContainer.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (var exhibit in exhibits)
        {
            if (exhibit.name.ToLower().Contains(query.ToLower()))
            {
                GameObject result = Instantiate(searchResultPrefab, searchResultsContainer.transform);
                result.GetComponentInChildren<TMP_Text>().text = exhibit.name;
                
                Button button = result.GetComponent<Button>();
                if (button != null)
                {
                    TeleportationAnchor anchor = exhibit.teleportAnchor;
                    button.onClick.RemoveAllListeners();

                    button.onClick.AddListener(() =>
                    {
                        searchForm.SetActive(false);
                        isSearchOpen = false;
                        TeleportToExhibit(anchor);
                    });
                }
            }
        }
    }

    void TeleportToExhibit(TeleportationAnchor anchor)
    {
        if (anchor != null)
        {
            anchor.gameObject.SetActive(true);

            if (teleportationProvider != null)
            {
                TeleportRequest request = new TeleportRequest
                {
                    destinationPosition = anchor.transform.position,
                    requestTime = Time.time,
                    matchOrientation = MatchOrientation.None
                };

                teleportationProvider.QueueTeleportRequest(request);
                StartCoroutine(SetPlayerRotationAfterTeleport(anchor.transform));
            }
            else
            {
                Debug.LogWarning("TeleportationProvider is not assigned. Using fallback teleport.");
                Camera.main.transform.position = anchor.transform.position;
            }
        }
        else
        {
            Debug.LogError("Teleportation Anchor is NULL!");
        }
    }

    private IEnumerator SetPlayerRotationAfterTeleport(Transform anchorTransform)
    {
        yield return null;

        Transform exhibitTransform = anchorTransform.parent;
        if (exhibitTransform == null)
        {
            Debug.LogError("Exhibit is NULL! Ensure Teleport Anchor has a parent.");
            yield break;
        }

        GameObject xrRig = Camera.main.transform.parent.gameObject;

        Vector3 exhibitForward = exhibitTransform.forward;

        // Flip the player's rotation by 180° to face the exhibit
        float targetYRotation = Quaternion.LookRotation(exhibitForward, Vector3.up).eulerAngles.y;
        targetYRotation += 180f;
        xrRig.transform.rotation = Quaternion.Euler(0, targetYRotation, 0);

        XRInputSubsystem subsystem = XRGeneralSettings.Instance.Manager.activeLoader.GetLoadedSubsystem<XRInputSubsystem>();
        if (subsystem != null)
        {
            subsystem.TryRecenter();
        }
        else
        {
            Debug.LogWarning("⚠️ XRInputSubsystem not found. Headset may not reset.");
        }
    }

    private IEnumerator FindTeleportationProvider()
    {
        TeleportationProvider provider = null;
        float timeout = 25f;
        float elapsedTime = 0f;

        while (provider == null && elapsedTime < timeout)
        {
            GameObject xrRig = GameObject.FindWithTag("Player");
            if (xrRig != null)
            {
                provider = xrRig.GetComponentInChildren<TeleportationProvider>();
            }

            if (provider == null)
            {
                yield return new WaitForSeconds(0.1f);
                elapsedTime += 0.1f;
            }
        }

        if (provider != null)
        {
            teleportationProvider = provider;
            Debug.Log("✅ TeleportationProvider found and assigned!");
        }
        else
        {
            Debug.LogError("🚨 TeleportationProvider could not be found!");
        }
    }

}
