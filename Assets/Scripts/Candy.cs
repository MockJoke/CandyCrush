using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Candy : MonoBehaviour
{
    public int row, col, OldRow, OldCol, TargetPosX, TargetPosY;
    Vector3 OldPos, NewPos;
    float SwipeAngle;
    public GameObject SwipedCandy;
    Board board;
    public bool CandyMatched, isRowBomb, isColBomb, isColourBomb, HorizontalMove, VerticalMove;
    MatchFinder matchFinder;
    public GameObject RowBomb, ColBomb, ColourBomb;

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        matchFinder = FindObjectOfType<MatchFinder>();
    }

    // Update is called once per frame
    void Update()
    {
        TargetPosX = col;   //position to be moved to 
        TargetPosY = row; 

        if(Mathf.Abs(TargetPosX - transform.position.x) > 0f)   //if there's diff bw actual pos and pos to be moved to then 
        {
            Vector2 TempPos = new Vector2(TargetPosX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, TempPos, .6f);     //exchanges the pos  


            if (this.gameObject != board.NewCandy[col, row])
            {
                board.NewCandy[col, row] = this.gameObject;     //after swapping the pos, coordinates need to be changed in the array of candies  
            }

            matchFinder.FindAllMatches();

        }

        if (Mathf.Abs(TargetPosY - transform.position.y) > 0f)
        {
            Vector2 TempPos = new Vector2(transform.position.x, TargetPosY);
            transform.position = Vector2.Lerp(transform.position, TempPos, .6f);

            if (this.gameObject != board.NewCandy[col, row])
            {
                board.NewCandy[col, row] = this.gameObject;
            }

            matchFinder.FindAllMatches();

        }

        //MatchCandy(); 

        if (row == 0)
        {
            if(gameObject.tag == "token")
            {
                GetToken();
            }
        }

    }

    private void OnMouseDown()
    {
        
        OldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
         
    }

    private void OnMouseUp()
    {
        
        NewPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        FindAngle();
        
    }

    public void FindAngle()
    {
        board.CurrentMovedCandy = this.gameObject;

        SwipeAngle = Mathf.Atan2(NewPos.y - OldPos.y, NewPos.x - OldPos.x) * Mathf.Rad2Deg;
        MoveCandy(); 
    }

    public void MoveCandy()     //coordintes changed to targeted pos 
    {
        if (SwipeAngle >= -45f && SwipeAngle <= 45f)
        {
            HorizontalMove = true;
            OldCol = col;
            OldRow = row;
            SwipedCandy = board.NewCandy[col + 1, row];     //pos changed to move right 
            SwipedCandy.GetComponent<Candy>().col -= 1;
            col++;
        }
        else if (SwipeAngle >= 45f && SwipeAngle <= 135f)
        {
            VerticalMove = true;
            OldCol = col;
            OldRow = row;
            SwipedCandy = board.NewCandy[col, row + 1];
            SwipedCandy.GetComponent<Candy>().row -= 1;
            row++;
        }
        else if (SwipeAngle >= 135f || SwipeAngle <= -135f)
        {
            HorizontalMove = true;
            OldCol = col;
            OldRow = row;
            SwipedCandy = board.NewCandy[col - 1, row];
            SwipedCandy.GetComponent<Candy>().col += 1;
            col--;
        }
        else if (SwipeAngle >= -135f && SwipeAngle <= -45f)
        {
            VerticalMove = true;
            OldCol = col;
            OldRow = row;
            SwipedCandy = board.NewCandy[col, row - 1];
            SwipedCandy.GetComponent<Candy>().row += 1;
            row--;
        }

        StartCoroutine(CheckMatchCandy());

    }

    public void MatchCandy()        //if not matching candies after swipping then not allowed to move
    {
         
            if (row > 0 && row < board.rows - 1)
            {
                GameObject DownCandy = board.NewCandy[col, row - 1];    //candy below to swipped one  
                GameObject UpCandy = board.NewCandy[col, row + 1];

                if (DownCandy != null && UpCandy != null)
                {
                    if (DownCandy.tag == gameObject.tag && UpCandy.tag == gameObject.tag)        //if all candies matched 
                    {
                        CandyMatched = true;        //flags to be made true for all 
                        DownCandy.GetComponent<Candy>().CandyMatched = true;
                        UpCandy.GetComponent<Candy>().CandyMatched = true;
                    }
                }
            }

            if (col > 0 && col < board.cols - 1)
            {
                GameObject LeftCandy = board.NewCandy[col - 1, row];
                GameObject RightCandy = board.NewCandy[col + 1, row];

                if (LeftCandy != null && RightCandy != null)
                {
                    if (LeftCandy.tag == gameObject.tag && RightCandy.tag == gameObject.tag)
                    {
                        CandyMatched = true;
                        LeftCandy.GetComponent<Candy>().CandyMatched = true;
                        RightCandy.GetComponent<Candy>().CandyMatched = true;
                    }
                }
            }
    }

    IEnumerator CheckMatchCandy()
    {
        yield return new WaitForSeconds(.5f);       //delay  

        if (SwipedCandy != null)     //if swipped 
        {
                if (!SwipedCandy.GetComponent<Candy>().CandyMatched && !CandyMatched)        //if flags not true 
                {
                    if(SwipedCandy.GetComponent<Candy>().isColourBomb)
                    {
                        
                    }

                    SwipedCandy.GetComponent<Candy>().row = row;        //pos changed to previous to not allow the movement 
                    SwipedCandy.GetComponent<Candy>().col = col;
                    row = OldRow;
                    col = OldCol;
                    SwipedCandy = null;

                }
                else
                {
                    board.DestroyMatchedCandies();
                }
        }

    }

    public void GetToken()
    {
        GameObject[] ReceivedToken = GameObject.FindGameObjectsWithTag("token");
        Destroy(ReceivedToken[0]);
        board.TokenNo--; 
        board.TokenCnt++;

    }

    public void GenRowBomb()
    {
        isRowBomb = true; 
        GameObject bomb = Instantiate(RowBomb, transform.position, Quaternion.identity);
        bomb.transform.parent = this.transform;
    }

    public void GenColBomb()
    {
        isColBomb = true; 
        GameObject bomb = Instantiate(ColBomb, transform.position, Quaternion.identity);
        bomb.transform.parent = this.transform; 
    }

    public void GenColourBomb()
    {
        isColourBomb = true; 
        GameObject bomb = Instantiate(ColourBomb, transform.position, Quaternion.identity);
        bomb.transform.parent = this.transform;
    }
}
