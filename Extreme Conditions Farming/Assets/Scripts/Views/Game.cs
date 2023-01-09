using System;
using System.Collections.Generic;
using ECF.Domain.Common;
using ECF.Domain.Game;
using ECF.Behaviours;
using ECF.Behaviours.Services;
using ECF.Behaviours.Systems;
using ECF.Domain;
using ECF.Views.UI;
using UnityEngine;

namespace ECF.Views
{
    public class Game : MonoBehaviour
    {
        [SerializeField] private ToolController toolController;
        [SerializeField] private ViewController viewController;
        [SerializeField] private WindowManager windowManager;
        [SerializeField] private Sounds sounds;
        [SerializeField] private GameSettings settings;
        public ToolController Tools => toolController;

        public Sounds Sounds => sounds;
        public GameSettings Settings => settings;
        public Dictionary<string, CropConfig> CropConfigs { get; } = new();
        public ViewController ViewController => viewController;
        
        public ObservableValue<GamePhase> Phase { get; } = new (GamePhase.MainMenu);
        public IStorageService StorageService { get; set; }
        public ISimulation Simulation { get; private set; }
        public event Action OnNewSimulationCreated;

        public WindowManager WindowManager => windowManager;
        private float lastTick;
        private PlayerSave save;

        public bool HasSavedSimulation => save.SimulationState != null;
        
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ReloadDomain()
        {
            instance = null;
        }
        
        private static Game instance;
        
        public static Game Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<Game>();
                }
                return instance;
            }
        }

        public void Init()
        {
            Application.targetFrameRate = 144;
            StorageService = new PlayerPrefsStorage();
            save = StorageService.Load(() => new PlayerSave());
            windowManager.Show<MainMenu>();
            foreach (var cropConfig in Resources.LoadAll<CropConfig>("CropConfigs"))
            {
                CropConfigs.Add(cropConfig.name, cropConfig);
            }
        }

        private void Start()
        {
            Init();
        }
        
        public void StartNew()
        {
            save.SimulationState = null;
            StartSimulation(save);
            Save();
        }

        public void Continue()
        {
            StartSimulation(save);
        }
        
        public void StartSimulation(PlayerSave save)
        {
            var config = settings.GetSimulationConfig();
            config.Templates = new List<CropTemplate>();
            foreach (var cropConfig in CropConfigs)
            {
                config.Templates.Add(cropConfig.Value.ToTemplate());
            }

            config.StartItems = new List<InventoryItemData>()
            {
                new()
                {
                    Amount = settings.startSeedsAmount,
                    Id = config.Templates[0].SeedId
                },
                new ()
                {
                    Amount = settings.startCoins,
                    Id = InventoryItems.Coins
                }
            };
            Simulation = new Simulation(config, save.SimulationState);
            Simulation.CreateSystems();
            OnNewSimulationCreated?.Invoke();
            Simulation.OnGameOver += OnGameOver;
            Phase.Value = GamePhase.Playing;
        }

        private void OnGameOver()
        {
            Phase.Value = GamePhase.GameOver;
            WindowManager.Show<GameOverScreen>();
            save.SimulationState = null;
        }

        private void Update()
        {
            if (Phase.Value != GamePhase.Playing)
            {
               return;
            }

            if (Time.time - lastTick >= 0.5f)
            {
                lastTick = Time.time;
                Simulation.Tick(1);
            }
        }

        public void Quit()
        {
            Save();
            Application.Quit();
        }

        public void Save()
        {
            if (save == null)
            {
                return;
            }
            if (Simulation != null)
            {
                Simulation.SaveState();
                save.SimulationState = Simulation.State;
            }
            
            StorageService.Save(save);
        }

        public void ToMainMenu()
        {
            if (Phase.Value != GamePhase.Paused)
            {
                return;
            }
            
            Save();
            Phase.Value = GamePhase.MainMenu;
            windowManager.Show<MainMenu>();
        }
        
        public void Pause()
        {
            if (Phase.Value != GamePhase.Playing)
            {
                return;
            }
            Phase.Value = GamePhase.Paused;
            windowManager.Show<PauseMenu>();
        }

        private void OnApplicationQuit()
        {
            Save();
        }

        public void Resume()
        {
            if (Phase.Value != GamePhase.Paused)
            {
                return;
            }
            Phase.Value = GamePhase.Playing;
        }
    }
}