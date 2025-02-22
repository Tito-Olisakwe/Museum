using UnityEngine;
using TMPro;
using System.Text;

public class LogExhibitNames : MonoBehaviour
{
    public Transform exhibitsParent; // Assign the "exhibits" parent GameObject

    void Start()
    {
        if (exhibitsParent == null)
        {
            Debug.LogError("Exhibits Parent is not assigned!");
            return;
        }

        StringBuilder namesList = new StringBuilder("Exhibit Names:\n");

        foreach (Transform exhibit in exhibitsParent)
        {
            // Find the "Name" object inside "Main Showcase"
            Transform nameObject = exhibit.Find("Main Showcase/Name");
            if (nameObject != null)
            {
                TMP_Text nameText = nameObject.GetComponent<TMP_Text>();
                if (nameText != null)
                {
                    namesList.AppendLine(nameText.text); // Only log the actual name
                }
                else
                {
                    namesList.AppendLine($"[Missing TMP_Text in] {exhibit.name}");
                }
            }
            else
            {
                namesList.AppendLine($"[Missing 'Name' GameObject] in {exhibit.name}");
            }
        }

        Debug.Log(namesList.ToString()); // Log everything in one console message
    }
}
