using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TankPicker : MonoBehaviour
{
    #region Turret/Hulls
    public Dropdown hullDropdown;
    public Dropdown turretDropdown;
    public Dropdown hullModDropdown;
    public Dropdown turretModDropdown;
    public Dropdown paint;
    public Button playButton;

    private string[] availableHulls = { "Wasp", "Hornet", "Hunter", "Viking", "Dictator", "Titan", "Mammoth" };
    private string[] availableTurrets = { "Smoky", "Firebird", "Freeze", "Railgun", "Ricochet", "Shaft", "Thunder", "Twins" };
    private string[] availableModsTurret = { "M0", "M1", "M2", "M3" };
    private string[] availableModsHull = { "M0", "M1", "M2", "M3" };
    private string[] avaiblePaints = { "black", "blue", "dirt", "green", "lead", "red", "orange", "white" };

    #endregion

    public Button[] allButtons;

    #region SCREENS
    public Transform StartMenu;
    public Transform PlayMenu;
    public Transform ShopMenu;

    public GameObject BlackScreen;
    #endregion

    #region START MENU
    public Button MenuPlayButton;
    public Button MenuShopButton;
    public Button MenuQuitButton;

    #endregion

    #region PLAY MENU
    public Button PlayBackButton;
    public Button PlayPlayButton;
    #endregion

    #region SHOP MENU

    public Button ShopBackButton;


    #endregion


    private async void Start()
    {
        
        PopulateDropdown(hullDropdown, availableHulls);
        PopulateDropdown(turretDropdown, availableTurrets);
        PopulateDropdown(hullModDropdown, availableModsHull);
        PopulateDropdown(turretModDropdown, availableModsTurret);
        PopulateDropdown(paint, avaiblePaints);

        playButton.onClick.AddListener(OnPlayButtonClicked);
        
        //start menu
        MenuPlayButton.onClick.AddListener(OnPlayMenuButtonClicked);
        MenuShopButton.onClick.AddListener(OnShopMenuButtonClicked);
        MenuQuitButton.onClick.AddListener(OnQuitMenuButtonClicked);
        //play menu
        PlayBackButton.onClick.AddListener(OnPlayBackButtonClicked);

        //shop menu

        ShopBackButton.onClick.AddListener(OnShopBackButtonClicked);
    }

    private void PopulateDropdown(Dropdown dropdown, string[] options)
    {
        dropdown.ClearOptions();
        dropdown.AddOptions(new System.Collections.Generic.List<string>(options));
    }

    private void OnPlayButtonClicked()
    {
        string selectedHull = availableHulls[hullDropdown.value];
        string selectedTurret = availableTurrets[turretDropdown.value];
        string selectedHullMod = availableModsHull[hullModDropdown.value];
        string selectedTurretMod = availableModsTurret[turretModDropdown.value];
        string selectedPaint = avaiblePaints[paint.value];

        PlayerPrefs.SetString("SelectedHull", selectedHull);
        PlayerPrefs.SetString("SelectedTurret", selectedTurret);
        PlayerPrefs.SetString("SelectedHullMod", selectedHullMod);
        PlayerPrefs.SetString("SelectedTurretMod", selectedTurretMod);
        PlayerPrefs.SetString("SelectedPaint", selectedPaint);

        SceneManager.LoadScene("DM Maps");
    }

    //Start menu

    private void OnPlayMenuButtonClicked()
    {
        StartCoroutine(ShrinkMenu(StartMenu,PlayMenu));
    }

    private void OnShopMenuButtonClicked()
    {
        StartCoroutine(ShrinkMenu(StartMenu, ShopMenu));
    }

    private void OnQuitMenuButtonClicked()
    {
        Application.Quit();
    }

    //Play menu

    private void OnPlayBackButtonClicked()
    {
        StartCoroutine(ShrinkMenu(PlayMenu, StartMenu));
    }
    private void OnPlayPlayButtonClicked()
    {
        // after player choosed lvl and pressed this button - load map 
    }

    //Shop menu

    private void OnShopBackButtonClicked()
    {
        StartCoroutine(ShrinkMenu(ShopMenu, StartMenu));
    }


    private IEnumerator ShrinkMenu(Transform From, Transform To)
    {

        foreach (Button btn in allButtons)
            btn.interactable = false;

        Vector3 targetScale = new Vector3(0, From.localScale.y, From.localScale.z);
        float speed = 5f;

        while (From.localScale.x > 0.01f)
        {
            From.localScale = Vector3.Lerp(From.localScale, targetScale, Time.deltaTime * speed);
            yield return null;
        }
        From.localScale = targetScale; 

        targetScale = new Vector3(1, To.localScale.y, To.localScale.z);

        while (To.localScale.x < 0.99)
        {
            To.localScale = Vector3.Lerp(To.localScale, targetScale, Time.deltaTime * speed);
            yield return null;
        }
        To.localScale = targetScale;

        foreach (Button btn in allButtons)
            btn.interactable = true;

    }

    private void Fadder()
    {

    }
}
