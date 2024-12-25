using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaleUIActions : MonoBehaviour
{
    [SerializeField] private  GameObject sliderGame;
    [SerializeField] private  RectTransform cursor;
    [SerializeField] private  RectTransform redZone;
    [SerializeField] private  RectTransform greenZone;
    [SerializeField] private  GameObject errorCursor;
    private RectTransform errorCursorRectTransform;
    private Image errorCursorImage;
    [SerializeField] private const float speed = 1000f;
    private bool isGameOn = false;
    private int direction = 1;
    private int level = 1;
    private float xPosition = -750f;
    [SerializeField] private const float buttonCD = 1f;
    private float startCD = -buttonCD;
    private float roundStartTime = 0f;

    public GameObject buttonPrefab; // Prefab for the button sprite
    public GameObject timerObject; // Circular timer object
    public string[] buttonNames = {"Z", "X", "C"}; // Array of button names (e.g., ["A", "B", "X", "Y"])
    public Color color1; // Color for failure
    public Color color2; // Color for success
    public float maxTime = 2f; // Maximum time to press the correct button
    public float delayBetweenQTEs = 1f; // Delay between each QTE

    private string currentButtonName;
    private GameObject currentButton;
    private Image timerImage;
    private bool inputEnabled = false;



    private void OnEnable() {
        if (InactiveQuestsStruct.currentQuest.minigame == "slider") {
            sliderGame.SetActive(true);
            isGameOn = true;
        }
        else if (InactiveQuestsStruct.currentQuest.minigame == "qte")
        {
            StartCoroutine(StartNextQTE());
        }
    }
    private void Start() {
        errorCursorRectTransform = errorCursor.GetComponent<RectTransform>();
        errorCursorImage = errorCursor.GetComponent<Image>();
        timerImage = timerObject.GetComponent<Image>();
    }
    void Update()
    {
        bool isQPressed = Input.GetKeyDown(KeyCode.Q);
        if (InactiveQuestsStruct.currentQuest.minigame == "slider") 
        {
            if (Time.time - startCD >= buttonCD)
            {
                errorCursor.SetActive(false);
            }
            else
            {
                errorCursorImage.color = new Color(1, 0, 0.4f, 1 + (startCD - Time.time) / buttonCD);
            }
            if (isGameOn)
            {
                xPosition += direction * speed * (redZone.rect.width / 1920f) * level * Time.deltaTime;
                if (Mathf.Abs(xPosition) * 2 >=  redZone.rect.width) {
                    direction *= -1;
                }
                xPosition = Mathf.Clamp(xPosition, -redZone.rect.width / 2, redZone.rect.width / 2);

                cursor.anchoredPosition = new Vector2(xPosition, cursor.anchoredPosition.y);
            }

            if (isQPressed && Time.time - startCD >= buttonCD)
            {
                if (Mathf.Abs(xPosition) * 2 <= greenZone.rect.width)
                {
                    xPosition = -redZone.rect.width / 2;
                    direction = 1;
                    cursor.anchoredPosition = new Vector2(xPosition, cursor.anchoredPosition.y);
                    if (level++ == 4)
                    {
                        SetGameState(false);
                    }
                }
                else
                {
                    startCD = Time.time;
                    errorCursor.SetActive(true);
                    errorCursorRectTransform.anchoredPosition = cursor.anchoredPosition;
                }
            }
        }
        else if (InactiveQuestsStruct.currentQuest.minigame == "qte")
        {
            HandleInput();
        }
        else
        {
            if (isQPressed) { 
                this.gameObject.SetActive(false);
                //Time.timeScale = 1.0f;
            }
        }
    }

    public void SetGameState(bool state)
    {
        isGameOn = state;
        if (state)
        {
            xPosition = -redZone.rect.width / 2;
            direction = 1;
            cursor.anchoredPosition = new Vector2(xPosition, cursor.anchoredPosition.y);
            roundStartTime = Time.time;
        }
        else
        {
            level = 1;
            sliderGame.SetActive(false);
            this.gameObject.SetActive(false);
        }
    }

    private IEnumerator StartNextQTE()
    {
        yield return new WaitForSeconds(delayBetweenQTEs);

        GenerateNewButton();
        inputEnabled = true;

        float timeLeft = maxTime;
        while (timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;
            timerImage.fillAmount = timeLeft / maxTime;
            Debug.Log(timeLeft / maxTime);
            yield return null;
        }

        // Timer ran out
        FailQTE();
    }

    private void GenerateNewButton()
    {
        // Destroy old button if it exists
        if (currentButton != null)
        {
            Destroy(currentButton);
        }

        // Create a new button
        currentButtonName = buttonNames[Random.Range(0, buttonNames.Length)];
        currentButton = Instantiate(buttonPrefab, transform);
        Text buttonText = currentButton.GetComponentInChildren<Text>();
        buttonText.text = currentButtonName;
        // Randomize button position on the screen
        RectTransform buttonRect = currentButton.GetComponent<RectTransform>();
        Vector2 newPos = GetRandomScreenPosition();
        buttonRect.anchoredPosition = newPos;

        // Randomize timer position on the screen
        RectTransform timerRect = timerObject.GetComponent<RectTransform>();
        timerRect.anchoredPosition = newPos;

        // Reset timer UI
        timerImage.fillAmount = 1;
        timerObject.SetActive(true);
    }


    private Vector2 GetRandomScreenPosition()
    {
        // Get canvas size
        //RectTransform canvasRect = GetComponent<RectTransform>();
        //float x = Random.Range(-canvasRect.rect.width / 2, canvasRect.rect.width / 2);
        //float y = Random.Range(-canvasRect.rect.height / 2, canvasRect.rect.height / 2);
        float x = Random.Range(-500, 500);
        float y = Random.Range(-250, 250);

        return new Vector2(x, y);
    }

    private void HandleInput()
    {
        if (!inputEnabled || !Input.anyKeyDown) return;

        string input = Input.inputString.ToUpper(); // Get user input
        StopAllCoroutines();
        if (input == currentButtonName)
        {
            SuccessQTE();
        }
        else
        {
            FailQTE();
        }
    }

    private void SuccessQTE()
    {
        inputEnabled = false;
        timerObject.SetActive(false);

        // Change button color to success
        currentButton.GetComponent<Image>().color = color2;
        level++;
        StartCoroutine(RemoveButtonAfterDelay());
    }

    private void FailQTE()
    {
        inputEnabled = false;
        timerObject.SetActive(false);

        // Change button color to failure
        currentButton.GetComponent<Image>().color = color1;

        StartCoroutine(RemoveButtonAfterDelay());
    }

    private IEnumerator RemoveButtonAfterDelay()
    {
        yield return new WaitForSeconds(0.5f); // Short delay for effect
        Destroy(currentButton);
        if (level == 5)
        {
            SetGameState(false);
        }
        else
        {
            StartCoroutine(StartNextQTE());
        }
    }
}
