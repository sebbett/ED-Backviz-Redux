using UnityEngine;
using UnityEngine.UI;

public class ui_about : MonoBehaviour
{
    public Button
            button_close,
            button_discord,
            button_github,
            button_kofi;

    void Start()
    {
        button_close.onClick.AddListener(() => Application.Quit());
        button_discord.onClick.AddListener(() => Application.OpenURL("https://discord.gg/TzmJfrPFK2"));
        button_github.onClick.AddListener(() => Application.OpenURL("https://github.com/sebbett/ED-Backviz-Redux"));
        button_kofi.onClick.AddListener(() => Application.OpenURL("https://ko-fi.com/sebinspace"));
    }
}
