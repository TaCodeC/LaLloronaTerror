    using UnityEngine;
    public class NoiseEmitter :  MonoBehaviour
    {
        [Header("Fuentes de Ruido")]
        public float walkNoise = 0.1f;
        public float runNoise  = 1f;
        public float idleNoise  = 0.05f;
        public float collisionNoise  = 0.4f;
        
        [Header("Decay: ")]
        public float decayTime = 0.5f;

        [Header("Mas popurri que no se que nombre ponerle: ")]
        public float currentNoise = 0f;
        private CharacterController cc;
        public AudioSource audioSource;
        
        [Header("Los clips de audio")]
        public AudioClip[] walkClips;
        public AudioClip touchClip;
        public float walkNoiseMultiplier = 0.5f;
        public float runNoiseMultiplier = 1f;
        public float stepInterval = 0.6f;
        public float runInterval = 0.16f;
        private float stepSoundTimer;
        
        public float speedDivider = 40;
        void Start()
        {
            cc= GetComponent<CharacterController>();
            audioSource.spatialBlend = 1f;
            audioSource.playOnAwake = false;
        }

        void Update()
        {
            float speed = cc.velocity.magnitude/speedDivider;
            

            if (speed > 0f && speed < 4f)
            {
             AddNoise(walkNoise * Time.deltaTime);
             HandleSteps(walkNoiseMultiplier,stepInterval);
            } else if (speed > 4f)
            {
                AddNoise(runNoise * Time.deltaTime);
                HandleSteps(runNoiseMultiplier,stepInterval);
            }
            if (speed < 0.1f)
            {
                currentNoise = Mathf.MoveTowards(currentNoise, idleNoise, decayTime * Time.deltaTime);
                AudioManager.Instance.RegisterNoise(transform.position, currentNoise);
            }

            
        }

        private void AddNoise(float Amount)
        {
            currentNoise = Mathf.Clamp01(currentNoise + Amount / 1.0f);
            AudioManager.Instance.RegisterNoise(transform.position, currentNoise);
        }

        void OnCollisionEnter(Collision collision)
        {
            AddNoise(collisionNoise);
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.volume = collision.relativeVelocity.magnitude;
            audioSource.PlayOneShot(touchClip);
        }

        private void HandleSteps(float volume, float stepsInterval)
        {
            if (walkClips.Length == 0 || cc.velocity.magnitude < 0.1f) return;
            
            stepSoundTimer += Time.deltaTime;
            if (stepSoundTimer > stepsInterval)
            {
                stepSoundTimer = 0f;
                AudioClip clip = walkClips[Random.Range(0, walkClips.Length)];
                audioSource.pitch = Random.Range(0.9f, 1.1f);
                audioSource.volume = volume * Mathf.Lerp(0.3f,1f,currentNoise);
                audioSource.PlayOneShot(clip);
            }
        }

    }
