using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;



static class Constants
{
    public const int MAT_W = 30;
    public const int MAT_H = 50;
    public const int NUM_SNAKES = 5;
    public const int SNAKE_LENGTH = 5;
    public const int NUM_COLORS = 4;
}


class nodeM
{
    public int snakeid;
    public bool ishead;
    public nodeM()
    {
        snakeid = 0;
        ishead = false;
    }

    public void update_info(int sid, bool head)
    {
        snakeid = sid;
        ishead = head;
    }
}


class snake
{
    public int colour;
    public List position;
    public int id;
    public snake()
    {
        colour = 0;
        position = new List();
        id = 0;
    }
}


class nodeL
{
    public int x, y;
    public nodeL next, prev;

    public nodeL(int xi, int yi)
    {
        x = xi; y = yi;
    }
    public nodeL()
    {
        x = 0;
        y = 0;
    }

}


class List
{
    // public int x;
    //public int y;
    // public List next;
    // public List prev;
    public nodeL head, tail;
    public int length;

    public List()
    {
        head = new nodeL();
        tail = new nodeL();
        length = 0;
        head.prev = null; tail.next = null;
        head.next = tail;
        tail.prev = head;
    }

    public void InsertBegin(nodeL node)
    {
        node.next = head.next;
        node.prev = head;
        head.next.prev = node;
        head.next = node;
        length++;
    }

    public void InsertEnd(nodeL node)
    {
        node.prev = tail.prev;
        node.next = tail;
        tail.prev.next = node;
        tail.prev = node;
        length++;
    }

    public void DelEnd()
    {
        tail.prev = tail.prev.prev;
        tail.prev.next = tail;
        length--;
    }

}














class God
{
    public nodeM[,] MainMatrix;
    public snake[] Snakes;
    public int[] snakeBool;
    Random rnd = new Random();
    public int life, score;
    public bool isCrash, isTurn;
    public God()
    {

        life = 5; score = 0;
        isCrash = false; isTurn = false;

        MainMatrix = new nodeM[Constants.MAT_W, Constants.MAT_H];
        for (int ii = 0; ii < Constants.MAT_W; ii++)
        {
            for (int jj = 0; jj < Constants.MAT_H; jj++)
            {
                MainMatrix[ii, jj] = new nodeM();
            }
        }
        Snakes = new snake[Constants.NUM_SNAKES];
        for (int ii = 0; ii < Constants.NUM_SNAKES; ii++)
        {
            Snakes[ii] = new snake();
            Snakes[ii].id = ii;
        }
        snakeBool = new int[Constants.NUM_SNAKES];
        for (int ii = 0; ii < Constants.NUM_SNAKES; ii++)
        {
            snakeBool[ii] = 0;
        }
    }

    public void UpdateAll()
    {
        for (int i = 1; i < Constants.NUM_SNAKES; i++)
        {
            int x = Snakes[i].position.head.next.x;
            int y = Snakes[i].position.head.next.y;
            nextcoord(x, y, snakeBool[i]);
        }
    }



    public int[] Smoothify(int x, int y) {
        int[] ans = new int[2];
        ans[0]=x; ans[1]=y;
        if (!MainMatrix[x, y].ishead) {
            if (MainMatrix[x, (y - 1 + 50) % 50].ishead) {
                ans[1] = (y - 1 + 50) % 50;
                return ans;
            }
            if (MainMatrix[(x + 1) % 30, y].ishead)
            {
                ans[0] = (x + 1) % 30;
                return ans;
            }
            if (MainMatrix[x, (y + 1 ) % 50].ishead)
            {
                ans[1] = (y + 1 ) % 50;
                return ans;
            }
            if (MainMatrix[(x - 1 + 30) % 30, y].ishead)
            {
                ans[0] = (x - 1 + 30) % 30;
                return ans;
            }

            if (MainMatrix[(x + 1) % 30, (y-1+50)%50].ishead)
            {
                ans[0] = (x + 1) % 30;
                ans[1] = (y - 1 + 50) % 50;
                return ans;
            }
            if (MainMatrix[(x + 1) % 30, (y + 1 + 50) % 50].ishead)
            {
                ans[0] = (x + 1) % 30;
                ans[1] = (y + 1 + 50) % 50;
                return ans;
            }
            if (MainMatrix[(x -1 + 30) % 30, (y + 1) % 50].ishead)
            {
                ans[0] = (x - 1 + 30) % 30;
                ans[1] = (y + 1 ) % 50;
                return ans;
            }

            if (MainMatrix[(x - 1 +30 ) % 30, (y - 1 + 50) % 50].ishead)
            {
                ans[0] = (x - 1+ 30) % 30;
                ans[1] = (y - 1 + 50) % 50;
                return ans;
            }
        }
        return ans;
    }






    public void UpdateA(int[] A, int sid)
    {
        int prevscore = score;
        int nextid = MainMatrix[A[0], A[1]].snakeid;
        nodeL n = new nodeL(A[0], A[1]);
        if (nextid!= 0)
        {
            isCrash = true;
            if (Snakes[sid].colour != Snakes[nextid].colour)
            {
                DeleteAppearSnake(nextid);
                score += 100;
            }
            else {
                DeleteAppearSnake(sid);
                DeleteAppearSnake(nextid);
                score = (score - 50);
                if (life > 0)
                {
                    life--;
                }
            }
        }
        else
        {
            Snakes[sid].position.InsertBegin(n);
            MainMatrix[A[0], A[1]].update_info(sid, true);
        }
        int slength = rnd.Next(3, 13);
        if (Snakes[sid].position.length > slength)
        {
            nodeL n1 = Snakes[sid].position.tail.prev;
            MainMatrix[n1.x, n1.y].update_info(0, false);
            Snakes[sid].position.DelEnd();
        }

        if (prevscore / 500 < score / 500 && life < 5 ) {
            life++;
        }

    }


    public int calDir(int x, int y)
    {
        int direc = 0 ;
        if (Snakes[MainMatrix[x, y].snakeid].position.length == 1)
        {
            if (x == 0)
                direc = 2;
            else if (x == 29)
                direc = 4;
            else if (y == 0)
                direc = 3;
            else
                direc = 1;

        }
        else
        {
            List L = Snakes[MainMatrix[x, y].snakeid].position;
            int nextx = L.head.next.next.x;
            int nexty = L.head.next.next.y;
            if (nextx - x == 0)
            {
                if (y == 49 && nexty == 0)
                    direc = 1;
                else if (y == 0 && nexty == 49)
                    direc = 3;
                else if (nexty > y ) 
                    direc = 1;
                 else if (nexty < y  )
                    direc = 3;
            }

            else 
            {
                if (x == 29 && nextx == 0)
                    direc = 4;
                else if (x == 0 && nextx == 29)
                    direc = 2;
                else if (nextx > x)
                    direc = 4;
                else if (nextx < x)
                    direc = 2;
            }

        }

        return direc;
    }


    public void nextcoord(int x, int y, int direc)
    {
        int[] A = new int[2];
        int originaldir = direc;
        int sid = MainMatrix[x, y].snakeid;
        if (MainMatrix[x, y].ishead)
        {

            direc = calDir(x, y);

            if (((originaldir + direc) % 2 == 1) && originaldir != 0)
            {
                isTurn = true;
                direc = originaldir;
            }

            switch (direc)
            {
                case (1):
                    A[0] = x % 30;
                    A[1] = (y - 1 + 50) % 50;
                    break;
                case (2):
                    A[0] = (x + 1) % 30;
                    A[1] = y % 50;
                    break;
                case (3):
                    A[0] = x % 30;
                    A[1] = (y + 1) % 50;
                    break;
                case (4):
                    A[0] = (x - 1 + 30) % 30;
                    A[1] = y % 50;
                    break;
                default:
                    break;



            }
        }
        UpdateA(A, sid);
    }








    public void Appear(int in_id)
    {
        int r = 0; int c = 0; //r and c are the row and column on the screen not the matrix
        int tc, tr;

        int in_color = rnd.Next(1, Constants.NUM_COLORS);
        while (true)
        {
            tc = rnd.Next(0, Constants.MAT_W);
            tr = rnd.Next(0, Constants.MAT_H);


            if (MainMatrix[tc, tr].snakeid == 0)
            {
                break;
            }
        }

        int temptoss = rnd.Next(1, 5);
        switch (temptoss)
        {
            case (1):
                //Appear form top
                r = 0;
                c = tc;
                break;
            case (2):
                //Appear from Bottom
                r = Constants.MAT_H - 1;
                c = tc;
                break;
            case (3):
                //Appear form Left
                c = 0;
                r = tr;
                break;
            case (4):
                //Appear from Right
                c = Constants.MAT_W - 1;
                r = tr;
                break;
        }


        Snakes[in_id].colour = in_color;
        Snakes[in_id].position.InsertBegin(new nodeL(c, r));
        MainMatrix[c, r].update_info(in_id, true);
    }


    public void Collision()
    {

    }

    public void DeleteAppearSnake(int sid)
    {
        //snake curs = Snakes[sid];

        nodeL tempn = Snakes[sid].position.tail.prev;
        while (tempn != Snakes[sid].position.head)
        {
            MainMatrix[tempn.x, tempn.y].update_info(0, false);
            Snakes[sid].position.DelEnd();
            tempn = Snakes[sid].position.tail.prev;
        }

        Appear(sid);

    }

}
