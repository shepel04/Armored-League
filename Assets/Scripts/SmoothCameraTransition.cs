using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform startTransform; // Початкова позиція камери
    public Transform endTransform; // Кінцева позиція камери
    public float moveSpeed = 1.0f; // Швидкість переміщення камери
    public float rotateSpeed = 1.0f; // Швидкість повороту камери

    private bool isMoving = false; // Прапорець, який вказує, чи камера зараз знаходиться в русі
    private float startTime; // Час початку руху
    private Transform originalEndTransform; // Оригінальна кінцева позиція камери

    void Start()
    {
        originalEndTransform = endTransform;
    }

    void Update()
    {
        // Перевіряємо, чи натиснута клавіша пробілу і камера не знаходиться в русі
        if (Input.GetKeyDown(KeyCode.Space) && !isMoving)
        {
            // Позначаємо початок руху та встановлюємо прапорець, що камера рухається
            startTime = Time.time;
            isMoving = true;
        }

        // Якщо камера в русі, переміщуємо її плавно
        if (isMoving)
        {
            float journeyLength = Vector3.Distance(startTransform.position, originalEndTransform.position); // Відстань, яку треба подолати
            float distanceCovered = (Time.time - startTime) * moveSpeed; // Відстань, пройдена за час руху
            float fracJourney = distanceCovered / journeyLength; // Пройдений відсоток від загальної відстані

            transform.position = Vector3.Lerp(startTransform.position, originalEndTransform.position, fracJourney); // Плавне переміщення камери
            transform.rotation = Quaternion.Slerp(startTransform.rotation, originalEndTransform.rotation, fracJourney); // Плавний поворот камери

            // Якщо камера досягла кінцевої позиції, закінчуємо рух
            if (distanceCovered >= journeyLength)
            {
                isMoving = false;
                // Після закінчення руху змінюємо початкову позицію на кінцеву і навпаки
                Transform tempTransform = startTransform;
                startTransform = originalEndTransform;
                originalEndTransform = tempTransform;
            }
        }
    }
}
