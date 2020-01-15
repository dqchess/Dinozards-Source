﻿using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using XInputDotNetPure;

public class PlayerInput : MonoBehaviour
{
    #region Fields    

    public PlayerIndex playerIndex;
    [System.NonSerialized] public GamePadState state;
    [System.NonSerialized] public GamePadState prevState;

    bool playerIndexSet = false;

    public bool RTButtonDown;
    public bool RTButtonUp;
    public bool LTButtonDown;
    public bool LTButtonUp;

    private int currentMana;
    //FireballAbility fireball;
    private ReflectAbility reflect;
    private DashAbility dash;
    private ProjectileAbility projectile;
    private ChargeManager chargeManager;

    [Header("Audio List Names")]
    public List<string> castFireballAudioNames;
    public List<string> reflectAudioNames;
    public List<string> dashAudioNames;

    public int playerIndexInt;

    PlayerStates playerState;

    #endregion Fields

    private void Awake()
    {
        playerState = GetComponent<PlayerStates>();

        reflect = GetComponentInChildren<ReflectAbility>();
        Assert.IsNotNull(reflect, "Reflect ability was not found.");

        dash = GetComponentInChildren<DashAbility>();
        Assert.IsNotNull(dash, "Dash ability was not found.");

        projectile = GetComponentInChildren<ProjectileAbility>();
        Assert.IsNotNull(projectile, "Projectile ability was not found.");

        chargeManager = GetComponent<ChargeManager>();
        Assert.IsNotNull(chargeManager, "ChargeManager was not found.");

        if (playerIndex.ToString() == "One")
        {
            playerIndexInt = 1;
        }
        else if (playerIndex.ToString() == "Two")
        {
            playerIndexInt = 2;
        }
        else if (playerIndex.ToString() == "Three")
        {
            playerIndexInt = 3;
        }
        else if (playerIndex.ToString() == "Four")
        {
            playerIndexInt = 4;
        }

    }

    private void Update()
    {
        if (!playerState.IsState(PlayerStates.PossibleStates.IsWinning) && !playerState.IsState(PlayerStates.PossibleStates.IsDead)) {

            currentMana = chargeManager.CurrentCharges;

            prevState = state;
            state = GamePad.GetState(playerIndex);

            RTButtonDown = state.Triggers.Right >= 0.9f && RTButtonUp;
            RTButtonUp = state.Triggers.Right <= 0.9f;

            LTButtonDown = state.Triggers.Left >= 0.1f && LTButtonUp;
            LTButtonUp = state.Triggers.Left <= 0.1f;


            //FIREBALL
            if (IsButtonDown(state.Buttons.X, prevState.Buttons.X) || RTButtonDown) {
                if (projectile.manaCost <= currentMana && projectile.CanActivate()) {

                    currentMana -= projectile.OnActivate(gameObject);
                    chargeManager.CurrentCharges = currentMana;
                    int listIndex = Random.Range(0, castFireballAudioNames.Count);
                    AudioManager.instance.PlayWithRandomPitch(castFireballAudioNames[listIndex]);
                }

            }

            //Start Button
            if (IsButtonDown(state.Buttons.Start, prevState.Buttons.Start)) {

                if (GameManager.managerInstance.gameState == GameManager.eGameState.PLAYING)
                {
                    GameManager.managerInstance.gameState = GameManager.eGameState.PAUSE;
                    GameManager.managerInstance.OnGameStateSet();
                }


                //if (IsButtonDown(state.Buttons.Start, prevState.Buttons.Start) && IsButtonDown(state.Buttons.Back, prevState.Buttons.Back)) {
                //    GameManager.managerInstance.LoadScene(2);

                //}
            }

            //REFLECT
            if (IsButtonDown(state.Buttons.A, prevState.Buttons.A) || LTButtonDown) {
                if (reflect.manaCost <= currentMana && reflect.CanActivate()) {

                    currentMana -= reflect.OnActivate(gameObject);
                    chargeManager.CurrentCharges = currentMana;

                    int listIndex = Random.Range(0, reflectAudioNames.Count);
                    AudioManager.instance.PlayWithRandomPitch(reflectAudioNames[listIndex]);

                }

            }

            //DASH


                if (GameManager.managerInstance.gameState == GameManager.eGameState.PAUSE)
                {
                    if (IsButtonDown(state.Buttons.B, prevState.Buttons.B))
                    {
                        GameManager.managerInstance.UnpauseGame();
                    }
            }

            else if (IsButtonDown(state.Buttons.B, prevState.Buttons.B) ||
                IsButtonDown(state.Buttons.LeftShoulder, prevState.Buttons.LeftShoulder) ||
                IsButtonDown(state.Buttons.RightShoulder, prevState.Buttons.RightShoulder)) {
                if (dash.manaCost <= currentMana && dash.CanActivate()) {
                    currentMana -= dash.OnActivate(gameObject);
                    chargeManager.CurrentCharges = currentMana;

                    int listIndex = Random.Range(0, dashAudioNames.Count);
                    AudioManager.instance.PlayWithRandomPitch(dashAudioNames[listIndex]);
                }

            }
        }
    }

    public void SetPlayerIndex(int index)
    {
        playerIndex = (PlayerIndex)index;
        state = GamePad.GetState(playerIndex);
        if (state.IsConnected)
        {
            playerIndexSet = true;
        }
    }

    private bool IsButtonDown(ButtonState button, ButtonState previousState)
    {
        return previousState == ButtonState.Released && button == ButtonState.Pressed;
    }
}
