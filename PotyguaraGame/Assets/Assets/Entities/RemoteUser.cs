using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RemoteUser : MonoBehaviour
{
    private Animator currentAnimator;

    [Header("Interpolation")]
    private Vector3 lastPosition;
    private Vector3 targetPosition;
    private float lastUpdateTime;
    [SerializeField] private float sendRate = 0.1f; // tempo entre updates (100ms)

    public void OnPositionUpdate(Vector3 newPosition)
    {
        // Esta é a forma robusta de verificar se dois vetores são praticamente idênticos.
        // Usamos sqrMagnitude por ser muito mais rápido que Vector3.Distance.
        if ((targetPosition - newPosition).sqrMagnitude < 0.0001f)
        {
            // O novo alvo é o mesmo que o nosso alvo atual. Não há nada a fazer.
            return; 
        }

        // Se chegamos aqui, o alvo é genuinamente novo.
        // Inicie uma nova interpolação a partir da posição atual na tela.
        lastPosition = transform.parent.position;
        targetPosition = newPosition;
        lastUpdateTime = Time.time;
    }
    private void Update()
    {
        float time = (Time.time - lastUpdateTime) / sendRate;
        time = Mathf.Clamp01(time);
        transform.parent.position = Vector3.Lerp(lastPosition, targetPosition, time);
    }
}