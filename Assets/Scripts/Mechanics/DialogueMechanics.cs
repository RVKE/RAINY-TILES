using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueMechanics : MonoBehaviour
{
    public bool disableIntro;

    private CameraController camController;

    public bool introGaande;
    public bool doneStage1;
    public bool doneStage2;
    public bool doneStage3;
    public bool doneStage4;

    public GameObject playerModel;
    public GameObject ragdoll;

    [SerializeField] private GameObject player;
    [SerializeField] private GameObject playerSpawn;

    [SerializeField] private Vector3 coinSpawnPos;
    [SerializeField] private Vector3 rainSpawnPos;

    [SerializeField] Vector3 cameraPosJemairo;
    [SerializeField] Vector3 cameraPosJemairoStart;
    [SerializeField] Vector3 cameraPosBob;
    [SerializeField] Vector3 cameraPosHouse;
    [SerializeField] float cameraZoomDialogue;
    [SerializeField] float cameraZoomEnd;
    [SerializeField] private Animator jemairoAnimator;
    [SerializeField] private GameObject DialogueBoxJemairo;
    [SerializeField] private GameObject DialogueBoxBob;
    [SerializeField] private GameObject DialogueTextJemairo;
    [SerializeField] private GameObject DialogueTextBob;
    [SerializeField] private GameObject bobModel;
    [SerializeField] private GameObject jemairoModel;

    public GameObject gun;
    public GameObject hand;

    [SerializeField] private GameObject muzzleFlashEmitter;
    [SerializeField] private GameObject[] muzzleFlashes;
    [SerializeField] private ParticleSystem casingParticles;

    [SerializeField] private float jemairoShotCount;
    [SerializeField] private float fireRate;

    public GameObject waterPlane;

    [SerializeField] private GameObject buildMarker;

    [SerializeField] private GameObject introRainPrefab;

    [SerializeField] private GameObject hintUI;
    [SerializeField] private GameObject hintTextUIObject;

    [SerializeField] private GameObject blackBarUp;
    [SerializeField] private GameObject blackBarDown;
    [SerializeField] private GameObject blackBarUpR;
    [SerializeField] private GameObject blackBarDownR;


    private GameObject canvas;

    private void Awake()
    {
        if (GameManager.gameMode == 0)
        {
            disableIntro = false;
        }
        if (GameManager.gameMode == 1 || GameManager.gameMode == 2)
        {
            disableIntro = true;
        }
    }

    private void Start()
    {
        camController = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();
        canvas = GameObject.FindGameObjectWithTag("Canvas");

        if (disableIntro)
        {
            Invoke("StartTheGame", 1.0f);
        } else
        {
            introGaande = true;
            doneStage1 = false;
            doneStage2 = false;
            doneStage3 = false;
            doneStage4 = false;
            DialogueBoxBob.SetActive(false);
            DialogueBoxJemairo.SetActive(false);
            blackBarDown.SetActive(true);
            blackBarUp.SetActive(true);
            hintUI.transform.position = hintUI.GetComponent<UIFixedAnimation>().startV3;
            hintUI.SetActive(false);
            camController.ChangeZoom(cameraZoomDialogue, Mathf.Infinity);
            player.transform.position = playerSpawn.transform.position;

            StartCoroutine(StartStage1()); // <- start intro stage 1
        }
    }

    public IEnumerator LostDaGame()
    {
        jemairoAnimator.SetBool("IsTalking", false);

        DialogueTextJemairo.GetComponent<TextWritingEffect>().StopAllCoroutines();
        DialogueBoxJemairo.SetActive(false);

        yield return new WaitForSeconds(0.5f);

        Instantiate(waterPlane, Vector3.zero, transform.rotation);

        yield return new WaitForSeconds(4.0f);

        FindObjectOfType<AudioManager>().PauseSpecificAudioObject("Chune");
        canvas.GetComponent<MenuMechanics>().EnableMusic();

        blackBarDownR.SetActive(true);
        blackBarUpR.SetActive(true);
        blackBarUpR.GetComponent<UIFixedAnimation>().Play();
        blackBarDownR.GetComponent<UIFixedAnimation>().Play();


        camController.ChangePosition(cameraPosJemairoStart, 2.0f);
        camController.ChangeZoom(cameraZoomEnd, 3.0f);

        GetComponent<TileMechanics>().StopAllCoroutines();

        yield return new WaitForSeconds(2.0f);

        hand.GetComponent<DitzelGames.FastIK.FastIKFabric>().enabled = true;
        gun.SetActive(true);

        FindObjectOfType<AudioManager>().Play("LoadGun");

        yield return new WaitForSeconds(1.0f);

        jemairoAnimator.SetFloat("TalkSpeed", 2.0f);
        jemairoAnimator.SetBool("IsTalking", true);

        playerModel.SetActive(false);
        ragdoll.SetActive(true);

        for (int i = 0; i < jemairoShotCount; i++)
        {
            GameObject muzzleFlash = Instantiate(muzzleFlashes[Random.Range(0, muzzleFlashes.Length)], muzzleFlashEmitter.transform.position, muzzleFlashEmitter.transform.rotation);
            FindObjectOfType<AudioManager>().Play("SubMachineGun");
            CameraShake.Shake(0.07f, 0.07f);
            casingParticles.Emit(1);
            gun.GetComponent<Animator>().Play("Shoot", 0, 0f);
            yield return new WaitForSeconds(1 / (fireRate/60));
            Destroy(muzzleFlash);
            FindObjectOfType<AudioManager>().Play("Shell");
        }

        yield return new WaitForSeconds(0.5f);

        camController.ResetPosition(0.5f);

        camController.ResetZoom(1.0f);

        yield return new WaitForSeconds(6.5f);

        camController.ChangePosition(cameraPosJemairo, 1.0f);
        camController.ChangeZoom(cameraZoomDialogue, 1.5f);

        yield return new WaitForSeconds(2.0f);

        hand.GetComponent<DitzelGames.FastIK.FastIKFabric>().enabled = false;
        gun.SetActive(false);

        FindObjectOfType<AudioManager>().Play("Unequip");

        DialogueBoxJemairo.transform.localScale = DialogueBoxJemairo.GetComponent<UIFixedAnimation>().startV3;

        StartCoroutine(StartJemairoDialogue("That's what happens when you mess with a Crip!!!", 0.1f, false, 0.0f, 1.0f));

        yield return new WaitForSeconds(6.0f);

        canvas.GetComponent<MenuMechanics>().pauseEnabled = false;

        canvas.GetComponent<MenuMechanics>().ActivateLoseUI();

        canvas.GetComponent<MenuMechanics>().ChangeMasterPitch(0.5f, false, 1.0f);

    }

    public IEnumerator WonDaGame()
    {
        jemairoAnimator.SetBool("IsTalking", false);

        DialogueTextJemairo.GetComponent<TextWritingEffect>().StopAllCoroutines();
        DialogueBoxJemairo.SetActive(false);

        yield return new WaitForSeconds(2.0f);

        camController.ChangePosition(cameraPosHouse, 1.0f);

        yield return new WaitForSeconds(2.0f);

        StartCoroutine(StartJemairoDialogue("LETSGOOO!", 0.05f, false, 0.0f, 1.0f));

        yield return new WaitForSeconds(1.0f);

        FindObjectOfType<AudioManager>().PauseSpecificAudioObject("Chune");
        canvas.GetComponent<MenuMechanics>().EnableMusic();

        blackBarDownR.SetActive(true);
        blackBarUpR.SetActive(true);
        blackBarUpR.GetComponent<UIFixedAnimation>().Play();
        blackBarDownR.GetComponent<UIFixedAnimation>().Play();

        camController.ChangePosition(cameraPosJemairo, 2.0f);
        camController.ChangeZoom(cameraZoomDialogue, 3.0f);

        player.GetComponent<Player>().desiredPos = new Vector3(0, 0.475f, 0);

        yield return new WaitForSeconds(2.0f);

        bobModel.transform.LookAt(new Vector3(jemairoModel.transform.position.x, bobModel.transform.position.y, jemairoModel.transform.position.z));

        StartCoroutine(StartJemairoDialogue("You did it!", 0.1f, false, 0.0f, 2.0f));

        yield return new WaitForSeconds(3.0f);

        StartCoroutine(StartJemairoDialogue("You're officially a Crip now!!", 0.1f, false, 0.0f, 1.0f));

        yield return new WaitForSeconds(4.6f);

        StartCoroutine(StartJemairoDialogue("Join us and you're gonna love it!", 0.1f, false, 0.0f, 1.5f));

        yield return new WaitForSeconds(5.0f);

        StartCoroutine(StartJemairoDialogue("We've got loads of cash money and you will get your own Bugatti!", 0.1f, true, 8.0f, 1.5f));

        yield return new WaitForSeconds(8.0f);

        camController.ChangePosition(cameraPosBob, 2.0f);

        yield return new WaitForSeconds(1.5f);

        DialogueBoxBob.SetActive(true);

        DialogueBoxBob.GetComponent<UIFixedAnimation>().Play();

        StartCoroutine(StartBobDialogue("Uhmm...", 0.15f, false, 0.0f));

        yield return new WaitForSeconds(2.5f);

        StartCoroutine(StartBobDialogue("I'm not so sure about that...", 0.1f, false, 0.0f));

        yield return new WaitForSeconds(4.5f);

        StartCoroutine(StartBobDialogue("You are not my only client you know...", 0.1f, false, 0.0f));

        yield return new WaitForSeconds(5.5f);

        StartCoroutine(StartBobDialogue("And besides, being a gangster ain't my thing!", 0.1f, true, 5.5f));

        yield return new WaitForSeconds(5.5f);

        camController.ChangePosition(cameraPosJemairo, 2.0f);

        yield return new WaitForSeconds(1.5f);

        StartCoroutine(StartJemairoDialogue("Then I'm afraid I'll have to kill you...", 0.1f, true, 6.0f, 0.5f));

        yield return new WaitForSeconds(6.0f);

        camController.ChangePosition(cameraPosBob, 2.0f);

        yield return new WaitForSeconds(1.5f);

        StartCoroutine(StartBobDialogue("What?", 0.1f, false, 0.0f));

        yield return new WaitForSeconds(1.0f);

        StartCoroutine(StartBobDialogue("Why?!", 0.1f, true, 2.5f));

        yield return new WaitForSeconds(2.5f);

        camController.ChangePosition(cameraPosJemairo, 2.0f);

        yield return new WaitForSeconds(1.5f);

        StartCoroutine(StartJemairoDialogue("I know you're gonna snitch me to the police...", 0.1f, true, 5.5f, 0.5f));

        yield return new WaitForSeconds(5.5f);

        camController.ChangePosition(cameraPosBob, 2.0f);

        yield return new WaitForSeconds(2.0f);

        StartCoroutine(StartBobDialogue("I don't know what you're talking about...", 0.1f, false, 0.0f));

        yield return new WaitForSeconds(5.0f);

        StartCoroutine(StartBobDialogue("Now let me go!!!", 0.1f, true, 3.0f));

        yield return new WaitForSeconds(3.0f);

        camController.ChangePosition(cameraPosJemairo, 2.0f);

        yield return new WaitForSeconds(1.0f);

        StartCoroutine(StartJemairoDialogue("Then I'm sorry my brother...", 0.1f, true, 3.0f, 0.5f));

        yield return new WaitForSeconds(3.0f);

        camController.ChangePosition(cameraPosJemairoStart, 2.0f);
        camController.ChangeZoom(cameraZoomEnd, 3.0f);

        yield return new WaitForSeconds(1.5f);

        hand.GetComponent<DitzelGames.FastIK.FastIKFabric>().enabled = true;
        gun.SetActive(true);

        FindObjectOfType<AudioManager>().Play("LoadGun");

        yield return new WaitForSeconds(1.0f);

        jemairoAnimator.SetFloat("TalkSpeed", 2.0f);
        jemairoAnimator.SetBool("IsTalking", true);

        playerModel.SetActive(false);
        ragdoll.SetActive(true);

        for (int i = 0; i < jemairoShotCount; i++)
        {
            GameObject muzzleFlash = Instantiate(muzzleFlashes[Random.Range(0, muzzleFlashes.Length)], muzzleFlashEmitter.transform.position, muzzleFlashEmitter.transform.rotation);
            FindObjectOfType<AudioManager>().Play("SubMachineGun");
            CameraShake.Shake(0.07f, 0.07f);
            casingParticles.Emit(1);
            gun.GetComponent<Animator>().Play("Shoot", 0, 0f);
            yield return new WaitForSeconds(1 / (fireRate / 60));
            Destroy(muzzleFlash);
            FindObjectOfType<AudioManager>().Play("Shell");
        }

        yield return new WaitForSeconds(0.5f);

        camController.ResetPosition(0.5f);

        camController.ResetZoom(1.0f);

        yield return new WaitForSeconds(6.5f);

        camController.ChangePosition(cameraPosJemairo, 1.0f);
        camController.ChangeZoom(cameraZoomDialogue, 1.5f);

        yield return new WaitForSeconds(2.0f);

        hand.GetComponent<DitzelGames.FastIK.FastIKFabric>().enabled = false;
        gun.SetActive(false);

        FindObjectOfType<AudioManager>().Play("Unequip");

        DialogueBoxJemairo.transform.localScale = DialogueBoxJemairo.GetComponent<UIFixedAnimation>().startV3;

        StartCoroutine(StartJemairoDialogue("I hope he learned his lesson...", 0.1f, false, 0.0f, 1.0f));

        yield return new WaitForSeconds(6.0f);

        canvas.GetComponent<MenuMechanics>().pauseEnabled = false;

        canvas.GetComponent<MenuMechanics>().ActivateVictoryUI();

        canvas.GetComponent<MenuMechanics>().ChangeMasterPitch(0.5f, false, 1.0f);

    }

    IEnumerator StartStage1()
    {
        yield return new WaitForSeconds(2.5f);

        player.GetComponent<Player>().setPosition = true;

        yield return new WaitForSeconds(3.0f);

        camController.ResetZoom(2.0f);

        yield return new WaitForSeconds(2.5f);

        camController.ChangePosition(cameraPosJemairoStart, 2.0f);
        camController.ChangeZoom(cameraZoomDialogue, 3.0f);

        yield return new WaitForSeconds(2.0f);
        
        camController.ChangePosition(cameraPosJemairo, 4.0f);
        
        yield return new WaitForSeconds(0.5f);

        bobModel.transform.LookAt(new Vector3(jemairoModel.transform.position.x, bobModel.transform.position.y, jemairoModel.transform.position.z));

        //REFERENTIE: StartCoroutine(StartJemairoDialogue("TEXT", SNELHEID LETTERS, LENGTE PRAATANIMATIE, DISABLE?, DISABLE TIJD, PRAATSNELHEID)); ----# JEMAIRO #----

        //REFERENTIE: StartCoroutine(StartJemairoDialogue("TEXT", SNELHEID LETTERS, DISABLE?, DISABLE TIJD)); ----# BOB #----

        StartCoroutine(StartJemairoDialogue("You the guy I hired last week?", 0.1f, true, 5.0f, 1.0f));

        yield return new WaitForSeconds(5.0f);

        camController.ChangePosition(cameraPosBob, 2.0f);

        yield return new WaitForSeconds(1.5f);

        StartCoroutine(StartBobDialogue("Yes sir, it's me Bob!", 0.1f, false, 0.0f));

        yield return new WaitForSeconds(4.0f);

        StartCoroutine(StartBobDialogue("If I understand correctly...", 0.1f, false, 7.0f));

        yield return new WaitForSeconds(3.5f);

        StartCoroutine(StartBobDialogue("You wanted me to cover your garden with tiles?", 0.1f, true, 7.0f));

        yield return new WaitForSeconds(7.0f);

        camController.ChangePosition(cameraPosJemairo, 2.0f);

        yield return new WaitForSeconds(2.0f);

        StartCoroutine(StartJemairoDialogue("Yeah, that's about right...", 0.1f, false, 0.0f, 1.0f));

        yield return new WaitForSeconds(5.5f);

        StartCoroutine(StartJemairoDialogue("The rain is flooding my garden and it's damaging my house!", 0.1f, false, 0.0f, 1.0f));

        yield return new WaitForSeconds(6.5f);

        StartCoroutine(StartJemairoDialogue("But lemme make something very clear...", 0.1f, false, 0.0f, 1.5f));

        yield return new WaitForSeconds(5.5f);

        StartCoroutine(StartJemairoDialogue("I'm a Crips gangsta and you're in official Crips territory!", 0.1f, false, 4.0f, 1.0f));

        yield return new WaitForSeconds(8.5f);

        StartCoroutine(StartJemairoDialogue("So...", 0.2f, false, 2.0f, 1.0f));

        yield return new WaitForSeconds(2.0f);

        StartCoroutine(StartJemairoDialogue("Don't mess with me or imma shoot yo ass!", 0.1f, true, 5.5f, 1.0f));

        yield return new WaitForSeconds(5.5f);

        camController.ChangePosition(cameraPosBob, 2.0f);

        yield return new WaitForSeconds(1.0f);
        
        StartCoroutine(StartBobDialogue("What the hell is a Crip doing in the middle of the woods?", 0.1f, false, 3.5f));

        yield return new WaitForSeconds(7.0f);

        StartCoroutine(StartBobDialogue("Aren't you supposed to be in the hood right now?", 0.1f, true, 7.0f));

        yield return new WaitForSeconds(7.0f);

        camController.ChangePosition(cameraPosJemairo, 2.0f);

        yield return new WaitForSeconds(1.0f);

        StartCoroutine(StartJemairoDialogue("That's racist...", 0.1f, false, 0.0f, 1.0f));

        yield return new WaitForSeconds(3.0f);

        StartCoroutine(StartJemairoDialogue("Now stop asking questions or I will put a bullet in your brain.", 0.1f, false, 0.0f, 1.0f));

        yield return new WaitForSeconds(6.5f);

        StartCoroutine(StartJemairoDialogue("Just fix my garden, it's all I'm asking!", 0.1f, true, 5.0f, 1.5f));

        yield return new WaitForSeconds(5.0f);

        camController.ResetPosition(2.0f);

        camController.ResetZoom(1.0f);

        player.GetComponent<Player>().allowSmoothAppear = true;

        yield return new WaitForSeconds(2.0f);

        GetComponent<MoneyMechanics>().spawnCoin(false, coinSpawnPos);

        yield return new WaitForSeconds(3.0f);

        camController.ChangePosition(cameraPosJemairo, 2.0f);
        camController.ChangeZoom(cameraZoomDialogue, 3.0f);

        yield return new WaitForSeconds(2.0f);

        StartCoroutine(StartJemairoDialogue("Do you see that cash money?", 0.1f, false, 0.0f, 0.5f));

        yield return new WaitForSeconds(4.0f);

        StartCoroutine(StartJemairoDialogue("You better pick it up!", 0.1f, false, 0.0f, 0.5f));

        yield return new WaitForSeconds(4.0f);

        camController.ResetPosition(2.0f);

        camController.ResetZoom(1.0f);

        blackBarUp.GetComponent<UIFixedAnimation>().Play();
        blackBarDown.GetComponent<UIFixedAnimation>().Play();

        yield return new WaitForSeconds(2.0f);

        hintUI.transform.position = hintUI.GetComponent<UIFixedAnimation>().startV3;
        hintUI.SetActive(true);
        hintUI.GetComponent<UIFixedAnimation>().Play();

        hintTextUIObject.GetComponent<Text>().text = "CLICK ON A SPOT TO MOVE AND PICK UP THE COIN!";

        GetComponent<TileMechanics>().enableMouseOverTileMaterial = true;

        player.GetComponent<Player>().movementEnabled = true; //enable movement
    }


    IEnumerator StartStage2()
    {
        hintUI.transform.position = hintUI.GetComponent<UIFixedAnimation>().startV3;
        hintUI.SetActive(false);
        GetComponent<TileMechanics>().enableMouseOverTileMaterial = false;
        yield return new WaitForSeconds(1.0f);
        canvas.GetComponent<MenuMechanics>().ActivateCoinUI(true);

        GetComponent<MoneyMechanics>().UpdateMoneyText();

        yield return new WaitForSeconds(1.0f);

        GetComponent<TileMechanics>().DoCustomRainWarning(rainSpawnPos);

        yield return new WaitForSeconds(2.0f);

        camController.ChangePosition(cameraPosJemairo, 2.0f);
        camController.ChangeZoom(cameraZoomDialogue, 3.0f);

        yield return new WaitForSeconds(1.5f);

        StartCoroutine(StartJemairoDialogue("Oh hell nah... the rain is coming!", 0.1f, false, 0.0f, 2.0f));

        yield return new WaitForSeconds(4.0f);

        StartCoroutine(StartJemairoDialogue("Quickly, move to a nearby position and place a tile!", 0.1f, false, 0.0f, 1.0f));

        yield return new WaitForSeconds(5.0f);

        camController.ResetPosition(2.0f);

        camController.ResetZoom(1.0f);

        yield return new WaitForSeconds(2.0f);

        player.GetComponent<Player>().movementEnabled = true;

        GetComponent<TileMechanics>().enableMouseOverTileMaterial = true;
    }

    IEnumerator StartStage3()
    {
        player.GetComponent<Player>().positionedOnArrow = false;

        GetComponent<TileMechanics>().enableMouseOverTileMaterial = false;

        GameObject[] arrows = GameObject.FindGameObjectsWithTag("Arrow");
        foreach (GameObject arrow in arrows)
            Destroy(arrow);

        yield return new WaitForSeconds(1.0f);

        StartCoroutine(StartJemairoDialogue("Now use the money to place a tile!!", 0.1f, false, 0.0f, 1.0f));

        hintUI.SetActive(true);
        hintUI.GetComponent<UIFixedAnimation>().Play();

        hintTextUIObject.GetComponent<Text>().text = "HOLD [SHIFT] AND CLICK ON THE MARKED LOCATION TO PLACE A TILE!";

        yield return new WaitForSeconds(0.5f);

        player.GetComponent<Player>().enableExclusiveBuildPos = true;
        player.GetComponent<Player>().exclusiveBuildPos = rainSpawnPos;

        player.GetComponent<Player>().buildingEnabled = true;

        Instantiate(buildMarker, rainSpawnPos, transform.rotation);

        GetComponent<TileMechanics>().enableMouseOverTileMaterial = true;

    }

    IEnumerator StartStage4()
    {
        Destroy(GameObject.FindGameObjectWithTag("BuildMarker"));

        GetComponent<TileMechanics>().enableMouseOverTileMaterial = false;

        hintUI.transform.position = hintUI.GetComponent<UIFixedAnimation>().startV3;
        hintUI.SetActive(false);

        yield return new WaitForSeconds(1.0f);

        camController.ChangePosition(cameraPosJemairo, 2.0f);
        camController.ChangeZoom(cameraZoomDialogue, 3.0f);

        yield return new WaitForSeconds(1.5f);

        StartCoroutine(StartJemairoDialogue("Good job!", 0.2f, false, 0.0f, 0.5f));

        yield return new WaitForSeconds(2.0f);

        canvas.GetComponent<MenuMechanics>().ActivateXPUI(true);

        yield return new WaitForSeconds(0.5f);

        StartCoroutine(StartJemairoDialogue("By building tiles you get XP!", 0.1f, false, 0.0f, 1.5f));

        yield return new WaitForSeconds(4.5f);

        StartCoroutine(StartJemairoDialogue("Once you get enough XP, you can upgrade the garden!", 0.1f, false, 0.0f, 1.5f));

        yield return new WaitForSeconds(5.0f);

        Instantiate(introRainPrefab, rainSpawnPos, transform.rotation);

        yield return new WaitForSeconds(1.0f);

        camController.ResetPosition(2.0f);

        camController.ResetZoom(1.0f);

        yield return new WaitForSeconds(3.0f);

        StartCoroutine(StartJemairoDialogue("Looks like you were just in time!", 0.1f, false, 0.0f, 1.5f));

        yield return new WaitForSeconds(3.5f);

        camController.ChangePosition(cameraPosJemairo, 2.0f);
        camController.ChangeZoom(cameraZoomDialogue, 3.0f);

        yield return new WaitForSeconds(3.0f);

        Destroy(GameObject.FindGameObjectWithTag("RainWarningIntro"));

        canvas.GetComponent<MenuMechanics>().ActivateShopButtonUI(true);

        yield return new WaitForSeconds(0.5f);

        StartCoroutine(StartJemairoDialogue("Now go to the upgrade section to view the different upgrades!", 0.1f, false, 0.0f, 1.5f));

        hintUI.SetActive(true);
        hintUI.GetComponent<UIFixedAnimation>().Play();

        hintTextUIObject.GetComponent<Text>().text = "PRESS [TAB] OR PRESS THE BUTTON TO GO TO THE UPGRADE SECTION!";

        yield return new WaitForSeconds(5.5f);

        canvas.GetComponent<MenuMechanics>().shopEnabled = true;
    }

    IEnumerator StartStage5()
    {
        hintUI.transform.position = hintUI.GetComponent<UIFixedAnimation>().startV3;

        hintUI.SetActive(false);

        yield return new WaitForSeconds(0.5f);
       
        StartCoroutine(StartJemairoDialogue("Well done!", 0.1f, false, 0.0f, 1.0f));

        yield return new WaitForSeconds(3.0f);

        StartCoroutine(StartJemairoDialogue("The storm will start any moment now!", 0.1f, false, 0.0f, 1.5f));

        yield return new WaitForSeconds(5.0f);

        StartCoroutine(StartJemairoDialogue("So there's no time to waste!", 0.1f, false, 0.0f, 2.0f));
        
        yield return new WaitForSeconds(3.5f);

        camController.ResetPosition(2.0f);
        camController.ResetZoom(1.0f);

        yield return new WaitForSeconds(1.0f);
        StartCoroutine(StartJemairoDialogue("And try to keep an eye on the water level and house health!", 0.1f, false, 0.0f, 1.0f));

        yield return new WaitForSeconds(1.5f);
        canvas.GetComponent<MenuMechanics>().ActivatePauseButtonUI(true);
        canvas.GetComponent<MenuMechanics>().ActivateHealthUI(true);
        canvas.GetComponent<MenuMechanics>().ActivateLevelUI(true);

        yield return new WaitForSeconds(2.0f);

        player.GetComponent<Player>().movementEnabled = true;
        player.GetComponent<Player>().buildingEnabled = true;
        canvas.GetComponent<MenuMechanics>().shopEnabled = true;

        GetComponent<TileMechanics>().enableMouseOverTileMaterial = true;

        yield return new WaitForSeconds(0.5f);

        hintUI.SetActive(true);
        hintUI.GetComponent<UIFixedAnimation>().Play();

        hintTextUIObject.GetComponent<Text>().text = "THE TWO UPPER BARS SHOW THE WATER LEVEL AND HOUSE HEALTH";

        yield return new WaitForSeconds(9.0f);

        hintUI.GetComponent<UIFixedAnimation>().PlayReverse();

        StartTheGame();

        StartCoroutine(StartJemairoDialogue("Good luck g!", 0.1f, true, 5.5f, 1.5f));
    }

    public IEnumerator StartDialogueAfterWave1()
    {
        yield return new WaitForSeconds(1.5f);

        DialogueBoxJemairo.SetActive(true);
        DialogueBoxJemairo.GetComponent<UIFixedAnimation>().Play();

        StartCoroutine(StartJemairoDialogue("You completed the first wave, big up!", 0.1f, false, 0.0f, 1.5f));

        yield return new WaitForSeconds(1.0f);

        camController.ChangePosition(cameraPosJemairo, 2.0f);
        camController.ChangeZoom(cameraZoomDialogue, 3.0f);

        yield return new WaitForSeconds(4.5f);

        StartCoroutine(StartJemairoDialogue("But listen up!", 0.15f, false, 0.0f, 1.0f));

        yield return new WaitForSeconds(3.0f);

        StartCoroutine(StartJemairoDialogue("I want you to upgrade all tiles to level 9!", 0.1f, false, 0.0f, 2.0f));

        yield return new WaitForSeconds(5.5f);

        StartCoroutine(StartJemairoDialogue("That should stop the rain!", 0.1f, true, 4.5f, 1.0f));

        yield return new WaitForSeconds(4.5f);

        camController.ResetPosition(2.0f);
        camController.ResetZoom(1.0f);
        
        yield return new WaitForSeconds(1.5f);

        canvas.GetComponent<MenuMechanics>().ActivateGoalUI(true);

    }

    public IEnumerator StartDialogueAfterWave2()
    {
        yield return new WaitForSeconds(1.5f);

        DialogueBoxJemairo.SetActive(true);
        DialogueBoxJemairo.GetComponent<UIFixedAnimation>().Play();

        StartCoroutine(StartJemairoDialogue("Nice work g!", 0.1f, false, 0.0f, 1.5f));

        yield return new WaitForSeconds(2.5f);

        StartCoroutine(StartJemairoDialogue("My name is Jemairo by the way!", 0.1f, false, 0.0f, 1.0f));

        yield return new WaitForSeconds(5.0f);

        StartCoroutine(StartJemairoDialogue("But they call me Eazy-J around the block.", 0.1f, true, 7.0f, 1.0f));
    }

    public IEnumerator StartDialogueAfterWave3()
    {
        yield return new WaitForSeconds(1.5f);

        DialogueBoxJemairo.SetActive(true);
        DialogueBoxJemairo.GetComponent<UIFixedAnimation>().Play();

        StartCoroutine(StartJemairoDialogue("The waves will get harder!", 0.1f, false, 0.0f, 1.5f));

        yield return new WaitForSeconds(4.5f);

        StartCoroutine(StartJemairoDialogue("Just try to keep this up!", 0.1f, true, 6.5f, 1.0f));
    }

    public IEnumerator StartDialogueAfterWave4()
    {
        yield return new WaitForSeconds(1.5f);

        DialogueBoxJemairo.SetActive(true);
        DialogueBoxJemairo.GetComponent<UIFixedAnimation>().Play();

        StartCoroutine(StartJemairoDialogue("Yo this garden is getting lit!", 0.1f, false, 0.0f, 1.5f));

        yield return new WaitForSeconds(4.5f);

        StartCoroutine(StartJemairoDialogue("Oh and ignore those weapons...", 0.1f, false, 0.0f, 1.0f));

        yield return new WaitForSeconds(5.0f);

        StartCoroutine(StartJemairoDialogue("I swear on my momma they ain't mine!!", 0.1f, true, 7.5f, 1.0f));
    }

    public IEnumerator StartDialogueAfterWave5()
    {
        yield return new WaitForSeconds(1.5f);

        DialogueBoxJemairo.SetActive(true);
        DialogueBoxJemairo.GetComponent<UIFixedAnimation>().Play();

        StartCoroutine(StartJemairoDialogue("Good work g!", 0.1f, false, 0.0f, 1.5f));

        yield return new WaitForSeconds(4.0f);

        StartCoroutine(StartJemairoDialogue("Keep grinding!", 0.1f, true, 6.0f, 1.0f));
    }

    public IEnumerator StartDialogueAfterWave6()
    {
        yield return new WaitForSeconds(1.5f);

        DialogueBoxJemairo.SetActive(true);
        DialogueBoxJemairo.GetComponent<UIFixedAnimation>().Play();

        StartCoroutine(StartJemairoDialogue("Damn this is getting chaotic...", 0.1f, false, 0.0f, 1.5f));

        yield return new WaitForSeconds(4.5f);

        StartCoroutine(StartJemairoDialogue("Don't fail on me or I will actually kill you!", 0.1f, false, 0.0f, 1.0f));

        yield return new WaitForSeconds(5.5f);

        StartCoroutine(StartJemairoDialogue("Hahahaha I was just kidding!!", 0.1f, true, 6.5f, 1.0f));
    }

    public IEnumerator StartDialogueAfterWave7()
    {
        yield return new WaitForSeconds(1.5f);

        DialogueBoxJemairo.SetActive(true);
        DialogueBoxJemairo.GetComponent<UIFixedAnimation>().Play();

        StartCoroutine(StartJemairoDialogue("Big up!", 0.1f, false, 0.0f, 2.5f));

        yield return new WaitForSeconds(3.5f);

        StartCoroutine(StartJemairoDialogue("I respect your grind!", 0.1f, true, 6.0f, 1.5f));
    }

    public IEnumerator StartDialogueAfterWave8()
    {
        yield return new WaitForSeconds(1.5f);

        DialogueBoxJemairo.SetActive(true);
        DialogueBoxJemairo.GetComponent<UIFixedAnimation>().Play();

        StartCoroutine(StartJemairoDialogue("Good job bro!", 0.1f, false, 0.0f, 2.5f));

        yield return new WaitForSeconds(3.0f);

        StartCoroutine(StartJemairoDialogue("You can do this!!", 0.1f, true, 6.0f, 1.5f));
    }

    public IEnumerator StartDialogueAfterWave9()
    {
        yield return new WaitForSeconds(1.5f);

        DialogueBoxJemairo.SetActive(true);
        DialogueBoxJemairo.GetComponent<UIFixedAnimation>().Play();

        StartCoroutine(StartJemairoDialogue("You're on fire", 0.1f, false, 0.0f, 2.5f));

        yield return new WaitForSeconds(4.5f);

        StartCoroutine(StartJemairoDialogue("Just keep pushing!", 0.1f, false, 0.0f, 1.5f));
    }

    public IEnumerator StartDialogueAfterWave10()
    {
        yield return new WaitForSeconds(1.5f);

        DialogueBoxJemairo.SetActive(true);
        DialogueBoxJemairo.GetComponent<UIFixedAnimation>().Play();

        StartCoroutine(StartJemairoDialogue("You're getting real close my g!", 0.1f, true, 5.0f, 2.5f));
    }

    public IEnumerator StartDialogueBeforeBossWave()
    {
        yield return new WaitForSeconds(3.5f);

        DialogueBoxJemairo.SetActive(true);
        DialogueBoxJemairo.GetComponent<UIFixedAnimation>().Play();

        StartCoroutine(StartJemairoDialogue("COME ON YOU CAN DO THIS!", 0.05f, false, 0.0f, 2.5f));

        yield return new WaitForSeconds(3.0f);

        StartCoroutine(StartJemairoDialogue("WE'VE COME SO FAR!", 0.05f, false, 0.0f, 2.5f));

        yield return new WaitForSeconds(3.0f);

        StartCoroutine(StartJemairoDialogue("I BELIEVE IN YOU!!", 0.05f, true, 3.5f, 2.5f));
    }

    private void Update() {
        if (introGaande)
        {
            if (GetComponent<MoneyMechanics>().money >= 100 && doneStage1 == false)
            {
                player.GetComponent<Player>().movementEnabled = false;
                doneStage1 = true;
                FindObjectOfType<AudioManager>().Play("Done");
                StartCoroutine(StartStage2());
            }
            if (player.GetComponent<Player>().positionedOnArrow && doneStage2 == false)
            {
                player.GetComponent<Player>().movementEnabled = false;
                doneStage2 = true;
                FindObjectOfType<AudioManager>().Play("Done");
                StartCoroutine(StartStage3());
            }
            if (player.GetComponent<Player>().placedFirstTile && doneStage3 == false)
            {
                player.GetComponent<Player>().buildingEnabled = false;
                player.GetComponent<Player>().enableExclusiveBuildPos = false;
                player.GetComponent<Player>().movementEnabled = false;
                doneStage3 = true;
                FindObjectOfType<AudioManager>().Play("Done");
                StartCoroutine(StartStage4());
            }
            if (canvas.GetComponent<MenuMechanics>().wentBackToGameAtleastOnce && doneStage4 == false)
            {
                canvas.GetComponent<MenuMechanics>().shopEnabled = false;
                doneStage4 = true;
                FindObjectOfType<AudioManager>().Play("Done");
                StartCoroutine(StartStage5());
            }
        }
    }

    IEnumerator StartJemairoDialogue(string text, float letterDelay, bool disableWhenDone, float disableTime, float talkSpeed)
    {
        DialogueBoxJemairo.SetActive(true);
        DialogueTextJemairo.GetComponent<TextWritingEffect>().SetText(text, letterDelay, "ShowLetterJemairo");
        jemairoAnimator.SetFloat("TalkSpeed", talkSpeed);
        jemairoAnimator.SetBool("IsTalking", true);
        if (disableWhenDone)
        {
            yield return new WaitForSeconds(disableTime);
            DialogueBoxJemairo.transform.localScale = Vector3.zero;
            DialogueBoxJemairo.SetActive(false);
            jemairoAnimator.SetBool("IsTalking", false);
        }
    }

    IEnumerator StartBobDialogue(string text, float letterDelay, bool disableWhenDone, float disableTime)
    {
        DialogueBoxBob.SetActive(true);
        DialogueTextBob.GetComponent<TextWritingEffect>().SetText(text, letterDelay, "ShowLetterBob");
        if (disableWhenDone)
        {
            yield return new WaitForSeconds(disableTime);
            DialogueBoxBob.transform.localScale = Vector3.zero;
            DialogueBoxBob.SetActive(false);
        }
    }

    private void StartTheGame() //start da game
    {
        GetComponent<LevelMechanics>().StartGame(); // deze moet aanblijven
        introGaande = false;
    }
}
