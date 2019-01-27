﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Valve.VR;

public class PlayerManager : MonoBehaviour {

    FMOD.Studio.EventInstance ambianceMusic; 

    FMOD.Studio.EventInstance oxygeneSound; 
    FMOD.Studio.ParameterInstance oxygeneSoundLevel;

    FMOD.Studio.EventInstance checkpointMusic;
    FMOD.Studio.ParameterInstance checkpointMusicNumber;

    FMOD.Studio.EventInstance collisionSound;
    FMOD.Studio.ParameterInstance collisionSoundForce;

    private bool[] checkpoint = new bool[] {false,false,false,false,false};
    private Vector3 lastCheckpointPosition;
    private Quaternion lastCheckpointRotation;

    private float oxygen = 10f;
    public float timerMaxOxygenSeconds = 120f;

    private float oxygenTimer = 0f;

    private void Start()
    {
        ambianceMusic = FMODUnity.RuntimeManager.CreateInstance("event:/Music/Ambiant"); // Chemin du son
        ambianceMusic.start(); // Joue le son

        //ambianceMusic.stop(); lors de la mort de l'avatar

        oxygeneSound = FMODUnity.RuntimeManager.CreateInstance("event:/Astronaut/Breathing"); // Chemin du son
        oxygeneSound.getParameter("OxygeneLevel", out oxygeneSoundLevel); // Va chercher le paramètre FMOD et le stocke 
        oxygeneSoundLevel.setValue(9.95f); // Valeur du paramètre en début de partie (Le paramètre descend jusqu'à 0, proportionnellement à la quantité d'oxygène actuelle de l'avatar)

        checkpointMusic = FMODUnity.RuntimeManager.CreateInstance("event:/Music/Checkpoint"); // Chemin du son
        checkpointMusic.getParameter("CheckpointIndex", out checkpointMusicNumber); // Va chercher le paramètre FMOD et le stocke
        checkpointMusicNumber.setValue(0.0f); // Valeur du paramètre en début de partie

        collisionSound = FMODUnity.RuntimeManager.CreateInstance("event:/Astronaut/Collision"); // Chemin du son
        collisionSound.getParameter("CollisionForce", out collisionSoundForce); // Va chercher le paramètre FMOD et le stocke 
        collisionSoundForce.setValue(0.0f); // Valeur du paramètre en début de partie

    }

    void Update ()
    {
        if (SteamVR_Input._default.inActions.Teleport.GetStateDown(SteamVR_Input_Sources.Any))
        {
            Restart();
        }

        oxygenTimer += Time.deltaTime;
        oxygeneSoundLevel.setValue(10-(oxygenTimer / (timerMaxOxygenSeconds/10)));

        if (oxygenTimer >= timerMaxOxygenSeconds)
        {
            Death();
        }
    }

    private void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        transform.position = lastCheckpointPosition + Vector3.one / 2;
        transform.rotation = lastCheckpointRotation;
        oxygeneSoundLevel.setValue(9.95f);
        //to do : coroutine de fade screen
    }

    private void Death()
    {
        oxygeneSoundLevel.setValue(10f);
    }


    public void Checkpoint(int level, Vector3 chekpointPosition, Quaternion checkpointRotation)
    {
        checkpoint[level] = true;
        lastCheckpointPosition = chekpointPosition;
        lastCheckpointRotation = checkpointRotation;
        oxygeneSoundLevel.setValue(10f);
        checkpointMusicNumber.setValue(Mathf.Clamp(level + 1, 1, 4));
        checkpointMusic.start(); // Joue le son
    }

    
}
