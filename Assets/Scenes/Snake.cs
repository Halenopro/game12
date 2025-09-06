using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Snake : MonoBehaviour
{
    [Header("Map Settings")]
    public int xSize, ySize;
    public GameObject block;

    [Header("Sprites")]
    public Sprite headSprite, tailSprite, foodSprite, borderSprite;

    [Header("UI")]
    public Text points;
    public GameObject gameOverUI;

    private GameObject head;
    private List<GameObject> tail;
    private GameObject food;

    private Vector2 dir;
    private bool isAlive;

    private float passedTime;
    private float timeBetweenMovements;

    void Start()
    {
        timeBetweenMovements = 0.5f;
        dir = Vector2.right;

        createGrid();
        createPlayer();
        spawnFood();

        block.SetActive(false);
        isAlive = true;
    }

    private Vector2 getRandomPos()
    {
        return new Vector2(Random.Range(-xSize / 2 + 1, xSize / 2),
                           Random.Range(-ySize / 2 + 1, ySize / 2));
    }

    private bool containedInSnake(Vector2 spawnPos)
    {
        if (spawnPos == (Vector2)head.transform.position) return true;

        foreach (var item in tail)
        {
            if ((Vector2)item.transform.position == spawnPos) return true;
        }
        return false;
    }

    private void spawnFood()
    {
        Vector2 spawnPos = getRandomPos();
        while (containedInSnake(spawnPos))
        {
            spawnPos = getRandomPos();
        }

        food = Instantiate(block);
        food.transform.position = new Vector3(spawnPos.x, spawnPos.y, 0);
        food.GetComponent<SpriteRenderer>().sprite = foodSprite;
        food.SetActive(true);
    }

    private void createPlayer()
    {
        head = Instantiate(block);
        head.GetComponent<SpriteRenderer>().sprite = headSprite;
        setRotation(head, dir);
        tail = new List<GameObject>();
    }

    private void createGrid()
    {
        for (int x = 0; x <= xSize; x++)
        {
            GameObject borderBottom = Instantiate(block);
            borderBottom.transform.position = new Vector3(x - xSize / 2, -ySize / 2, 0);
            borderBottom.GetComponent<SpriteRenderer>().sprite = borderSprite;

            GameObject borderTop = Instantiate(block);
            borderTop.transform.position = new Vector3(x - xSize / 2, ySize - ySize / 2, 0);
            borderTop.GetComponent<SpriteRenderer>().sprite = borderSprite;
        }

        for (int y = 0; y <= ySize; y++)
        {
            GameObject borderRight = Instantiate(block);
            borderRight.transform.position = new Vector3(-xSize / 2, y - (ySize / 2), 0);
            borderRight.GetComponent<SpriteRenderer>().sprite = borderSprite;

            GameObject borderLeft = Instantiate(block);
            borderLeft.transform.position = new Vector3(xSize - (xSize / 2), y - (ySize / 2), 0);
            borderLeft.GetComponent<SpriteRenderer>().sprite = borderSprite;
        }
    }

    private void gameOver()
    {
        isAlive = false;
        gameOverUI.SetActive(true);
    }

    public void restart()
    {
        SceneManager.LoadScene(0);
    }

    private void setRotation(GameObject obj, Vector2 direction)
    {
        float angle = 0;

        if (direction == Vector2.up) angle = 90;
        else if (direction == Vector2.down) angle = -90;
        else if (direction == Vector2.left) angle = 180;
        else if (direction == Vector2.right) angle = 0;

        obj.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void Update()
    {
        // Điều khiển
        if (Input.GetKey(KeyCode.DownArrow) && dir != Vector2.up) dir = Vector2.down;
        else if (Input.GetKey(KeyCode.UpArrow) && dir != Vector2.down) dir = Vector2.up;
        else if (Input.GetKey(KeyCode.RightArrow) && dir != Vector2.left) dir = Vector2.right;
        else if (Input.GetKey(KeyCode.LeftArrow) && dir != Vector2.right) dir = Vector2.left;

        passedTime += Time.deltaTime;
        if (timeBetweenMovements < passedTime && isAlive)
        {
            passedTime = 0;
            Vector3 newPosition = head.transform.position + new Vector3(dir.x, dir.y, 0);


            if (newPosition.x >= xSize / 2
            || newPosition.x <= -xSize / 2
            || newPosition.y >= ySize / 2
            || newPosition.y <= -ySize / 2)
            {
                gameOver();
                return;
            }


            foreach (var item in tail)
            {
                if (item.transform.position == newPosition)
                {
                    gameOver();
                    return;
                }
            }


            Vector3 foodPos = new Vector3(Mathf.Round(food.transform.position.x), Mathf.Round(food.transform.position.y), 0);


            if (newPosition == foodPos)
            {

                head.GetComponent<SpriteRenderer>().sprite = tailSprite;
                tail.Add(head);


                head = Instantiate(block);
                head.transform.position = foodPos;
                head.GetComponent<SpriteRenderer>().sprite = headSprite;
                setRotation(head, dir);

                Destroy(food);
                spawnFood();


                points.text = "Points: " + tail.Count;


                if (timeBetweenMovements > 0.1f)
                    timeBetweenMovements -= 0.01f;
            }
            else
            {

                if (tail.Count == 0)
                {
                    head.transform.position = newPosition;
                    setRotation(head, dir);
                }
                else
                {
                    head.GetComponent<SpriteRenderer>().sprite = tailSprite;
                    tail.Add(head);

                    head = tail[0];
                    head.GetComponent<SpriteRenderer>().sprite = headSprite;
                    tail.RemoveAt(0);

                    head.transform.position = newPosition;
                    setRotation(head, dir);
                }
            }
        }
    }
}
