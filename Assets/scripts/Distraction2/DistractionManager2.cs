using UnityEngine;

public class DistractionManager2 : MonoBehaviour
{
    public GameObject DistractionPrefab;
    private GameObject bot;

    private Transform head;

    [Header("Movement Bubble")]
    public float minDistance = 0.6f;
    public float maxDistance = 1.6f;
    public float headTarget = 0.5f;


    [Header("Movement Speeds")]
    public float baseMoveSpeed = 2.5f;
    public float idleSpeedMultiplier = 0.5f;
    public float arcSpeedMultiplier = 1.0f;
    public float zigZagSpeedMultiplier = 1.5f;
    public float orbitSpeedMultiplier = 1.0f;

    [Header("Height Limits (relative to head)")]
    public float minHeight = -0.3f;   // below head (meters)
    public float maxHeight = 0.8f;    // above head (meters)


    private enum MoveState { IdleHover, ShortArc, ZigZag, FullOrbit }
    private MoveState state;

    private float stateTimer;

    [System.Serializable]
    public struct FloatRange
    {
        public float min;
        public float max;
    }

    public FloatRange stateTimeRange;
    public FloatRange idleTimeRange;

    private float currentAngle;
    private float targetAngle;
    private float distance;
    private Vector3 zigTarget;

    void Start()
    {
        head = CameraManager.Instance.head;
        bot = Instantiate(DistractionPrefab);

      
    }

    void Update()
    {
        if (head == null || bot == null) return;

        stateTimer -= Time.deltaTime;
        if (stateTimer <= 0)
            PickNewState();

        switch (state)
        {
            case MoveState.IdleHover: UpdateIdle(); break;
            case MoveState.ShortArc: UpdateShortArc(); break;
            case MoveState.ZigZag: UpdateZigZag(); break;
            case MoveState.FullOrbit: UpdateFullOrbit(); break;
        }

        // Always apply jitter every frame
        ApplyHoverJitter();
        ApplyTiltJitter();

        ClampHeight();

        // Aim 50 cm below the head
        Vector3 safeTarget = head.position + Vector3.down * headTarget;
        Vector3 direction = (safeTarget - bot.transform.position).normalized;

        // Keep bat upright
        Quaternion targetRot = Quaternion.LookRotation(direction, Vector3.up);
        bot.transform.rotation = targetRot;

        


    }

    public void PickNewState()
    {
        float roll = Random.value;

        // FIXED incorrect logic ordering
        if (roll < 0.30f)
            state = MoveState.FullOrbit;
        else if (roll < 0.35f)
            state = MoveState.ShortArc;
        else if (roll < 0.50f)
            state = MoveState.ZigZag;
        else
            state = MoveState.IdleHover;

        if (state == MoveState.IdleHover)
            stateTimer = Random.Range(idleTimeRange.min, idleTimeRange.max);
        else
            stateTimer = Random.Range(stateTimeRange.min, stateTimeRange.max);

        distance = Random.Range(minDistance, maxDistance);
        currentAngle = AngleFromUser(bot.transform.position);

        if (state == MoveState.ShortArc)
        {
            float arc = Random.Range(20f, 90f);
            if (Random.value < 0.5f) arc *= -1f;
            targetAngle = currentAngle + arc;
        }

        if (state == MoveState.ZigZag)
        {
            zigTarget = head.position +
                        head.right * Random.Range(-0.5f, 0.5f) +
                        head.forward * Random.Range(0.5f, 1.2f);

            zigTarget.y = head.position.y + Random.Range(-0.3f, 0.4f);
        }
    }

    void UpdateIdle()
    {
        MoveBot(bot.transform.position, idleSpeedMultiplier);
    }

    void UpdateShortArc()
    {
        currentAngle = Mathf.Lerp(currentAngle, targetAngle, Time.deltaTime * 1.2f);
        Vector3 pos = PolarToPosition(currentAngle, distance);
        MoveBot(pos, arcSpeedMultiplier);
    }

    void UpdateZigZag()
    {
        zigTarget += new Vector3(
            Mathf.Sin(Time.time * 12f) * 0.01f,
            Mathf.Cos(Time.time * 8f) * 0.015f,
            0
        );

        MoveBot(zigTarget, zigZagSpeedMultiplier);
    }

    void UpdateFullOrbit()
    {
        currentAngle += 60f * Time.deltaTime;
        Vector3 pos = PolarToPosition(currentAngle, distance);
        MoveBot(pos, orbitSpeedMultiplier);
    }


    /// <summary>
    /// /this function controls of the moving speed
    /// </summary>
    /// 

    public float acceleration = 5f;
    private float currentSpeed;

    void MoveBot(Vector3 target, float speedMultiplier)
    {
        float speed = baseMoveSpeed * speedMultiplier;
        currentSpeed = Mathf.Lerp(currentSpeed, speed,Time.deltaTime * acceleration);

        bot.transform.position = Vector3.MoveTowards(
            bot.transform.position,
            target,
            currentSpeed * Time.deltaTime
        );
    }

    /// <summary>
    /// /this function controls of height of the movement
    /// </summary>
    /// 
    void ClampHeight()
    {
        Vector3 p = bot.transform.position;
        float minY = head.position.y + minHeight;
        float maxY = head.position.y + maxHeight;

        p.y = Mathf.Clamp(p.y, minY, maxY);
        bot.transform.position = p;
    }



    // ---------------------------
    // ALWAYS-ON JITTER FUNCTIONS
    // ---------------------------

    void ApplyHoverJitter()
    {
        Vector3 p = bot.transform.position;

        p.y += Mathf.Sin(Time.time * 3f) * 0.015f;   // soft up/down
        p.x += Mathf.Sin(Time.time * 1.8f) * 0.005f; // small horizontal wobble
        p.z += Mathf.Cos(Time.time * 2.1f) * 0.005f;

        bot.transform.position = p;
    }

    void ApplyTiltJitter()
    {
        Vector3 tilt = new Vector3(
            Mathf.Sin(Time.time * 2f) * 3f,
            Mathf.Cos(Time.time * 3f) * 2f,
            Mathf.Sin(Time.time * 1.5f) * 2f
        );

        bot.transform.localRotation *= Quaternion.Euler(tilt * Time.deltaTime);
    }

    Vector3 PolarToPosition(float angle, float distance)
    {
        float rad = angle * Mathf.Deg2Rad;
        Vector3 pos = head.position +
                      new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad)) * distance;

        return pos;
    }

    float AngleFromUser(Vector3 pos)
    {
        Vector3 dir = pos - head.position;
        return Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;
    }
}
