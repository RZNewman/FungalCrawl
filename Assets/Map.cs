using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    GameObject TilePre;
    public static Map world;
    Tile[] tiles;

    public Material[] colors;

    public Slime[] slimes;

    public struct Action
    {
        //ActionType type;
        public Tile target;
        public float amount;
        public float factor;
        public int owner;
        public Action(Tile t, float a, int o, float f)
        {
            target = t;
            amount = a;
            owner = o;
            factor = f;
        }
    }
    //public enum ActionType
    //{
    //    grow,
    //    attack

    //}

    List<Action> actions;
    // Start is called before the first frame update
    void Start()
    {
        actions = new List<Action>();
        world = this;
        findTiles();
        linkTiles();
    }

    void findTiles()
    {
        tiles = GetComponentsInChildren<Tile>();
    }
    void linkTiles()
    {
        foreach (Tile t in tiles)
        {
            t.link();
        }
    }
    void tickTiles()
    {
        foreach (Tile t in tiles)
        {
            t.SelfTick();
        }
    }
    void processActions()
    {
        foreach (Action a in actions)
        {
            takeAction(a);
        }
        actions = new List<Action>();
    }
    void sizeTiles()
    {
        foreach (Tile t in tiles)
        {
            t.resize();
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            //Debug.Log("click");
            tickTiles();
            processActions();
            sizeTiles();
        }
    }


    void takeAction(Action a)
    {
        Slime slT = slimes[a.target.owner];
        if (a.target.owner == 0)
        {
            if (a.target.amount == 0)
            {
                a.target.amount = a.amount;
                a.target.ChangeOwner(a.owner);
            }
            else
            {
                float r = a.target.amount * slT.defendFactor - a.amount * a.factor;
                //Debug.Log(r);
                if (r <= 0)
                {
                    a.target.ChangeOwner(a.owner);
                    a.target.amount = -r / a.factor;
                }
                else
                {
                    a.target.amount = r / slT.defendFactor;
                }
            }
            
        }
        else if (a.target.owner == a.owner)
        {
            if (a.amount < 0)
            {
                Debug.Log(a.amount);
            }
            a.target.amount += a.amount;
            if (a.target.amount > slT.max)
            {
                a.target.amount = slT.max;
            }
        }
        else
        {
            
            float r = a.target.amount * slT.defendFactor - a.amount * a.factor;
            
            if (r <= 0)
            {
                a.target.ChangeOwner(a.owner);
                a.target.amount = -r / a.factor;
            }
            else
            {
                a.target.amount = r / slT.defendFactor;
            }
            
        }
    }
    public void createAction(Tile target, Tile host)
    {
        Slime slH = slimes[host.owner];
        if (target.amount == 0)
        {
            //int transfer = Mathf.Max(host.split, Mathf.FloorToInt(host.amount * host.commitFactor));
            float transfer = host.amount * slH.commitFactor;
            host.amount -= transfer;
            actions.Add(new Action(target, transfer, host.owner, slH.attackFac));
        }
        else if (target.owner == host.owner)
        {
            float amt = Mathf.Min(host.amount, host.amount*slH.transfer * host.transFac);
            amt = Mathf.Min(amt, (host.amount - target.amount)/* / 2*/);
            host.amount -= amt;
            actions.Add(new Action(target, amt, host.owner, slH.attackFac));
        }
        else
        {
            float transfer = host.amount * slH.attackCommit;
            host.amount -= transfer;
            actions.Add(new Action(target, transfer, host.owner, slH.attackFac));
        }
    }
}
