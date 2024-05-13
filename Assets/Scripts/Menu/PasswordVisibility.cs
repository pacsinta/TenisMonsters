using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PasswordVisibility : MonoBehaviour
{
    public Toggle passwordVisibilityToggle;
    private TMP_InputField passwordInput;

    void Start()
    {
        passwordInput = GetComponent<TMP_InputField>();
        passwordInput.contentType = TMP_InputField.ContentType.Password;
        passwordVisibilityToggle.onValueChanged.AddListener(ChangeVisibility);
        passwordVisibilityToggle.isOn = false;
    }

    void ChangeVisibility(bool visible)
    {
        passwordInput.contentType = visible ? TMP_InputField.ContentType.Standard : TMP_InputField.ContentType.Password;
        passwordInput.ForceLabelUpdate();
    }
}
