using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace RPGSystem
{
    [Serializable]
    public struct GameReferences
    {
        public Player player;
        public Image flashScreen;
    }

    [RequireComponent(typeof(AudioManager))]
    public class RPGManager : MonoBehaviour
    {
        public static RPGManager Instance;

        public static GameReferences refs = new();
        public static GameState gameState = new();

        bool isInteractionAvailable = true;
        bool isMovementAvailable = true;

        public FogData fogData;

        public bool IsInteractionAvailable => isInteractionAvailable && true;
        public bool IsMovementAvailable => isMovementAvailable && true;

        void Awake()
        {
            if (Instance != null) Destroy(this.gameObject);
            else
            {
                Instance = this;
                DontDestroyOnLoad(this.gameObject);
                refs.player = GameObject.Find("PLAYER").GetComponent<Player>();
                refs.flashScreen = GameObject.Find("RPGFlashScreen").GetComponent<Image>();
                SceneManager.activeSceneChanged += OnActiveSceneChanged;
            }
        }

        void Update()
        {
            // Save & Load
            if (Input.GetKeyDown(KeyCode.F6)) gameState.SaveGameStateSlot(0);
            if (Input.GetKeyDown(KeyCode.F9)) gameState.LoadGameStateSlot(0).Forget();
        }

        void OnActiveSceneChanged(Scene arg0, Scene arg1)
        {
            try { SpawnPlayer(); } catch { print("No se encontraron Player Spawn Points"); }
        }

        public void SpawnPlayer()
        {
            refs.player.LookAtDirection(gameState.savedFaceDir);
            refs.player.transform.position = gameState.savedMapSpawnIndex >= 0 ?
                GameObject.Find("[SPAWN]").transform.GetChild(gameState.savedMapSpawnIndex).position
                : gameState.savedPosition;
            CameraController.SetPosition(refs.player.transform.position);
        }

        public void SetPlayerFreeze(FreezeType freezeType)
        {
            switch (freezeType)
            {
                case FreezeType.FreezeAll: isInteractionAvailable = isMovementAvailable = false; break;
                case FreezeType.FreezeInteraction: isInteractionAvailable = false; isMovementAvailable = true; break;
                case FreezeType.FreezeMovement: isInteractionAvailable = true; isMovementAvailable = false; break;
                case FreezeType.None: isInteractionAvailable = isMovementAvailable = true; break;
            }
        }
    }

}