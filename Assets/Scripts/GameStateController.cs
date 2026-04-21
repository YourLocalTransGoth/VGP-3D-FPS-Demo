using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class GameStateController : MonoBehaviour
{
    [System.Serializable]
    private class SceneObjectState
    {
        public string path;
        public bool activeSelf;
        public Vector3 localPosition;
        public Quaternion localRotation;
        public Vector3 localScale;
    }

    [System.Serializable]
    private class SaveData
    {
        public string sceneName;
        public List<SceneObjectState> objects = new List<SceneObjectState>();
        public float healthFill = -1f;
        public float playerEnemyCount = 0f;
        public int globalEnemyDestroyed = 0;
    }

    public static GameStateController Instance { get; private set; }

    [SerializeField] private GameObject pausePanel;
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private string playerObjectName = "Player";

    private const string SAVE_KEY = "SavedLevelName";

    private bool isPaused = false;
    private bool shouldRestoreSnapshot = false;
    private SaveData pendingSaveData;

    private string SaveFilePath
    {
        get { return Path.Combine(Application.persistentDataPath, "savegame.json"); }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            SceneManager.sceneLoaded += OnSceneLoaded;
            TryFindPausePanel();
            WirePauseMenuButtons();

            if (pausePanel != null)
            {
                pausePanel.SetActive(false);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        TryFindPausePanel();
        WirePauseMenuButtons();

        if (shouldRestoreSnapshot && pendingSaveData != null)
        {
            StartCoroutine(RestoreSnapshotWhenReady());
        }

        if (!isPaused && pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M) && !isPaused)
        {
            GoToMainMenu();
        }
    }

    public void GoToMainMenu()
    {
        TryFindPausePanel();

        isPaused = true;
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
            return;
        }

        if (!SceneManager.GetSceneByName(mainMenuSceneName).isLoaded)
        {
            SceneManager.LoadScene(mainMenuSceneName, LoadSceneMode.Additive);
        }
    }

    public void ResumeGame()
    {
        TryFindPausePanel();

        isPaused = false;
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
            return;
        }

        if (SceneManager.GetSceneByName(mainMenuSceneName).isLoaded)
        {
            SceneManager.UnloadSceneAsync(mainMenuSceneName);
        }
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        Debug.Log("Quit requested.");
        Application.Quit();
    }

    public void SaveGame()
    {
        string currentLevel = SceneManager.GetActiveScene().name;
        PlayerPrefs.SetString(SAVE_KEY, currentLevel);

        SaveData saveData = new SaveData();
        saveData.sceneName = currentLevel;

        Scene activeScene = SceneManager.GetActiveScene();
        GameObject[] roots = activeScene.GetRootGameObjects();

        for (int i = 0; i < roots.Length; i++)
        {
            Transform[] transforms = roots[i].GetComponentsInChildren<Transform>(true);
            for (int j = 0; j < transforms.Length; j++)
            {
                Transform t = transforms[j];
                if (IsPauseUiObject(t))
                {
                    continue;
                }

                SceneObjectState state = new SceneObjectState();
                state.path = GetTransformPath(t);
                state.activeSelf = t.gameObject.activeSelf;
                state.localPosition = t.localPosition;
                state.localRotation = t.localRotation;
                state.localScale = t.localScale;
                saveData.objects.Add(state);
            }
        }

        HealthBar healthBar = FindObjectOfType<HealthBar>();
        if (healthBar != null && healthBar.healthBarSprite != null)
        {
            saveData.healthFill = healthBar.healthBarSprite.fillAmount;
        }

        PlayerMovement playerMovement = FindObjectOfType<PlayerMovement>();
        if (playerMovement != null)
        {
            saveData.playerEnemyCount = playerMovement.numOfEnDest;
        }

        saveData.globalEnemyDestroyed = GunShoot.enemyDestroyed;

        string json = JsonUtility.ToJson(saveData);
        File.WriteAllText(SaveFilePath, json);

        PlayerPrefs.Save();
        Debug.Log("Game Saved at: " + currentLevel + " (" + saveData.objects.Count + " objects)");
    }

    public void LoadGame()
    {
        if (!PlayerPrefs.HasKey(SAVE_KEY))
        {
            Debug.LogWarning("No save data found!");
            return;
        }

        if (!File.Exists(SaveFilePath))
        {
            Debug.LogWarning("No save file found at: " + SaveFilePath);
            return;
        }

        string json = File.ReadAllText(SaveFilePath);
        SaveData loadedData = JsonUtility.FromJson<SaveData>(json);
        if (loadedData == null || string.IsNullOrEmpty(loadedData.sceneName))
        {
            Debug.LogWarning("Save file is invalid.");
            return;
        }

        pendingSaveData = loadedData;
        shouldRestoreSnapshot = true;

        string levelToLoad = loadedData.sceneName;

        // Reuse the same unpause/close-menu flow as Resume.
        ResumeGame();

        // Current scene UI objects will be recreated after load.
        pausePanel = null;

        SceneManager.LoadScene(levelToLoad);
        Debug.Log("Loading Saved Game: " + levelToLoad);
    }

    private IEnumerator RestoreSnapshotWhenReady()
    {
        // Wait one frame to allow scene objects to finish initialization.
        yield return null;

        if (pendingSaveData == null)
        {
            shouldRestoreSnapshot = false;
            yield break;
        }

        Scene scene = SceneManager.GetActiveScene();
        Dictionary<string, SceneObjectState> savedStateByPath = new Dictionary<string, SceneObjectState>();
        for (int i = 0; i < pendingSaveData.objects.Count; i++)
        {
            SceneObjectState state = pendingSaveData.objects[i];
            if (!string.IsNullOrEmpty(state.path) && !savedStateByPath.ContainsKey(state.path))
            {
                savedStateByPath.Add(state.path, state);
            }
        }

        Dictionary<string, Transform> sceneTransforms = BuildSceneTransformMap(scene);

        foreach (KeyValuePair<string, SceneObjectState> pair in savedStateByPath)
        {
            Transform t;
            if (!sceneTransforms.TryGetValue(pair.Key, out t))
            {
                continue;
            }

            t.localPosition = pair.Value.localPosition;
            t.localRotation = pair.Value.localRotation;
            t.localScale = pair.Value.localScale;
        }

        foreach (KeyValuePair<string, Transform> pair in sceneTransforms)
        {
            if (IsPauseUiObject(pair.Value))
            {
                continue;
            }

            SceneObjectState state;
            if (savedStateByPath.TryGetValue(pair.Key, out state))
            {
                pair.Value.gameObject.SetActive(state.activeSelf);
            }
            else
            {
                // Missing from save means it should not exist/should be inactive.
                pair.Value.gameObject.SetActive(false);
            }
        }

        if (pendingSaveData.healthFill >= 0f)
        {
            HealthBar healthBar = FindObjectOfType<HealthBar>();
            if (healthBar != null)
            {
                healthBar.UpdateHealthBar(pendingSaveData.healthFill);
            }
        }

        PlayerMovement playerMovement = FindObjectOfType<PlayerMovement>();
        if (playerMovement != null)
        {
            playerMovement.numOfEnDest = pendingSaveData.playerEnemyCount;
        }

        GunShoot.enemyDestroyed = pendingSaveData.globalEnemyDestroyed;

        shouldRestoreSnapshot = false;
        pendingSaveData = null;

        TryFindPausePanel();
        WirePauseMenuButtons();

        if (!isPaused && pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        Debug.Log("Save data restored.");
    }

    private Dictionary<string, Transform> BuildSceneTransformMap(Scene scene)
    {
        Dictionary<string, Transform> map = new Dictionary<string, Transform>();
        GameObject[] roots = scene.GetRootGameObjects();

        for (int i = 0; i < roots.Length; i++)
        {
            Transform[] transforms = roots[i].GetComponentsInChildren<Transform>(true);
            for (int j = 0; j < transforms.Length; j++)
            {
                Transform t = transforms[j];
                string path = GetTransformPath(t);
                if (!map.ContainsKey(path))
                {
                    map.Add(path, t);
                }
            }
        }

        return map;
    }

    private string GetTransformPath(Transform t)
    {
        if (t == null)
        {
            return string.Empty;
        }

        string path = t.name;
        Transform current = t.parent;

        while (current != null)
        {
            path = current.name + "/" + path;
            current = current.parent;
        }

        return path;
    }

    private bool IsPauseUiObject(Transform t)
    {
        if (pausePanel == null || t == null)
        {
            return false;
        }

        return t == pausePanel.transform || t.IsChildOf(pausePanel.transform);
    }

    private void TryFindPausePanel()
    {
        if (pausePanel != null)
        {
            return;
        }

        GameObject panel = GameObject.Find("MainMenuPanel");
        if (panel == null)
        {
            panel = GameObject.Find("Panel");
        }

        if (panel == null)
        {
            GameObject resumeButton = GameObject.Find("ResumeButton");
            if (resumeButton != null && resumeButton.transform.parent != null)
            {
                panel = resumeButton.transform.parent.gameObject;
            }
        }

        if (panel != null)
        {
            pausePanel = panel;
        }
    }

    private void WirePauseMenuButtons()
    {
        if (pausePanel == null)
        {
            return;
        }

        BindButton("ResumeButton", ResumeGame);
        BindButton("SaveButton", SaveGame);
        BindButton("LoadButton", LoadGame);
        BindButton("QuitButton", QuitGame);
    }

    private void BindButton(string buttonName, UnityAction handler)
    {
        Transform buttonTransform = pausePanel.transform.Find(buttonName);
        if (buttonTransform == null)
        {
            return;
        }

        Button button = buttonTransform.GetComponent<Button>();
        if (button == null)
        {
            return;
        }

        button.onClick.RemoveListener(handler);
        button.onClick.AddListener(handler);
    }
}
