using UnityEngine;
using System.Collections.Generic;

public class KeybindManager : MonoBehaviour
{
    public static KeybindManager Instance;

    public Dictionary<string, KeyCode> keybinds = new Dictionary<string, KeyCode>();

    public delegate void KeybindChanged();
    public static event KeybindChanged OnKeybindChanged;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // Set default keybinds
        SetDefaultKeybinds();
    }

    private void SetDefaultKeybinds()
    {
        keybinds.Add("Forward", KeyCode.W);
        keybinds.Add("Backward", KeyCode.S);
        keybinds.Add("Left", KeyCode.A);
        keybinds.Add("Right", KeyCode.D);
        keybinds.Add("Sprint", KeyCode.LeftShift);
        keybinds.Add("Crouch", KeyCode.LeftControl);
        keybinds.Add("Jump", KeyCode.Space);
    }
    
    public KeyCode GetKey(string actionName)
    {
        if (keybinds.ContainsKey(actionName))
        {
            return keybinds[actionName];
        }
        return KeyCode.None;
    }
    
    public void ChangeKeybind(string actionName, KeyCode newKey)
    {
        if (keybinds.ContainsKey(actionName))
        {
            keybinds[actionName] = newKey;
            OnKeybindChanged?.Invoke(); // Notify all listeners
        }
    }
}