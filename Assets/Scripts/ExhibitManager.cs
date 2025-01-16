using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ExhibitManager : MonoBehaviour
{
    [System.Serializable]
    public class ExhibitData
    {
        public string Name;
        public Sprite Portrait;
        public string DisplayName;
        public string Details;
        public Sprite CountryImage;
    }

    public GameObject prefab;
    public List<ExhibitData> exhibitDataList;

    public Vector3 position = new Vector3(125.040802f, 3.284781f, -148.472122f);
    public Quaternion rotation = new Quaternion(-0.8660254f, 0f, 0.5f, 0f);
    public Vector3 scale = new Vector3(0.05f, 0.05f, 0.05f);

    void Start()
    {
        foreach (ExhibitData data in exhibitDataList)
        {
            CreateExhibit(data);
        }
    }

    void CreateExhibit(ExhibitData data)
    {
        GameObject exhibit = Instantiate(prefab, position, rotation);
        exhibit.transform.localScale = scale;

        exhibit.name = data.Name;

        Transform mainShowcase = exhibit.transform.Find("Main Showcase");
        Image portrait = mainShowcase.Find("Portrait").GetComponent<Image>();
        portrait.sprite = data.Portrait;

        TextMeshProUGUI nameText = mainShowcase.Find("Name").GetComponent<TextMeshProUGUI>();
        nameText.text = data.DisplayName;

        Transform shortBio = mainShowcase.Find("ShortBio");
        TextMeshProUGUI detailsText = shortBio.Find("Details").GetComponent<TextMeshProUGUI>();
        detailsText.text = data.Details;

        Image countryImage = shortBio.Find("Country").GetComponent<Image>();
        countryImage.sprite = data.CountryImage;
    }
}
