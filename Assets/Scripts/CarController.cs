using System;
using UnityEngine;
using UnityEngine.UI;

public class CarController : MonoBehaviour
{
    private Rigidbody _rb;
    public float speed = 8f, finalSpeed = 15f, rotateSpeed = 350f;
    private bool isClicked;

    private float curPointX, curPointY;
    [NonSerialized] public Vector3 FinalPosition;

    public enum Axis
    {
        Vertical, Horizontal
    }

    public Axis CarAxis;

    private enum Direction
    {
        Right, Left, Top, Bottom, None
    }

    public Text CountMoves, CountMoney;
    public GameObject StartGameBtn;

    private Direction CarDirectionX = Direction.None;
    private Direction CarDirectionY = Direction.None;

    private static int CountCars = 0;

    public ParticleSystem CrashEffect;

    private void Awake()
    {
        CountCars++;
        _rb = GetComponent<Rigidbody>();
    }

    void OnMouseDown()
    {
        if (!StartGame.IsGameStarted) return;

        curPointX = Input.mousePosition.x;
        curPointY = Input.mousePosition.y;
    }

    private void OnMouseUp()
    {
        if (!StartGame.IsGameStarted) return;

        if (Input.mousePosition.x - curPointX > 0)
            CarDirectionX = Direction.Right;
        else
            CarDirectionX = Direction.Left;

        if (Input.mousePosition.y - curPointY > 0)
            CarDirectionY = Direction.Top;
        else
            CarDirectionY = Direction.Bottom;

        isClicked = true;

        CountMoves.text = Convert.ToString(Convert.ToInt32(CountMoves.text) - 1);
    }

    //void OnMouseDown()
    //{
    //    curPointX = Input.GetTouch(0).position.x;
    //    curPointY = Input.GetTouch(0).position.y;
    //}

    //private void OnMouseUp()
    //{
    //    if (Input.GetTouch(0).position.x - curPointX > 0)
    //        CarDirectionX = Direction.Right;
    //    else
    //        CarDirectionX = Direction.Left;

    //    if (Input.GetTouch(0).position.y - curPointY > 0)
    //        CarDirectionY = Direction.Top;
    //    else
    //        CarDirectionY = Direction.Bottom;

    //    isClicked = true;
    //}

    private void Update()
    {
        if (CountMoves.text == "0" && CountCars > 0 && !isClicked)
            StartGameBtn.GetComponent<StartGame>().LoseGame();

        if (FinalPosition.x != 0)
        {
            transform.position = Vector3.MoveTowards(transform.position, FinalPosition, finalSpeed * Time.deltaTime);

            Vector3 lookAtPos = FinalPosition - transform.position;
            lookAtPos.y = 0;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(lookAtPos), Time.deltaTime * rotateSpeed);
        }

        if (transform.position == FinalPosition)
        {
            PlayerPrefs.SetInt("CarCoins", PlayerPrefs.GetInt("CarCoins") + 1);
            CountMoney.text = Convert.ToString(Convert.ToInt32(CountMoney.text) + 1);
            CountCars--;

            if (CountCars == 0) StartGameBtn.GetComponent<StartGame>().WinGame();

            Destroy(gameObject);
        }
            
    }

    private void FixedUpdate()
    {
        if (isClicked && FinalPosition.x == 0)
        {
            Vector3 whichWay = CarAxis == Axis.Horizontal ? Vector3.forward : Vector3.left;

            speed = Math.Abs(speed);
            if (CarDirectionX == Direction.Left && CarAxis == Axis.Horizontal)
                speed *= -1;
            else if (CarDirectionY == Direction.Bottom && CarAxis == Axis.Vertical)
                speed *= -1;

            _rb.MovePosition(_rb.position + whichWay * speed * Time.fixedDeltaTime);
        }
            
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Car") || other.CompareTag("Barrier"))
        {
            Destroy(Instantiate(CrashEffect, other.ClosestPoint(transform.position), Quaternion.Euler(new Vector3(270, 0, 0))), 2f);

            if (CarAxis == Axis.Horizontal && isClicked)
            {
                float adding = CarDirectionX == Direction.Left ? 0.5f : -0.5f;
                transform.position = new Vector3(transform.position.x, 0, transform.position.z + adding);
            }

            if (CarAxis == Axis.Vertical && isClicked)
            {
                float adding = CarDirectionY == Direction.Top ? 0.5f : -0.5f;
                transform.position = new Vector3(transform.position.x + adding, 0, transform.position.z);
            }

            isClicked = false;
        }
    }
}
