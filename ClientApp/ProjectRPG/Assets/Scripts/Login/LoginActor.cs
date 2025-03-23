using TMPro;
using UnityEngine;

public class LoginActor : MonoBehaviour
{
    [SerializeField] private TMP_Text _inputId;
    [SerializeField] private TMP_InputField _inputIdField;
    [SerializeField] private TMP_Text _inputPass;
    [SerializeField] private TMP_InputField _inputPassField;
    [SerializeField] private GameObject _button;

    private TMP_Text _selectedInput;

    private void Awake()
    {
        _selectedInput = _inputId;

        // Add listener.
        _inputIdField.onSelect.AddListener(OnSelectInputId);
        _inputIdField.onValueChanged.AddListener(OnValueChangeId);
        _inputPassField.onSelect.AddListener(OnSelectInputPass);
        _inputPassField.onValueChanged.AddListener(OnValueChangePass);
    }


    public void OnChangeText()
    {
        Debug.Log("OnChangeText");
    }

    public void OnValueChangeId(string str)
    {
        Debug.Log("OnValueChangeId");
        _inputIdField.text += str;
    }

    public void OnSelectInputId(string str)
    {
        Debug.Log("OnSelectInputId");
        _selectedInput = _inputId;
    }

    public void OnValueChangePass(string str)
    {
        Debug.Log("OnValueChangePass");
        _inputIdField.text += str;
    }

    public void OnSelectInputPass(string str)
    {
        Debug.Log("OnSelectInputPass");
        _selectedInput = _inputPass;
    }
}
