using UnityEngine;

public class DistractionManager2 : MonoBehaviour
{
    public GameObject DistractionPrefab;
    private GameObject bot;

    private Transform head;

    [Header("Movement Bubble")]
    public float minDistance = 0.6f;
    public float maxDistance = 1.6f;

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

        // Always look at the user
        bot.transform.LookAt(head.position + Vector3.up * 0.1f);
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
        MoveBot(bot.transform.position, 2f);
    }

    void UpdateShortArc()
    {
        currentAngle = Mathf.Lerp(currentAngle, targetAngle, Time.deltaTime * 1.2f);
        Vector3 pos = PolarToPosition(currentAngle, distance);
        MoveBot(pos, 4f);
    }

    void UpdateZigZag()
    {
        zigTarget += new Vector3(
            Mathf.Sin(Time.time * 12f) * 0.01f,
            Mathf.Cos(Time.time * 8f) * 0.015f,
            0
        );

        MoveBot(zigTarget, 6f);
    }

    void UpdateFullOrbit()
    {
        currentAngle += 60f * Time.deltaTime;
        Vector3 pos = PolarToPosition(currentAngle, distance);
        MoveBot(pos, 4f);
    }

    void MoveBot(Vector3 target, float speed)
    {
        bot.transform.position = Vector3.Lerp(bot.transform.position, target, Time.deltaTime * speed);
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
