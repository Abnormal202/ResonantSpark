﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Input.Combinations;
using ResonantSpark.Input;

namespace ResonantSpark {
    namespace CharacterStates {
        public class Stand : BaseState {

            public Walk walk;
            public WalkSlow walkSlow;
            public Still still;

            public float maxForwardForce;
            public float maxBackwardForce;
            public float maxHorizontalForce;

            public AnimationCurve forwardForce;
            public AnimationCurve horizontalForce;

            public float movementChangeDampTime;
            public Vector3 deadZone;

            //private Vector3 charVel = Quaternion.Inverse(fgChar.rigidbody.rotation) * fgChar.rigidbody.velocity;
            private Vector3 smoothedInput = Vector3.zero;
            private Input.FightingGameInputCodeDir dirPress = Input.FightingGameInputCodeDir.None;

            private float charRotation;

            private bool upJump = false;

            [Tooltip("in degrees per frame (1/60 s)")]
            public float maxRotation;

            public new void Start() {
                base.Start();
                states.Register(this, "stand");

                RegisterInputCallbacks()
                    //.On<DirectionPress>(OnDirectionPress)
                    .On<DoubleTap>(OnDoubleTap)
                    .On<ButtonsCurrent>(OnButtonsCurrent)
                    .On<ButtonPress>(OnButtonPress)
                    .On<DirectionCurrent>(OnDirectionCurrent);

                RegisterEnterCallbacks()
                    .On<DirectionPress>(GivenDirectionPress)
                    .On<DirectionCurrent>(GivenDirectionCurrent)
                    .On<NeutralReturn>(GivenNeutralReturn)
                    .On<Empty>(GivenNothing);
            }

            public override void Enter(int frameIndex, IState previousState) {
                    // Start OnEnter with this
                GivenInput(fgChar.GivenCombinations());

                fgChar.SetLocalMoveDirection(0.0f, 0.0f);
                fgChar.Play("stand", 0, 0.0f);
            }

            public override void Execute(int frameIndex) {
                FindInput(fgChar.GetFoundCombinations());

                //...((FightingGameInputCodeDir)((verticalInput + 1) * 3 + (horizontalInput + 1) + 1));

                int cameraX = (((int) dirPress) - 1) % 3 - 1;
                int cameraZ = (((int) dirPress) - 1) / 3 - 1;

                //Debug.Log("FGInput = " + dirPress + " (" + ((int) dirPress) + ") | Char X = " + charX + ", Char Z = " + charZ);

                Vector3 localVelocity = fgChar.GetLocalVelocity();
                Vector3 localInput = fgChar.CameraToChar(new Vector3(cameraX, 0, upJump ? 0 : cameraZ));

                    // Move the character
                WalkCharacter(localVelocity, localInput);
                TurnCharacter(localInput);

                    // Use helper states to animate the character
                AnimateCharacter(localVelocity, localInput);
            }

            public override void Exit(int frameIndex) {
                // do nothing
            }

            private void WalkCharacter(Vector3 localVelocity, Vector3 localInput) {
                smoothedInput = Vector3.Lerp(smoothedInput, localInput, movementChangeDampTime * fgChar.gameTime);

                if (smoothedInput.sqrMagnitude > deadZone.sqrMagnitude) {

                    Vector3 movementForce = new Vector3 {
                        x = smoothedInput.x,
                        y = smoothedInput.y,
                        z = smoothedInput.z,
                    };
                    movementForce.Scale(new Vector3 {
                        x = maxHorizontalForce * horizontalForce.Evaluate(localVelocity.x),
                        y = 1.0f,
                        z = (localVelocity.z > 0 ? maxForwardForce : maxBackwardForce) * forwardForce.Evaluate(localVelocity.z)
                    });

                    //Debug.Log("Local Velocity: " + localVelocity.ToString("F3"));
                    //Debug.Log("Smoothed Input: " + smoothedInput.ToString("F3"));
                    //Debug.Log("Adding local force to character: " + movementForce.ToString("F3"));
                    fgChar.rigidbody.AddRelativeForce(movementForce);
                }
            }

            private void TurnCharacter(Vector3 localInput) {
                if (fgChar.Grounded(out Vector3 currStandingPoint)) {
                    if (localInput.sqrMagnitude > 0.0f) {
                        charRotation = fgChar.LookToMoveAngle() / fgChar.gameTime;
                        if (charRotation != 0.0f) {
                            fgChar.rigidbody.MoveRotation(Quaternion.AngleAxis(Mathf.Clamp(charRotation, -maxRotation, maxRotation) * fgChar.realTime, Vector3.up) * fgChar.rigidbody.rotation);
                        }
                    }
                }
                else {
                    //TODO: don't turn character while in mid air
                    Debug.LogError("Character not grounded while in 'Stand' character state");
                }
            }

            private void AnimateCharacter(Vector3 localVelocity, Vector3 localInput) {

            }

            private void StateSelect_upJump(Action stop, Combination combo) {
                switch (this.dirPress) {
                    case FightingGameInputCodeDir.UpLeft:
                    case FightingGameInputCodeDir.Up:
                    case FightingGameInputCodeDir.UpRight:
                        fgChar.UseCombination(combo);
                        stop();
                        changeState(states.Get("jump"));
                        break;
                    case FightingGameInputCodeDir.Left:
                    case FightingGameInputCodeDir.Right:
                        stop();
                        break;
                    case FightingGameInputCodeDir.DownLeft:
                    case FightingGameInputCodeDir.Down:
                    case FightingGameInputCodeDir.DownRight:
                        fgChar.UseCombination(combo);
                        stop();
                        changeState(states.Get("crouch"));
                        break;
                }
            }

            private void OnDirectionPress(Action stop, Combination combo) {
                stop();
                this.dirPress = ((DirectionPress) combo).direction;
                if (upJump) {
                    StateSelect_upJump(stop, combo);
                }
            }

            private void OnDoubleTap(Action stop, Combination combo) {
                var doubleTap = (DoubleTap) combo;
                //doubleTap.inUse = true;
                //stop.Invoke();
                //changeState(states.Get("run").Message(combo));
            }

            private void OnDirectionCurrent(Action stop, Combination combo) {
                this.dirPress = ((DirectionCurrent) combo).direction;
                if (upJump) {
                    StateSelect_upJump(stop, combo);
                }
            }

            private void OnButtonPress(Action stop, Combination combo) {
                var buttonPress = (ButtonPress) combo;

                if (buttonPress.button0 == FightingGameInputCodeBut.A) {
                    fgChar.UseCombination(combo);
                    stop();
                    changeState(states.Get("attack"));
                }
            }

            private void OnButtonsCurrent(Action stop, Combination combo) {
                ButtonsCurrent curr = (ButtonsCurrent) combo;

                this.upJump = curr.butS;
            }

            private void GivenDirectionPress(Action stop, Combination combo) {
                dirPress = ((DirectionPress) combo).direction;
            }

            private void GivenDirectionCurrent(Action stop, Combination combo) {
                dirPress = ((DirectionCurrent) combo).direction;
            }

            private void GivenNeutralReturn(Action stop, Combination combo) {
                dirPress = Input.FightingGameInputCodeDir.Neutral;
            }

            private void GivenNothing(Action stop, Combination combo) {
                dirPress = Input.FightingGameInputCodeDir.Neutral;
            }
        }
    }
}
