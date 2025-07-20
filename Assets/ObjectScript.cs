using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ObjectScript : MonoBehaviour
{
    public GameObject floorObject;
    public GameObject background;
    private float lastPosX = 0f;
    private float xPos = 50f;

    public Rigidbody2D rb;
    public GameObject trace;
    public Camera camera;
    public CircleCollider2D circleCollider;

    public Text angleText;
    public Text timerText;

    public Text distanceTravelledText;
    public Text maxHeightText;

    public Button showMaxHeightButton;
    public Button showDistanceTravelledButton;

    public Slider angleSlider;
    public GameObject angleMarker;

    public InputField velocityInput;
    public InputField heightInput;

    public Toggle showTime;
    public Toggle showVelocity;
    public Toggle showHeight;
    public Toggle showAngle;

    private float velocity;
    private float cameraSize = 6;

    private float rotation;
    private bool AnimationStarted = false;
    private bool AnimationEnded = false;

    private float timer = 0f;
    private bool timerIsRunning = false;
    private float seconds = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        circleCollider.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(0,0,0);

        if (timerIsRunning)
        {
            timer += Time.deltaTime;
            seconds = Mathf.Round((timer % 60f) * 1000f) / 1000f; ;
            timerText.text = $"{seconds} seconds";
        }

        camera.orthographicSize = cameraSize;
        angleText.text = $"{angleSlider.value}°";

        if (velocityInput.text != "")
        {
            velocity = float.Parse(velocityInput.text);
        }

        if (heightInput.text != "" && !AnimationStarted)
        {
            transform.position = new Vector3(transform.position.x, float.Parse(heightInput.text), transform.position.z);
        }

        angleMarker.transform.rotation = Quaternion.Euler(0, 0, -angleSlider.value);

        if (Input.GetKeyDown(KeyCode.Space) && !AnimationStarted)
        {
            AnimationStarted = true;
            timerIsRunning = true;
            circleCollider.enabled = true;

            float angleInRadians = (-angleSlider.value + 90f) * Mathf.Deg2Rad;
            Vector2 direction = new Vector2(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians)).normalized;
            rb.AddForce(direction * velocity, ForceMode2D.Impulse);
        }

        SetEnabled(timerText, showTime.isOn);
        SetActive(velocityInput.gameObject, showVelocity.isOn);
        SetActive(heightInput.gameObject, showHeight.isOn);
        SetActive(angleSlider.gameObject, showAngle.isOn);

        if (AnimationStarted)
        {
            rb.gravityScale = 1;
            ZoomOutAnim();
            angleMarker.SetActive(false);

            if (!AnimationEnded)
            {
                Instantiate(trace, transform.position, transform.rotation);
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if (transform.position.x > lastPosX + 50f)
        {
            Instantiate(floorObject, new Vector3(xPos + 25, -1.05f, 0), transform.rotation);
            Instantiate(background, new Vector3(xPos + 25, 6.8f, 0), transform.rotation);
            lastPosX = transform.position.x;
            xPos += 50;
        }
    }

    void SetActive(GameObject obj, bool isActive)
    {
        obj.SetActive(isActive);
    }

    void SetEnabled(Behaviour component, bool isEnabled)
    {
        component.enabled = isEnabled;
    }

    void ZoomOutAnim()
    {
        if (cameraSize < 10f)
        {
            cameraSize += 1f * Time.deltaTime;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        rb.angularVelocity = 0;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        AnimationEnded = true;
        rb.gravityScale = 0;
        timerIsRunning = false;

        showMaxHeightButton.gameObject.SetActive(true);
        showDistanceTravelledButton.gameObject.SetActive(true);
    }

    public void CalculateDistanceTravelled()
    {
        float distanceTravelled = seconds * velocity * Mathf.Cos((90 - angleSlider.value) * Mathf.Deg2Rad);
        distanceTravelledText.text = distanceTravelled.ToString();
    }

    public void CalculateMaxHeight()
    {
        float maxHeight = Mathf.Pow(velocity * Mathf.Sin((90 - angleSlider.value) * Mathf.Deg2Rad), 2) / (2 * 9.81f);
        maxHeightText.text = maxHeight.ToString();
    }

    public void RandomiseVelocity()
    {
        float randomVelocity = Mathf.Round(Random.Range(0f, 100f) * 10f) / 10f;

        velocityInput.text = randomVelocity.ToString();
        velocity = randomVelocity;
    }

    public void RandomiseHeight()
    {
        float randomHeight = Mathf.Round(Random.Range(0f, 100f) * 10f) / 10f;

        heightInput.text = randomHeight.ToString();
    }

    public void RandomiseAngle()
    {
        int randomAngle = Random.Range(0, 90);

        angleSlider.value = randomAngle;
        angleText.text = randomAngle.ToString();
    }
}
