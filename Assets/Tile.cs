using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public enum neighDir{
        up,
        down,
        right,
        left,
    }
    static Vector3[] neighVec = {
        new Vector3(0, 0, 1),
        new Vector3(0, 0, -1),
        new Vector3(1, 0, 0),
        new Vector3(-1, 0, 0),
    };
    Tile[] neigh = new Tile[4];

    public float amount = 0;
    public float growthFac = 1;
    public float transFac = 1;

    public int owner = 0;

    public enum LastAction
    {
        None,
        Split,
        Transfer,
        AttackE,
        AttackN
    }
    public LastAction la= LastAction.None;


    public void ChangeOwner(int o)
    {
        owner = o;
        mr.material = Map.world.colors[o-1];
    }
    MeshRenderer mr;
    // Start is called before the first frame update
    void Start()
    {
        mr = GetComponent<MeshRenderer>();
    }
    public void link()
    {
        for(int i=0;i<neigh.Length; i++)
        {
            RaycastHit hit;
            Vector3 gridPos = transform.position;
            gridPos.y = 0;
            if(Physics.Raycast(gridPos + neighVec[i] + Vector3.down, Vector3.up, out hit, 2))
            {
                neigh[i] = hit.transform.GetComponent<Tile>();
            }
        }
    }
    // Update is called once per frame
    public void SelfTick()
    {
        Slime sl = Map.world.slimes[owner];
        if (owner != 0)
        {
            if (!(amount > sl.max))
            {
                amount += sl.growth * growthFac;
                if (amount > sl.max)
                {
                    amount = sl.max;
                }
            }


            List<Tile> nArr = new List<Tile>(neigh);
            int count = nArr.Count;
            List<Tile> n = new List<Tile>();
            for (int i = 0; i < count; i++)
            {
                int r = Random.Range(1, count - i) - 1;
                n.Add(nArr[r]);
                nArr.RemoveAt(r);
            }
            bool adjEmpty = false;
            foreach (Tile t in n)
            {
                if (t != null)
                {
                    if (t.amount == 0)
                    {
                        adjEmpty = true;
                        if (amount > sl.split)
                        {
                            Map.world.createAction(t, this);
                            la = LastAction.Split;
                            return;
                        }
                    }
                }

            }
            
            if (amount > sl.attack)
            {
                
                foreach (Tile t in n)
                {
                    if (t != null)
                    {
                        if (t.owner!=0&&t.owner != owner)
                        {
                            Map.world.createAction(t, this);
                            la = LastAction.AttackE;
                            return;
                        }
                    }

                }
                foreach (Tile t in n)
                {
                    if (t != null)
                    {
                        if (t.owner ==0 && t.amount>0)
                        {
                            //Debug.Log("a Neut");
                            Map.world.createAction(t, this);
                            la = LastAction.AttackN;
                            return;
                        }
                    }

                }
            }

            foreach (Tile t in n)
            {
                Tile transferTarget = null;
                if (t != null && !adjEmpty)
                {
                    if (t.owner == owner && t.amount <= amount)
                    {
                        if(transferTarget == null)
                        {
                            transferTarget = t;
                        }
                        else if (t.amount < transferTarget.amount)
                        {
                            transferTarget = t;
                        }
                    }
                }
                if (transferTarget != null)
                {
                    Map.world.createAction(transferTarget, this);
                    la = LastAction.Transfer;
                    return;
                }

            }
            la = LastAction.None;
            //if (amount > sl.split)
            //{

            //}
        }
        

        



       
        
        

    }
    public void resize()
    {
        float scale = (amount + 1) / 100;
        transform.localScale = new Vector3(1, scale, 1);
    }



}
