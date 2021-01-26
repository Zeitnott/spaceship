using UnityEngine.SceneManagement;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    Rigidbody _rigidbody;  
    AudioSource audioSource;
    [SerializeField] float thrustSpeed = 5f;
    [SerializeField] float rotationSpeed = 10f;
    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip playerDeath;
    [SerializeField] AudioClip levelComplete;
    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem playerDeathParticles;
    [SerializeField] ParticleSystem levelCompleteParticles;
    [SerializeField] float levelLoadDelay = 2f;

    enum State {Alive, Dying, Transcending}
    State state = State.Alive;
    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

   
    void Update()
    {
        if (state == State.Alive)
        {
            Thrust();
            Rotate();
        }
    }

    void Thrust()
    {
        if (Input.GetKey(KeyCode.Space)) // can thrust while rotating
        {
            _rigidbody.AddRelativeForce(Vector3.up * thrustSpeed * Time.deltaTime);
            AudioPlayback();

        }
        else
        {
            audioSource.Stop();
            mainEngineParticles.Stop();
        }
       
    }

    private void AudioPlayback()
    {
        if (!audioSource.isPlaying) // so it doesn't layer
        {
            audioSource.PlayOneShot(mainEngine);
            mainEngineParticles.Play();
        }
    }

    private void Rotate()
    {
        _rigidbody.freezeRotation = true;
        if (Input.GetKey(KeyCode.A)) // cannot rotating in left and in right at the same time
        {
            transform.Rotate(Vector3.forward * Time.deltaTime * rotationSpeed);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * Time.deltaTime * rotationSpeed);
        }
        _rigidbody.freezeRotation = false;
    }
    private void OnCollisionEnter(Collision collision)
    {   
        if (state != State.Alive)
        {
            return; // ignore collisions when dead
        }

        switch (collision.gameObject.tag)
        {
            case "Friendly":
                // do nothing
                break;
            case "Fuel":
                print("Fuel");
                break;
            case "Finish":
                state = State.Transcending;
                StartFinishMethod();
                Invoke("LoadNextScene", levelLoadDelay);
                break;
            default:
                state = State.Dying;
                StartDeathMethod();
                Invoke("LoadStartScene", levelLoadDelay);
                break;
        }
    }

    private void StartDeathMethod()
    {
        audioSource.Stop();
        audioSource.PlayOneShot(playerDeath);
        playerDeathParticles.Play();
    }

    private void StartFinishMethod()
    {
        audioSource.Stop();
        audioSource.PlayOneShot(levelComplete);
        levelCompleteParticles.Play();
    }

    private void LoadStartScene()
    {
        SceneManager.LoadScene(0); // start over
    }

    private void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1; //todo the right thing
        if (nextSceneIndex == SceneManager.sceneCountInBuildSettings)
        {
            nextSceneIndex = 0; //loop back to start
        }
        SceneManager.LoadScene(nextSceneIndex);

    }
}
