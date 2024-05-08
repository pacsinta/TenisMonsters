using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PasswordVisibility : MonoBehaviour
{
    public Toggle passwordVisibilityToggle;
    public TMP_InputField passwordInput;
    public TMP_InputField passwordChangeInput;

    void Start()
    {
        passwordInput.contentType = TMP_InputField.ContentType.Password;
        passwordChangeInput.contentType = TMP_InputField.ContentType.Password;
        passwordVisibilityToggle.onValueChanged.AddListener(ChangeVisibility);
        passwordVisibilityToggle.isOn = false;
    }

    void ChangeVisibility(bool visible)
    {
        passwordInput.contentType = visible ? TMP_InputField.ContentType.Standard : TMP_InputField.ContentType.Password;
        passwordChangeInput.contentType = visible ? TMP_InputField.ContentType.Standard : TMP_InputField.ContentType.Password;

        passwordInput.ForceLabelUpdate();
        passwordChangeInput.ForceLabelUpdate();
    }
}
