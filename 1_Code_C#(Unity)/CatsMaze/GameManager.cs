using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum AudioClipEnum
{
    Main, Intro, CutScene1, CutScene2, Town, WorldMap, Max
}

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    public CardManager cardManager;
    public ScriptManager scriptManager;
    public TutorialManager tutorialManager;
    public SynergyChecker synergyChecker;
    public Spawner spawner;

    public bool isPause = false;
    public bool isOptionPause = false;
    public bool isProgressing = false;
    public bool isCardDrag = false;
    public bool isBeforeScene = true;

    public bool[] isTutorialFinish = new bool[3] { false, false, false };

    public int stageNum = 0;
    public int chapterNum = 0;

    public string userName = "Nabi";

    public TMP_Dropdown resolutions;
    public TMP_Dropdown language;
    public Toggle fullScreenToggle;
    int resolutionOptionNum = 0;
    FullScreenMode screenMode;

    public GameObject OptionPanel;
    public GameObject SubOptionPanel;
    public GameObject PausePanel;

    public GameObject worldMapButton;

    public AudioMixer audioMixer;
    public Slider MasterSlider;
    public TMP_Text MasterValue;
    public Slider BGMSlider;
    public TMP_Text BGMValue;
    public Slider EffectSlider;
    public TMP_Text EffectValue;

    bool isFull = true;

    public AudioSource audioSource;

    public AudioClip[] audioClips;
    int currentAudioClipIndex = 0;

    public bool isSceneLoading = false;

    public bool isOptionInit = false;

    public Button startButton;
    public Button loadButton;
    public Button optionButton;
    public Button exitButton;

    public GameObject saveLoadPanel;

    private new void OnDestroy()
    {
        if (initialized)
            MessageQueue.Instance.DetachListener(typeof(StageFailMessage), StageFail);
        base.OnDestroy();
    }

    private void Awake()
    {
        if (Instance != null)
        {
            if (initialized == false)
                DestroyImmediate(gameObject);
        }

        if (initialized)
            MessageQueue.Instance.AttachListener(typeof(StageFailMessage), StageFail);

        Time.timeScale = 1;
    }

    private void Start()
    {
#if UNITY_EDITOR
        PlayerPrefs.DeleteAll();
#endif
        SetTitleButton();

        DontDestroyOnLoad(this);

        MasterSlider.value = PlayerPrefs.GetFloat("MasterVolume", 0.5f);
        BGMSlider.value = PlayerPrefs.GetFloat("BGMVolume", 0.5f);
        EffectSlider.value = PlayerPrefs.GetFloat("EffectVolume", 0.5f);

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = (int)Screen.currentResolution.refreshRateRatio.value;
        Application.runInBackground = true;

        resolutions.options.Clear();

        resolutionOptionNum = 0; foreach (var resolution in Screen.resolutions)
        {
            TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData();
            optionData.text = resolution.width + " X " + resolution.height + " " + Mathf.RoundToInt((float)resolution.refreshRateRatio.value) + "hz";
            resolutions.options.Add(optionData);

            if (resolution.width == Screen.width && resolution.height == Screen.height)
            {
                resolutions.value = resolutionOptionNum;
                resolutions.GetComponentInChildren<ScrollRect>().verticalScrollbar.value = Mathf.Max(0.001f, 1.0f - resolutions.value / (float)(resolutions.options.Count - 1));
            }
            resolutionOptionNum++;
        }

        TMP_Dropdown.OptionData languageData = new TMP_Dropdown.OptionData();
        languageData.text = "English";
        language.options.Add(languageData);

        //languageData = new TMP_Dropdown.OptionData();
        //languageData.text = "日本語";
        //language.options.Add(languageData);

        languageData = new TMP_Dropdown.OptionData();
        languageData.text = "한국어";
        language.options.Add(languageData);

        //languageData = new TMP_Dropdown.OptionData();
        //languageData.text = "Tiếng Việt";
        //language.options.Add(languageData);

        resolutions.RefreshShownValue();
    }

    public void OnClickSaveSlot(int index)
    {
        UserData.Instance.selectSlotIndex = index;
        saveLoadPanel.SetActive(true);
    }

    public void OptionInit()
    {
        if (isOptionInit) return;

        isOptionInit = true;

        for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; ++i)
        {
            if (LocalizationSettings.SelectedLocale == LocalizationSettings.AvailableLocales.Locales[i])
            {
                language.value = i;
            }
        }

        language.RefreshShownValue();

        if (screenMode == FullScreenMode.FullScreenWindow) isFull = true;
        else isFull = false;

        fullScreenToggle.isOn = isFull;
    }

    public bool IsTutorialFinish()
    {
        return isTutorialFinish[0] && isTutorialFinish[1] && isTutorialFinish[2];
    }

    public void OnClickStartButton()
    {
        UserData.Instance.ClearAllData();

        if (IsTutorialFinish() == false)
        {
            LoadNextScene("IntroScene", (int)AudioClipEnum.Intro);
        }
        else
        {
            LoadNextScene("TownScene", (int)AudioClipEnum.Town);
        }
    }

    public void OnClickOptionButton()
    {
        OptionPanel.SetActive(true);
        SubOptionPanel.SetActive(true);
        isOptionPause = true;
        Pause();
    }

    public void OnClickToTitleButton()
    {
        LoadNextScene("TitleScene", (int)AudioClipEnum.Main);
        OptionPanel.SetActive(false);
        isOptionPause = false;
        Pause();
    }

    public void OnClickToWorldMapButton()
    {
        LoadNextScene("WorldMapScene", (int)AudioClipEnum.WorldMap);
        OptionPanel.SetActive(false);
        isOptionPause = false;
        Pause();
    }

    public void SetTitleButton()
    {
        Button[] bt = FindObjectsOfType<Button>();

        foreach (var b in bt)
        {
            if (b.name.Equals("StartButton"))
            {
                startButton = b;
                startButton.onClick.AddListener(OnClickStartButton);
            }
            else if (b.name.Equals("LoadButton"))
            {
                loadButton = b;
                loadButton.onClick.AddListener(OptionInit);
            }
            else if (b.name.Equals("OptionButton"))
            {
                optionButton = b;
                optionButton.onClick.AddListener(OptionInit);
                optionButton.onClick.AddListener(OnClickOptionButton);
            }
            else if (b.name.Equals("QuitButton"))
            {
                exitButton = b;
                exitButton.onClick.AddListener(OnClickExitButton);
            }
        }
    }

    public void LoadNextScene(string sceneName, int audioIndex)
    {
        isSceneLoading = true;

        LoadSceneManager.LoadScene(sceneName);

        if (currentAudioClipIndex != audioIndex)
        {
            currentAudioClipIndex = audioIndex;

            audioSource.clip = audioClips[audioIndex];
            audioSource.Play();
        }

        if (sceneName.Equals("StageScene"))
        {
            worldMapButton.SetActive(true);
        }
        else
        {
            worldMapButton.SetActive(false);
        }
    }

    public bool StageFail(BaseMessage msg)
    {
        if (msg == null)
        {
            Debug.Log("HandleYourMesssageListener : Message is null!");
            return false;
        }

        StageFailMessage castMsg = msg as StageFailMessage;

        if (castMsg == null)
        {
            Debug.Log("HandleYourMesssageListener : Cast Message is null!");
            return false;
        }

        Debug.Log(string.Format("HandleYourMesssageListener : Got the message! - {0}", castMsg.name));

        isPause = true;
        Pause();

        return true;
    }

    public void ResoultionDropDownOptionChange(TMP_Dropdown change)
    {
        resolutionOptionNum = change.value;
    }

    public void LanguageDropDownOptionChange(TMP_Dropdown change)
    {
        if (LocalizationSettings.SelectedLocale != LocalizationSettings.AvailableLocales.Locales[change.value])
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[change.value];
    }

    public void FullScreenToggle()
    {
        isFull = !isFull;

        if (isFull)
        {
            screenMode = FullScreenMode.FullScreenWindow;
        }
        else
        {
            screenMode = FullScreenMode.Windowed;
        }
    }

    public void SaveLoadFinish()
    {
        isOptionPause = !isOptionPause;

        Pause();

        OptionPanel.SetActive(isOptionPause);
        SubOptionPanel.SetActive(isOptionPause);

        saveLoadPanel.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (startButton == null)
        {
            SetTitleButton();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isSceneLoading || SceneManager.GetActiveScene().buildIndex == 0) return;

            isOptionPause = !isOptionPause;

            if (tutorialManager != null &&
                tutorialManager.tutorialCanvas.activeSelf == true)
                if (isOptionPause)
                {
                    if (isPause) isPause = true;
                    else isPause = false;
                }

            Pause();

            OptionPanel.SetActive(isOptionPause);
            SubOptionPanel.SetActive(isOptionPause);

            MasterSlider.value = PlayerPrefs.GetFloat("MasterVolume", 0.5f);
            MasterValue.text = (int)(MasterSlider.value * 100) + "%";
            BGMSlider.value = PlayerPrefs.GetFloat("BGMVolume", 0.5f);
            BGMValue.text = (int)(BGMSlider.value * 100) + "%";
            EffectSlider.value = PlayerPrefs.GetFloat("EffectVolume", 0.5f);
            EffectValue.text = (int)(EffectSlider.value * 100) + "%";
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isSceneLoading || isOptionPause ||
                SceneManager.GetActiveScene().buildIndex == 1 ||
                SceneManager.GetActiveScene().buildIndex == 6 ||
                SceneManager.GetActiveScene().buildIndex == 7 ||
                SceneManager.GetActiveScene().buildIndex == 8 ||
                (tutorialManager != null && tutorialManager.tutorialCanvas.activeSelf == true)) return;

            EventSystem.current.SetSelectedGameObject(null);

            isPause = !isPause;

            Pause();

            OptionPanel.SetActive(isPause);
            SubOptionPanel.SetActive(false);
        }
    }

    public void DoPause()
    {
        isPause = true;
        Pause();
    }

    public void DoRelease()
    {
        isPause = false;
        Pause();
    }

    public void Pause()
    {
        if (isPause || isOptionPause)
        {
            Time.timeScale = 0;
            PausePanel.SetActive(true);
        }
        else
        {
            Time.timeScale = 1;
            PausePanel.SetActive(false);
        }
    }

    public void ClickOKButton()
    {
        if (resolutionOptionNum < Screen.resolutions.Length)
        {
            Screen.SetResolution(
                Screen.resolutions[resolutionOptionNum].width,
                Screen.resolutions[resolutionOptionNum].height,
                screenMode);
            Application.targetFrameRate = (int)Screen.resolutions[resolutionOptionNum].refreshRateRatio.value;
        }

        if (cardManager != null)
            cardManager.CardAlignment();

        if (tutorialManager != null &&
            tutorialManager.tutorialCanvas.activeSelf == false)
            if (isPause) isPause = true;
            else isPause = false;

        isOptionPause = false;

        PlayerPrefs.SetFloat("MasterVolume", MasterSlider.value);
        PlayerPrefs.SetFloat("BGMVolume", BGMSlider.value);
        PlayerPrefs.SetFloat("EffectVolume", EffectSlider.value);
        PlayerPrefs.Save();

        Pause();

    }

    public void OnClickExitButton()
    {
        Application.Quit();
    }

    public void SetMasterSound(float value)
    {
        audioMixer.SetFloat("Master", Mathf.Log(value) * 20);
        MasterValue.text = (int)(value * 100) + "%";
    }

    public void SetBGMSound(float value)
    {
        audioMixer.SetFloat("BGM", Mathf.Log(value) * 20);
        BGMValue.text = (int)(value * 100) + "%";
    }

    public void SetEffectSound(float value)
    {
        audioMixer.SetFloat("Effect", Mathf.Log(value) * 20);
        EffectValue.text = (int)(value * 100) + "%";
    }
}
