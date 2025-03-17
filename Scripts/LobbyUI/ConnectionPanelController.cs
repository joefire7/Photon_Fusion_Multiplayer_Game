using System;
using System.Collections;
using System.Threading;
using Extensions;
using Fusion;
using TMPro;
using UnityEngine;

namespace LobbyUI
{
    public class ConnectionPanelController : MonoBehaviour
    {
        [SerializeField] private GameObject hostButton;
        [SerializeField] private GameObject joinButton;
        [SerializeField] private GameObject roomCodeInputField;
        [SerializeField] private GameObject enterButton;
        [SerializeField] private GameObject backButton;
        [SerializeField] private GameObject cancelButton;
        [SerializeField] private TextMeshProUGUI infoText;
        
        private TMP_InputField _roomCodeInputField;
        public String GetRoomCodeInputField { get { return _roomCodeInputField.text; } }
       

        private IEnumerator _waitingStartGameTextCoroutine;
        private CancellationTokenSource _cancellationTokenSource;
        private readonly string _hostWaitingText = "Waiting to create game";
        private readonly string _clientWaitingText = "Waiting to join game";
        private readonly float _timeToChangeDot = 0.5f;
        
        private void Awake()
        {
            _roomCodeInputField = roomCodeInputField.GetComponent<TMP_InputField>();
        }
        
        private void Start()
        {
            SetDefaultState();
        }
        
        public async void OnHostButtonClicked()
        {
            cancelButton.SetActive(true);
            hostButton.SetActive(false);
            joinButton.SetActive(false);

            _waitingStartGameTextCoroutine = WaitingTextTask(true);
            StartCoroutine(_waitingStartGameTextCoroutine);

            _cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = _cancellationTokenSource.Token;
         
            await Managers.Instance.networkManager.StartGame(GameMode.Host, SessionCodeGenerator.GenerateSessionCode(), cancellationToken);


            // Stop Text Coroutine... loading
            if(_waitingStartGameTextCoroutine != null)
            {
                StartCoroutine(_waitingStartGameTextCoroutine);
            }

            infoText.text = String.Empty;

            if(cancelButton != null)
            {
                cancelButton.SetActive(false);
            }
        }
        
        public async void OnClientEnteredButtonClicked()
        {
            cancelButton.SetActive(true);
            roomCodeInputField.SetActive(false);
            enterButton.SetActive(false);
            backButton.SetActive(false);

            _waitingStartGameTextCoroutine = WaitingTextTask(true);
            StartCoroutine(_waitingStartGameTextCoroutine);


            _cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = _cancellationTokenSource.Token;

            await Managers.Instance.networkManager.StartGame(GameMode.Client, _roomCodeInputField.text, cancellationToken);

            // Stop Text Coroutine... loading
            if (_waitingStartGameTextCoroutine != null)
            {
                StartCoroutine(_waitingStartGameTextCoroutine);
            }

            infoText.text = String.Empty;


            if (cancelButton != null)
            {
                cancelButton.SetActive(false);
            }

        }
        
        public void OnCancelButtonClicked()
        {
            infoText.text = String.Empty;
            _cancellationTokenSource?.Cancel();

            // Stop Text Coroutine... loading
            if (_waitingStartGameTextCoroutine != null)
            {
                StartCoroutine(_waitingStartGameTextCoroutine);
            }

            infoText.text = String.Empty;

            SetDefaultState();
        }

        public void OnJoinButtonClicked()
        {
            cancelButton.SetActive(false);
            hostButton.SetActive(false);
            joinButton.SetActive(false);
            roomCodeInputField.SetActive(true);
            enterButton.SetActive(true);
            backButton.SetActive(true);
        }

        public void OnBackButtonClicked()
        {
            SetDefaultState();
        }
        
        private void SetDefaultState()
        {
            hostButton.SetActive(true);
            joinButton.SetActive(true);
            roomCodeInputField.SetActive(false);
            enterButton.SetActive(false);
            backButton.SetActive(false);
            cancelButton.SetActive(false);
            infoText.text = String.Empty;
        }
        
        private IEnumerator WaitingTextTask(bool isHost)
        {
            while (true)
            {
                infoText.text = isHost ? _hostWaitingText : _clientWaitingText;
            
                yield return new WaitForSeconds(_timeToChangeDot);
                infoText.text += ".";
                yield return new WaitForSeconds(_timeToChangeDot);
                infoText.text += ".";
                yield return new WaitForSeconds(_timeToChangeDot);
                infoText.text += ".";
                yield return new WaitForSeconds(_timeToChangeDot);
                infoText.text = infoText.text.Remove(infoText.text.Length - 3);
            }
        }
    }
}
