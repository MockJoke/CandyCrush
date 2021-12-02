using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MatchFinder : MonoBehaviour
{
    Board board;
    Candy candy; 
    public List<GameObject> CurrentMatchedCandies = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        candy = FindObjectOfType<Candy>();
    }

    public void FindAllMatches()
    {
        StartCoroutine(CheckMatches());
    }

    IEnumerator CheckMatches()
    {
        yield return new WaitForSeconds(.2f);

        for (int i = 0; i < board.cols; i++)
        {
            for (int j = 0; j < board.rows; j++)
            {
                GameObject CurrentCandy = board.NewCandy[i, j];

                if (CurrentCandy != null)
                {
                    //row match
                    if (i > 0 && i < board.cols - 1)
                    {
                        GameObject LeftCandy = board.NewCandy[i - 1, j];
                        GameObject RightCandy = board.NewCandy[i + 1, j];
                        if (LeftCandy != null && RightCandy != null)
                        {
                            if (LeftCandy.tag == CurrentCandy.tag && RightCandy.tag == CurrentCandy.tag)
                            {
                                LeftCandy.GetComponent<Candy>().CandyMatched = true;
                                RightCandy.GetComponent<Candy>().CandyMatched = true;
                                CurrentCandy.GetComponent<Candy>().CandyMatched = true;
                                //currentCandy.GetComponent<Candy>().hasRowBomb = true;

                                if (!CurrentMatchedCandies.Contains(LeftCandy))
                                {
                                    CurrentMatchedCandies.Add(LeftCandy);
                                }
                                if (!CurrentMatchedCandies.Contains(RightCandy))
                                {
                                    CurrentMatchedCandies.Add(RightCandy);
                                }
                                if (!CurrentMatchedCandies.Contains(CurrentCandy))
                                {
                                    CurrentMatchedCandies.Add(CurrentCandy);
                                }

                                CheckRowCandies(LeftCandy.GetComponent<Candy>(), RightCandy.GetComponent<Candy>(), CurrentCandy.GetComponent<Candy>());
                                CheckColCandies(LeftCandy.GetComponent<Candy>(), RightCandy.GetComponent<Candy>(), CurrentCandy.GetComponent<Candy>());
                            
                            }

                        }
                    }

                    //column match
                    if (j > 0 && j < board.rows - 1)
                    {
                        GameObject DownCandy = board.NewCandy[i, j - 1];
                        GameObject UpCandy = board.NewCandy[i, j + 1];

                        if (DownCandy != null && UpCandy != null)
                        {
                            if (DownCandy.tag == CurrentCandy.tag && UpCandy.tag == CurrentCandy.tag)
                            {
                                DownCandy.GetComponent<Candy>().CandyMatched = true;
                                UpCandy.GetComponent<Candy>().CandyMatched = true;
                                CurrentCandy.GetComponent<Candy>().CandyMatched = true;

                                if (!CurrentMatchedCandies.Contains(DownCandy))
                                {
                                    CurrentMatchedCandies.Add(DownCandy);
                                }
                                if (!CurrentMatchedCandies.Contains(UpCandy))
                                {
                                    CurrentMatchedCandies.Add(UpCandy);
                                }
                                if (!CurrentMatchedCandies.Contains(CurrentCandy))
                                {
                                    CurrentMatchedCandies.Add(CurrentCandy);
                                }

                                CheckRowCandies(DownCandy.GetComponent<Candy>(), UpCandy.GetComponent<Candy>(), CurrentCandy.GetComponent<Candy>());
                                CheckColCandies(DownCandy.GetComponent<Candy>(), UpCandy.GetComponent<Candy>(), CurrentCandy.GetComponent<Candy>());

                            }

                        }
                    }
                }
            }
        }
    }

    public void CheckForBomb()
    {
        if (board.CurrentMovedCandy != null)
        {
            Candy CurCandy = board.CurrentMovedCandy.GetComponent<Candy>();
            if (CurCandy.CandyMatched)
            {
                CurCandy.CandyMatched = false;
                if (CurCandy.HorizontalMove)
                {
                    CurCandy.GenRowBomb();
                }
                else
                {
                    CurCandy.GenColBomb();
                }
            }
        }
    }

    List<GameObject> CheckRowCandies(Candy can1, Candy can2, Candy can3)
    {
        List<GameObject> candiesList = new List<GameObject>();
        if (can1.isRowBomb)
        {
            CurrentMatchedCandies.Union(collectRowCandies(can1.row));
        }
        if (can2.isRowBomb)
        {
            CurrentMatchedCandies.Union(collectRowCandies(can2.row));
        }
        if (can3.isRowBomb)
        {
            CurrentMatchedCandies.Union(collectRowCandies(can3.row));
        }
        return candiesList;
    }
    List<GameObject> collectRowCandies(int row)
    {
        List<GameObject> candiesList = new List<GameObject>();
        for (int i = 0; i < board.cols; i++)
        {
            board.NewCandy[i, row].GetComponent<Candy>().CandyMatched = true;
            candiesList.Add(board.NewCandy[i, row]);
        }
        return candiesList;
    }

    List<GameObject> CheckColCandies(Candy can1, Candy can2, Candy can3)
    {
        List<GameObject> candiesList = new List<GameObject>();
        if (can1.isColBomb)
        {
            CurrentMatchedCandies.Union(collectColumnCandies(can1.col));
        }
        if (can2.isColBomb)
        {
            CurrentMatchedCandies.Union(collectColumnCandies(can2.col));
        }
        if (can3.isColBomb)
        {
            CurrentMatchedCandies.Union(collectColumnCandies(can3.col));
        }
        return candiesList;
    }
    List<GameObject> collectColumnCandies(int col)
    {
        List<GameObject> candiesList = new List<GameObject>();
        for (int j = 0; j < board.rows; j++)
        {
            board.NewCandy[col, j].GetComponent<Candy>().CandyMatched = true;
            candiesList.Add(board.NewCandy[col, j]);
        }
        return candiesList;
    }

    public void CheckForColourBomb()
    {
        if (board.CurrentMovedCandy != null)
        {
            Candy CurCandy = board.CurrentMovedCandy.GetComponent<Candy>();
            if (CurCandy.CandyMatched)
            {
                CurCandy.CandyMatched = false;

                Destroy(board.NewCandy[CurCandy.col, CurCandy.row]); 
                
                CurCandy.GenColourBomb();              
                
            }
        }
    }

    

    List<GameObject> collectcolourcandies()
    {
        List<GameObject> candieslist = new List<GameObject>();

        for (int i = 0; i < board.cols; i++)
        {
            for (int j = 0; j < board.rows; j++)
            {
                if (board.NewCandy[i, j].tag == candy.SwipedCandy.tag)
                {
                    board.NewCandy[i, j].GetComponent<Candy>().CandyMatched = true;
                    candieslist.Add(board.NewCandy[i, j]);
                }
            }
        }

        return candieslist;
    }

}

