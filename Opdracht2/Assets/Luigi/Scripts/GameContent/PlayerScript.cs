using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerScript : MonoBehaviour
{
    private Rigidbody _rb;
    private float _xPos;
    private float _yPos;
    [SerializeField]
    private float _posAmplitude;
    //UI
    [SerializeField]
    private GameObject _DeathPanel;
    [SerializeField]
    private AudioSource _music;
    [SerializeField]
    private Camera _cam;
    void Start()
    {
        _music.Play();
        _rb = GetComponent<Rigidbody>();
        _xPos = 0;
        _yPos = 0;
        _DeathPanel.SetActive(false);
    }
    void Update()
    {
        _rb.transform.position = _cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 20f));
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Danger"))
        {
            _music.mute = true;
            _DeathPanel.SetActive(true);
            this.gameObject.SetActive(false);
        }
    }

    public void TryAgain()
    {

        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
        _music.Stop();
    }
}
