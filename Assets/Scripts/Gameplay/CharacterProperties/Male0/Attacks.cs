using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using ResonantSpark.Builder;
using ResonantSpark.Utility;
using ResonantSpark.Input;
using ResonantSpark.Character;
using ResonantSpark.Gameplay;
using ResonantSpark.Service;

namespace ResonantSpark {
    namespace CharacterProperties {
        namespace Male0 {
            public class Attacks {

                private IFightingGameService fgService;
                private IAudioService audioService;
                private IProjectileService projectileService;
                private IParticleService particleService;
                private ICameraService cameraService;
                private ITimeService timeService;

                private Dictionary<string, AudioClip> audioMap;
                private Dictionary<string, ParticleEffect> particleMap;
                private Dictionary<string, AnimationCurveCallback> attackMovementMap;

                                        // Action<> attackMovementCallback = (frame) => {
                                        //     return animationCurve.Evaluate(frame);
                                        // };

                public Attacks(AllServices services, Dictionary<string, AudioClip> audioMap) {
                    fgService = services.GetService<IFightingGameService>();
                    audioService = services.GetService<IAudioService>();
                    projectileService = services.GetService<IProjectileService>();
                    particleService = services.GetService<IParticleService>();
                    cameraService = services.GetService<ICameraService>();
                    timeService = services.GetService<ITimeService>();

                    this.audioMap = audioMap;
                }

                public void Init(ICharacterPropertiesCallbackObj charBuilder, FightingGameCharacter fgChar) {

                    Attack atkSpw5A = new Attack(atkBuilder => { atkBuilder
                        .Name("southpaw_5A")
                        .Orientation(Orientation.SOUTHPAW)
                        .GroundRelation(GroundRelation.GROUNDED)
                        .Input(InputNotation._4A, InputNotation._5A)
                        .AnimationState("southpaw_5A")
                        .Movement(attackMovementMap["southpaw_5A.x"], null, attackMovementMap["southpaw_5A.z"])
                        .Frames(
                            FrameUtil.CreateList(f => { f
                                .SpecialCancellable(true)
                                .CancellableOnWhiff(true)
                                .ChainCancellable(true)
                                .From(1)
                                    .HitBox()
                                .From(4)
                                    .Track((target) => {
                                            // the vector that forward should point to, and a max turn speed per frame.
                                        fgChar.LookTowards(fgChar.rigidbody.position - target.position, 20.0f);
                                    })
                                .To(8)
                                .From(7)
                                    .CancellableOnWhiff(false)
                                .From(8)
                                    .Hit(hit => {
                                        hit.HitDamage(800)
                                            .BlockDamage(0)
                                            .HitStun(20.0f)
                                            .BlockStun(8.0f)
                                            .ComboScaling(0.65f)
                                            .Priority(AttackPriority.LightAttack);

                                        // hit.Block(); default is to allow any kind of blocking

                                        hit.HitBox(hb => {
                                            hb.Relative(fgChar.transform);
                                            hb.Point0(new Vector3(0, 0, 0));
                                            hb.Point1(new Vector3(0, 1, 0));
                                            hb.Radius(0.25f);
                                            hb.Event("onHitFGChar", 
                                                CommonGroundedOnHitFGChar(
                                                    hitSound: audioMap["weakHit"],
                                                    attackPriority: AttackForcePriority.LightAttack,
                                                    groundedForceMagnitude: 0.5f,
                                                    airborneForceDirection: new Vector3(0.0f, 1.0f, 1.0f),
                                                    airborneForceMagnitude: 1.0f));
                                        });
                                    })
                                .To(10)
                                .From(10)
                                    .HitBox()
                                    .ChainCancellable(true)
                                .To(22);
                            }))
                        .CleanUp(ReturnToPreviousState);
                    });

                    Attack atkSpw5AA = new Attack(atkBuilder => { atkBuilder
                        .Name("southpaw_5AA")
                        .Orientation(Orientation.SOUTHPAW)
                        .GroundRelation(GroundRelation.GROUNDED)
                        .Input(InputNotation._5A)
                        .AnimationState("southpaw_5AA")
                        .Movement(attackMovementMap["southpaw_5AA.x"], null, attackMovementMap["southpaw_5AA.z"])
                        .Frames(
                            FrameUtil.CreateList(f => { f
                                .SpecialCancellable(true)
                                .CancellableOnWhiff(false)
                                .From(1)
                                    .HitBox()
                                    .ChainCancellable(false)
                                .From(4)
                                    .Track((target) => {
                                        fgChar.LookTowards(fgChar.rigidbody.position - target.position, 30.0f);
                                    })
                                .To(12)
                                .From(10)
                                    .ChainCancellable(true)
                                .From(12)
                                    .Hit(hit => {
                                        hit.HitDamage(1000)
                                            .BlockDamage(0)
                                            .HitStun(30.0f)
                                            .BlockStun(12.0f)
                                            .ComboScaling(1.0f)
                                            .Priority(AttackPriority.LightAttack);

                                        hit.HitBox(hb => {
                                            hb.Relative(fgChar.transform);
                                            hb.Point0(new Vector3(0, 0, 0));
                                            hb.Point1(new Vector3(0, 1, 0));
                                            hb.Radius(0.25f);
                                            hb.Event("onHitFGChar",
                                                CommonGroundedOnHitFGChar(
                                                    hitSound: audioMap["mediumHit"],
                                                    attackPriority: AttackForcePriority.MediumAttack,
                                                    groundedForceMagnitude: 0.5f,
                                                    airborneForceDirection: new Vector3(0.0f, 1.0f, 1.0f),
                                                    airborneForceMagnitude: 1.0f));

                                            hb.Event("onEqualPriorityHitBox", (hitInfo) => {
                                                if (hitInfo.hitEntity != fgChar) {
                                                    fgChar.PredeterminedActions("verticalClash");
                                                }
                                            });
                                        });
                                    })
                                .To(14)
                                .From(14)
                                    .HitBox()
                                .To(30);
                            }))
                        .CleanUp(ReturnToPreviousState);
                    });

                    Attack atkSpw5AAA = new Attack(atkBuilder => { atkBuilder
                        .Name("southpaw_5AAA")
                        .Orientation(Orientation.SOUTHPAW)
                        .GroundRelation(GroundRelation.GROUNDED)
                        .Input(InputNotation._5A)
                        .AnimationState("southpaw_5AAA")
                        .Movement(attackMovementMap["southpaw_5AAA.x"], null, attackMovementMap["southpaw_5AAA.z"])
                        .Rotation((localFrame, target) => {
                            if (localFrame >= 22.0f && localFrame <= 30.0f) {
                                float p = (localFrame - 22.0f) / 30.0f;
                                fgChar.SetLookAt(Quaternion.Euler(0.0f, -180f * p, 0.0f) * (fgChar.rigidbody.position - target.position));
                                    // fgChar.GetOrientation is done automatically on each frame
                            }
                        })
                        .Frames(
                            FrameUtil.CreateList(f => { f
                                .SpecialCancellable(true)
                                .CancellableOnWhiff(false)
                                .From(1)
                                    .HitBox()
                                    .ChainCancellable(false)
                                .To(20)
                                .From(20)
                                    .Hit(hit => {
                                        hit.HitDamage(1000)
                                            .BlockDamage(0)
                                            .HitStun(30.0f)
                                            .BlockStun(12.0f)
                                            .Priority(AttackPriority.MediumAttack);

                                        hit.HitBox(hb => {
                                            hb.Relative(fgChar.transform);
                                            hb.Point0(new Vector3(0, 0, 0));
                                            hb.Point1(new Vector3(0, 1, 0));
                                            hb.Radius(0.25f);
                                            hb.Event("onHitFGChar",
                                                CommonGroundedOnHitFGChar(
                                                    hitSound: audioMap["mediumHit"],
                                                    attackPriority: AttackForcePriority.MediumAttack,
                                                    groundedForceMagnitude: 0.5f,
                                                    airborneForceDirection: new Vector3(0.0f, 1.0f, 1.0f),
                                                    airborneForceMagnitude: 1.0f));

                                            hb.Event("onEqualPriorityHitBox", (hitInfo) => {
                                                if (hitInfo.hitEntity != fgChar) {
                                                    fgChar.PredeterminedActions("horizontalClashSwingFromLeft");
                                                }
                                            });
                                        });
                                    })
                                .To(21)
                                .From(22)
                                    .HitBox()
                                .To(30);
                            }))
                        .CleanUp(ReturnToPreviousState);
                    });

                    Attack atkOrt5A = new Attack(atkBuilder => { atkBuilder
                        .Name("orthodox_5A")
                        .Orientation(Orientation.ORTHODOX)
                        .GroundRelation(GroundRelation.GROUNDED)
                        .Input(InputNotation._4A, InputNotation._5A)
                        .AnimationState("orthodox_5A")
                        .Movement(attackMovementMap["orthodox_5A.x"], null, attackMovementMap["orthodox_5A.z"])
                        .Frames(
                            FrameUtil.CreateList(f => { f
                                .SpecialCancellable(true)
                                .CancellableOnWhiff(true)
                                .ChainCancellable(true)
                                .From(1)
                                    .HitBox()
                                .From(4)
                                    .Track((target) => {
                                        fgChar.LookTowards(fgChar.rigidbody.position - target.position, 20.0f);
                                    })
                                .To(10)
                                .From(9)
                                    .CancellableOnWhiff(false)
                                .From(10)
                                    .Hit(hit => {
                                        hit.HitDamage(900)
                                            .BlockDamage(0)
                                            .HitStun(24.0f)
                                            .BlockStun(10.0f)
                                            .Priority(AttackPriority.LightAttack);

                                        hit.HitBox(hb => {
                                            hb.Relative(fgChar.transform);
                                            hb.Point0(new Vector3(0, 0, 0));
                                            hb.Point1(new Vector3(0, 1, 0));
                                            hb.Radius(0.25f);
                                            hb.Event("onHitFGChar",
                                                CommonGroundedOnHitFGChar(
                                                    hitSound: audioMap["weakHit"],
                                                    attackPriority: AttackForcePriority.LightAttack,
                                                    groundedForceMagnitude: 0.5f,
                                                    airborneForceDirection: new Vector3(0.0f, 1.0f, 1.0f),
                                                    airborneForceMagnitude: 1.0f));
                                        });
                                    })
                                .To(10)
                                .From(10)
                                    .HitBox()
                                    .ChainCancellable(true)
                                .To(22);
                            }))
                        .CleanUp(ReturnToPreviousState);
                    });

                    Attack atkOrt5AA = new Attack(atkBuilder => { atkBuilder
                        .Name("orthodox_5AA")
                        .Orientation(Orientation.ORTHODOX)
                        .GroundRelation(GroundRelation.GROUNDED)
                        .Input(InputNotation._5A)
                        .AnimationState("orthodox_5AA")
                        .Movement(attackMovementMap["orthodox_5AA.x"], null, attackMovementMap["orthodox_5AA.z"])
                        .Rotation((localFrame, target) => {
                            if (localFrame >= 20.0f && localFrame <= 26.0f) {
                                float p = (localFrame - 20.0f) / 26.0f;
                                fgChar.SetLookAt(Quaternion.Euler(0.0f, -180f * p, 0.0f) * (fgChar.rigidbody.position - target.position));
                                    // fgChar.GetOrientation is done automatically on each frame
                            }
                        })
                        .Frames(
                            FrameUtil.CreateList(f => { f
                                .SpecialCancellable(true)
                                .ChainCancellable(false)
                                .CancellableOnWhiff(false)
                                .From(1)
                                    .HitBox()
                                .From(4)
                                    .Track((target) => {
                                        fgChar.LookTowards(fgChar.rigidbody.position - target.position, 20.0f);
                                    })
                                .To(16)
                                .From(16)
                                    .Hit(hit => {
                                        hit.HitDamage(1200)
                                            .BlockDamage(0)
                                            .HitStun(24.0f)
                                            .BlockStun(10.0f)
                                            .Priority(AttackPriority.MediumAttack);

                                        hit.HitBox(hb => {
                                            hb.Relative(fgChar.transform);
                                            hb.Point0(new Vector3(0, 0, 0));
                                            hb.Point1(new Vector3(0, 1, 0));
                                            hb.Radius(0.25f);
                                            hb.Event("onHitFGChar",
                                                CommonGroundedOnHitFGChar(
                                                    hitSound: audioMap["mediumHit"],
                                                    attackPriority: AttackForcePriority.MediumAttack,
                                                    groundedForceMagnitude: 0.5f,
                                                    airborneForceDirection: new Vector3(0.0f, 1.0f, 1.0f),
                                                    airborneForceMagnitude: 1.0f));

                                            hb.Event("onEqualPriorityHitBox", (hitInfo) => {
                                                if (hitInfo.hitEntity != fgChar) {
                                                    fgChar.PredeterminedActions("horizontalClashSwingFromRight");
                                                }
                                            });
                                        });
                                    })
                                .To(17)
                                .From(17)
                                    .HitBox()
                                .To(26);
                            }))
                        .CleanUp(ReturnToPreviousState);
                    });

                    Attack atkSpw2A = new Attack(atkBuilder => { atkBuilder
                        .Name("southpaw_2A")
                        .Orientation(Orientation.SOUTHPAW)
                        .GroundRelation(GroundRelation.GROUNDED)
                        .Input(InputNotation._2A)
                        .AnimationState("southpaw_2A")
                        .Movement(attackMovementMap["southpaw_2A.x"], null, attackMovementMap["southpaw_2A.z"])
                        .Frames(
                            FrameUtil.CreateList(f => { f
                                .SpecialCancellable(true)
                                .ChainCancellable(false)
                                .CancellableOnWhiff(false)
                                .From(1)
                                    .HitBox()
                                .To(8)
                                .From(8)
                                    .Hit(hit => {
                                        hit.HitDamage(600)
                                            .BlockDamage(0)
                                            .HitStun(10.0f)
                                            .BlockStun(8.0f);

                                        hit.Block(Block.LOW);

                                        hit.HitBox(hb => {
                                            hb.Relative(fgChar.transform);
                                            hb.Point0(new Vector3(0, 0, 0));
                                            hb.Point1(new Vector3(0, 1, 0));
                                            hb.Radius(0.25f);
                                            hb.Event("onHitFGChar", (hitInfo) => {
                                                if (hitInfo.hitEntity != fgChar) {
                                                    audioService.PlayOneShot(hitInfo.position, audioMap["weakHit"]);
                                                    fgService.Hit(hitInfo.hitEntity, hitInfo.hitBox, (opponent, hitAtSameTimeByAttackPriority) => {
                                                        opponent.LoseHealth(hitInfo.damage); // hitInfo.damage will include combo scaling.
                                                        opponent.Force(
                                                            attackPriority: AttackForcePriority.LightAttack,
                                                            launch: false,
                                                            forceDirection: fgChar.transform.rotation * new Vector3(0.0f, 1.0f, 1.0f),
                                                            forceMagnitude: 1.0f);
                                                    });
                                                }
                                            });
                                        });
                                    })
                                .To(9)
                                .From(9)
                                    .HitBox()
                                .From(14)
                                    .ChainCancellable(true)
                                .To(22);
                            }))
                        .CleanUp(ReturnToPreviousState);
                    });

                    Attack atkSpw2AA = new Attack(atkBuilder => { atkBuilder
                        .Name("southpaw_2AA")
                        .Orientation(Orientation.SOUTHPAW)
                        .GroundRelation(GroundRelation.GROUNDED)
                        .Input(InputNotation._2A)
                        .AnimationState("southpaw_2AA")
                        .Movement(attackMovementMap["southpaw_2AA.x"], null, attackMovementMap["southpaw_2AA.z"])
                        .Frames(
                            FrameUtil.CreateList(f => { f
                                .SpecialCancellable(true)
                                .ChainCancellable(false)
                                .CancellableOnWhiff(false)
                                .From(1)
                                    .HitBox()
                                .From(4)
                                    .Track((target) => {
                                        fgChar.LookTowards(fgChar.rigidbody.position - target.position, 20.0f);
                                    })
                                .To(13)
                                .From(13)
                                    .Hit(hit => {
                                        hit.HitDamage(800)
                                            .BlockDamage(0)
                                            .HitStun(20.0f)
                                            .BlockStun(20.0f)
                                            .ComboScaling(0.5f);

                                        hit.Block(Block.LOW);

                                        hit.HitBox(hb => {
                                            hb.Relative(fgChar.transform);
                                            hb.Point0(new Vector3(0, 0, 0));
                                            hb.Point1(new Vector3(0, 1, 0));
                                            hb.Radius(0.25f);
                                            hb.Event("onHitFGChar",
                                                CommonGroundedOnHitFGChar(
                                                    hitSound: audioMap["weakHit"],
                                                    attackPriority: AttackForcePriority.LightAttack,
                                                    groundedForceMagnitude: 0.5f,
                                                    airborneForceDirection: new Vector3(0.0f, 1.0f, 1.0f),
                                                    airborneForceMagnitude: 1.0f));
                                        });
                                    })
                                .To(14)
                                .From(14)
                                    .HitBox()
                                .To(28);
                            }))
                        .CleanUp((prevState) => {
                            fgChar.SetState(fgChar.State("stand"));
                        });
                    });

                    Attack atkOrt2A = new Attack(atkBuilder => { atkBuilder
                        .Name("orthodox_2A")
                        .Orientation(Orientation.ORTHODOX)
                        .GroundRelation(GroundRelation.GROUNDED)
                        .Input(InputNotation._2A)
                        .AnimationState("orthodox_2A")
                        .Movement(attackMovementMap["orthodox_2A.x"], null, attackMovementMap["orthodox_2A.z"])
                        .Frames(
                            FrameUtil.CreateList(f => { f
                                .SpecialCancellable(true)
                                .ChainCancellable(false)
                                .CancellableOnWhiff(false)
                                .From(1)
                                    .HitBox()
                                .To(12)
                                .From(12)
                                    .Hit(hit => {
                                        hit.HitDamage(600)
                                            .BlockDamage(0)
                                            .HitStun(12.0f)
                                            .BlockStun(8.0f);

                                        hit.HitBox(hb => {
                                            hb.Relative(fgChar.transform);
                                            hb.Point0(new Vector3(0, 0, 0));
                                            hb.Point1(new Vector3(0, 1, 0));
                                            hb.Radius(0.25f);
                                            hb.Event("onHitFGChar", (hitInfo) => {
                                                if (hitInfo.hitEntity != fgChar) {
                                                    audioService.PlayOneShot(hitInfo.position, audioMap["weakHit"]);
                                                    fgService.Hit(hitInfo.hitEntity, hitInfo.hitBox, (opponent, hitAtSameTimeByAttackPriority) => {
                                                        opponent.LoseHealth(hitInfo.damage); // hitInfo.damage will include combo scaling.
                                                        opponent.Force(
                                                            attackPriority: AttackForcePriority.LightAttack,
                                                            launch: false,
                                                            forceDirection: fgChar.transform.rotation * Vector3.forward,
                                                            forceMagnitude: 0.5f);
                                                    });
                                                }
                                            });
                                        });
                                    })
                                .To(13)
                                .From(13)
                                    .HitBox()
                                .From(15)
                                    .ChainCancellable(true)
                                .To(23);
                            }))
                        .CleanUp(ReturnToPreviousState);
                    });

                    Attack atkJumpSpw5A = new Attack(atkBuilder => { atkBuilder
                        .Name("jump_orthodox_5A")
                        .Orientation(Orientation.ORTHODOX)
                        .GroundRelation(GroundRelation.AIRBORNE)
                        .Input(InputNotation._5A)
                        .AnimationState("jump_orthodox_5A")
                        .StandCollider((localFrame, target) => {
                            if (localFrame >= 8.0f && localFrame <= 11.0f) {
                                float p = Mathf.Lerp(8.0f, 11.0f, localFrame);
                                fgChar.SetStandCollider(p * 0.5f * Vector3.up);
                            }
                        })
                        .Frames(
                            FrameUtil.CreateList(f => { f
                                .SpecialCancellable(true)
                                .ChainCancellable(true)
                                .CancellableOnWhiff(true)
                                .From(1)
                                    .HitBox()
                                .To(8)
                                .From(6)
                                    .CancellableOnWhiff(false)
                                .From(8)
                                    .Hit(hit => {
                                        hit.HitDamage(700)
                                            .BlockDamage(0)
                                            .HitStun(12.0f)
                                            .BlockStun(30.0f);

                                        hit.HitBox(hb => {
                                            hb.Relative(fgChar.transform);
                                            hb.Point0(new Vector3(0, 0, 0));
                                            hb.Point1(new Vector3(0, 1, 0));
                                            hb.Radius(0.25f);
                                            hb.Event("onHitFGChar",
                                                CommonGroundedOnHitFGChar(
                                                    hitSound: audioMap["mediumHit"],
                                                    attackPriority: AttackForcePriority.MediumAttack,
                                                    groundedForceMagnitude: 0.5f,
                                                    airborneForceDirection: new Vector3(0.0f, 1.0f, 1.0f),
                                                    airborneForceMagnitude: 1.0f));
                                        });
                                    })
                                .To(11)
                                .From(11)
                                    .HitBox()
                                .From(13)
                                    .ChainCancellable(true)
                                .To(23);
                            }))
                        .CleanUp((prevState) => {
                            fgChar.SetStandCollider(Vector3.zero);
                            fgChar.SetState(prevState);
                        });
                    });

                    Attack atkJumpSpw5AA = new Attack(atkBuilder => { atkBuilder
                        .Name("jump_southpaw_5AA")
                        .Orientation(Orientation.SOUTHPAW)
                        .GroundRelation(GroundRelation.AIRBORNE)
                        .Input(InputNotation._5A)
                        .AnimationState("jump_southpaw_5AA")
                        .StandCollider((localFrame, target) => {
                            if (localFrame >= 8.0f && localFrame <= 11.0f) {
                                float p = Mathf.Lerp(8.0f, 11.0f, localFrame);
                                fgChar.SetStandCollider(p * 0.5f * Vector3.up);
                            }
                        })
                        .Frames(
                            FrameUtil.CreateList(f => { f
                                .SpecialCancellable(true)
                                .ChainCancellable(false)
                                .CancellableOnWhiff(false)
                                .From(1)
                                    .HitBox()
                                .From(4)
                                    .Track((target) => {
                                        fgChar.LookTowards(fgChar.rigidbody.position - target.position, 20.0f);
                                    })
                                .To(12)
                                .From(12)
                                    .Hit(hit => {
                                        hit.HitDamage(700)
                                            .BlockDamage(0)
                                            .HitStun(12.0f)
                                            .BlockStun(12.0f);

                                        hit.HitBox(hb => {
                                            hb.Relative(fgChar.transform);
                                            hb.Point0(new Vector3(0, 0, 0));
                                            hb.Point1(new Vector3(0, 1, 0));
                                            hb.Radius(0.25f);
                                            hb.Event("onHitFGChar",
                                                CommonGroundedOnHitFGChar(
                                                    hitSound: audioMap["weakHit"],
                                                    attackPriority: AttackForcePriority.LightAttack,
                                                    groundedForceMagnitude: 0.5f,
                                                    airborneForceDirection: new Vector3(0.0f, 1.0f, 1.0f),
                                                    airborneForceMagnitude: 1.0f));
                                        });
                                    })
                                .To(13)
                                .From(13)
                                    .HitBox()
                                .From(16)
                                    .ChainCancellable(true)
                                .To(30);
                            }))
                        .CleanUp((prevState) => {
                            fgChar.SetStandCollider(Vector3.zero);
                            fgChar.SetState(prevState);
                        });
                    });

                    Attack atkJumpSpw5AAA = new Attack(atkBuilder => { atkBuilder
                        .Name("jump_southpaw_5AAA")
                        .Orientation(Orientation.SOUTHPAW)
                        .GroundRelation(GroundRelation.AIRBORNE)
                        .Input(InputNotation._5A)
                        .AnimationState("jump_southpaw_5AAA")
                        .StandCollider((localFrame, target) => {
                            if (localFrame >= 8.0f && localFrame <= 11.0f) {
                                float p = Mathf.Lerp(8.0f, 11.0f, localFrame);
                                fgChar.SetStandCollider(p * 0.5f * Vector3.up);
                            }
                        })
                        .Frames(
                            FrameUtil.CreateList(f => { f
                                .SpecialCancellable(true)
                                .ChainCancellable(false)
                                .CancellableOnWhiff(false)
                                .From(1)
                                    .HitBox()
                                .From(4)
                                    .Track((target) => {
                                        fgChar.LookTowards(fgChar.rigidbody.position - target.position, 20.0f);
                                    })
                                .To(12)
                                .From(20)
                                    .Hit(hit => {
                                        hit.HitDamage(2000)
                                            .BlockDamage(0)
                                            .HitStun(40.0f)
                                            .BlockStun(30.0f)
                                            .Priority(AttackPriority.HeavyAttack)
                                            .ComboScaling(1.0f);

                                        hit.HitBox(hb => {
                                            hb.Relative(fgChar.transform);
                                            hb.Point0(new Vector3(0, 0, 0));
                                            hb.Point1(new Vector3(0, 1, 0));
                                            hb.Radius(0.25f);
                                            hb.Event("onHitFGChar", (hitInfo) => {
                                                if (hitInfo.hitEntity != fgChar) {
                                                    audioService.PlayOneShot(hitInfo.position, audioMap["strongHit"]);
                                                        // This exists to make characters hitting each other async
                                                    fgService.Hit(hitInfo.hitEntity, hitInfo.hitBox, (opponent, hitAtSameTimeByAttackPriority) => {
                                                        opponent.LoseHealth(hitInfo.damage); // hitInfo.damage will include combo scaling.

                                                        if (hitAtSameTimeByAttackPriority == AttackPriority.HeavyAttack) { // colloquially known as trading
                                                            cameraService.PredeterminedActions("zoomIn", 0.5f);
                                                            timeService.PredeterminedActions("timeSlow", 0.5f);

                                                            opponent.Force(
                                                                attackPriority: AttackForcePriority.HeavyAttack,
                                                                launch: false,
                                                                forceDirection: fgChar.transform.rotation * Vector3.forward,
                                                                forceMagnitude: 0.5f);
                                                        }
                                                        else {
                                                            opponent.Force(
                                                                attackPriority: AttackForcePriority.HeavyAttack,
                                                                launch: true,
                                                                forceDirection: fgChar.transform.rotation * new Vector3(0.0f, 1.0f, 1.0f),
                                                                forceMagnitude: 2.0f);
                                                        }
                                                    });
                                                }
                                            });
                                            hb.Event("onHitProjectile", (hitInfo) => {
                                                Projectile proj = (Projectile) hitInfo.hitEntity;
                                                if (proj.health <= 2) {
                                                    fgService.Hit(proj, hitInfo.hitBox, (projectile, hitAtSameTimeByAttackPriority) => {
                                                        if (hitAtSameTimeByAttackPriority != null) {
                                                            particleService.PlayOneShot(hitInfo.position, particleMap["destroyParticle"]);
                                                                // these lines might be redundant
                                                            particleService.PlayOneShot(projectile.DestroyedParticle());
                                                            projectile.Destroy();
                                                        }
                                                    });
                                                }
                                            });
                                        });
                                    })
                                .To(21)
                                .From(21)
                                    .HitBox()
                                .To(40);
                            }))
                        .CleanUp((prevState) => {
                            fgChar.SetStandCollider(Vector3.zero);
                            fgChar.SetState(prevState);
                        });
                    });

                    Attack atkJumpOrt5A = new Attack(atkBuilder => { atkBuilder
                        .Name("jump_orthodox_5A")
                        .Orientation(Orientation.ORTHODOX)
                        .GroundRelation(GroundRelation.AIRBORNE)
                        .Input(InputNotation._5A)
                        .AnimationState("jump_orthodox_5A")
                        .StandCollider((localFrame, target) => {
                            if (localFrame >= 8.0f && localFrame <= 11.0f) {
                                float p = Mathf.Lerp(8.0f, 11.0f, localFrame);
                                fgChar.SetStandCollider(p * 0.5f * Vector3.up);
                            }
                        })
                        .Frames(
                            FrameUtil.CreateList(f => { f
                                .SpecialCancellable(true)
                                .CancellableOnWhiff(false)
                                .ChainCancellable(false)
                                .From(1)
                                    .HitBox()
                                .From(4)
                                    .Track((target) => {
                                        fgChar.LookTowards(fgChar.rigidbody.position - target.position, 20.0f);
                                    })
                                .To(10)
                                .From(10)
                                    .CancellableOnWhiff(false)
                                    .Hit(hit => {
                                        hit.HitDamage(700)
                                            .BlockDamage(0)
                                            .HitStun(12.0f)
                                            .BlockStun(12.0f)
                                            .ComboScaling(0.85f);

                                        hit.HitBox(hb => {
                                            hb.Relative(fgChar.transform);
                                            hb.Point0(new Vector3(0, 0, 0));
                                            hb.Point1(new Vector3(0, 1, 0));
                                            hb.Radius(0.25f);
                                            hb.Event("onHitFGChar",
                                                CommonGroundedOnHitFGChar(
                                                    hitSound: audioMap["mediumHit"],
                                                    attackPriority: AttackForcePriority.MediumAttack,
                                                    groundedForceMagnitude: 0.5f,
                                                    airborneForceDirection: new Vector3(0.0f, 1.0f, 1.0f),
                                                    airborneForceMagnitude: 1.0f));
                                        });
                                    })
                                .To(13)
                                .From(13)
                                    .HitBox()
                                .From(16)
                                    .ChainCancellable(true)
                                .To(30);
                            }))
                        .CleanUp((prevState) => {
                            fgChar.SetStandCollider(Vector3.zero);
                            fgChar.SetState(prevState);
                        });
                    });

                    Attack atkJumpOrt5AA = new Attack(atkBuilder => { atkBuilder
                        .Name("jump_orthodox_5AA")
                        .Orientation(Orientation.ORTHODOX)
                        .GroundRelation(GroundRelation.AIRBORNE)
                        .Input(InputNotation._5A)
                        .AnimationState("jump_orthodox_5AA")
                        .StandCollider((localFrame, target) => {
                            if (localFrame >= 8.0f && localFrame <= 11.0f) {
                                float p = Mathf.Lerp(8.0f, 11.0f, localFrame);
                                fgChar.SetStandCollider(p * 0.5f * Vector3.up);
                            }
                        })
                        .Frames(
                            FrameUtil.CreateList(f => { f
                                .SpecialCancellable(true)
                                .CancellableOnWhiff(false)
                                .ChainCancellable(false)
                                .From(1)
                                    .HitBox()
                                .From(4)
                                    .Track((target) => {
                                        fgChar.LookTowards(fgChar.rigidbody.position - target.position, 20.0f);
                                    })
                                .To(12)
                                .From(12)
                                    .Hit(hit => {
                                        hit.HitDamage(700)
                                            .BlockDamage(0)
                                            .HitStun(12.0f)
                                            .BlockStun(12.0f);

                                        hit.HitBox(hb => {
                                            hb.Relative(fgChar.transform);
                                            hb.Point0(new Vector3(0, 0, 0));
                                            hb.Point1(new Vector3(0, 1, 0));
                                            hb.Radius(0.25f);
                                            hb.Event("onHitFGChar",
                                                CommonGroundedOnHitFGChar(
                                                    hitSound: audioMap["mediumHit"],
                                                    attackPriority: AttackForcePriority.MediumAttack,
                                                    groundedForceMagnitude: 0.5f,
                                                    airborneForceDirection: new Vector3(0.0f, 1.0f, 1.0f),
                                                    airborneForceMagnitude: 1.0f));
                                        });
                                    })
                                .To(13)
                                .From(13)
                                    .HitBox()
                                .From(16)
                                    .ChainCancellable(true)
                                .To(30);
                            }))
                        .CleanUp((prevState) => {
                            fgChar.SetStandCollider(Vector3.zero);
                            fgChar.SetState(prevState);
                        });
                    });

                    Attack hadouken = new Attack(atkBuilder => { atkBuilder
                        .Name("hadouken")
                        .Orientation(Orientation.SOUTHPAW)
                        .GroundRelation(GroundRelation.GROUNDED)
                        .Input(InputNotation._236C)
                        .AnimationState("hadouken")
                        .Movement(null, null, attackMovementMap["hadouken.z"])
                        .Frames(
                            FrameUtil.CreateList(f => { f
                                .SpecialCancellable(false)
                                .ChainCancellable(false)
                                .From(1)
                                    .HitBox()
                                .From(3)
                                    .Sound(audioMap["hadouken"], soundResource => {
                                        soundResource.transform.position = fgChar.GetSpeakPosition();
                                        audioService.Play(soundResource);
                                    })
                                    .Track((target) => {
                                        fgChar.LookTowards(fgChar.rigidbody.position - target.position, 5.0f);
                                    })
                                .From(12)
                                    .Projectile(projectileMap["hadouken"], proj => {
                                        projectileService.FireProjectile(proj.id);
                                    })
                                .To(46);
                            }))
                        .CleanUp(ReturnToPreviousState);
                    });

                    charBuilder
                        .Attack(atkSpw5A, (charState, currAttack, prevAttacks) => { // prevAttacks is a List<Attack>
                            if (prevAttacks.Where(prev => prev == atkSpw5A).Count() >= 3) {
                                return false;
                            }
                            if (currAttack == null ||
                                currAttack == atkSpw2A ||
                                currAttack == atkSpw2AA) {
                                return true;
                            }
                            return false;
                        })
                        .Attack(atkSpw5AA, (charState, currAttack, prevAttacks) => {
                            if (currAttack == atkSpw5A) {
                                return true;
                            }
                            return false;
                        })
                        .Attack(atkSpw5AAA, (charState, currAttack, prevAttacks) => {
                            if (currAttack == atkSpw5AA) {
                                return true;
                            }
                            return false;
                        })
                        .Attack(atkOrt5A, (charState, currAttack, prevAttacks) => {
                            if (prevAttacks.Where(prev => prev == atkOrt5A).Count() >= 2) {
                                return false;
                            }
                            if (currAttack == null ||
                                currAttack == atkOrt5A ||
                                currAttack == atkOrt2A) {
                                return true;
                            }
                            return false;
                        })
                        .Attack(atkOrt5AA, (charState, currAttack, prevAttacks) => {
                            if (currAttack == atkOrt5A) {
                                return true;
                            }
                            return false;
                        })
                        .Attack(atkSpw2A, (charState, currAttack, prevAttacks) => {
                            if (prevAttacks.Where(prev => prev == atkSpw5A).Count() > 1 ||
                                prevAttacks.Contains(atkSpw2A)) {
                                return false;
                            }
                            if (currAttack == null ||
                                currAttack == atkSpw5A) {
                                return true;
                            }
                            return false;
                        })
                        .Attack(atkSpw2AA, (charState, currAttack, prevAttacks) => {
                            if (currAttack == atkSpw2A) {
                                return true;
                            }
                            return false;
                        })
                        .Attack(atkOrt2A, (charState, currAttack, prevAttacks) => {
                            if (prevAttacks.Where(prev => prev == atkOrt5A).Count() > 1 ||
                                prevAttacks.Contains(atkOrt2A)) {
                                return false;
                            }
                            if (currAttack == null ||
                                currAttack == atkOrt2A ||
                                currAttack == atkOrt5A) {
                                return true;
                            }
                            return false;
                        })
                        .Attack(atkJumpSpw5A, (charState, currAttack, prevAttacks) => {
                            if (currAttack == null) {
                                return true;
                            }
                            return false;
                        })
                        .Attack(atkJumpSpw5AA, (charState, currAttack, prevAttacks) => {
                            if (currAttack == atkJumpSpw5A) {
                                return true;
                            }
                            return false;
                        })
                        .Attack(atkJumpSpw5AAA, (charState, currAttack, prevAttacks) => {
                            if (currAttack == atkJumpSpw5AA) {
                                return true;
                            }
                            return false;
                        })
                        .Attack(atkJumpOrt5A, (charState, currAttack, prevAttacks) => {
                            if (currAttack == null) {
                                return true;
                            }
                            return false;
                        })
                        .Attack(atkJumpOrt5AA, (charState, currAttack, prevAttacks) => {
                            if (currAttack == atkJumpOrt5A) {
                                return true;
                            }
                            return false;
                        })
                        .Attack(hadouken, (charState, currAttack, prevAttacks) => {
                            if (!prevAttacks.Contains(hadouken)) {
                                return true;
                            }
                            return false;
                        });
                }

                private void ReturnToPreviousState(CharacterStates.BaseState prevState) {
                    fgChar.SetState(prevState);
                }

                private Action<HitInfo> CommonGroundedOnHitFGChar(AudioClip hitSound, attackPriority, float groundedForceMagnitude, Vector3 airborneForceDirection, float airborneForceMagnitude) {
                    return hitInfo => {
                        if (hitInfo.hitEntity != fgChar) {
                            audioService.PlayOneShot(hitInfo.position, hitSound);
                                // This exists to make characters hitting each other async
                            fgService.Hit(hitInfo.hitEntity, hitInfo.hitBox, (opponent, hitAtSameTimeByAttackPriority) => {
                                opponent.LoseHealth(hitInfo.damage); // hitInfo.damage will include combo scaling.

                                switch (opponent.GetGroundRelation()) {
                                    case GroundRelation.GROUNDED:
                                        opponent.Force(
                                            attackPriority: attackPriority,
                                            launch: false,
                                            forceDirection: fgChar.transform.rotation * Vector3.forward,
                                            forceMagnitude: groundedForceMagnitude);
                                        break;
                                    case GroundRelation.AIRBORNE:
                                        opponent.Force(
                                            attackPriority: AttackForcePriority.LightAttack,
                                            launch: true,
                                            forceDirection: fgChar.transform.rotation * airborneForceDirection,
                                            forceMagnitude: airborneForceMagnitude);
                                        break;
                                }
                            });
                        }
                    }
                }
            }
        }
    }
}
