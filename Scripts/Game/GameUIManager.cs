using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Fusion;
using UnityEngine.EventSystems;

public class GameUIManager : NetworkBehaviour//, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    private TMP_Text _roomCodeText;

    [SerializeField]
    private Button _jumpBtn;
    [SerializeField]
    private Button _attackBtn;

    public Button AttackBtn
    {
        get
        {
            return _attackBtn;
        }
    }

    public FloatingJoystick floatingJoyStick;

    public float horizon;
    public float vertical;

    public bool isAttackButtonpPress;
    public bool isJumpingButtonPress;

    private Coroutine coroutineBtn;

    //private GameUIManager _instance;
    //public GameUIManager Instance
    //{
    //    get
    //    {
    //        return _instance;
    //    }
    //}

    //private void Awake()
    //{

    //    if (_instance != null)
    //    {
    //        Destroy(gameObject);
    //        return;
    //    }

    //    _instance = this;

    //    DontDestroyOnLoad(gameObject);


    //}

    // Start is called before the first frame update
    void Start()
    {
        NetworkManager networkManager = Managers.Instance.networkManager;

        if(networkManager != null)
        {
            _roomCodeText.text = "Room Code: " + networkManager.RoomCode;
        }

            if (_jumpBtn != null)
            {
                _jumpBtn.onClick.AddListener(() =>
                {
                    //Managers.Instance.JumpPressButton();
                    if (coroutineBtn != null)
                    {
                        coroutineBtn = null;
                    }
                    coroutineBtn = StartCoroutine(OnJumpingBtnWithDelay());
                });
            }

            if (_attackBtn != null)
            {
                _attackBtn.onClick.AddListener(() =>
                {
                    //Managers.Instance.ShootPressButton();
                    //OnAttackBtnClick();
                    if(coroutineBtn != null)
                    {
                        coroutineBtn = null;
                    }
                    coroutineBtn = StartCoroutine(OnAttackBtnWithDelay());
                });
            }

        // UI Attacks/Ability Button

        // Subcribe

        //if (HasStateAuthority)
        //{
        //    Managers.Instance.UpdatePlayerHealthEvent += SetHealthBarUI;
        //}


    }


    // Update is called once per frame
    void Update()
    {
       
    }

    public IEnumerator OnAttackBtnWithDelay()
    {
        _attackBtn.interactable = false;
        isAttackButtonpPress = true;
        yield return new WaitForSeconds(0.0100f);
        _attackBtn.interactable = true;
        yield return new WaitForEndOfFrame();
        isAttackButtonpPress = false;

    }

    public void OnAttackBtnClick()
    {
        isAttackButtonpPress = true;
    }

    public void OnAttackBtnUp()
    {
        isAttackButtonpPress = false;
    }


    public IEnumerator OnJumpingBtnWithDelay()
    {
        _jumpBtn.interactable = false;
        isJumpingButtonPress = true;
        yield return new WaitForSeconds(0.0100f);
        _jumpBtn.interactable = true;
        yield return new WaitForEndOfFrame();
        isJumpingButtonPress = false;
    }
}
