using UnityEngine;
using TMPro;
using System.Collections;

public class RaceCountdown : MonoBehaviour
{
    public TextMeshProUGUI countdownText;
    public CarController carController;

    private Rigidbody carRb;

    private void Awake()
    {
        carRb = carController.GetComponent<Rigidbody>();
    }

    private IEnumerator Start()
    {
        Debug.Log("COUNTDOWN START");

        carController.canDrive = false;

        carRb.linearVelocity = Vector3.zero;
        carRb.angularVelocity = Vector3.zero;

        countdownText.gameObject.SetActive(true);

        countdownText.text = "3";
        yield return new WaitForSecondsRealtime(1f);

        countdownText.text = "2";
        yield return new WaitForSecondsRealtime(1f);

        countdownText.text = "1";
        yield return new WaitForSecondsRealtime(1f);

        countdownText.text = "GO!";
        Debug.Log("GO!");

        carController.canDrive = true;
        Debug.Log("canDrive = " + carController.canDrive);

        if (GameManager.Instance != null)
        {
            Debug.Log("StartRace Dipanggil");
            GameManager.Instance.StartRace();
        }
        else
        {
            Debug.LogError("GameManager Instance NULL");
        }

        yield return new WaitForSecondsRealtime(1f);

        countdownText.gameObject.SetActive(false);
    }
}