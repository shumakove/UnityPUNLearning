using UnityEngine;
using UnityEngine.UI;

namespace Com.Soho.MyGame 
{
    /// <summary>
    /// Player name input field. Let the user input his name, will appear above the player in the game.
    /// </summary>
    [RequireComponent(typeof(InputField))]
    public class PlayerNameInputField : MonoBehaviour
    {
        #region Private Variables
        // store the PlayerPref Key to avoid typos
        static string playerNamePrefKey = "PlayerName";

        #endregion

        #region MonoBehaviour CallBacks

        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during initialization phase.
        /// </summary>
        private void Start()
        {
            string defaultName = "";
            InputField _inputField = this.GetComponent<InputField>();
            if (_inputField != null)
            {
                if (PlayerPrefs.HasKey(playerNamePrefKey))
                {
                    defaultName = PlayerPrefs.GetString(playerNamePrefKey);
                    _inputField.text = defaultName;
                }
            }
            PhotonNetwork.playerName = defaultName;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sets the name of player and save it in the PlayerPrefs for further sessions.
        /// </summary>
        /// <param name="value">The name of the Player</param>
        public void SetPlayerName(string value)
        {
            // #Important
            PhotonNetwork.playerName = value + " "; //force a trailing space string in case value is an empty string,


            PlayerPrefs.SetString(playerNamePrefKey, value);
        }

        #endregion
    }
}