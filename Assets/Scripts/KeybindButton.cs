using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class KeybindButton : MonoBehaviour
{
    public string actionName; // e.g., "Forward", "Sprint"
    public TextMeshProUGUI keyText;

    private bool isListeningForInput = false;

    void Start()
    {
        UpdateKeyText();
    }

    public void OnClickChangeKey()
    {
        if (!isListeningForInput)
        {
            isListeningForInput = true;
            keyText.text = "Press a key...";
            StartCoroutine(ListenForNewKey());
        }
    }
    
    IEnumerator ListenForNewKey()
    {
        while (isListeningForInput)
        {
            foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(keyCode))
                {
                    KeybindManager.Instance.ChangeKeybind(actionName, keyCode);
                    isListeningForInput = false;
                    UpdateKeyText();
                    yield break;
                }
            }
            yield return null;
        }
    }
    
    public void UpdateKeyText()
    {
        if (KeybindManager.Instance != null && KeybindManager.Instance.keybinds.ContainsKey(actionName))
        {
            keyText.text = KeybindManager.Instance.keybinds[actionName].ToString();
        }
    }
}