using TMPro;
using UnityEngine;

public class LoginActor : MonoBehaviour
{
    [SerializeField] private TMP_InputField _inputIdField;
    [SerializeField] private TMP_InputField _inputPassField;
	[SerializeField] private GameObject _button;

    private LoginManager _loginManager;

    private void Start()
    {
        _loginManager = GetComponent< LoginManager >();
    }

    public void OnClickLogin()
    {
        StartCoroutine(_loginManager.WebReq_Auth(_inputIdField.text));
    }
}
