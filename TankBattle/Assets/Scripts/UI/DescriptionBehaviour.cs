using UnityEngine;

public class DescriptionBehaviour : MonoBehaviour
{
    public TMPro.TMP_Text Name;
    public TMPro.TMP_Text DescriptionText;


    public void Initialize(Description description)
    {
        Name.text= description.Name;
        DescriptionText.text= description.DescriptionText;
    }
}
