using Assets.Scripts;
using System;
using Unity.Netcode;
using UnityEngine;

public partial class PlayerController
{
    private bool wasInKickState = false;
    private Kick kick = new();
    public float maxForce;
    public float minForce;
    public float maxKickTime;
    public float sideKickForce;

    private void setRacketColorByKickForce()
    {
        var color = Color.Lerp(Color.white, Color.red, kick.force / maxForce);
        Material newRacketMaterial = new Material(racketMaterial);
        newRacketMaterial.color = color;
        racket.GetComponent<Renderer>().material = newRacketMaterial;
    }

    private void StartKick()
    {
        animator.enabled = true;
        animator.SetTrigger("Kick");
    }
    private void EndKick()
    {
        animator.enabled = false;
        racket.GetComponent<Renderer>().material = racketMaterial;
        kick = new(); // reset kick
        wasInKickState = false;
    }

    private void Kicking(NetworkObject ball, Kick kickData, bool clientKick = false, bool rotationKick = false, bool gravityChange = false)
    {
        if (IsHost)
        {
            int direction = clientKick ? -1 : 1;

            ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
            ball.transform.position += new Vector3(0, 0, 0.5f * direction); // move ball a bit forward to avoid animation clipping
            ball.GetComponent<Rigidbody>().AddForce(direction * kickData.XdirectionForce,
                                                    Math.Clamp(kickData.force / 2, 4, 7),
                                                    direction * kickData.force,
                                                    ForceMode.Impulse);

            print("Kicking ball with force: " + kickData.force + ", upForce: " + (Math.Clamp(kickData.force / 2, 4, 7)) + " and X direction force: " + kickData.XdirectionForce + " by " +
                 (clientKick ? "client" : "host") + " player ");


            BallController ballController = ball.GetComponent<BallController>();

            bool enableRotationKickEffect = clientKick ? rotationKick : currentEffects.BallRotationPowerDuration > 0;
            bool enableGravityChangeEffect = clientKick ? gravityChange : currentEffects.GravityPowerDuration > 0;

            ballController.Kicked(clientKick ? PlayerSide.Client : PlayerSide.Host, enableRotationKickEffect);

            if (enableGravityChangeEffect)
            {
                ballController.DecreaseWeight();
            }
        }
        else
        {
            bool rotationKickEffect = currentEffects.BallRotationPowerDuration > 0;
            bool gravityChangeEffect = currentEffects.GravityPowerDuration > 0;
            KickingServerRpc(ball, kickData, rotationKickEffect, gravityChangeEffect);
        }
    }
    private void DetermineKickForce(Vector2 kickMouseStartPos, Vector2 kickMouseEndPos, float kickMouseEndTime, float kickMouseStartTime)
    {
        float mouseTime = kickMouseEndTime - kickMouseStartTime;
        mouseTime = Mathf.Clamp(mouseTime, 0, maxKickTime);

        Vector2 kickDirection = kickMouseEndPos - kickMouseStartPos;
        kickDirection.Normalize();

        kick = new Kick
        {
            force = minForce + (maxForce - minForce) * (mouseTime / maxKickTime),
            XdirectionForce = kickDirection.x * sideKickForce
        };
    }
}