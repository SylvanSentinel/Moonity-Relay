using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerID : MonoBehaviour
{
    public int pID;
    public string playerName = "";
    public TextMeshProUGUI nameText;
    private TwitchIRC IRC;
    public Chatter latestChatter;

    void Start()
    {
        pID = UnityEngine.Random.Range(int.MinValue, int.MaxValue);

        // Place TwitchIRC.cs script on an gameObject called "TwitchIRC"
        IRC = GameObject.Find("TwitchIRC").GetComponent<TwitchIRC>();

        // Add an event listener
        IRC.newChatMessageEvent.AddListener(NewMessage);

    }


    public void NewMessage(Chatter chatter)
    {
        Debug.Log("New chatter object received! Chatter name: " + chatter.tags.displayName);

        // Some examples on how you could use the chatter objects...

        if (chatter.tags.displayName == "Lexone")
            Debug.Log("Chat message was sent by Lexone!");

        if (chatter.HasBadge("subscriber"))
            Debug.Log("Chat message sender is a subscriber");

        if (chatter.HasBadge("moderator"))
            Debug.Log("Chat message sender is a channel moderator");

        if (chatter.MessageContainsEmote("25")) //25 = Kappa emote ID
            Debug.Log("Chat message contained the Kappa emote");

        if (chatter.message == "!join")
            Debug.Log(chatter.tags.displayName + " said !join");

        // Get chatter's name color (RGBA Format)
        //
        Color nameColor = chatter.GetRGBAColor();

        // Check if chatter's display name is "font safe"
        //
        // Most fonts don't support unusual characters
        // If that's the case then you could use their login name instead (chatter.login) or use a fallback font
        // Login name is always lowercase and can only contain characters: a-z, A-Z, 0-9, _
        //
        if (chatter.IsDisplayNameFontSafe())
            Debug.Log("Chatter's displayName is font-safe (only characters: a-z, A-Z, 0-9, _)");



        // This is just to show how the Chatter object looks like inside the Inspector
        latestChatter = chatter;
    }

    [SerializeField] TextMeshProUGUI error;
    private void Update()
    {
        error.text = IRC.errorString;

        nameText.text = "Display your Twitch name: " + playerName;
    }


    public void GET_KEY()
    {
        Application.OpenURL("https://twitchapps.com/tmi/");
    }

    [SerializeField] TMP_InputField authKeyInput;
    public void IRC_CONNECT()
    {
        IRC.twitchDetails.oauth = authKeyInput.text;
        IRC.settings.debugIRC = true;
        IRC.IRC_Connect();
    }

}
